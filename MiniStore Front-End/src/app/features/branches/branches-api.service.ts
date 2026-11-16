import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

export interface BranchWarehouseSummary { id:number; name:string; isDefaultForPos:boolean; allowPosSales:boolean; allowPurchases:boolean; allowTransferOut:boolean; allowTransferIn:boolean; }
export interface BranchTerminalSummary { id:number; name:string; defaultWarehouseName:string; isActive:boolean; }
export interface BranchOperations { salesAmount:number; salesCount:number; stockQuantity:number; stockValue:number; openTransfers:number; transferExceptions:number; openCashSessions:number; cashSessionsAwaitingApproval:number; cashVariance:number; }
export interface BranchSummary { id:number; code:string; name:string; isActive:boolean; warehouses:BranchWarehouseSummary[]; posTerminals:BranchTerminalSummary[]; operations:BranchOperations|null; }
export interface BranchCommand { code:string; name:string; }
export interface BranchRow { id:number; code:string; name:string; isActive:boolean; }
export interface BranchWorkspace { branches:BranchSummary[]; periodFromUtc:string; periodToUtc:string; company:BranchOperations|null; }
export interface BranchAccessOption { id:number; code:string; name:string; isActive:boolean; }
export interface BranchAccessUser { id:string; displayName:string; email:string|null; branchIds:number[]; }
export interface BranchAccessManagement { branches:BranchAccessOption[]; users:BranchAccessUser[]; }

@Injectable({ providedIn: 'root' })
export class BranchesApiService {
  private readonly http = inject(HttpClient);
  getWorkspace() { return this.http.get<BranchWorkspace>('/api/v1/branches'); }
  create(command: BranchCommand) { return this.http.post<BranchRow>('/api/v1/branches', command); }
  update(id: number, command: BranchCommand) { return this.http.put<BranchRow>(`/api/v1/branches/${id}`, command); }
  setActive(id: number, isActive: boolean) { return this.http.put<BranchRow>(`/api/v1/branches/${id}/status`, { isActive }); }
  getAccessManagement() { return this.http.get<BranchAccessManagement>('/api/v1/branches/access-management'); }
  replaceUserGrants(userId: string, branchIds: number[]) {
    return this.http.put<BranchAccessManagement>('/api/v1/branches/access-management', { userId, branchIds });
  }
}
