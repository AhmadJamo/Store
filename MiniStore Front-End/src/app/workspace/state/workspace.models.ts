import { NavigationTextKey } from '../navigation/navigation-localization.service';

export interface NavigationItem {
  readonly id: string;
  readonly route: string;
  readonly labelKey: NavigationTextKey;
  readonly icon: string;
  readonly requiredPermission?: string;
  readonly requiredAnyPermissions?: readonly string[];
  readonly showInSidebar?: boolean;
  readonly hubSectionKey?: NavigationTextKey;
}

export interface WorkspaceHistoryEntry {
  readonly route: string;
  readonly contextKey?: string;
}

export interface WorkspaceTab {
  readonly id: string;
  readonly navigationId: string;
  readonly route: string;
  readonly titleKey: NavigationTextKey;
  readonly contextKey?: string;
  readonly dirty: boolean;
  readonly history: readonly WorkspaceHistoryEntry[];
  readonly historyIndex: number;
}

export interface WorkspaceState {
  readonly tabs: readonly WorkspaceTab[];
  readonly activeTabId: string | null;
  readonly recentlyClosed: readonly WorkspaceTab[];
}

export type CloseRequest = 'closed' | 'confirmation-required' | 'not-found';

export interface WorkspaceOptions {
  readonly maximumTabs: number;
  readonly maximumHistoryEntries: number;
  readonly recentlyClosedLimit: number;
}
