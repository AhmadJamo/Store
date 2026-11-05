import { ChangeDetectionStrategy, Component, input } from '@angular/core';

export type WorkflowStepState = 'complete' | 'current' | 'upcoming' | 'stopped';

export interface WorkflowStep {
  readonly label: string;
  readonly state: WorkflowStepState;
}

@Component({
  selector: 'app-workflow-progress',
  templateUrl: './workflow-progress.html',
  styleUrl: './workflow-progress.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class WorkflowProgressComponent {
  readonly label = input.required<string>();
  readonly steps = input.required<readonly WorkflowStep[]>();
  readonly guidance = input<string>('');
}
