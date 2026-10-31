import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { PublicPlansApiService } from './public-plans-api.service';

describe('PublicPlansApiService', () => {
  it('loads public plan cards from the anonymous versioned API', () => {
    TestBed.configureTestingModule({
      providers: [PublicPlansApiService, provideHttpClient(), provideHttpClientTesting()],
    });
    const api = TestBed.inject(PublicPlansApiService);
    const http = TestBed.inject(HttpTestingController);
    api.getPlans().subscribe();
    expect(http.expectOne('/api/v1/public/plans').request.method).toBe('GET');
    http.verify();
  });
});
