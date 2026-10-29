import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { SubscriptionApiService } from './subscription-api.service';

describe('SubscriptionApiService', () => {
  let api: SubscriptionApiService;
  let http: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [SubscriptionApiService, provideHttpClient(), provideHttpClientTesting()],
    });
    api = TestBed.inject(SubscriptionApiService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('reads the tenant subscription and creates checkout on the protected endpoint', () => {
    api.get().subscribe();
    expect(http.expectOne('/api/v1/subscription').request.method).toBe('GET');

    api.checkout({ planId: 2, cycle: 1, promotionCode: 'SAVE10' }).subscribe();
    const request = http.expectOne('/api/v1/subscription/checkout');
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual({ planId: 2, cycle: 1, promotionCode: 'SAVE10' });
  });
});
