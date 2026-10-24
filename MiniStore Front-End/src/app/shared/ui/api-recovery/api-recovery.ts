import { ChangeDetectionStrategy, Component, inject, input, output, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { NormalizedApiProblem } from '../../../core/api/api.models';
import { LocalizationService, MessageKey } from '../../../core/localization/localization.service';

@Component({
  selector: 'app-api-recovery',
  imports: [RouterLink],
  templateUrl: './api-recovery.html',
  styleUrl: './api-recovery.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ApiRecoveryComponent {
  protected readonly i18n = inject(LocalizationService);
  readonly problem = input.required<NormalizedApiProblem>();
  readonly allowRetry = input(true);
  readonly sourceUrl = input<string | null>(null);
  readonly retry = output<void>();
  protected readonly copied = signal(false);

  protected descriptionKey(): MessageKey {
    return ({
      validation: 'recoveryValidation', duplicate: 'recoveryDuplicate', concurrency: 'recoveryConcurrency',
      notFound: 'recoveryNotFound', unauthorized: 'recoveryUnauthorized', forbidden: 'recoveryForbidden',
      transient: 'recoveryTransient', unknown: 'recoveryUnknown',
    } as const)[this.problem().kind];
  }

  protected async copyReference(): Promise<void> {
    const reference = this.problem().correlationId;
    if (!reference) return;
    try {
      await globalThis.navigator?.clipboard?.writeText(reference);
      this.copied.set(true);
    } catch { this.copied.set(false); }
  }
}
