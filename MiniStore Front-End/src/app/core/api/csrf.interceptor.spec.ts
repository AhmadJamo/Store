import { HttpRequest, HttpResponse } from '@angular/common/http';
import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { ApiSecurityService } from './api-security.service';
import { csrfInterceptor } from './csrf.interceptor';

describe('csrfInterceptor', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        {
          provide: ApiSecurityService,
          useValue: { token: { token: 'request-token', headerName: 'X-CSRF-TOKEN' } }
        }
      ]
    });
  });

  it('adds the server-issued token to unsafe internal API requests', () => {
    let forwarded: HttpRequest<unknown> | undefined;
    const request = new HttpRequest('POST', '/api/v1/security/mutation-probe', {});

    TestBed.runInInjectionContext(() =>
      csrfInterceptor(request, (nextRequest) => {
        forwarded = nextRequest;
        return of(new HttpResponse({ status: 200 }));
      }).subscribe()
    );

    expect(forwarded?.headers.get('X-CSRF-TOKEN')).toBe('request-token');
  });

  it('does not add the token to safe requests', () => {
    let forwarded: HttpRequest<unknown> | undefined;
    const request = new HttpRequest('GET', '/api/v1/session');

    TestBed.runInInjectionContext(() =>
      csrfInterceptor(request, (nextRequest) => {
        forwarded = nextRequest;
        return of(new HttpResponse({ status: 200 }));
      }).subscribe()
    );

    expect(forwarded?.headers.has('X-CSRF-TOKEN')).toBe(false);
  });
});
