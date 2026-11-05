import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { HttpParams } from '@angular/common/http';
import { JournalEntry, JournalEntryListPage, JournalEntryListQuery } from './journal-entry.models';
@Injectable({ providedIn: 'root' })
export class JournalEntryApiService {
  constructor(private readonly http: HttpClient) {}
  get(id: number) {
    return this.http.get<JournalEntry>(`/api/v1/journal-entries/${id}`);
  }
  getPage(query: JournalEntryListQuery) {
    let params = new HttpParams().set('page', query.page).set('pageSize', query.pageSize);
    if (query.search) params = params.set('search', query.search);
    if (query.fromDate) params = params.set('fromDate', query.fromDate);
    if (query.toDate) params = params.set('toDate', query.toDate);
    if (query.status !== 'all') params = params.set('status', query.status);
    return this.http.get<JournalEntryListPage>('/api/v1/journal-entries', { params });
  }
}
