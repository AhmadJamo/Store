import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
@Injectable({ providedIn: 'root' })
export class SupplierIntegrationsApiService {
  constructor(private readonly h: HttpClient) {}
  get() {
    return this.h.get<any>('/api/v1/supplier-integrations');
  }
  create(v: any) {
    return this.h.post<any>('/api/v1/supplier-integrations/credentials', v);
  }
  deactivate(id: number, rowVersion: string) {
    return this.h.post<void>(
      `/api/v1/supplier-integrations/credentials/${id}/deactivate`,
      rowVersion,
    );
  }
  retry(id: number) {
    return this.h.post<void>(`/api/v1/supplier-integrations/outbox/${id}/retry`, {});
  }
  submission(id: number) {
    return this.h.get<any>(`/api/v1/supplier-integrations/submissions/${id}`);
  }
  download(id: number) {
    return `/api/v1/supplier-integrations/submissions/${id}/download`;
  }
  ubl(id: number) {
    return `/api/v1/supplier-integrations/purchase-orders/${id}/ubl`;
  }
  accesses(type: number, id: number) {
    return this.h.get<any[]>(`/api/v1/supplier-integrations/portal-access/${type}/${id}`);
  }
  createAccess(v: any) {
    return this.h.post<any>('/api/v1/supplier-integrations/portal-access', v);
  }
  revoke(id: number) {
    return this.h.post<void>(`/api/v1/supplier-integrations/portal-access/${id}/revoke`, {});
  }
  credentials(supplierId: number, orderId: number) {
    return this.h.get<any[]>(
      `/api/v1/supplier-integrations/credentials/supplier/${supplierId}?purchaseOrderId=${orderId}`,
    );
  }
  queue(v: any) {
    return this.h.post<void>('/api/v1/supplier-integrations/outbox/purchase-orders', v);
  }
}
