import { MessageKey } from '../../core/localization/localization.service';

export type QuickPreviewProviderId = 'customers' | 'suppliers';
export type CommandCenterViewMode = 'comfortable' | 'compact';

export interface QuickPreviewProvider {
  readonly id: QuickPreviewProviderId;
  readonly labelKey: MessageKey;
  readonly descriptionKey: MessageKey;
  readonly navigationId: string;
  readonly requiredPermission: string;
}

export interface QuickPreviewRow {
  readonly id: number;
  readonly title: string;
  readonly reference?: string;
  readonly secondary?: string;
  readonly route: string;
  readonly active: boolean;
}

export interface QuickPreviewResult {
  readonly providerId: QuickPreviewProviderId;
  readonly rows: readonly QuickPreviewRow[];
  readonly totalCount: number;
}

export interface CommandCenterSnapshot {
  readonly id: string;
  readonly providerId: QuickPreviewProviderId;
  readonly viewMode: CommandCenterViewMode;
}
