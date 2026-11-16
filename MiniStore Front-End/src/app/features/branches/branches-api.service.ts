import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

export interface BranchWarehouseSummary { id:number; name:string; isDefaultForPos:boolean; allowPosSales:boolean; allowPurchases:boolean; allowTransferOut:boolean; allowTransferIn:boolean; }
export interface BranchTerminalSummary { id:number; name:string; defaultWarehouseName:string; isActive:boolean; }
export interface BranchSummary { id:number; code:string; name:string; isActive:boolean; warehouses:BranchWarehouseSummary[]; posTerminals:BranchTerminalSummary[]; }
export interface BranchWorkspace { branches:BranchSummary[]; }
export interface BranchAccessOption { id:number; code:string; name:string; isActive:boolean; }
export interface BranchAccessUser { id:string; displayName:string; email:string|null; branchIds:number[]; }
export interface BranchAccessManagement { branches:BranchAccessOption[]; users:BranchAccessUser[]; }

@Injectable({ providedIn: 'root' })
export class BranchesApiService {
  private readonly http = inject(HttpClient);
  getWorkspace() { return this.http.get<BranchWorkspace>('/api/v1/branches'); }
  getAccessManagement() { return this.http.get<BranchAccessManagement>('/api/v1/branches/access-management'); }
  replaceUserGrants(userId: string, branchIds: number[]) {
    return this.http.put<BranchAccessManagement>('/api/v1/branches/access-management', { userId, branchIds });
  }
}
