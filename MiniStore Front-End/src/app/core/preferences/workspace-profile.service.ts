import { DOCUMENT } from '@angular/common';
import { inject, Injectable, signal } from '@angular/core';
export type WorkspaceProfile = 'focused' | 'operator' | 'analyst';
interface StoredWorkspaceProfile { version: 1; profile: WorkspaceProfile; splitView: boolean; }
@Injectable({ providedIn: 'root' })
export class WorkspaceProfileService {
  private readonly key = 'ministore.workspace.profile'; private readonly document = inject(DOCUMENT);
  readonly profile = signal<WorkspaceProfile>('focused'); readonly splitView = signal(false);
  constructor() { this.restore(); this.apply(); }
  setProfile(value: WorkspaceProfile): void { if (!['focused', 'operator', 'analyst'].includes(value)) return; this.profile.set(value); this.save(); this.apply(); }
  setSplitView(value: boolean): void { this.splitView.set(value === true); this.save(); this.apply(); }
  reset(): void { this.profile.set('focused'); this.splitView.set(false); try { localStorage.removeItem(this.key); } catch { /* memory fallback */ } this.apply(); }
  private restore(): void { try { const value = JSON.parse(localStorage.getItem(this.key) ?? 'null') as Partial<StoredWorkspaceProfile> | null; if (value?.version === 1 && ['focused', 'operator', 'analyst'].includes(value.profile ?? '')) this.profile.set(value.profile!); if (value?.version === 1 && typeof value.splitView === 'boolean') this.splitView.set(value.splitView); } catch { /* safe defaults */ } }
  private save(): void { try { localStorage.setItem(this.key, JSON.stringify({ version: 1, profile: this.profile(), splitView: this.splitView() } satisfies StoredWorkspaceProfile)); } catch { /* memory fallback */ } }
  private apply(): void { this.document.documentElement.dataset['workspaceProfile'] = this.profile(); this.document.documentElement.dataset['splitView'] = String(this.splitView()); }
}
