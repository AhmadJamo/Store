import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class FormFocusService {
  focusFirstInvalid(container: HTMLElement): void {
    queueMicrotask(() => container.querySelector<HTMLElement>('[formControlName].ng-invalid')?.focus());
  }

  focusField(container: HTMLElement, serverName: string): void {
    const clean = serverName.replace(/^Form\./i, '');
    const name = clean ? clean[0].toLocaleLowerCase() + clean.slice(1) : '';
    if (!/^[A-Za-z][A-Za-z0-9_]*$/.test(name)) return;
    queueMicrotask(() => container.querySelector<HTMLElement>(`[formControlName="${name}"]`)?.focus());
  }
}
