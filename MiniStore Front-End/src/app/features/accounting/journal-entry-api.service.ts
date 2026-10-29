import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { JournalEntry } from './journal-entry.models';
@Injectable({ providedIn: 'root' })
export class JournalEntryApiService {
  constructor(private readonly http: HttpClient) {}
  get(id: number) {
    return this.http.get<JournalEntry>(`/api/v1/journal-entries/${id}`);
  }
}
