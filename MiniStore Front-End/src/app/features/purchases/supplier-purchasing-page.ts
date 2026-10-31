import { DatePipe, DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { DataTableComponent } from '../../shared/ui/data-table/data-table';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { SupplierPurchasingApiService } from './supplier-purchasing-api.service';
import { SupplierPurchasingLocalizationService } from './supplier-purchasing-localization.service';
import {
  ImportPreview,
  PriceBreakPage,
  PriceExceptionPage,
  PriceHistoryPage,
  SupplierPurchasingPage,
  SupplierPurchasingRow,
} from './supplier-purchasing.models';
@Component({
  selector: 'app-supplier-purchasing-page',
  imports: [
    DatePipe,
    DecimalPipe,
    ReactiveFormsModule,
    RouterLink,
    ApiRecoveryComponent,
    DataTableComponent,
    FeedbackStateComponent,
    PageHeaderComponent,
  ],
  templateUrl: './supplier-purchasing-page.html',
  styleUrl: './supplier-purchasing-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SupplierPurchasingPageComponent {
  protected readonly i18n = inject(SupplierPurchasingLocalizationService);
  protected readonly session = inject(ApiSessionService);
  private readonly api = inject(SupplierPurchasingApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly route = inject(ActivatedRoute);
  private readonly fb = inject(FormBuilder);
  private readonly workspace = inject(WorkspaceService);
  private readonly destroy = inject(DestroyRef);
  protected readonly mode = signal<'main' | 'breaks' | 'history' | 'exceptions'>('main');
  protected readonly page = signal<SupplierPurchasingPage | null>(null);
  protected readonly breakPage = signal<PriceBreakPage | null>(null);
  protected readonly historyPage = signal<PriceHistoryPage | null>(null);
  protected readonly exceptionPage = signal<PriceExceptionPage | null>(null);
  protected readonly preview = signal<ImportPreview | null>(null);
  protected readonly loading = signal(true);
  protected readonly loadingProblem = signal<any>(null);
  protected readonly saving = signal(false);
  protected readonly problem = signal('');
  protected readonly success = signal('');
  protected readonly rows = this.fb.array<FormGroup>([]);
  protected readonly breaks = this.fb.array<FormGroup>([]);
  constructor() {
    this.rows.valueChanges.pipe(takeUntilDestroyed(this.destroy)).subscribe(() => this.dirty());
    this.breaks.valueChanges.pipe(takeUntilDestroyed(this.destroy)).subscribe(() => this.dirty());
    this.route.url.pipe(takeUntilDestroyed(this.destroy)).subscribe(() => this.resolve());
  }
  canDeactivate() {
    return !(this.rows.dirty || this.breaks.dirty) || globalThis.confirm(this.i18n.text('unsaved'));
  }
  protected loadSupplier(value: string) {
    const id = Number(value);
    if (id) this.loadMain(id);
  }
  protected saveBulk() {
    const p = this.page(),
      rows = this.rows.getRawValue();
    if (!p || !rows.some((x: any) => x.selected)) return this.problem.set(this.i18n.text('fix'));
    this.saving.set(true);
    this.api.saveBulk({ supplierId: p.selectedSupplierId, rows }).subscribe({
      next: () => {
        this.success.set(this.i18n.text('success'));
        this.loadMain(p.selectedSupplierId);
      },
      error: (e) => this.failSave(e),
    });
  }
  protected selectRow(i: number) {
    this.rows.at(i).get('selected')?.setValue(true);
  }
  protected toggle(x: SupplierPurchasingRow) {
    this.saving.set(true);
    this.api
      .setActive(x.id, !x.isActive, x.rowVersion)
      .subscribe({ next: () => this.loadMain(x.supplierId), error: (e) => this.failSave(e) });
  }
  protected addBreak() {
    const p = this.breakPage();
    if (!p || this.breaks.length >= 20) return;
    this.breaks.push(
      this.fb.nonNullable.group({
        selected: [true],
        minimumQuantity: [p.minimumOrderQuantity, Validators.min(p.minimumOrderQuantity)],
        unitPrice: [p.baseUnitPrice, Validators.min(0)],
        discountPercent: [0, [Validators.min(0), Validators.max(100)]],
      }),
    );
  }
  protected removeBreak(i: number) {
    this.breaks.removeAt(i);
    this.breaks.markAsDirty();
  }
  protected saveBreaks() {
    const p = this.breakPage();
    if (!p) return;
    this.saving.set(true);
    this.api
      .saveBreaks(p.purchasingInfoId, {
        purchasingInfoId: p.purchasingInfoId,
        supplierId: p.supplierId,
        rowVersion: p.form.rowVersion,
        rows: this.breaks.getRawValue(),
      })
      .subscribe({
        next: () => this.loadBreaks(p.purchasingInfoId),
        error: (e) => this.failSave(e),
      });
  }
  protected loadExceptions(type?: number, page = 1) {
    this.start();
    this.api.exceptions(type, page).subscribe({
      next: (x) => {
        this.exceptionPage.set(x);
        this.loading.set(false);
      },
      error: (e) => this.fail(e),
    });
  }
  protected previewFile(files: FileList | null, format: 'csv' | 'excel') {
    const p = this.page(),
      file = files?.item(0);
    if (!p?.selectedSupplierId || !file) return;
    this.saving.set(true);
    this.api.preview(p.selectedSupplierId, file, format).subscribe({
      next: (x) => {
        this.preview.set(x);
        this.saving.set(false);
      },
      error: (e) => this.failSave(e),
    });
  }
  protected importPreview() {
    const p = this.preview();
    if (!p?.canImport) return;
    this.saving.set(true);
    this.api.import(p).subscribe({
      next: () => {
        this.preview.set(null);
        this.success.set(this.i18n.text('imported'));
        this.loadMain(p.supplierId);
      },
      error: (e) => this.failSave(e),
    });
  }
  protected download(format: 'csv' | 'excel') {
    const id = this.page()?.selectedSupplierId;
    if (!id) return;
    this.api.download(id, format).subscribe({
      next: (blob) => {
        const url = URL.createObjectURL(blob),
          a = document.createElement('a');
        a.href = url;
        a.download = `supplier-${id}-purchasing.${format === 'csv' ? 'csv' : 'xlsx'}`;
        a.click();
        URL.revokeObjectURL(url);
      },
      error: (e) => this.failSave(e),
    });
  }
  protected retry() {
    this.resolve();
  }
  private resolve() {
    const path = this.route.snapshot.routeConfig?.path ?? '',
      id = Number(this.route.snapshot.paramMap.get('id'));
    if (path.endsWith('price-breaks')) {
      this.mode.set('breaks');
      this.loadBreaks(id);
    } else if (path.endsWith('history')) {
      this.mode.set('history');
      this.start();
      this.api.history(id).subscribe({
        next: (x) => {
          this.historyPage.set(x);
          this.loading.set(false);
        },
        error: (e) => this.fail(e),
      });
    } else if (path.endsWith('exceptions')) {
      this.mode.set('exceptions');
      this.loadExceptions();
    } else {
      this.mode.set('main');
      this.loadMain(Number(this.route.snapshot.queryParamMap.get('supplierId')) || undefined);
    }
  }
  private loadMain(id?: number) {
    this.start();
    this.api.page(id).subscribe({
      next: (p) => {
        this.page.set(p);
        this.rows.clear();
        for (const r of p.bulkForm.rows)
          this.rows.push(
            this.fb.nonNullable.group({
              selected: [false],
              id: [r.id],
              productId: [r.productId],
              purchaseMeasurementUnitId: [r.purchaseMeasurementUnitId, Validators.min(1)],
              supplierProductCode: [
                r.supplierProductCode,
                [Validators.required, Validators.maxLength(100)],
              ],
              supplierDescription: [r.supplierDescription ?? '', Validators.maxLength(300)],
              minimumOrderQuantity: [r.minimumOrderQuantity, Validators.min(0.000001)],
              orderMultiple: [r.orderMultiple, Validators.min(0.000001)],
              leadTimeDays: [r.leadTimeDays, [Validators.min(0), Validators.max(3650)]],
              unitPrice: [r.unitPrice, Validators.min(0)],
              currencyCode: [p.baseCurrency],
              validFrom: [r.validFrom, Validators.required],
              validTo: [r.validTo ?? ''],
              isPreferred: [r.isPreferred],
              priority: [r.priority, [Validators.min(0), Validators.max(9999)]],
              isActive: [r.isActive],
              rowVersion: [r.rowVersion],
            }),
          );
        this.rows.markAsPristine();
        this.dirty();
        this.saving.set(false);
        this.loading.set(false);
      },
      error: (e) => this.fail(e),
    });
  }
  private loadBreaks(id: number) {
    this.start();
    this.api.breaks(id).subscribe({
      next: (p) => {
        this.breakPage.set(p);
        this.breaks.clear();
        for (const r of p.form.rows)
          this.breaks.push(
            this.fb.nonNullable.group({
              selected: [true],
              minimumQuantity: [r.minimumQuantity, Validators.min(p.minimumOrderQuantity)],
              unitPrice: [r.unitPrice, Validators.min(0)],
              discountPercent: [r.discountPercent, [Validators.min(0), Validators.max(100)]],
            }),
          );
        this.breaks.markAsPristine();
        this.dirty();
        this.saving.set(false);
        this.loading.set(false);
      },
      error: (e) => this.fail(e),
    });
  }
  private start() {
    this.loading.set(true);
    this.loadingProblem.set(null);
    this.problem.set('');
  }
  private failSave(e: unknown) {
    this.saving.set(false);
    const p = this.errors.normalize(e);
    this.problem.set(p.detail ?? p.title ?? this.i18n.text('error'));
  }
  private fail(e: unknown) {
    this.loadingProblem.set(this.errors.normalize(e));
    this.loading.set(false);
  }
  private dirty() {
    const t = this.workspace.activeTab();
    if (t) this.workspace.markDirty(t.id, this.rows.dirty || this.breaks.dirty);
  }
}
