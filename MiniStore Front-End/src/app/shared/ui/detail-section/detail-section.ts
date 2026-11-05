import { ChangeDetectionStrategy, Component, input } from '@angular/core';

@Component({
  selector: 'app-detail-section',
  templateUrl: './detail-section.html',
  styleUrl: './detail-section.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class DetailSectionComponent {
  readonly title = input.required<string>();
  readonly description = input<string>('');
}
