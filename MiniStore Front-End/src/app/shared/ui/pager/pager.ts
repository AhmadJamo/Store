import { ChangeDetectionStrategy, Component, input, output } from '@angular/core';

@Component({ selector: 'app-pager', templateUrl: './pager.html', styleUrl: './pager.scss', changeDetection: ChangeDetectionStrategy.OnPush })
export class PagerComponent {
  readonly page = input.required<number>();
  readonly totalPages = input.required<number>();
  readonly totalCount = input.required<number>();
  readonly totalLabel = input.required<string>();
  readonly previousLabel = input.required<string>();
  readonly nextLabel = input.required<string>();
  readonly pageChange = output<number>();
}
