export interface TenantUser {
  id: string;
  userName: string | null;
  email: string | null;
  emailConfirmed: boolean;
  roleNames: readonly string[];
}

export interface RoleOption { id: number; name: string; isSystem: boolean; }
export interface UserCreatePage { roles: readonly RoleOption[]; }
export interface CreateTenantUser { userName: string; email: string; password: string; roleId: number; }

export interface TenantRoleSummary {
  id: number;
  name: string;
  description: string | null;
  isSystem: boolean;
  userCount: number;
  permissionCount: number;
}

export interface PermissionOption { id: number; code: string; groupCode: string; }
export interface TenantRoleForm {
  id: number;
  name: string;
  description: string | null;
  isSystem: boolean;
  selectedPermissionIds: readonly number[];
  permissions: readonly PermissionOption[];
  rowVersion: string;
}

export interface SaveTenantRole {
  id: number;
  name: string;
  description: string | null;
  selectedPermissionIds: readonly number[];
  rowVersion: string;
}
