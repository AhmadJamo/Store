import { TestBed } from '@angular/core/testing';
import { ActivatedRoute, provideRouter } from '@angular/router';
import { of, throwError } from 'rxjs';
import { ApiErrorService } from '../../core/api/api-error.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { LocalizationService } from '../../core/localization/localization.service';
import { CustomerApiService } from './data/customer-api.service';
import { CustomerDetailsPageComponent } from './customer-details-page';

describe('CustomerDetailsPageComponent', () => {
  const customer = {
    id: 8, customerCode: 'CUS-008', type: 1, name: 'Acme', legalName: 'Acme LLC', phone: '555-0100', email: 'hello@acme.test',
    accountName: 'Accounts Receivable', isActive: true, website: 'https://acme.test', preferredLanguage: 'en-US', timeZoneId: 'Asia/Amman',
    paymentTermDays: 30, preferredCurrencyCode: 'JOD', defaultTaxRateName: 'Standard', defaultSalesPriceListName: 'Retail', isTaxExempt: false,
    rowVersion: 'row-version', contacts: [{ id: 1, name: 'Mona', phone: '555-0101', isPrimary: true, isActive: true }],
    addresses: [{ id: 1, type: 1, label: 'Office', snapshot: 'Amman', isDefault: true, isActive: true }],
  };

  it('retries a recoverable details request without leaving the record route', async () => {
    const problem = { kind: 'transient', canRetry: true, title: 'Unavailable' };
    const get = vi.fn().mockReturnValueOnce(throwError(() => problem)).mockReturnValueOnce(of(customer));
    await TestBed.configureTestingModule({
      imports: [CustomerDetailsPageComponent],
      providers: [
        provideRouter([]),
        { provide: ActivatedRoute, useValue: { snapshot: { paramMap: { get: () => '8' } } } },
        { provide: CustomerApiService, useValue: { get } },
        { provide: ApiSessionService, useValue: { can: () => true } },
        { provide: ApiErrorService, useValue: { normalize: (error: unknown) => error } },
        { provide: LocalizationService, useValue: { text: (key: string) => key } },
      ],
    }).compileComponents();

    const fixture = TestBed.createComponent(CustomerDetailsPageComponent);
    fixture.detectChanges();
    fixture.nativeElement.querySelector('app-api-recovery button').click();
    fixture.detectChanges();

    expect(get).toHaveBeenCalledTimes(2);
    expect(fixture.nativeElement.querySelector('app-api-recovery')).toBeNull();
    expect(fixture.nativeElement.querySelector('[page-context]')?.textContent).toContain('CUS-008');
  });
  it('presents the complete customer record in the shared ERP hierarchy', async () => {
    await TestBed.configureTestingModule({
      imports: [CustomerDetailsPageComponent],
      providers: [
        provideRouter([]),
        { provide: ActivatedRoute, useValue: { snapshot: { paramMap: { get: () => '8' } } } },
        { provide: CustomerApiService, useValue: { get: () => of(customer) } },
        { provide: ApiSessionService, useValue: { can: () => true } },
        { provide: ApiErrorService, useValue: { normalize: (error: unknown) => error } },
        { provide: LocalizationService, useValue: { text: (key: string) => key } },
      ],
    }).compileComponents();

    const fixture = TestBed.createComponent(CustomerDetailsPageComponent);
    fixture.detectChanges();

    const context = fixture.nativeElement.querySelector('[page-context]')?.textContent;
    expect(context).toContain('active');
    expect(context).toContain('company');
    expect(context).toContain('CUS-008');
    expect(fixture.nativeElement.querySelectorAll('.details-grid app-detail-section')).toHaveLength(4);
    expect(fixture.nativeElement.querySelectorAll('[primary-actions]')).toHaveLength(1);
    expect(fixture.nativeElement.textContent).toContain('Mona');
    expect(fixture.nativeElement.textContent).toContain('Office');
  });
});
