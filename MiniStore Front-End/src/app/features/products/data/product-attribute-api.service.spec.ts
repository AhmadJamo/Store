import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { ProductAttributeApiService } from './product-attribute-api.service';

describe('ProductAttributeApiService', () => {
  let api: ProductAttributeApiService; let http: HttpTestingController;
  beforeEach(() => { TestBed.configureTestingModule({ providers: [ProductAttributeApiService, provideHttpClient(), provideHttpClientTesting()] }); api = TestBed.inject(ProductAttributeApiService); http = TestBed.inject(HttpTestingController); });
  afterEach(() => http.verify());
  it('uses the complete attribute operations', () => {
    const command = { name: 'Color', code: 'COLOR', dataType: 4 as const, isRequired: true, isVariantDefining: true, displayOrder: 1, productCategoryIds: [2], optionsText: 'Red|RED' };
    api.getPage().subscribe(); expect(http.expectOne('/api/v1/product-attributes').request.method).toBe('GET');
    api.create(command).subscribe(); const create = http.expectOne('/api/v1/product-attributes'); expect(create.request.method).toBe('POST'); expect(create.request.body.code).toBe('COLOR');
    api.setStatus(3, false).subscribe(); const status = http.expectOne('/api/v1/product-attributes/3/status'); expect(status.request.method).toBe('PUT'); status.flush({});
    api.getValues(9).subscribe(); expect(http.expectOne('/api/v1/product-attributes/products/9/values').request.method).toBe('GET');
    api.saveValues(9, [{ definitionId: 3, value: '7' }]).subscribe(); const save = http.expectOne('/api/v1/product-attributes/products/9/values'); expect(save.request.method).toBe('PUT'); expect(save.request.body.values[0].value).toBe('7'); save.flush({});
  });
});
