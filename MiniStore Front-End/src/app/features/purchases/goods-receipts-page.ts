import { DatePipe, DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormArray, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { DataTableComponent } from '../../shared/ui/data-table/data-table';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { GoodsReceiptApiService } from './goods-receipt-api.service';
import { GoodsReceiptKey, GoodsReceiptLocalizationService } from './goods-receipt-localization.service';
import { GoodsReceipt, GoodsReceiptCreatePage, GoodsReceiptScanPage } from './goods-receipt.models';

@Component({
  selector: 'app-goods-receipts-page',
  imports: [DatePipe, DecimalPipe, ReactiveFormsModule, RouterLink, ApiRecoveryComponent, DataTableComponent, FeedbackStateComponent, PageHeaderComponent],
  templateUrl: './goods-receipts-page.html',
  styleUrl: './goods-receipts-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class GoodsReceiptsPageComponent {
  protected readonly i18n = inject(GoodsReceiptLocalizationService);
  protected readonly session = inject(ApiSessionService);
  private readonly api = inject(GoodsReceiptApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly workspace = inject(WorkspaceService);
  private readonly destroyRef = inject(DestroyRef);
  protected readonly rows = signal<GoodsReceipt[]>([]);
  protected readonly detail = signal<GoodsReceipt | null>(null);
  protected readonly createPage = signal<GoodsReceiptCreatePage | null>(null);
  protected readonly scanPage = signal<GoodsReceiptScanPage | null>(null);
  protected readonly loading = signal(true);
  protected readonly loadingProblem = signal<any>(null);
  protected readonly saving = signal(false);
  protected readonly problem = signal('');
  protected readonly success = signal('');
  protected readonly search = new FormControl('', { nonNullable: true });
  protected readonly receiptForm = new FormGroup({
    receiptDate: new FormControl(new Date().toISOString().slice(0, 10), { nonNullable: true, validators: Validators.required }),
    notes: new FormControl('', { nonNullable: true, validators: Validators.maxLength(1000) }),
    lines: new FormArray<FormGroup>([]),
  });
  protected readonly scanForm = new FormGroup({
    productScan: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.maxLength(100)] }),
    quantity: new FormControl(1, { nonNullable: true, validators: Validators.min(0.000001) }),
    idempotencyKey: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.minLength(16), Validators.maxLength(100)] }),
    lotNumber: new FormControl('', { nonNullable: true, validators: Validators.maxLength(100) }),
    serialNumbers: new FormControl('', { nonNullable: true, validators: Validators.maxLength(2000) }),
    manufactureDate: new FormControl('', { nonNullable: true }),
    expirationDate: new FormControl('', { nonNullable: true }),
  });
  protected readonly inspectionForm = new FormGroup({
    notes: new FormControl('', { nonNullable: true, validators: Validators.maxLength(1000) }),
    lines: new FormArray<FormGroup>([]),
  });

  constructor() {
    this.receiptForm.valueChanges.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => this.markDirty());
    this.scanForm.valueChanges.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => this.markDirty());
    this.inspectionForm.valueChanges.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => this.markDirty());
    this.route.url.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => this.resolve());
  }

  canDeactivate() {
    return !this.isDirty() || globalThis.confirm(this.i18n.text('unsaved'));
  }
  protected loadList() {
    this.start();
    this.api.getAll(this.search.value).subscribe({ next: x => { this.rows.set(x); this.loading.set(false); }, error: e => this.fail(e) });
  }
  protected create(postNow: boolean) {
    const page = this.createPage();
    if (!page || this.receiptForm.invalid || this.receiptLines.length === 0) return this.invalid();
    const value = this.receiptForm.getRawValue();
    if (postNow) {
      const lines = value.lines.map(line => this.cleanTracking(line));
      this.execute(this.api.create({ purchaseOrderId: page.purchaseOrder.id, receiptDate: value.receiptDate, notes: value.notes.trim() || undefined, lines }), id => this.router.navigate(['/purchases/receipts', id]));
    } else {
      this.execute(this.api.createDraft({ purchaseOrderId: page.purchaseOrder.id, receiptDate: value.receiptDate, notes: value.notes.trim() || undefined }), id => this.router.navigate(['/purchases/receipts', id, 'scan']));
    }
  }
  protected recordScan() {
    const page = this.scanPage();
    if (!page || this.scanForm.invalid) return this.invalid();
    const value = this.cleanTracking(this.scanForm.getRawValue());
    this.execute(this.api.scan(page.receipt.id, { goodsReceiptId: page.receipt.id, ...value }), () => this.loadScan(page.receipt.id));
  }
  protected postDraft() {
    const page = this.scanPage();
    if (page) this.execute(this.api.post(page.receipt.id), () => this.router.navigate(['/purchases/receipts', page.receipt.id]));
  }
  protected inspect() {
    const receipt = this.detail();
    if (!receipt || this.inspectionForm.invalid || this.inspectionLines.length === 0) return this.invalid();
    const value = this.inspectionForm.getRawValue();
    this.execute(this.api.inspect(receipt.id, { goodsReceiptId: receipt.id, notes: value.notes.trim() || undefined, lines: value.lines }), () => this.loadDetail(receipt.id));
  }
  protected putAway() {
    const receipt = this.detail();
    if (receipt) this.execute(this.api.putAway(receipt.id), () => this.loadDetail(receipt.id));
  }
  protected statusName(value: number) {
    return this.i18n.text(({1:'draft',2:'posted',3:'cancelled',4:'reversed',5:'awaitingInspection',6:'awaitingPutaway',7:'qualityRejected'} as Record<number, GoodsReceiptKey>)[value] ?? 'draft');
  }
  protected trackingName(value: number) { return this.i18n.text(value === 1 ? 'lotTracking' : value === 2 ? 'serialTracking' : 'none'); }
  protected retry() { this.resolve(); }
  protected get receiptLines() { return this.receiptForm.controls.lines; }
  protected get inspectionLines() { return this.inspectionForm.controls.lines; }

  private resolve() {
    const path = this.route.snapshot.routeConfig?.path ?? '';
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (path.endsWith('/scan') && id) this.loadScan(id);
    else if (path.endsWith('/new')) this.loadCreate(Number(this.route.snapshot.queryParamMap.get('purchaseOrderId')));
    else if (id) this.loadDetail(id);
    else this.loadList();
  }
  private loadCreate(purchaseOrderId: number) {
    if (!purchaseOrderId) return this.fail(new Error('purchaseOrderId'));
    this.start();
    this.api.getCreateForm(purchaseOrderId).subscribe({
      next: page => {
        this.createPage.set(page);
        this.receiptForm.patchValue({ receiptDate: page.form.receiptDate, notes: page.form.notes ?? '' }, { emitEvent: false });
        this.receiptLines.clear();
        for (const line of page.remainingLines) this.receiptLines.push(new FormGroup({
          purchaseOrderLineId: new FormControl(line.purchaseOrderLineId, { nonNullable: true }),
          receivedQuantity: new FormControl(0, { nonNullable: true, validators: [Validators.required, Validators.min(0), Validators.max(line.remainingQuantity)] }),
          lotNumber: new FormControl('', { nonNullable: true, validators: Validators.maxLength(100) }),
          serialNumbers: new FormControl('', { nonNullable: true, validators: Validators.maxLength(2000) }),
          manufactureDate: new FormControl('', { nonNullable: true }),
          expirationDate: new FormControl('', { nonNullable: true }),
        }));
        this.receiptForm.markAsPristine(); this.markDirty(); this.loading.set(false);
      }, error: e => this.fail(e),
    });
  }
  private loadDetail(id: number) {
    this.start();
    this.api.get(id).subscribe({
      next: receipt => {
        this.detail.set(receipt);
        this.inspectionForm.controls.notes.setValue(receipt.inspectionNotes ?? '', { emitEvent: false });
        this.inspectionLines.clear();
        for (const line of receipt.lines) this.inspectionLines.push(new FormGroup({
          goodsReceiptLineId: new FormControl(line.id, { nonNullable: true }),
          acceptedQuantity: new FormControl(line.deliveredQuantity, { nonNullable: true, validators: Validators.min(0) }),
          rejectedQuantity: new FormControl(0, { nonNullable: true, validators: Validators.min(0) }),
          damagedQuantity: new FormControl(0, { nonNullable: true, validators: Validators.min(0) }),
          shortQuantity: new FormControl(0, { nonNullable: true, validators: Validators.min(0) }),
          reason: new FormControl('', { nonNullable: true, validators: Validators.maxLength(500) }),
        }));
        this.inspectionForm.markAsPristine(); this.markDirty(); this.loading.set(false);
      }, error: e => this.fail(e),
    });
  }
  private loadScan(id: number) {
    this.start();
    this.api.getScan(id).subscribe({
      next: page => {
        this.scanPage.set(page);
        this.scanForm.reset({ productScan:'', quantity:1, idempotencyKey:page.idempotencyKey, lotNumber:'', serialNumbers:'', manufactureDate:'', expirationDate:'' }, { emitEvent:false });
        this.scanForm.markAsPristine(); this.markDirty(); this.loading.set(false);
      }, error: e => this.fail(e),
    });
  }
  private execute(request: any, done: (id?: number) => void) {
    if (this.saving()) return;
    this.saving.set(true); this.problem.set(''); this.success.set('');
    request.subscribe({
      next: (result: { id?: number } | void) => {
        this.saving.set(false); this.success.set(this.i18n.text('success'));
        this.receiptForm.markAsPristine(); this.scanForm.markAsPristine(); this.inspectionForm.markAsPristine(); this.markDirty();
        done(result && 'id' in result ? result.id : undefined);
      },
      error: (e: unknown) => { this.saving.set(false); const p = this.errors.normalize(e); this.problem.set(p.detail ?? p.title ?? this.i18n.text('error')); },
    });
  }
  private cleanTracking(value: Record<string, any>) {
    return { ...value, lotNumber:value['lotNumber']?.trim() || undefined, serialNumbers:value['serialNumbers']?.trim() || undefined, manufactureDate:value['manufactureDate'] || undefined, expirationDate:value['expirationDate'] || undefined };
  }
  private invalid() { this.problem.set(this.i18n.text('fix')); }
  private start() { this.loading.set(true); this.loadingProblem.set(null); this.problem.set(''); this.success.set(''); }
  private fail(error: unknown) { this.loadingProblem.set(this.errors.normalize(error)); this.loading.set(false); }
  private isDirty() { return this.receiptForm.dirty || this.scanForm.dirty || this.inspectionForm.dirty; }
  private markDirty() { const tab = this.workspace.activeTab(); if (tab) this.workspace.markDirty(tab.id, this.isDirty()); }
}
