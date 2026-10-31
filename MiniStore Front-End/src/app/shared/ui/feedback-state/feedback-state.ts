import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';

export type FeedbackKind = 'loading' | 'empty' | 'error';

@Component({ selector: 'app-feedback-state', templateUrl: './feedback-state.html', styleUrl: './feedback-state.scss', changeDetection: ChangeDetectionStrategy.OnPush })
export class FeedbackStateComponent {
  readonly kind = input.required<FeedbackKind>();
  readonly title = input.required<string>();
  readonly description = input<string>('');
  readonly retryLabel = input<string>('');
  readonly retry = output<void>();
}
