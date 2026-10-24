import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { DocumentEvidenceComponent } from './document-evidence';

describe('DocumentEvidenceComponent', () => {
  it('renders authoritative journey, relations, and number parts', async () => {
    await TestBed.configureTestingModule({ imports: [DocumentEvidenceComponent], providers: [provideRouter([])] }).compileComponents();
    const fixture = TestBed.createComponent(DocumentEvidenceComponent);
    fixture.componentRef.setInput('evidence', {
      kind: 'customer-invoice', id: 3, number: 'INV-3', party: 'Acme', status: 'Posted', documentDate: '2026-10-24', currencyCode: 'JOD', totalAmount: 11.6,
      journey: [{ eventKey: 'Post', fromStatus: 'Approved', toStatus: 'Posted', actorUserId: 'user', occurredAtUtc: '2026-10-24T10:00:00Z' }],
      relatedDocuments: [{ kind: 'sales-order', id: 2, number: 'SO-2', relationshipKey: 'invoiceFromOrder', canOpen: true }],
      explainedNumbers: [{ numberKey: 'documentTotal', value: 11.6, currencyCode: 'JOD', decimalPlaces: 2, formulaKey: 'netPlusTax', parts: [{ partKey: 'netAmount', amount: 10, operation: 'add', sourceKey: 'invoiceSnapshot' }] }],
    });
    fixture.detectChanges();
    expect(fixture.nativeElement.textContent).toContain('رحلة المستند');
    expect(fixture.nativeElement.textContent).toContain('JOD');
    expect(fixture.nativeElement.querySelector('a')?.getAttribute('href')).toBe('/documents/sales-order/2');
  });
});
