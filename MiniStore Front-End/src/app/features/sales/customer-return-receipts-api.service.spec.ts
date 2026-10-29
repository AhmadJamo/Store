import { HttpClient } from '@angular/common/http';
import { of } from 'rxjs';
import { CustomerReturnReceiptsApiService } from './customer-return-receipts-api.service';

describe('CustomerReturnReceiptsApiService', () => {
  it('keeps inspection, disposition posting and cancellation independent', () => {
    const http = { post: vi.fn(() => of(null)) } as unknown as HttpClient;
    const api = new CustomerReturnReceiptsApiService(http);
    api.command(9, 'inspect', 'AQID').subscribe();
    api.command(9, 'post', 'AQID').subscribe();
    api.command(9, 'cancel', 'AQID', 'Changed').subscribe();
    expect((http.post as any).mock.calls.map((x: any[]) => x[0])).toEqual([
      '/api/v1/customer-return-receipts/9/inspect',
      '/api/v1/customer-return-receipts/9/post',
      '/api/v1/customer-return-receipts/9/cancel',
    ]);
  });
});
