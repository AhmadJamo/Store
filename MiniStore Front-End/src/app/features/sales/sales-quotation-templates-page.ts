import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormArray, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { ApiSessionService } from '../../core/api/api-session.service';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { SalesQuotationTemplatesApiService } from './sales-quotation-templates-api.service';
import { SalesQuotationTemplatesLocalizationService } from './sales-quotation-templates-localization.service';
import {
  CreateTemplate,
  CreateTemplateItem,
  TemplateDetails,
  TemplatePage,
  TemplateSummary,
} from './sales-quotation-templates.models';
@Component({
  selector: 'app-sales-quotation-templates-page',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    ApiRecoveryComponent,
    FeedbackStateComponent,
    PageHeaderComponent,
  ],
  templateUrl: './sales-quotation-templates-page.html',
  styleUrl: './sales-quotation-templates-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SalesQuotationTemplatesPageComponent {
  protected readonly i18n = inject(SalesQuotationTemplatesLocalizationService);
  protected readonly session = inject(ApiSessionService);
  private readonly api = inject(SalesQuotationTemplatesApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly workspace = inject(WorkspaceService);
  private readonly destroy = inject(DestroyRef);
  private readonly fb = inject(FormBuilder);
  protected readonly rows = signal<TemplateSummary[]>([]);
  protected readonly item = signal<TemplateDetails | null>(null);
  protected readonly page = signal<TemplatePage | null>(null);
  protected readonly loading = signal(true);
  protected readonly saving = signal(false);
  protected readonly failure = signal<NormalizedApiProblem | null>(null);
  protected readonly problem = signal<NormalizedApiProblem | null>(null);
  protected readonly search = this.fb.nonNullable.control('');
  protected readonly status = this.fb.nonNullable.control(0);
  protected readonly form = this.fb.group({
    sourceTemplateId: this.fb.control<number | null>(null),
    code: this.fb.nonNullable.control('QUOTE', [Validators.required, Validators.maxLength(50)]),
    version: this.fb.nonNullable.control(1, [Validators.min(1), Validators.max(9999)]),
    name: this.fb.nonNullable.control('', [Validators.required, Validators.maxLength(150)]),
    description: this.fb.control<string | null>(null, Validators.maxLength(1000)),
    category: this.fb.control<string | null>(null, Validators.maxLength(100)),
    tags: this.fb.control<string | null>(null, Validators.maxLength(500)),
    validityDays: this.fb.nonNullable.control(30, [Validators.min(0), Validators.max(3650)]),
    channel: this.fb.nonNullable.control(1),
    salesPriceListId: this.fb.control<number | null>(null),
    documentDesignId: this.fb.control<number | null>(null),
    allowAdditionalLines: this.fb.nonNullable.control(true),
    isDefault: this.fb.nonNullable.control(false),
    introduction: this.fb.control<string | null>(null, Validators.maxLength(4000)),
    termsAndConditions: this.fb.control<string | null>(null, Validators.maxLength(8000)),
    footerNotes: this.fb.control<string | null>(null, Validators.maxLength(4000)),
    items: this.fb.array([]),
  });
  private readonly id = Number(this.route.snapshot.paramMap.get('id'));
  private readonly isNew =
    this.route.snapshot.routeConfig?.path === 'sales/quotation-templates/new';
  constructor() {
    this.form.valueChanges.pipe(takeUntilDestroyed(this.destroy)).subscribe(() => this.dirty());
    this.load();
  }
  get items() {
    return this.form.controls.items as FormArray;
  }
  canDeactivate() {
    return !this.form.dirty || globalThis.confirm(this.i18n.text('unsaved'));
  }
  protected load() {
    this.loading.set(true);
    this.failure.set(null);
    this.problem.set(null);
    if (this.id)
      this.api.get(this.id).subscribe({
        next: (x) => {
          this.item.set(x);
          this.done();
        },
        error: (e) => this.failLoad(e),
      });
    else if (this.isNew) {
      const source = Number(this.route.snapshot.queryParamMap.get('sourceTemplateId')) || undefined;
      this.api.createForm(source).subscribe({
        next: (p) => {
          this.page.set(p);
          this.items.clear({ emitEvent: false });
          const rows = p.form.items.length ? p.form.items : [this.blankItem(p)];
          for (const row of rows) this.items.push(this.itemGroup(row), { emitEvent: false });
          this.form.patchValue({ ...p.form, items: undefined } as any, { emitEvent: false });
          this.form.markAsPristine();
          this.done();
          this.dirty();
        },
        error: (e) => this.failLoad(e),
      });
    } else
      this.api
        .list(this.status.value || undefined, this.search.value.trim() || undefined)
        .subscribe({
          next: (x) => {
            this.rows.set(x);
            this.done();
          },
          error: (e) => this.failLoad(e),
        });
  }
  protected addItem() {
    const page = this.page();
    if (page) this.items.push(this.itemGroup(this.blankItem(page)));
  }
  protected removeItem(index: number) {
    this.items.removeAt(index);
  }
  protected create() {
    if (this.form.invalid || !this.items.length || this.saving()) {
      this.form.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    this.api.create(this.form.getRawValue() as CreateTemplate).subscribe({
      next: (x) => {
        this.form.markAsPristine();
        this.dirty();
        void this.router.navigate(['/sales/quotation-templates', x.id]);
      },
      error: (e) => this.failAction(e),
    });
  }
  protected command(action: 'activate' | 'archive' | 'default') {
    const x = this.item();
    if (!x || this.saving()) return;
    this.saving.set(true);
    this.api
      .command(x.id, action, x.rowVersion)
      .subscribe({ next: () => this.load(), error: (e) => this.failAction(e) });
  }
  private itemGroup(x: CreateTemplateItem) {
    return this.fb.group({
      sequence: [x.sequence, [Validators.min(1), Validators.max(9999)]],
      sectionTitle: [x.sectionTitle, [Validators.required, Validators.maxLength(150)]],
      productId: [x.productId, [Validators.min(1)]],
      measurementUnitId: [x.measurementUnitId, [Validators.min(1)]],
      defaultQuantity: [x.defaultQuantity, [Validators.min(0.000001)]],
      minimumQuantity: [x.minimumQuantity, [Validators.min(0.000001)]],
      maximumQuantity: [x.maximumQuantity, [Validators.min(0.000001)]],
      allowQuantityChange: x.allowQuantityChange,
      taxRateId: x.taxRateId,
      itemType: x.itemType,
      isDefaultSelected: x.isDefaultSelected,
      optionGroupCode: [x.optionGroupCode, Validators.maxLength(50)],
      optionGroupName: [x.optionGroupName, Validators.maxLength(150)],
      groupMinimumSelections: [x.groupMinimumSelections, [Validators.min(0), Validators.max(100)]],
      groupMaximumSelections: [x.groupMaximumSelections, [Validators.min(0), Validators.max(100)]],
      notes: [x.notes, Validators.maxLength(500)],
    });
  }
  private blankItem(page: TemplatePage): CreateTemplateItem {
    return {
      sequence: this.items.length + 1,
      sectionTitle: this.i18n.text('general'),
      productId: page.products[0]?.id ?? 0,
      measurementUnitId: page.units[0]?.id ?? 0,
      defaultQuantity: 1,
      minimumQuantity: 1,
      maximumQuantity: 1,
      allowQuantityChange: false,
      taxRateId: null,
      itemType: 1,
      isDefaultSelected: true,
      optionGroupCode: null,
      optionGroupName: null,
      groupMinimumSelections: 0,
      groupMaximumSelections: 0,
      notes: null,
    };
  }
  private done() {
    this.loading.set(false);
    this.saving.set(false);
  }
  private failLoad(e: unknown) {
    this.failure.set(this.errors.normalize(e));
    this.loading.set(false);
  }
  private failAction(e: unknown) {
    this.problem.set(this.errors.normalize(e));
    this.saving.set(false);
  }
  private dirty() {
    const t = this.workspace.activeTab();
    if (t) this.workspace.markDirty(t.id, this.form.dirty);
  }
}
