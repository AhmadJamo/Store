import { ChangeDetectionStrategy, Component, inject, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { LocalizationService, MessageKey } from '../../../core/localization/localization.service';
import { DocumentEvidence } from '../../../features/document-evidence/data/document-evidence.models';

@Component({ selector: 'app-document-evidence', imports: [RouterLink], templateUrl: './document-evidence.html', styleUrl: './document-evidence.scss', changeDetection: ChangeDetectionStrategy.OnPush })
export class DocumentEvidenceComponent {
  protected readonly i18n = inject(LocalizationService);
  readonly evidence = input.required<DocumentEvidence>();
  protected routeFor(kind: string, id: number): string | null { return kind === 'sales-order' || kind === 'customer-invoice' ? `/documents/${kind}/${id}` : null; }
  protected number(value: number, digits: number): string { return new Intl.NumberFormat(this.i18n.locale(), { minimumFractionDigits: digits, maximumFractionDigits: digits }).format(value); }
  protected date(value: string): string { return new Intl.DateTimeFormat(this.i18n.locale(), { dateStyle: 'medium', timeStyle: 'short' }).format(new Date(value)); }
  protected label(value: string): string {
    const keys: Record<string, MessageKey> = {
      Draft: 'evidenceDraft', Confirmed: 'evidenceConfirmed', Cancelled: 'evidenceCancelled', Approved: 'evidenceApproved', Posted: 'evidencePosted', Reversed: 'evidenceReversed',
      Created: 'evidenceCreated', Create: 'evidenceCreated', Confirm: 'evidenceConfirm', Cancel: 'evidenceCancel', Approve: 'evidenceApprove', Post: 'evidencePost', Reverse: 'evidenceReverse',
      invoiceFromOrder: 'invoiceFromOrder', orderFromQuotation: 'orderFromQuotation', invoiceFromDelivery: 'invoiceFromDelivery',
      documentTotal: 'documentTotal', outstandingAmount: 'outstandingAmount', netAmount: 'netAmount', taxAmount: 'taxAmount', paidAmount: 'paidAmount', creditedAmount: 'creditedAmount',
      netPlusTax: 'netPlusTax', totalMinusSettlements: 'totalMinusSettlements', orderSnapshot: 'orderSnapshot', invoiceSnapshot: 'invoiceSnapshot', postedPayments: 'postedPayments', postedCredits: 'postedCredits',
      add: 'operationAdd', subtract: 'operationSubtract', start: 'operationStart',
    };
    return keys[value] ? this.i18n.text(keys[value]) : value;
  }
}
