import { Injectable, signal } from '@angular/core';

export type ToastKind = 'error' | 'warning' | 'success' | 'info';
export interface ToastMessage { id: number; kind: ToastKind; title: string; detail?: string; correlationId?: string; }

@Injectable({ providedIn: 'root' })
export class ToastService {
  readonly messages = signal<readonly ToastMessage[]>([]);
  private nextId = 1;
  show(message: Omit<ToastMessage, 'id'>, durationMs = 7000): number {
    const id = this.nextId++;
    this.messages.update(items => [...items.slice(-2), { ...message, id }]);
    globalThis.setTimeout(() => this.dismiss(id), durationMs);
    return id;
  }
  error(title: string, detail?: string, correlationId?: string): number {
    return this.show({ kind: 'error', title, detail, correlationId }, 9000);
  }
  dismiss(id: number): void { this.messages.update(items => items.filter(item => item.id !== id)); }
}
