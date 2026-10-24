import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { LocalizationService, MessageKey } from '../../core/localization/localization.service';
import { WorkspaceService } from '../../workspace/state/workspace.service';

@Component({
  selector: 'app-shell',
  imports: [RouterOutlet],
  templateUrl: './app-shell.html',
  styleUrl: './app-shell.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AppShell {
  protected readonly i18n = inject(LocalizationService);
  protected readonly workspace = inject(WorkspaceService);
  protected readonly navigation: readonly { icon: string; key: MessageKey }[] = [
    { icon: '⌂', key: 'dashboard' },
    { icon: '◎', key: 'customers' },
    { icon: '↗', key: 'navigation' },
  ];
}
