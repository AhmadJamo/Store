import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {
  StockTransferDetails,
  StockTransferFormPage,
  StockTransferInput,
  StockTransferRow,
  StockTransferStatus,
} from './stock-transfer.models';

@Injectable({ providedIn: 'root' })
export class StockTransferApiService {
  private readonly baseUrl = '/api/v1/stock-transfers';

  constructor(private readonly http: HttpClient) {}

  getAll(search?: string, status?: StockTransferStatus) {
    let params = new HttpParams();
    if (search?.trim()) params = params.set('search', search.trim());
    if (status) params = params.set('status', status);
    return this.http.get<StockTransferRow[]>(this.baseUrl, { params });
  }

  getDetails(id: number) {
    return this.http.get<StockTransferDetails>(this.baseUrl + '/' + id);
  }

  getCreateForm() {
    return this.http.get<StockTransferFormPage>(this.baseUrl + '/create-form');
  }

  getEditForm(id: number) {
    return this.http.get<StockTransferFormPage>(this.baseUrl + '/' + id + '/edit-form');
  }

  create(input: StockTransferInput) {
    return this.http.post<{ id: number }>(this.baseUrl, input);
  }

  update(id: number, input: StockTransferInput) {
    return this.http.put<void>(this.baseUrl + '/' + id, input);
  }

  submit(id: number) { return this.command(id, 'submit'); }
  approve(id: number) { return this.command(id, 'approve'); }
  returnToDraft(id: number) { return this.command(id, 'return-to-draft'); }
  post(id: number) { return this.command(id, 'post'); }
  reject(id: number, reason: string) { return this.command(id, 'reject', { reason }); }
  cancel(id: number, reason: string) { return this.command(id, 'cancel', { reason }); }

  private command(id: number, action: string, body: object = {}) {
    return this.http.post<void>(this.baseUrl + '/' + id + '/' + action, body);
  }
}
