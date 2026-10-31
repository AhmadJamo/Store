import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { PurchaseAgreementApiService } from './purchase-agreement-api.service';
describe('PurchaseAgreementApiService', () => {
  let a: PurchaseAgreementApiService, h: HttpTestingController;
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [PurchaseAgreementApiService, provideHttpClient(), provideHttpClientTesting()],
    });
    a = TestBed.inject(PurchaseAgreementApiService);
    h = TestBed.inject(HttpTestingController);
  });
  afterEach(() => h.verify());
  it('uses every agreement and template endpoint', () => {
    a.getAll().subscribe();
    expect(h.expectOne('/api/v1/purchase-agreements').request.method).toBe('GET');
    a.get(2).subscribe();
    expect(h.expectOne('/api/v1/purchase-agreements/2').request.method).toBe('GET');
    a.getCreateForm(7).subscribe();
    expect(
      h.expectOne((x) => x.url === '/api/v1/purchase-agreements/create-form').request.method,
    ).toBe('GET');
    a.create({}).subscribe();
    expect(h.expectOne('/api/v1/purchase-agreements').request.method).toBe('POST');
    a.activate(2, 'AQID').subscribe();
    expect(h.expectOne('/api/v1/purchase-agreements/2/activate').request.method).toBe('POST');
    a.cancel(2, 'AQID', 'reason').subscribe();
    expect(h.expectOne('/api/v1/purchase-agreements/2/cancel').request.method).toBe('POST');
    a.release(2, {}).subscribe();
    expect(h.expectOne('/api/v1/purchase-agreements/2/release').request.method).toBe('POST');
    a.getTemplates().subscribe();
    expect(h.expectOne('/api/v1/purchase-templates').request.method).toBe('GET');
    a.createTemplate({}).subscribe();
    expect(h.expectOne('/api/v1/purchase-templates/from-order').request.method).toBe('POST');
    a.generate(3).subscribe();
    expect(h.expectOne('/api/v1/purchase-templates/3/generate').request.method).toBe('POST');
    a.setActive(3, false).subscribe();
    expect(h.expectOne('/api/v1/purchase-templates/3/active').request.method).toBe('POST');
  });
});
