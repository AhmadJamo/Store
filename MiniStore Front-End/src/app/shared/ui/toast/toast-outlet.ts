import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { LocalizationService } from '../../../core/localization/localization.service';
import { ToastService } from './toast.service';

@Component({
  selector: 'app-toast-outlet',
  template: `
    <section class="toast-region" aria-live="assertive" aria-atomic="false">
      @for (message of toasts.messages(); track message.id) {
        <article class="toast" [class]="message.kind" role="alert">
          <div><strong>{{ message.title }}</strong>@if(message.detail){<p>{{ message.detail }}</p>}@if(message.correlationId){<small>{{ i18n.text('supportReference') }}: {{ message.correlationId }}</small>}</div>
          <button type="button" (click)="toasts.dismiss(message.id)" [attr.aria-label]="i18n.text('close')">×</button>
        </article>
      }
    </section>`,
  styles: [`
    .toast-region{position:fixed;z-index:160;inset-block-start:1rem;inset-inline-end:1rem;display:grid;gap:.65rem;width:min(25rem,calc(100vw - 2rem));pointer-events:none}.toast{pointer-events:auto;display:flex;justify-content:space-between;gap:1rem;padding:.9rem 1rem;border:1px solid var(--ui-border);border-inline-start:4px solid var(--erp-color-danger);border-radius:var(--ui-radius);color:var(--ui-text);background:var(--ui-surface);box-shadow:var(--erp-shadow-md)}.toast.warning{border-inline-start-color:var(--erp-color-warning)}.toast.success{border-inline-start-color:var(--erp-color-success)}.toast.info{border-inline-start-color:var(--erp-color-primary-strong)}.toast strong,.toast p,.toast small{display:block;margin:0}.toast p{margin-block-start:.25rem;color:var(--ui-muted);font-size:.88rem}.toast small{margin-block-start:.35rem;color:var(--ui-muted);font-size:.72rem;direction:ltr;text-align:start}.toast button{align-self:start;border:0;background:transparent;color:inherit;font-size:1.25rem;cursor:pointer}@media(max-width:680px){.toast-region{inset-block-start:.5rem;inset-inline:.5rem;width:auto}}
  `],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ToastOutletComponent { protected readonly toasts=inject(ToastService); protected readonly i18n=inject(LocalizationService); }
