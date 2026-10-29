export interface ApprovalRule {
  id: number;
  name: string;
  customerName: string;
  metric: number;
  comparison: number;
  threshold: number;
  action: number;
  priority: number;
  effectiveFrom: string | null;
  effectiveTo: string | null;
  allowRequesterApproval: boolean;
  requireDistinctApprovers: boolean;
  isActive: boolean;
  rowVersion: string;
  approverRoles: string[];
}
export interface ApprovalOption {
  id: number;
  label: string;
}
export interface CreateApprovalRule {
  name: string;
  customerId: number | null;
  metric: number;
  comparison: number;
  threshold: number;
  action: number;
  priority: number;
  effectiveFrom: string | null;
  effectiveTo: string | null;
  allowRequesterApproval: boolean;
  requireDistinctApprovers: boolean;
  approverRoles: string[];
}
export interface ApprovalRulesPage {
  form: CreateApprovalRule;
  rules: ApprovalRule[];
  customers: ApprovalOption[];
  roles: string[];
}
