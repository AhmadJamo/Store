import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateTenantUser, SaveTenantRole, TenantRoleForm, TenantRoleSummary, TenantUser, UserCreatePage } from './identity.models';

@Injectable({ providedIn: 'root' })
export class IdentityApiService {
  constructor(private readonly http: HttpClient) {}
  getUsers(): Observable<readonly TenantUser[]> { return this.http.get<readonly TenantUser[]>('/api/v1/users'); }
  getUserCreatePage(): Observable<UserCreatePage> { return this.http.get<UserCreatePage>('/api/v1/users/form'); }
  createUser(value: CreateTenantUser): Observable<TenantUser> { return this.http.post<TenantUser>('/api/v1/users', value); }
  getRoles(): Observable<readonly TenantRoleSummary[]> { return this.http.get<readonly TenantRoleSummary[]>('/api/v1/roles'); }
  getRoleCreateForm(): Observable<TenantRoleForm> { return this.http.get<TenantRoleForm>('/api/v1/roles/form'); }
  getRoleEditForm(id: number): Observable<TenantRoleForm> { return this.http.get<TenantRoleForm>(`/api/v1/roles/${id}`); }
  createRole(value: SaveTenantRole): Observable<TenantRoleForm> { return this.http.post<TenantRoleForm>('/api/v1/roles', value); }
  updateRole(value: SaveTenantRole): Observable<TenantRoleForm> { return this.http.put<TenantRoleForm>(`/api/v1/roles/${value.id}`, value); }
  deleteRole(id: number): Observable<void> { return this.http.delete<void>(`/api/v1/roles/${id}`); }
}
