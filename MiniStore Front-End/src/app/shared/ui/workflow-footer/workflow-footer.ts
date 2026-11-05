import { ChangeDetectionStrategy, Component, input } from '@angular/core';

@Component({
  selector: 'app-workflow-footer',
  templateUrl: './workflow-footer.html',
  styleUrl: './workflow-footer.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class WorkflowFooterComponent {
  readonly label = input.required<string>();
}
