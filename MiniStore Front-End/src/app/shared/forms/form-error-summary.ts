import { ChangeDetectionStrategy, Component, computed, input, output } from '@angular/core';

@Component({ selector: 'app-form-error-summary', templateUrl: './form-error-summary.html', styleUrl: './form-error-summary.scss', changeDetection: ChangeDetectionStrategy.OnPush })
export class FormErrorSummaryComponent {
  readonly title = input.required<string>();
  readonly errors = input<Record<string, readonly string[]>>({});
  readonly fieldRequested = output<string>();
  readonly entries = computed(() => Object.entries(this.errors()).flatMap(([field, messages]) => messages.map((message) => ({ field, message }))));
}
