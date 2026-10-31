import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { LocalizationService } from '../../core/localization/localization.service';
import { SafeFormShortcutsDirective } from '../../shared/forms/safe-form-shortcuts.directive';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { SettingsApiService } from './data/settings-api.service';
import { BranchWarehouseAccess, InventoryAccessBranch, InventoryAccessSettings, InventoryAccessWarehouse, PosTerminalAccess } from './data/settings.models';

@Component({
  selector: 'app-inventory-access-page',
  imports: [ReactiveFormsModule, RouterLink, SafeFormShortcutsDirective, ApiRecoveryComponent, FeedbackStateComponent, PageHeaderComponent],
  templateUrl: './inventory-access-page.html',
  styleUrl: './settings-forms.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class InventoryAccessPageComponent {
  protected readonly i18n = inject(LocalizationService);
  private readonly api = inject(SettingsApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly workspace = inject(WorkspaceService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly fb = inject(FormBuilder).nonNullable;

  protected readonly loading = signal(true);
  protected readonly saving = signal(false);
  protected readonly message = signal<'branch' | 'terminal' | 'terminalWarehouse' | null>(null);
  protected readonly problem = signal<NormalizedApiProblem | null>(null);
  protected readonly branches = signal<InventoryAccessBranch[]>([]);
  protected readonly warehouses = signal<InventoryAccessWarehouse[]>([]);
  protected readonly branchAccesses = signal<BranchWarehouseAccess[]>([]);
  protected readonly posTerminals = signal<PosTerminalAccess[]>([]);

  protected readonly branchForm = this.fb.group({
    branchId: [0, Validators.min(1)], warehouseId: [0, Validators.min(1)], priority: [1, [Validators.min(1), Validators.max(999)]],
    isDefaultForPos: false, allowPosSales: false, allowPurchases: false,
    allowTransferOut: true, allowTransferIn: true, allowReplenishment: true,
  });
  protected readonly terminalForm = this.fb.group({
    name: ['', [Validators.required, Validators.maxLength(100)]], branchId: [0, Validators.min(1)], defaultWarehouseId: [0, Validators.min(1)],
  });
  protected readonly terminalWarehouseForm = this.fb.group({
    posTerminalId: [0, Validators.min(1)], warehouseId: [0, Validators.min(1)], priority: [1, [Validators.min(1), Validators.max(999)]], makeDefault: false,
  });

  constructor() {
    this.load();
    this.branchForm.valueChanges.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => this.syncDirty());
    this.terminalForm.valueChanges.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => this.syncDirty());
    this.terminalWarehouseForm.valueChanges.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => this.syncDirty());
    this.terminalForm.controls.branchId.valueChanges.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(branchId => {
      if (!this.allowedPosWarehouses(branchId).some(item => item.id === this.terminalForm.controls.defaultWarehouseId.value)) {
        this.terminalForm.controls.defaultWarehouseId.setValue(0);
      }
    });
    this.terminalWarehouseForm.controls.posTerminalId.valueChanges.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => {
      const allowed = this.allowedTerminalWarehouses();
      if (!allowed.some(item => item.id === this.terminalWarehouseForm.controls.warehouseId.value)) {
        this.terminalWarehouseForm.controls.warehouseId.setValue(0);
      }
    });
  }

  canDeactivate(): boolean { return !this.isDirty() || globalThis.confirm(this.i18n.text('unsavedDescription')); }
  protected allowedPosWarehouses(branchId: number): InventoryAccessWarehouse[] {
    const allowedIds = new Set(this.branchAccesses().filter(item => item.branchId === branchId && item.allowPosSales).map(item => item.warehouseId));
    return this.warehouses().filter(item => item.allowPosSales && allowedIds.has(item.id));
  }
  protected allowedTerminalWarehouses(): InventoryAccessWarehouse[] {
    const terminal = this.posTerminals().find(item => item.id === this.terminalWarehouseForm.controls.posTerminalId.value);
    return terminal ? this.allowedPosWarehouses(terminal.branchId) : [];
  }
  protected terminalWarehouseSummary(terminal: PosTerminalAccess): string {
    return terminal.warehouses.map(item => `${item.priority}. ${item.warehouseName}`).join(' → ');
  }
  protected load(): void {
    this.loading.set(true); this.problem.set(null);
    this.api.getInventoryAccess().subscribe({ next: page => { this.apply(page); this.resetAll(); this.loading.set(false); }, error: error => { this.problem.set(this.errors.normalize(error)); this.loading.set(false); } });
  }
  protected saveBranchAccess(): void {
    if (!this.ready(this.branchForm.invalid)) return;
    this.start(); this.api.configureBranchWarehouse(this.branchForm.getRawValue()).subscribe({
      next: page => { this.apply(page); this.resetBranchForm(); this.message.set('branch'); this.saving.set(false); }, error: error => this.fail(error),
    });
  }
  protected createTerminal(): void {
    if (!this.ready(this.terminalForm.invalid)) return;
    const value = this.terminalForm.getRawValue(); this.start();
    this.api.createPosTerminal({ ...value, name: value.name.trim() }).subscribe({
      next: page => { this.apply(page); this.resetTerminalForm(); this.message.set('terminal'); this.saving.set(false); }, error: error => this.fail(error),
    });
  }
  protected saveTerminalWarehouse(): void {
    if (!this.ready(this.terminalWarehouseForm.invalid)) return;
    this.start(); this.api.configurePosTerminalWarehouse(this.terminalWarehouseForm.getRawValue()).subscribe({
      next: page => { this.apply(page); this.resetTerminalWarehouseForm(); this.message.set('terminalWarehouse'); this.saving.set(false); }, error: error => this.fail(error),
    });
  }
  private ready(invalid: boolean): boolean {
    if (this.saving() || invalid) {
      this.branchForm.markAllAsTouched(); this.terminalForm.markAllAsTouched(); this.terminalWarehouseForm.markAllAsTouched(); return false;
    }
    return true;
  }
  private start(): void { this.saving.set(true); this.message.set(null); this.problem.set(null); }
  private fail(error: unknown): void { this.problem.set(this.errors.normalize(error)); this.saving.set(false); }
  private apply(page: InventoryAccessSettings): void {
    this.branches.set(page.branches); this.warehouses.set(page.warehouses); this.branchAccesses.set(page.branchAccesses); this.posTerminals.set(page.posTerminals);
  }
  private resetAll(): void { this.resetBranchForm(); this.resetTerminalForm(); this.resetTerminalWarehouseForm(); }
  private resetBranchForm(): void { this.branchForm.reset({ branchId: 0, warehouseId: 0, priority: 1, isDefaultForPos: false, allowPosSales: false, allowPurchases: false, allowTransferOut: true, allowTransferIn: true, allowReplenishment: true }); this.branchForm.markAsPristine(); this.syncDirty(); }
  private resetTerminalForm(): void { this.terminalForm.reset({ name: '', branchId: 0, defaultWarehouseId: 0 }); this.terminalForm.markAsPristine(); this.syncDirty(); }
  private resetTerminalWarehouseForm(): void { this.terminalWarehouseForm.reset({ posTerminalId: 0, warehouseId: 0, priority: 1, makeDefault: false }); this.terminalWarehouseForm.markAsPristine(); this.syncDirty(); }
  private isDirty(): boolean { return this.branchForm.dirty || this.terminalForm.dirty || this.terminalWarehouseForm.dirty; }
  private syncDirty(): void { const tab = this.workspace.activeTab(); if (tab) this.workspace.markDirty(tab.id, this.isDirty()); }
}
