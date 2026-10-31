export interface RequestOption {
  id: number;
  label: string;
}
export interface CreateRequestLine {
  productId: number;
  measurementUnitId: number;
  requestedQuantity: number;
  suggestedSupplierId?: number;
  notes?: string;
}
export interface CreateRequest {
  warehouseId: number;
  neededByDate: string;
  priority: number;
  justification: string;
  notes?: string;
  lines: CreateRequestLine[];
}
export interface CreateRequestPage {
  form: CreateRequest;
  warehouses: RequestOption[];
  products: RequestOption[];
  units: RequestOption[];
  suppliers: RequestOption[];
}
export interface RequestLine {
  productId: number;
  productName: string;
  productCode: string;
  measurementUnitId: number;
  unitName: string;
  requestedQuantity: number;
  stockQuantity: number;
  suggestedSupplierName?: string;
  notes?: string;
}
export interface RequestHistory {
  fromStatus: number;
  toStatus: number;
  action: number;
  userId: string;
  reason?: string;
  createdAtUtc: string;
}
export interface ApprovalStep {
  sequence: number;
  roleName: string;
  status: number;
  decidedByUserId?: string;
  decidedAtUtc?: string;
  note?: string;
}
export interface Approval {
  status: number;
  ruleName: string;
  currentRole?: string;
  canCurrentUserDecide: boolean;
  steps: ApprovalStep[];
}
export interface PurchaseRequest {
  id: number;
  requestNumber: string;
  warehouseId: number;
  warehouseName: string;
  neededByDate: string;
  priority: number;
  justification: string;
  notes?: string;
  sourceType?: string;
  sourceReference?: string;
  status: number;
  createdByUserId: string;
  createdAtUtc: string;
  rowVersion: string;
  approval?: Approval;
  lines: RequestLine[];
  history: RequestHistory[];
}
export interface ApprovalRule {
  id: number;
  name: string;
  warehouseName: string;
  minimumPriority: number;
  maximumPriority: number;
  isActive: boolean;
  rowVersion: string;
  approverRoles: string[];
}
export interface ApprovalRulesPage {
  form: {
    name: string;
    warehouseId?: number;
    minimumPriority: number;
    maximumPriority: number;
    approverRoles: string[];
  };
  rules: ApprovalRule[];
  warehouses: RequestOption[];
  roles: string[];
}
