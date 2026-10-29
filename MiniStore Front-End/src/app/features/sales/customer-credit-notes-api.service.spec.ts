import { HttpClient } from '@angular/common/http';
import { of } from 'rxjs';
import { CustomerCreditNotesApiService } from './customer-credit-notes-api.service';
describe('CustomerCreditNotesApiService', () => {
  it('posts through the dedicated idempotent endpoint', () => {
    const http = { post: vi.fn(() => of({ id: 4 })) } as unknown as HttpClient;
    const api = new CustomerCreditNotesApiService(http);
    api
      .create({
        customerInvoiceId: 3,
        requestKey: 'key',
        creditDate: '2026-10-29',
        reason: 'Return',
        lines: [],
      })
      .subscribe();
    expect(http.post).toHaveBeenCalledWith(
      '/api/v1/customer-credit-notes',
      expect.objectContaining({ requestKey: 'key' }),
    );
  });
});
