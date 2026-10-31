import { HttpClient } from '@angular/common/http';
import { of } from 'rxjs';
import { CustomerPaymentsApiService } from './customer-payments-api.service';

describe('CustomerPaymentsApiService', () => {
  it('keeps create/post and reversal explicit', () => {
    const http = { post: vi.fn(() => of(null)) } as unknown as HttpClient;
    const api = new CustomerPaymentsApiService(http);
    api
      .create({
        sourceCustomerInvoiceId: 4,
        paymentMethodId: 2,
        paymentDate: '2026-10-29',
        externalReference: null,
        notes: null,
        lines: [{ customerInvoiceId: 4, amount: 10 }],
      })
      .subscribe();
    api.reverse(8, 'AQID', '2026-10-29', 'Correction').subscribe();
    expect((http.post as any).mock.calls.map((x: any[]) => x[0])).toEqual([
      '/api/v1/customer-payments',
      '/api/v1/customer-payments/8/reverse',
    ]);
  });
});
