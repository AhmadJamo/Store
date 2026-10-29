import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { PlatformApiService } from './platform-api.service';

describe('PlatformApiService', () => {
  it('uses the isolated platform session endpoints', () => {
    TestBed.configureTestingModule({
      providers: [PlatformApiService, provideHttpClient(), provideHttpClientTesting()],
    });
    const api = TestBed.inject(PlatformApiService);
    const http = TestBed.inject(HttpTestingController);
    api.login({ username: 'operator', password: 'secret' }).subscribe();
    expect(http.expectOne('/api/v1/platform/login').request.method).toBe('POST');
    api.session().subscribe();
    expect(http.expectOne('/api/v1/platform/session').request.method).toBe('GET');
    api.overview().subscribe();
    expect(http.expectOne('/api/v1/platform/overview').request.method).toBe('GET');
    api.logout().subscribe();
    expect(http.expectOne('/api/v1/platform/logout').request.method).toBe('POST');
    api.management().subscribe();
    expect(http.expectOne('/api/v1/platform/management').request.method).toBe('GET');
    api.updateCompany({tenantId:2,planId:1,status:2,periodEnd:'2026-12-31',companyIsActive:true}).subscribe();
    expect(http.expectOne('/api/v1/platform/management/companies/2').request.method).toBe('PUT');
    api.createPlan({} as never).subscribe();
    expect(http.expectOne('/api/v1/platform/management/plans').request.method).toBe('POST');
    api.createPromotion({} as never).subscribe();
    expect(http.expectOne('/api/v1/platform/management/promotions').request.method).toBe('POST');
    api.confirmPayment('abc','BANK-1').subscribe();
    expect(http.expectOne('/api/v1/platform/management/payments/abc/confirm').request.method).toBe('POST');
    http.verify();
  });
});
