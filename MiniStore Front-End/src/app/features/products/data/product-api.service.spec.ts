import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { ProductApiService } from './product-api.service';
import { ProductCommand } from './product.models';

describe('ProductApiService', () => {
  const command: ProductCommand = {
    name: 'Coffee', purchasePrice: 2, wholesalePrice: 3, salePrice: 4,
    measurementUnitId: 1, productType: 2, isSellableInPos: true,
    isSellableInSales: true, isActive: true, supplierIds: [8],
    purchaseBillingPolicy: 2, allowNegativeRecipeConsumption: false,
    trackingPolicy: 0, requireExpirationDate: false, expirationWarningDays: 30,
    isFragile: false, keepDry: true, requiresRefrigeration: false,
    requiresFrozenStorage: false, isHazardous: false,
  };

  beforeEach(() => TestBed.configureTestingModule({ providers: [ProductApiService, provideHttpClient(), provideHttpClientTesting()] }));

  it('sends bounded paging, search and status filters', () => {
    const service = TestBed.inject(ProductApiService); const http = TestBed.inject(HttpTestingController);
    service.getPage({ page: 2, search: 'coffee', isActive: true }).subscribe();
    const request = http.expectOne(value => value.url === '/api/v1/products');
    expect(request.request.params.get('page')).toBe('2'); expect(request.request.params.get('pageSize')).toBe('25'); expect(request.request.params.get('search')).toBe('coffee'); expect(request.request.params.get('isActive')).toBe('true');
    request.flush({ query: {}, items: [], totalCount: 0, totalPages: 1, categories: [] }); http.verify();
  });

  it('uses dedicated form contracts and all product mutations', () => {
    const service = TestBed.inject(ProductApiService); const http = TestBed.inject(HttpTestingController);
    service.getForm().subscribe(); http.expectOne('/api/v1/products/form').flush({ form: {}, measurementUnits: [], categories: [], suppliers: [], expenseAccounts: [] });
    service.getForm(7).subscribe(); http.expectOne('/api/v1/products/7/form').flush({ form: {}, measurementUnits: [], categories: [], suppliers: [], expenseAccounts: [] });
    service.create(command).subscribe(); const create = http.expectOne('/api/v1/products'); expect(create.request.method).toBe('POST'); expect(create.request.body).toEqual(command); create.flush({ id: 7 });
    service.update(7, command).subscribe(); const update = http.expectOne('/api/v1/products/7'); expect(update.request.method).toBe('PUT'); expect(update.request.body.name).toBe('Coffee'); update.flush({ id: 7 });
    service.delete(7).subscribe(); const remove = http.expectOne('/api/v1/products/7'); expect(remove.request.method).toBe('DELETE'); remove.flush(null);
    http.verify();
  });
});
