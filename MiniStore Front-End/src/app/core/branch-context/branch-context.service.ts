import { computed, Injectable, signal } from '@angular/core';
import { ApiSession } from '../api/api.models';

export interface WorkspaceBranch {
  readonly id: number;
  readonly code: string;
  readonly name: string;
  readonly isActive: boolean;
}

@Injectable({ providedIn: 'root' })
export class BranchContextService {
  private readonly choicesState = signal<readonly WorkspaceBranch[]>([]);
  private readonly selectedIdState = signal<number | null>(null);
  private storageKey: string | null = null;

  readonly choices = this.choicesState.asReadonly();
  readonly selectedId = this.selectedIdState.asReadonly();
  readonly selected = computed(() => this.choicesState().find(branch => branch.id === this.selectedIdState()) ?? null);

  initialize(session: ApiSession | null, branches: readonly WorkspaceBranch[]): void {
    if (!session) { this.clear(); return; }
    this.storageKey = 'ministore.branch-context.v1:' + session.tenant.id + ':' + session.user.id;
    const active = branches.filter(branch => branch.isActive);
    this.choicesState.set(active);
    const storedId = this.readStoredId();
    const selectedId = active.some(branch => branch.id === storedId)
      ? storedId
      : active.length === 1 ? active[0].id : null;
    this.selectedIdState.set(selectedId);
    this.persist(selectedId);
  }

  select(branchId: number | null): boolean {
    const valid = branchId === null || this.choicesState().some(branch => branch.id === branchId);
    if (!valid) return false;
    this.selectedIdState.set(branchId);
    this.persist(branchId);
    return true;
  }

  clear(): void {
    this.choicesState.set([]);
    this.selectedIdState.set(null);
    this.storageKey = null;
  }

  private readStoredId(): number | null {
    if (!this.storageKey) return null;
    try {
      const value = Number(globalThis.localStorage?.getItem(this.storageKey));
      return Number.isSafeInteger(value) && value > 0 ? value : null;
    } catch { return null; }
  }

  private persist(branchId: number | null): void {
    if (!this.storageKey) return;
    try {
      if (branchId === null) globalThis.localStorage?.removeItem(this.storageKey);
      else globalThis.localStorage?.setItem(this.storageKey, String(branchId));
    } catch { /* in-memory fallback */ }
  }
}
