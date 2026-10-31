import { HttpClient } from '@angular/common/http';
import { of } from 'rxjs';
import { CustomerReturnRequestsApiService } from './customer-return-requests-api.service';
describe('CustomerReturnRequestsApiService', () => {
  it('keeps lifecycle commands independent', () => {
    const http = { post: vi.fn(() => of(null)) } as unknown as HttpClient;
    const api = new CustomerReturnRequestsApiService(http);
    for (const action of ['submit', 'approve', 'reject', 'cancel'] as const)
      api.command(7, action, 'AQID', 'Reason').subscribe();
    expect((http.post as any).mock.calls.map((x: any[]) => x[0])).toEqual([
      '/api/v1/customer-return-requests/7/submit',
      '/api/v1/customer-return-requests/7/approve',
      '/api/v1/customer-return-requests/7/reject',
      '/api/v1/customer-return-requests/7/cancel',
    ]);
  });
});
