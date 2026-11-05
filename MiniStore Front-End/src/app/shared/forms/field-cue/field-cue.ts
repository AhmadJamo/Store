import { ChangeDetectionStrategy, Component, input } from '@angular/core';

@Component({
  selector: 'app-field-cue',
  templateUrl: './field-cue.html',
  styleUrl: './field-cue.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FieldCueComponent {
  readonly label = input.required<string>();
  readonly required = input(false);
  readonly requiredText = input.required<string>();
  readonly optionalText = input.required<string>();
}
