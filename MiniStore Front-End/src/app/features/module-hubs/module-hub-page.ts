import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { NavigationLocalizationService, NavigationTextKey } from '../../workspace/navigation/navigation-localization.service';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { NavigationGroupId, NavigationRegistryService } from '../../workspace/navigation/navigation-registry.service';

@Component({
  selector: 'app-module-hub-page',
  imports: [RouterLink, PageHeaderComponent, FeedbackStateComponent],
  templateUrl: './module-hub-page.html',
  styleUrl: './module-hub-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ModuleHubPageComponent {
  protected readonly i18n = inject(NavigationLocalizationService);
  private readonly route = inject(ActivatedRoute);
  private readonly navigation = inject(NavigationRegistryService);
  protected readonly titleKey = this.route.snapshot.data['titleKey'] as NavigationTextKey;
  protected readonly descriptionKey = this.route.snapshot.data['descriptionKey'] as NavigationTextKey;
  private readonly groupId = this.route.snapshot.data['groupId'] as NavigationGroupId;
  protected readonly sections = computed(() => {
    const items = this.navigation.visibleItems().filter(
      (item) => this.navigation.groupForItem(item) === this.groupId && item.hubSectionKey,
    );
    return [...new Set(items.map((item) => item.hubSectionKey!))].map((key) => ({
      key,
      items: items.filter((item) => item.hubSectionKey === key),
    }));
  });
}
