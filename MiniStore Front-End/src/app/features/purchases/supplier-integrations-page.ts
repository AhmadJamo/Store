import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { SupplierIntegrationsApiService } from './supplier-integrations-api.service';
import { SupplierIntegrationsLocalizationService } from './supplier-integrations-localization.service';
@Component({
  selector: 'app-supplier-integrations-page',
  imports: [
    DatePipe,
    ReactiveFormsModule,
    RouterLink,
    ApiRecoveryComponent,
    FeedbackStateComponent,
    PageHeaderComponent,
  ],
  templateUrl: './supplier-integrations-page.html',
  styleUrl: './supplier-integrations-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SupplierIntegrationsPageComponent {
  protected readonly i18n = inject(SupplierIntegrationsLocalizationService);
  protected readonly session = inject(ApiSessionService);
  private readonly api = inject(SupplierIntegrationsApiService);
  private readonly errors = inject(ApiErrorService);
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  protected readonly page = signal<any>(null);
  protected readonly detail = signal<any>(null);
  protected readonly secret = signal('');
  protected readonly portalSecret = signal('');
  protected readonly accesses = signal<any[]>([]);
  protected readonly destinations = signal<any[]>([]);
  protected readonly loading = signal(true);
  protected readonly failure = signal<any>(null);
  protected readonly saving = signal(false);
  protected readonly scopes = [
    'rfq.read',
    'quotation.write',
    'po.read',
    'confirmation.write',
    'delivery.write',
    'invoice.write',
  ];
  protected readonly documentType = Number(
    this.route.snapshot.queryParamMap.get('documentType') ?? 0,
  );
  protected readonly documentId = Number(this.route.snapshot.queryParamMap.get('documentId') ?? 0);
  protected readonly supplierId = Number(this.route.snapshot.queryParamMap.get('supplierId') ?? 0);
  protected readonly supplierName = this.route.snapshot.queryParamMap.get('supplierName') ?? '';
  protected readonly documentNumber = this.route.snapshot.queryParamMap.get('documentNumber') ?? '';
  protected readonly contextForm = this.fb.nonNullable.group({
    validDays: [14, [Validators.min(1), Validators.max(90)]],
    credentialId: [0],
  });
  protected readonly form = this.fb.nonNullable.group({
    supplierId: [0, Validators.min(1)],
    name: ['', Validators.required],
    callbackUrl: [''],
    callbackBearerToken: [''],
    expiresAtUtc: [''],
    scopes: this.fb.nonNullable.control<string[]>([]),
  });
  constructor() {
    this.load();
    if (this.documentType && this.documentId) this.loadContext();
  }
  protected load() {
    this.loading.set(true);
    const id = Number(this.route.snapshot.paramMap.get('id') ?? 0);
    (id ? this.api.submission(id) : this.api.get()).subscribe({
      next: (x) => {
        id ? this.detail.set(x) : this.page.set(x);
        this.loading.set(false);
      },
      error: (e) => {
        this.failure.set(this.errors.normalize(e));
        this.loading.set(false);
      },
    });
  }
  protected toggle(scope: string, on: boolean) {
    const values = this.form.controls.scopes.value;
    this.form.controls.scopes.setValue(on ? [...values, scope] : values.filter((x) => x !== scope));
  }
  protected create() {
    if (this.form.invalid) return;
    this.saving.set(true);
    this.api.create(this.form.getRawValue()).subscribe({
      next: (x) => {
        this.secret.set(x.apiKey);
        this.saving.set(false);
        this.form.reset({
          supplierId: 0,
          name: '',
          callbackUrl: '',
          callbackBearerToken: '',
          expiresAtUtc: '',
          scopes: [],
        });
        this.load();
      },
      error: (e) => {
        this.failure.set(this.errors.normalize(e));
        this.saving.set(false);
      },
    });
  }
  protected deactivate(x: any) {
    this.api.deactivate(x.id, x.rowVersion).subscribe({
      next: () => this.load(),
      error: (e) => this.failure.set(this.errors.normalize(e)),
    });
  }
  protected retry(id: number) {
    this.api.retry(id).subscribe({
      next: () => this.load(),
      error: (e) => this.failure.set(this.errors.normalize(e)),
    });
  }
  protected download(id: number) {
    return this.api.download(id);
  }
  protected loadContext() {
    this.api
      .accesses(this.documentType, this.documentId)
      .subscribe({
        next: (x) => this.accesses.set(x),
        error: (e) => this.failure.set(this.errors.normalize(e)),
      });
    if (this.documentType === 2)
      this.api
        .credentials(this.supplierId, this.documentId)
        .subscribe({
          next: (x) => this.destinations.set(x),
          error: (e) => this.failure.set(this.errors.normalize(e)),
        });
  }
  protected createAccess() {
    this.api
      .createAccess({
        documentType: this.documentType,
        documentId: this.documentId,
        supplierId: this.supplierId,
        validDays: this.contextForm.controls.validDays.value,
      })
      .subscribe({
        next: (x) => {
          this.portalSecret.set(x.portalUrl);
          this.loadContext();
        },
        error: (e) => this.failure.set(this.errors.normalize(e)),
      });
  }
  protected revoke(id: number) {
    this.api
      .revoke(id)
      .subscribe({
        next: () => this.loadContext(),
        error: (e) => this.failure.set(this.errors.normalize(e)),
      });
  }
  protected queue() {
    const credentialId = this.contextForm.controls.credentialId.value;
    if (!credentialId) return;
    this.api.queue({ purchaseOrderId: this.documentId, credentialId }).subscribe({
      next: () => {
        this.load();
        this.loadContext();
      },
      error: (e) => this.failure.set(this.errors.normalize(e)),
    });
  }
}
