import { ChangeDetectionStrategy, Component, input } from '@angular/core';

export interface SummaryStripItem {
  readonly label: string;
  readonly value: string | number;
  readonly detail?: string;
  readonly tone?: 'neutral' | 'success' | 'warning' | 'danger';
}

@Component({
  selector: 'app-summary-strip',
  templateUrl: './summary-strip.html',
  styleUrl: './summary-strip.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SummaryStripComponent {
  readonly label = input.required<string>();
  readonly items = input.required<readonly SummaryStripItem[]>();
}
