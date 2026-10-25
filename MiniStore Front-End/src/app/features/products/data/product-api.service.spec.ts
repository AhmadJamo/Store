import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { ProductApiService } from './product-api.service';

describe('ProductApiService', () => {
  it('sends bounded paging, search and status filters', () => { TestBed.configureTestingModule({ providers: [ProductApiService, provideHttpClient(), provideHttpClientTesting()] }); const service = TestBed.inject(ProductApiService); const http = TestBed.inject(HttpTestingController); service.getPage({ page: 2, search: 'coffee', isActive: true }).subscribe(); const request = http.expectOne(value => value.url === '/api/v1/products'); expect(request.request.params.get('page')).toBe('2'); expect(request.request.params.get('pageSize')).toBe('25'); expect(request.request.params.get('search')).toBe('coffee'); expect(request.request.params.get('isActive')).toBe('true'); request.flush({ query: {}, items: [], totalCount: 0, totalPages: 1, categories: [] }); http.verify(); });
});
