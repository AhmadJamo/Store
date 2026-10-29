import { HttpClient } from '@angular/common/http';
import { of } from 'rxjs';
import { SalesQuotationTemplatesApiService } from './sales-quotation-templates-api.service';
describe('SalesQuotationTemplatesApiService', () => {
  it('keeps lifecycle commands explicit', () => {
    const http = { post: vi.fn(() => of(null)) } as unknown as HttpClient;
    const api = new SalesQuotationTemplatesApiService(http);
    for (const action of ['activate', 'archive', 'default'] as const)
      api.command(8, action, 'AQID').subscribe();
    expect((http.post as any).mock.calls.map((x: any[]) => x[0])).toEqual([
      '/api/v1/sales-quotation-templates/8/activate',
      '/api/v1/sales-quotation-templates/8/archive',
      '/api/v1/sales-quotation-templates/8/default',
    ]);
  });
});
