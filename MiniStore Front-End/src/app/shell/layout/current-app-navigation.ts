import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output, inject } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { NavigationGroup, NavigationGroupId } from '../../workspace/navigation/navigation-registry.service';
import { NavigationItem } from '../../workspace/state/workspace.models';
import { NavigationGroupStateService } from '../../workspace/navigation/navigation-group-state.service';
import { NavigationLocalizationService } from '../../workspace/navigation/navigation-localization.service';
import { LocalizationService } from '../../core/localization/localization.service';
import { ShellNavigationLocalizationService } from './shell-navigation-localization.service';

@Component({
  selector: 'app-current-app-navigation',
  host: { '[class.mobile-mode]': "mode === 'mobile'" },
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './current-app-navigation.html',
  styleUrl: './current-app-navigation.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CurrentAppNavigationComponent {
  protected readonly groupsState = inject(NavigationGroupStateService);
  protected readonly navigationI18n = inject(NavigationLocalizationService);
  protected readonly shellI18n = inject(ShellNavigationLocalizationService);
  protected readonly i18n = inject(LocalizationService);

  @Input({ required: true }) groups: readonly NavigationGroup[] = [];
  @Input() mode: 'sidebar' | 'mobile' = 'sidebar';
  @Output() readonly navigationSelected = new EventEmitter<NavigationItem>();

  protected toggle(groupId: NavigationGroupId, event: Event): void {
    this.groupsState.setOpen(groupId, (event.currentTarget as HTMLDetailsElement).open);
  }

  protected select(event: Event, item: NavigationItem): void {
    event.preventDefault();
    this.navigationSelected.emit(item);
    if (this.mode === 'mobile') {
      (event.currentTarget as HTMLElement).closest<HTMLDetailsElement>('.mobile-navigation')?.removeAttribute('open');
    }
  }
}
