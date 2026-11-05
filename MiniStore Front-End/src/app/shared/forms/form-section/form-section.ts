import { ChangeDetectionStrategy, Component, input } from '@angular/core';

@Component({
  selector: 'app-form-section',
  templateUrl: './form-section.html',
  styleUrl: './form-section.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FormSectionComponent {
  readonly title = input.required<string>();
  readonly description = input<string>('');
}
