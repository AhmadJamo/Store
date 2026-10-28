import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {
  ImportPreview,
  PriceBreakPage,
  PriceExceptionPage,
  PriceHistoryPage,
  SupplierPurchasingPage,
} from './supplier-purchasing.models';
@Injectable({ providedIn: 'root' })
export class SupplierPurchasingApiService {
  private readonly base = '/api/v1/supplier-purchasing';
  constructor(private readonly http: HttpClient) {}
  page(supplierId?: number) {
    return this.http.get<SupplierPurchasingPage>(this.base, {
      params: supplierId ? { supplierId } : {},
    });
  }
  exceptions(type?: number, page = 1) {
    let p = new HttpParams().set('page', page);
    if (type) p = p.set('type', type);
    return this.http.get<PriceExceptionPage>(`${this.base}/exceptions`, { params: p });
  }
  breaks(id: number) {
    return this.http.get<PriceBreakPage>(`${this.base}/${id}/price-breaks`);
  }
  history(id: number) {
    return this.http.get<PriceHistoryPage>(`${this.base}/${id}/history`);
  }
  saveBulk(input: unknown) {
    return this.http.post<{ count: number }>(`${this.base}/bulk`, input);
  }
  saveBreaks(id: number, input: unknown) {
    return this.http.post(`${this.base}/${id}/price-breaks`, input);
  }
  setActive(id: number, isActive: boolean, rowVersion: string) {
    return this.http.post(`${this.base}/${id}/active`, { isActive, rowVersion });
  }
  preview(supplierId: number, file: File, format: 'csv' | 'excel') {
    const form = new FormData();
    form.append('supplierId', String(supplierId));
    form.append('file', file);
    return this.http.post<ImportPreview>(`${this.base}/preview/${format}`, form);
  }
  import(preview: ImportPreview) {
    return this.http.post<{ count: number }>(
      `${this.base}/import/${preview.fileFormat}`,
      preview.fileFormat === 'csv'
        ? {
            supplierId: preview.supplierId,
            fileName: preview.fileName,
            csvContent: preview.csvContent,
          }
        : {
            supplierId: preview.supplierId,
            fileName: preview.fileName,
            encodedContent: preview.encodedContent,
          },
    );
  }
  download(supplierId: number, format: 'csv' | 'excel') {
    return this.http.get(`${this.base}/export/${format}`, {
      params: { supplierId },
      responseType: 'blob',
    });
  }
}
