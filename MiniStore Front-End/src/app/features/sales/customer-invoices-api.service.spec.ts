import { HttpClient } from '@angular/common/http';
import { of } from 'rxjs';
import { CustomerInvoicesApiService } from './customer-invoices-api.service';
describe('CustomerInvoicesApiService', () => {
  it('keeps invoice lifecycle mutations independent', () => {
    const http = { post: vi.fn(() => of(null)) } as unknown as HttpClient;
    const api = new CustomerInvoicesApiService(http);
    api.command(5, 'approve', 'AQID').subscribe();
    api.command(5, 'post', 'AQID').subscribe();
    api.command(5, 'cancel', 'AQID', 'Changed').subscribe();
    api.reverse(5, 'AQID', '2026-10-29', 'Correction').subscribe();
    expect((http.post as any).mock.calls.map((x: any[]) => x[0])).toEqual([
      '/api/v1/customer-invoices/5/approve',
      '/api/v1/customer-invoices/5/post',
      '/api/v1/customer-invoices/5/cancel',
      '/api/v1/customer-invoices/5/reverse',
    ]);
  });
});
