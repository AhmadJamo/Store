import { HttpClient } from '@angular/common/http';
import { of } from 'rxjs';
import { SalesApprovalRulesApiService } from './sales-approval-rules-api.service';
describe('SalesApprovalRulesApiService', () => {
  it('keeps create and activation mutations explicit', () => {
    const http = { post: vi.fn(() => of(null)) } as unknown as HttpClient;
    const api = new SalesApprovalRulesApiService(http);
    api
      .create({
        name: 'Limit',
        customerId: null,
        metric: 2,
        comparison: 1,
        threshold: 5,
        action: 2,
        priority: 100,
        effectiveFrom: null,
        effectiveTo: null,
        allowRequesterApproval: false,
        requireDistinctApprovers: true,
        approverRoles: ['Manager'],
      })
      .subscribe();
    api.setActive(4, false, 'AQID').subscribe();
    expect((http.post as any).mock.calls.map((x: any[]) => x[0])).toEqual([
      '/api/v1/sales-approval-rules',
      '/api/v1/sales-approval-rules/4/active',
    ]);
  });
});
