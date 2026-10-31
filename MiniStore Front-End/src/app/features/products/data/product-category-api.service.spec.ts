import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { ProductCategoryApiService } from './product-category-api.service';

describe('ProductCategoryApiService', () => {
  beforeEach(() => TestBed.configureTestingModule({ providers: [ProductCategoryApiService, provideHttpClient(), provideHttpClientTesting()] }));

  it('uses the versioned category read and mutation contracts', () => {
    const api = TestBed.inject(ProductCategoryApiService); const http = TestBed.inject(HttpTestingController);
    api.getAll().subscribe(); const list = http.expectOne('/api/v1/product-categories'); expect(list.request.method).toBe('GET'); list.flush([]);
    api.create({ name: 'Beverages', code: 'BEVERAGES' }).subscribe(); const create = http.expectOne('/api/v1/product-categories'); expect(create.request.method).toBe('POST'); expect(create.request.body.code).toBe('BEVERAGES'); create.flush({ id: 4 });
    api.setStatus(4, false).subscribe(); const status = http.expectOne('/api/v1/product-categories/4/status'); expect(status.request.method).toBe('PUT'); expect(status.request.body).toEqual({ isActive: false }); status.flush({ id: 4, isActive: false });
    http.verify();
  });
});
