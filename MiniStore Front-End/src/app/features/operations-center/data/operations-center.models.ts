export interface BackgroundJobStatus {
  jobKey: string;
  titleKey: string;
  permission: string;
  state: 'waiting' | 'running' | 'succeeded' | 'failed' | 'disabled';
  slaState: 'healthy' | 'at-risk' | 'breached' | 'disabled';
  expectedIntervalMinutes: number;
  lastStartedAtUtc?: string;
  lastCompletedAtUtc?: string;
  lastDurationMilliseconds?: number;
  nextExpectedAtUtc?: string;
}

export interface AnalyticsRefreshPlan {
  workspaceId: number;
  name: string;
  nameArabic?: string;
  moduleKey: string;
  refreshIntervalMinutes: number;
  isAutomaticDelivery: boolean;
}

export interface OperationsCenter {
  generatedAtUtc: string;
  jobs: readonly BackgroundJobStatus[];
  refreshPlans: readonly AnalyticsRefreshPlan[];
}
