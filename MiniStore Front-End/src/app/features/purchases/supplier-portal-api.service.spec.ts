import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { SupplierPortalApiService } from './supplier-portal-api.service';
describe('SupplierPortalApiService', () => {
  it('covers public token read and three submissions', () => {
    TestBed.configureTestingModule({
      providers: [SupplierPortalApiService, provideHttpClient(), provideHttpClientTesting()],
    });
    const a = TestBed.inject(SupplierPortalApiService),
      h = TestBed.inject(HttpTestingController);
    a.get('t').subscribe();
    expect(h.expectOne('/api/public/supplier-portal/t').request.method).toBe('GET');
    const quotation = {
      token: 't',
      idempotencyKey: 'q',
      currencyCode: 'JOD',
      supplierReference: '',
      quotationDate: '2026-10-28',
      validUntilDate: '2026-11-11',
      leadTimeDays: 0,
      paymentTerms: '',
      notes: '',
      lines: [],
    };
    const confirmation = {
      token: 't',
      idempotencyKey: 'c',
      confirmedExpectedDate: '2026-10-29',
      confirmationReference: 'C-1',
      note: '',
    };
    const delivery = {
      token: 't',
      idempotencyKey: 'd',
      deliveryDate: '2026-10-29',
      reference: 'D-1',
      carrier: '',
      trackingNumber: '',
      note: '',
    };
    for (const [call, url] of [
      [() => a.quotation(quotation), '/api/public/supplier-portal/quotation'],
      [() => a.confirm(confirmation), '/api/public/supplier-portal/order-confirmation'],
      [() => a.delivery(delivery), '/api/public/supplier-portal/delivery-notice'],
    ] as const) {
      call().subscribe();
      expect(h.expectOne(url).request.method).toBe('POST');
    }
    h.verify();
  });
});
