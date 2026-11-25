import { TestBed } from '@angular/core/testing';
import { ActivatedRoute, provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { ApiErrorService } from '../../core/api/api-error.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { ProductApiService } from './data/product-api.service';
import { ProductDetailsPageComponent } from './product-details-page';
import { ProductLocalizationService } from './product-localization.service';

describe('ProductDetailsPageComponent', () => {
  const product = {
    id: 4, productCode: 'P-004', name: 'Coffee', barcode: '1234', purchasePrice: 1, wholesalePrice: 2, salePrice: 3,
    inventoryBehavior: 1, productType: 2, stockUnit: 1, measurementUnitId: 1, measurementUnitName: 'Piece',
    allowNegativeRecipeConsumption: false, isSellableInPos: true, isSellableInSales: true, isActive: true,
    productCategoryName: 'Drinks', trackingPolicy: 0, requireExpirationDate: false, expirationWarningDays: 30,
    handlingRequirements: 0, supplierIds: [], supplierNames: [], purchaseBillingPolicy: 2,
  };

  it('presents a localized, permission-aware ERP record hierarchy', async () => {
    await TestBed.configureTestingModule({
      imports: [ProductDetailsPageComponent],
      providers: [
        provideRouter([]),
        { provide: ActivatedRoute, useValue: { snapshot: { paramMap: { get: () => '4' } } } },
        { provide: ProductApiService, useValue: { get: () => of(product), delete: vi.fn() } },
        { provide: ApiSessionService, useValue: { can: () => true } },
        { provide: ApiErrorService, useValue: { normalize: (error: unknown) => error } },
        { provide: ProductLocalizationService, useValue: { text: (key: string) => key } },
      ],
    }).compileComponents();

    const fixture = TestBed.createComponent(ProductDetailsPageComponent);
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).not.toContain('CATALOG');
    expect(fixture.nativeElement.querySelector('[page-context]')?.textContent).toContain('directSale');
    expect(fixture.nativeElement.querySelectorAll('.details-grid app-detail-section')).toHaveLength(3);
    expect(fixture.nativeElement.querySelectorAll('[primary-actions]')).toHaveLength(1);
    expect(fixture.nativeElement.querySelector('.danger-zone p')?.textContent).toContain('deleteProductDescription');
  });
});