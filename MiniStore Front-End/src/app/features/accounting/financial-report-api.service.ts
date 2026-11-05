import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { FinancialReportPage, FinancialReportQuery } from './financial-report.models';
@Injectable({providedIn:'root'})
export class FinancialReportApiService {
  constructor(private readonly http:HttpClient){}
  get(query:FinancialReportQuery){let params=new HttpParams();if(query.fromDate)params=params.set('fromDate',query.fromDate);if(query.toDate)params=params.set('toDate',query.toDate);return this.http.get<FinancialReportPage>('/api/v1/financial-reports',{params});}
}
