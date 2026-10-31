export interface AuditEventQuery {
  from?: string;
  to?: string;
  category?: number | null;
  outcome?: number | null;
  eventType?: string;
  actorId?: string;
  resourceType?: string;
  resourceId?: string;
  correlationId?: string;
  page: number;
  pageSize: number;
}

export interface AuditEventItem {
  id: number;
  eventId: string;
  scope: number;
  occurredAtUtc: string;
  category: number;
  severity: number;
  outcome: number;
  eventType: string;
  action: string;
  actorType: number;
  actorId: string;
  sourceChannel: number;
  correlationId: string;
  resourceType: string;
  resourceId: string;
  reasonCode: string | null;
  changesRedactedJson: string | null;
  metadataRedactedJson: string | null;
  requestPathTemplate: string | null;
  sourceIpMasked: string | null;
  userAgentHash: string | null;
}

export interface AuditEventPage {
  query: AuditEventQuery;
  items: readonly AuditEventItem[];
  totalCount: number;
  canViewSensitive: boolean;
  totalPages: number;
}
