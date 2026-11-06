import{HttpClient,HttpParams}from'@angular/common/http';
import{Injectable}from'@angular/core';
import{CashFlowClosePage,CashFlowCloseQuery}from'./cash-flow-close.models';
@Injectable({providedIn:'root'})export class CashFlowCloseApiService{constructor(private readonly http:HttpClient){}get(query:CashFlowCloseQuery){let params=new HttpParams();if(query.fromDate)params=params.set('fromDate',query.fromDate);if(query.toDate)params=params.set('toDate',query.toDate);return this.http.get<CashFlowClosePage>('/api/v1/cash-flow-close',{params});}}
