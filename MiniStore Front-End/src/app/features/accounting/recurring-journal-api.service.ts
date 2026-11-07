import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { RecurringJournalPage, RecurringJournalRunResult, SaveRecurringJournalTemplate } from './recurring-journal.models';
@Injectable({providedIn:'root'})
export class RecurringJournalApiService {
  constructor(private readonly http:HttpClient) {}
  getPage(){return this.http.get<RecurringJournalPage>('/api/v1/recurring-journals');}
  create(value:SaveRecurringJournalTemplate){return this.http.post<{id:number}>('/api/v1/recurring-journals',value);}
  update(id:number,value:SaveRecurringJournalTemplate){return this.http.put<{id:number}>(`/api/v1/recurring-journals/${id}`,value);}
  setState(id:number,isActive:boolean,rowVersion:string){return this.http.post<{id:number}>(`/api/v1/recurring-journals/${id}/state`,{isActive,rowVersion});}
  run(id:number,rowVersion:string,idempotencyKey:string){return this.http.post<RecurringJournalRunResult>(`/api/v1/recurring-journals/${id}/run`,{rowVersion,idempotencyKey});}
}
