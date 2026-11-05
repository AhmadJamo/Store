import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { FinancialReportApiService } from './financial-report-api.service';
describe('FinancialReportApiService',()=>{it('loads server-calculated reports for the selected period',()=>{TestBed.configureTestingModule({providers:[provideHttpClient(),provideHttpClientTesting()]});const api=TestBed.inject(FinancialReportApiService),http=TestBed.inject(HttpTestingController);api.get({fromDate:'2026-01-01',toDate:'2026-12-31',view:'trial'}).subscribe();const request=http.expectOne(value=>value.url==='/api/v1/financial-reports');expect(request.request.method).toBe('GET');expect(request.request.params.get('fromDate')).toBe('2026-01-01');expect(request.request.params.get('toDate')).toBe('2026-12-31');request.flush({});http.verify();});});
