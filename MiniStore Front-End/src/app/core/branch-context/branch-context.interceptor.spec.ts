import { HttpRequest, HttpResponse } from '@angular/common/http';
import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { branchContextInterceptor } from './branch-context.interceptor';
import { BranchContextService } from './branch-context.service';

describe('branchContextInterceptor', () => {
  it('adds only a validated selected branch to internal API requests', () => {
    TestBed.configureTestingModule({ providers: [{ provide: BranchContextService, useValue: { selectedId: () => 4 } }] });
    let forwarded: HttpRequest<unknown> | undefined;
    TestBed.runInInjectionContext(() => branchContextInterceptor(
      new HttpRequest('GET', '/api/v1/products'),
      request => { forwarded = request; return of(new HttpResponse({ status: 200 })); },
    ).subscribe());
    expect(forwarded?.headers.get('X-MiniStore-Branch-Id')).toBe('4');
  });

  it('does not disclose branch context to external requests', () => {
    TestBed.configureTestingModule({ providers: [{ provide: BranchContextService, useValue: { selectedId: () => 4 } }] });
    let forwarded: HttpRequest<unknown> | undefined;
    TestBed.runInInjectionContext(() => branchContextInterceptor(
      new HttpRequest('GET', 'https://example.test/data'),
      request => { forwarded = request; return of(new HttpResponse({ status: 200 })); },
    ).subscribe());
    expect(forwarded?.headers.has('X-MiniStore-Branch-Id')).toBe(false);
  });
});
