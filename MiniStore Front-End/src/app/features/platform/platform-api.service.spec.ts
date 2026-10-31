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
    const query={page:1,pageSize:25};
    api.auditEvents(query).subscribe();
    expect(http.expectOne('/api/v1/platform/audit-events?page=1&pageSize=25').request.method).toBe('GET');
    api.exportAudit(query,'InternalReview').subscribe();
    expect(http.expectOne('/api/v1/platform/audit-events/export').request.method).toBe('POST');
    api.auditRetention().subscribe();
    expect(http.expectOne('/api/v1/platform/audit-retention').request.method).toBe('GET');
    api.createAuditHold({scope:2,tenantId:null,fromUtc:'2026-01-01',toUtc:'2026-01-02',reason:'review'}).subscribe();
    expect(http.expectOne('/api/v1/platform/audit-retention/holds').request.method).toBe('POST');
    api.releaseAuditHold(4,'closed').subscribe();
    expect(http.expectOne('/api/v1/platform/audit-retention/holds/4/release').request.method).toBe('POST');
    api.createAuditCheckpoint().subscribe();
    expect(http.expectOne('/api/v1/platform/audit-retention/checkpoint').request.method).toBe('POST');
    api.runAuditRetention().subscribe();
    expect(http.expectOne('/api/v1/platform/audit-retention/run').request.method).toBe('POST');
    http.verify();
  });
});
