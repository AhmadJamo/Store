import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output, inject } from '@angular/core';
import { NavigationLocalizationService } from './navigation-localization.service';
import { NavigationApplicationId, NavigationRegistryService } from './navigation-registry.service';
import { NavigationItem } from '../state/workspace.models';

@Component({
  selector: 'app-application-switcher',
  templateUrl: './application-switcher.html',
  styleUrl: './application-switcher.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ApplicationSwitcherComponent {
  protected readonly navigation = inject(NavigationRegistryService);
  protected readonly navigationI18n = inject(NavigationLocalizationService);

  @Input() currentApplicationId: NavigationApplicationId | null = null;
  @Output() readonly applicationSelected = new EventEmitter<NavigationItem>();

  protected select(event: Event, entry: NavigationItem): void {
    event.preventDefault();
    this.applicationSelected.emit(entry);
    (event.currentTarget as HTMLElement).closest<HTMLDetailsElement>('details')?.removeAttribute('open');
  }
}
