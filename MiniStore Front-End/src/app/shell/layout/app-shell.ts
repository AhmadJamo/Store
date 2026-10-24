import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { LocalizationService, MessageKey } from '../../core/localization/localization.service';
import { DensityPreference, ThemePreference, UiPreferencesService } from '../../core/preferences/ui-preferences.service';
import { WorkspaceService } from '../../workspace/state/workspace.service';

@Component({
  selector: 'app-shell',
  imports: [RouterLink, RouterLinkActive, RouterOutlet],
  templateUrl: './app-shell.html',
  styleUrl: './app-shell.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AppShell {
  protected readonly i18n = inject(LocalizationService);
  protected readonly workspace = inject(WorkspaceService);
  protected readonly preferences = inject(UiPreferencesService);
  protected readonly navigation: readonly { icon: string; key: MessageKey; route: string }[] = [
    { icon: '⌂', key: 'dashboard', route: '/' },
    { icon: '◇', key: 'designSystem', route: '/design-system' },
  ];

  protected setTheme(value: string): void { this.preferences.setTheme(value as ThemePreference); }
  protected setDensity(value: string): void { this.preferences.setDensity(value as DensityPreference); }
}
