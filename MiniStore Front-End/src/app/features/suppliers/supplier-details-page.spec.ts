import { TestBed } from '@angular/core/testing';
import { ActivatedRoute, provideRouter } from '@angular/router';
import { of, throwError } from 'rxjs';
import { ApiErrorService } from '../../core/api/api-error.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { LocalizationService } from '../../core/localization/localization.service';
import { SupplierApiService } from './data/supplier-api.service';
import { SupplierDetailsPageComponent } from './supplier-details-page';

describe('SupplierDetailsPageComponent', () => {
  const supplier = {
    id: 11, name: 'North Supply', legalName: 'North Supply LLC', contactPerson: 'Omar', phone: '555-0200', email: 'sales@north.test',
    paymentTermsDays: 14, isActive: true, address: 'Amman', taxNumber: 'TAX-11', registrationNumber: 'REG-11', website: 'https://north.test',
    accountId: 7, productIds: [2, 3, 4], productCount: 3, rowVersion: 'row-version',
  };

  const providers = (get: ReturnType<typeof vi.fn>) => [
    provideRouter([]),
    { provide: ActivatedRoute, useValue: { snapshot: { paramMap: { get: () => '11' } } } },
    { provide: SupplierApiService, useValue: { get } },
    { provide: ApiSessionService, useValue: { can: () => true } },
    { provide: ApiErrorService, useValue: { normalize: (error: unknown) => error } },
    { provide: LocalizationService, useValue: { text: (key: string) => key } },
  ];

  it('presents the complete supplier record in the shared ERP hierarchy', async () => {
    await TestBed.configureTestingModule({ imports: [SupplierDetailsPageComponent], providers: providers(vi.fn(() => of(supplier))) }).compileComponents();
    const fixture = TestBed.createComponent(SupplierDetailsPageComponent);
    fixture.detectChanges();

    const context = fixture.nativeElement.querySelector('[page-context]')?.textContent;
    expect(fixture.nativeElement.textContent).not.toContain('P2P');
    expect(context).toContain('active');
    expect(context).toContain('assignedProducts');
    expect(context).toContain('3');
    expect(fixture.nativeElement.querySelectorAll('.details-grid app-detail-section')).toHaveLength(2);
    expect(fixture.nativeElement.querySelectorAll('[primary-actions]')).toHaveLength(1);
    expect(fixture.nativeElement.textContent).toContain('Omar');
    expect(fixture.nativeElement.textContent).toContain('TAX-11');
  });

  it('retries a recoverable details request without leaving the record route', async () => {
    const problem = { kind: 'transient', canRetry: true, title: 'Unavailable' };
    const get = vi.fn().mockReturnValueOnce(throwError(() => problem)).mockReturnValueOnce(of(supplier));
    await TestBed.configureTestingModule({ imports: [SupplierDetailsPageComponent], providers: providers(get) }).compileComponents();
    const fixture = TestBed.createComponent(SupplierDetailsPageComponent);
    fixture.detectChanges();
    fixture.nativeElement.querySelector('app-api-recovery button').click();
    fixture.detectChanges();

    expect(get).toHaveBeenCalledTimes(2);
    expect(fixture.nativeElement.querySelector('app-api-recovery')).toBeNull();
    expect(fixture.nativeElement.querySelector('[page-context]')?.textContent).toContain('3');
  });
});
