import { ChangeDetectionStrategy, Component, DestroyRef, ElementRef, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { FormErrorSummaryComponent } from '../../shared/forms/form-error-summary';
import { FormFocusService } from '../../shared/forms/form-focus.service';
import { SafeFormShortcutsDirective } from '../../shared/forms/safe-form-shortcuts.directive';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { ProductApiService } from './data/product-api.service';
import { Product, ProductCommand, ProductFormPage } from './data/product.models';
import { ProductLocalizationService } from './product-localization.service';

const ProductType = { Raw: 1, Direct: 2, Prepared: 3, Service: 4 } as const;
const BillingPolicy = { Ordered: 1, Received: 2, ServiceCompletion: 3, DirectDelivery: 4 } as const;
const Handling = { Fragile: 1, KeepDry: 2, Refrigerated: 4, Frozen: 8, Hazardous: 16 } as const;

@Component({ selector: 'app-product-form-page', imports: [ReactiveFormsModule, RouterLink, PageHeaderComponent, FeedbackStateComponent, FormErrorSummaryComponent, SafeFormShortcutsDirective, ApiRecoveryComponent], templateUrl: './product-form-page.html', styleUrl: './product-form-page.scss', changeDetection: ChangeDetectionStrategy.OnPush })
export class ProductFormPageComponent {
  protected readonly i18n = inject(ProductLocalizationService);
  private readonly api = inject(ProductApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly workspace = inject(WorkspaceService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly host = inject(ElementRef<HTMLElement>);
  private readonly focus = inject(FormFocusService);
  protected readonly id = Number(this.route.snapshot.paramMap.get('id')) || undefined;
  protected readonly page = signal<ProductFormPage | null>(null);
  protected readonly saving = signal(false);
  protected readonly problem = signal('');
  protected readonly recovery = signal<NormalizedApiProblem | null>(null);
  protected readonly fieldErrors = signal<Record<string, readonly string[]>>({});
  protected readonly massUnits = computed(() => this.page()?.measurementUnits.filter(unit => unit.dimension === 2) ?? []);
  protected readonly lengthUnits = computed(() => this.page()?.measurementUnits.filter(unit => unit.dimension === 4) ?? []);
  protected readonly form = inject(FormBuilder).nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(200)]], barcode: ['', Validators.maxLength(100)],
    productType: [ProductType.Direct as number, Validators.required], measurementUnitId: [0, Validators.min(1)], productCategoryId: [null as number | null],
    purchaseBillingPolicy: [BillingPolicy.Received as number, Validators.required], purchaseExpenseAccountId: [null as number | null],
    purchasePrice: [0, Validators.min(0)], wholesalePrice: [0, Validators.min(0)], salePrice: [0, Validators.min(0)],
    isSellableInPos: true, isSellableInSales: true, allowNegativeRecipeConsumption: false, isActive: true, supplierIds: [[] as number[]],
    trackingPolicy: 0, defaultShelfLifeDays: [null as number | null, [Validators.min(1), Validators.max(36500)]],
    requireExpirationDate: false, expirationWarningDays: [30, [Validators.min(0), Validators.max(3650)]],
    netWeight: [null as number | null, Validators.min(0)], grossWeight: [null as number | null, Validators.min(0)], weightMeasurementUnitId: [null as number | null],
    length: [null as number | null, Validators.min(0.000001)], width: [null as number | null, Validators.min(0.000001)], height: [null as number | null, Validators.min(0.000001)], dimensionMeasurementUnitId: [null as number | null],
    isFragile: false, keepDry: false, requiresRefrigeration: false, requiresFrozenStorage: false, isHazardous: false,
  });

  constructor() {
    this.loadForm();
    this.form.valueChanges.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(() => this.setWorkspaceDirty(this.form.dirty));
    this.form.controls.productType.valueChanges.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(type => this.applyTypeRules(type));
    this.form.controls.purchaseBillingPolicy.valueChanges.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(policy => { if (policy === BillingPolicy.Received) this.form.controls.purchaseExpenseAccountId.setValue(null, { emitEvent: false }); });
    this.form.controls.trackingPolicy.valueChanges.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(policy => { if (policy === 0) this.form.patchValue({ defaultShelfLifeDays: null, requireExpirationDate: false }, { emitEvent: false }); });
    this.form.controls.isActive.valueChanges.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(active => { if (!active) this.form.controls.supplierIds.setValue([], { emitEvent: false }); });
    this.form.controls.requiresRefrigeration.valueChanges.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(value => { if (value) this.form.controls.requiresFrozenStorage.setValue(false, { emitEvent: false }); });
    this.form.controls.requiresFrozenStorage.valueChanges.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(value => { if (value) this.form.controls.requiresRefrigeration.setValue(false, { emitEvent: false }); });
  }

  canDeactivate(): boolean { return !this.form.dirty || globalThis.confirm(this.i18n.text('unsavedDescription')); }
  protected isRaw(): boolean { return this.form.controls.productType.value === ProductType.Raw; }
  protected isPrepared(): boolean { return this.form.controls.productType.value === ProductType.Prepared; }
  protected isService(): boolean { return this.form.controls.productType.value === ProductType.Service; }
  protected needsExpenseAccount(): boolean { return this.form.controls.purchaseBillingPolicy.value !== BillingPolicy.Received; }
  protected canAssignSuppliers(): boolean { return this.form.controls.isActive.value && !this.isPrepared(); }

  protected save(): void {
    if (this.saving()) return;
    this.problem.set(''); this.fieldErrors.set({}); this.recovery.set(null);
    if (this.form.invalid || !this.crossFieldValid()) { this.form.markAllAsTouched(); this.fieldErrors.set(this.clientErrors()); this.problem.set(this.i18n.text('correctProductFields')); this.focus.focusFirstInvalid(this.host.nativeElement); return; }
    this.saving.set(true);
    const command = this.toCommand();
    const request = this.id ? this.api.update(this.id, command) : this.api.create(command);
    request.subscribe({ next: product => { this.form.markAsPristine(); this.setWorkspaceDirty(false); this.saving.set(false); void this.router.navigate(['/products', product.id]); }, error: error => { const failure = this.errors.normalize(error); const fieldErrors = failure.errors ?? {}; this.recovery.set(failure); this.fieldErrors.set(fieldErrors); this.problem.set(failure.detail ?? failure.title ?? this.i18n.text('errorDescription')); this.saving.set(false); const first = Object.keys(fieldErrors).find(key => key); if (first) this.focus.focusField(this.host.nativeElement, first); } });
  }

  protected loadForm(): void {
    this.problem.set(''); this.recovery.set(null);
    this.api.getForm(this.id).subscribe({ next: page => { this.page.set(page); this.form.reset(this.toControls(page.form)); this.applyTypeRules(page.form.productType, false); this.form.markAsPristine(); }, error: error => { const failure = this.errors.normalize(error); this.recovery.set(failure); this.problem.set(failure.title ?? this.i18n.text('errorDescription')); } });
  }

  protected errorFor(name: string): string { const server = this.fieldErrors()[name] ?? this.fieldErrors()[name[0].toUpperCase() + name.slice(1)]; if (server?.length) return server[0]; const control = this.form.get(name); return control?.touched && control.invalid ? this.i18n.text('invalidField') : ''; }
  protected focusField(name: string): void { this.focus.focusField(this.host.nativeElement, name); }
  private setWorkspaceDirty(dirty: boolean): void { const tab = this.workspace.activeTab(); if (tab) this.workspace.markDirty(tab.id, dirty); }
  private applyTypeRules(type: number, markDirty = true): void {
    if (type === ProductType.Raw) this.form.patchValue({ isSellableInPos: false, isSellableInSales: false }, { emitEvent: false });
    if (type === ProductType.Prepared) this.form.patchValue({ purchasePrice: 0, supplierIds: [], purchaseBillingPolicy: BillingPolicy.Received, purchaseExpenseAccountId: null }, { emitEvent: false });
    if (type === ProductType.Service && this.form.controls.purchaseBillingPolicy.value === BillingPolicy.Received) this.form.controls.purchaseBillingPolicy.setValue(BillingPolicy.ServiceCompletion, { emitEvent: false });
    if (type !== ProductType.Service && this.form.controls.purchaseBillingPolicy.value === BillingPolicy.ServiceCompletion) this.form.controls.purchaseBillingPolicy.setValue(BillingPolicy.Received, { emitEvent: false });
    if (markDirty) this.form.markAsDirty();
  }
  private crossFieldValid(): boolean {
    const value = this.form.getRawValue();
    if ((value.netWeight !== null || value.grossWeight !== null) && value.weightMeasurementUnitId === null) return false;
    const dimensions = [value.length, value.width, value.height];
    if (dimensions.some(item => item !== null) && (dimensions.some(item => item === null) || value.dimensionMeasurementUnitId === null)) return false;
    if (value.trackingPolicy === 0 && (value.defaultShelfLifeDays !== null || value.requireExpirationDate)) return false;
    if (value.purchaseBillingPolicy !== BillingPolicy.Received && value.purchaseExpenseAccountId === null) return false;
    return value.grossWeight === null || value.netWeight === null || value.grossWeight >= value.netWeight;
  }
  private clientErrors(): Record<string, readonly string[]> { return Object.fromEntries(Object.entries(this.form.controls).filter(([, control]) => control.invalid).map(([name]) => [name, [this.i18n.text('invalidField')]])); }
  private toControls(product: Product) { return { name: product.name ?? '', barcode: product.barcode ?? '', productType: product.productType || ProductType.Direct, measurementUnitId: product.measurementUnitId ?? 0, productCategoryId: product.productCategoryId ?? null, purchaseBillingPolicy: product.purchaseBillingPolicy || BillingPolicy.Received, purchaseExpenseAccountId: product.purchaseExpenseAccountId ?? null, purchasePrice: product.purchasePrice ?? 0, wholesalePrice: product.wholesalePrice ?? 0, salePrice: product.salePrice ?? 0, isSellableInPos: product.isSellableInPos ?? true, isSellableInSales: product.isSellableInSales ?? true, allowNegativeRecipeConsumption: product.allowNegativeRecipeConsumption ?? false, isActive: product.isActive ?? true, supplierIds: [...(product.supplierIds ?? [])], trackingPolicy: product.trackingPolicy ?? 0, defaultShelfLifeDays: product.defaultShelfLifeDays ?? null, requireExpirationDate: product.requireExpirationDate ?? false, expirationWarningDays: product.expirationWarningDays ?? 30, netWeight: product.netWeight ?? null, grossWeight: product.grossWeight ?? null, weightMeasurementUnitId: product.weightMeasurementUnitId ?? null, length: product.length ?? null, width: product.width ?? null, height: product.height ?? null, dimensionMeasurementUnitId: product.dimensionMeasurementUnitId ?? null, isFragile: !!(product.handlingRequirements & Handling.Fragile), keepDry: !!(product.handlingRequirements & Handling.KeepDry), requiresRefrigeration: !!(product.handlingRequirements & Handling.Refrigerated), requiresFrozenStorage: !!(product.handlingRequirements & Handling.Frozen), isHazardous: !!(product.handlingRequirements & Handling.Hazardous) }; }
  private toCommand(): ProductCommand { const raw = this.form.getRawValue(); return { ...raw, barcode: raw.barcode.trim() || undefined, productCategoryId: raw.productCategoryId ?? undefined, purchaseExpenseAccountId: raw.purchaseExpenseAccountId ?? undefined, netWeight: raw.netWeight ?? undefined, grossWeight: raw.grossWeight ?? undefined, weightMeasurementUnitId: raw.weightMeasurementUnitId ?? undefined, length: raw.length ?? undefined, width: raw.width ?? undefined, height: raw.height ?? undefined, dimensionMeasurementUnitId: raw.dimensionMeasurementUnitId ?? undefined, defaultShelfLifeDays: raw.defaultShelfLifeDays ?? undefined, supplierIds: this.canAssignSuppliers() ? raw.supplierIds : [], purchasePrice: this.isPrepared() ? 0 : raw.purchasePrice, isSellableInPos: this.isRaw() ? false : raw.isSellableInPos, isSellableInSales: this.isRaw() ? false : raw.isSellableInSales }; }
}
