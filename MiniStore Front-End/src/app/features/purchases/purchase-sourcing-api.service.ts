import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CreateSourcingInput, PurchaseSourcing, QuotationComparison, QuotationCreatePage, SourcingCreatePage } from './purchase-sourcing.models';

@Injectable({providedIn:'root'})
export class PurchaseSourcingApiService {
  private readonly sourcing='/api/v1/purchase-sourcing';
  private readonly quotations='/api/v1/supplier-quotations';
  constructor(private readonly http:HttpClient){}
  getAll(status?:number,search?:string){let params=new HttpParams();if(status)params=params.set('status',status);if(search?.trim())params=params.set('search',search.trim());return this.http.get<PurchaseSourcing[]>(this.sourcing,{params});}
  get(id:number){return this.http.get<PurchaseSourcing>(`${this.sourcing}/${id}`);}
  getCreateForm(purchaseRequestId:number){return this.http.get<SourcingCreatePage>(`${this.sourcing}/create-form`,{params:{purchaseRequestId}});}
  create(input:CreateSourcingInput,files:readonly File[]){const form=new FormData();for(const[key,value]of Object.entries(input)){if(value===undefined||value===null||value==='')continue;if(Array.isArray(value))value.forEach(item=>form.append(key,String(item)));else form.append(key,String(value));}files.forEach(file=>form.append('attachments',file,file.name));return this.http.post<{id:number}>(this.sourcing,form);}
  attachmentUrl(id:number,attachmentId:number){return`${this.sourcing}/${id}/attachments/${attachmentId}`;}
  getQuotationForm(sourcingEventId:number){return this.http.get<QuotationCreatePage>(`${this.quotations}/create-form`,{params:{sourcingEventId}});}
  createQuotation(input:unknown){return this.http.post<{id:number}>(this.quotations,input);}
  comparison(sourcingEventId:number){return this.http.get<QuotationComparison>(`${this.quotations}/comparison/${sourcingEventId}`);}
  award(input:unknown){return this.http.post<void>(`${this.quotations}/award`,input);}
}
