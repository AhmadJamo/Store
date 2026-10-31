export interface WorkCenterItem { itemKey: string; severity: 'critical' | 'attention'; documentKind: string; documentId: number; documentNumber: string; party: string; status: string; dueDate?: string; titleKey: string; helpKey: string; requiredPermission: string; }
export interface WorkCenter { generatedAtUtc: string; items: readonly WorkCenterItem[]; }
