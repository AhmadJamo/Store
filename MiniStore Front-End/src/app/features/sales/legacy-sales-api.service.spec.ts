import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { LegacySalesApiService } from './legacy-sales-api.service';

describe('LegacySalesApiService', () => {
  let api: LegacySalesApiService;
  let http: HttpTestingController;
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    api = TestBed.inject(LegacySalesApiService);
    http = TestBed.inject(HttpTestingController);
  });
  afterEach(() => http.verify());
  it('posts wholesale sale creation to the bounded endpoint', () => {
    const input = {
      invoiceNumber: '',
      warehouseId: 2,
      posTerminalId: null,
      customerId: null,
      paymentMethodId: 3,
      taxRateId: null,
      posOrderType: null,
      serviceReference: null,
      guestCount: null,
      date: '2026-10-29',
      notes: null,
      invoiceDiscountType: 1,
      invoiceDiscountValue: 0,
      items: [],
    };
    api.create(input).subscribe();
    const request = http.expectOne('/api/v1/legacy-sales');
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual(input);
    request.flush({ id: 9 });
  });
  it('posts retail POS creation to its explicit endpoint', () => {
    const input = {
      invoiceNumber: '',
      warehouseId: 2,
      posTerminalId: 4,
      customerId: null,
      paymentMethodId: 3,
      taxRateId: null,
      posOrderType: 1,
      serviceReference: null,
      guestCount: null,
      date: '2026-10-29',
      notes: null,
      invoiceDiscountType: 1,
      invoiceDiscountValue: 0,
      items: [],
    };
    api.createPos(input).subscribe();
    const request = http.expectOne('/api/v1/legacy-sales/pos');
    expect(request.request.method).toBe('POST');
    expect(request.request.body.posTerminalId).toBe(4);
    request.flush({ id: 10 });
  });
  it('uses the POS cash-session operational endpoints', () => {
    api.cashPage(4).subscribe(); expect(http.expectOne('/api/v1/pos-cash-sessions/terminal/4').request.method).toBe('GET');
    api.openCash({posTerminalId:4}).subscribe(); expect(http.expectOne('/api/v1/pos-cash-sessions/open').request.method).toBe('POST');
    api.cashMovement(8,{amount:5}).subscribe(); expect(http.expectOne('/api/v1/pos-cash-sessions/8/movements').request.method).toBe('POST');
    api.cashCount(8,100).subscribe(); expect(http.expectOne('/api/v1/pos-cash-sessions/8/count').request.method).toBe('POST');
    api.approveCash(8,{reason:'reviewed'}).subscribe(); expect(http.expectOne('/api/v1/pos-cash-sessions/8/approve').request.method).toBe('POST');
    api.linkCashSale(8,12).subscribe(); expect(http.expectOne('/api/v1/pos-cash-sessions/8/link-sale/12').request.method).toBe('POST');
  });
});
