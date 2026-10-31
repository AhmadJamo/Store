import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { ProductTemplateApiService } from './product-template-api.service';

describe('ProductTemplateApiService', () => {
  let api: ProductTemplateApiService; let http: HttpTestingController;
  beforeEach(() => { TestBed.configureTestingModule({ providers: [ProductTemplateApiService, provideHttpClient(), provideHttpClientTesting()] }); api = TestBed.inject(ProductTemplateApiService); http = TestBed.inject(HttpTestingController); });
  afterEach(() => http.verify());
  it('uses all current template operations', () => { api.getPage().subscribe(); expect(http.expectOne('/api/v1/product-templates').request.method).toBe('GET'); api.create({ name: 'Shirt', code: 'SHIRT', productCategoryId: 2 }).subscribe(); expect(http.expectOne('/api/v1/product-templates').request.method).toBe('POST'); api.assign({ productId: 4, productTemplateId: 3 }).subscribe(); expect(http.expectOne('/api/v1/product-templates/assignment').request.method).toBe('PUT'); const command = { productTemplateId: 3, sourceProductId: 4, optionIds: [8, 9] }; api.preview(command).subscribe(); expect(http.expectOne('/api/v1/product-templates/preview').request.method).toBe('POST'); api.generate(command).subscribe(); expect(http.expectOne('/api/v1/product-templates/generate').request.method).toBe('POST'); });
});
