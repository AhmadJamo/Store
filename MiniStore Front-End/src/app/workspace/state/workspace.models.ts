export interface WorkspaceTab {
  readonly id: string;
  readonly route: string;
  readonly title: string;
  readonly entityKey?: string;
  readonly dirty: boolean;
}

export interface WorkspaceState {
  readonly tabs: readonly WorkspaceTab[];
  readonly activeTabId: string | null;
}
