import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { LocalizationService } from '../../core/localization/localization.service';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { NavigationRegistryService } from '../../workspace/navigation/navigation-registry.service';
import { CommandCenterPreferencesService } from '../command-center/command-center-preferences.service';
import { CommandCenterRegistryService } from '../command-center/command-center-registry.service';
import { CommandCenterViewMode, QuickPreviewProviderId, QuickPreviewResult } from '../command-center/command-center.models';

@Component({
  selector: 'app-home-page',
  imports: [FormsModule, RouterLink, PageHeaderComponent, FeedbackStateComponent],
  templateUrl: './home-page.html',
  styleUrl: './home-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HomePage {
  protected readonly i18n = inject(LocalizationService);
  protected readonly registry = inject(CommandCenterRegistryService);
  protected readonly preferences = inject(CommandCenterPreferencesService);
  protected readonly navigation = inject(NavigationRegistryService);
  private readonly errors = inject(ApiErrorService);
  protected readonly selectedProvider = signal<QuickPreviewProviderId | null>(this.registry.availableProviders()[0]?.id ?? null);
  protected readonly viewMode = signal<CommandCenterViewMode>('comfortable');
  protected readonly search = signal('');
  protected readonly loading = signal(false);
  protected readonly problem = signal('');
  protected readonly preview = signal<QuickPreviewResult | null>(null);
  protected readonly matchingCommands = computed(() => {
    const query = this.search().trim().toLocaleLowerCase(this.i18n.locale());
    return this.navigation.visibleItems().filter((item) => !query || this.i18n.text(item.labelKey).toLocaleLowerCase(this.i18n.locale()).includes(query));
  });

  constructor() { if (this.selectedProvider()) this.loadPreview(); }

  protected chooseProvider(id: QuickPreviewProviderId): void { this.selectedProvider.set(id); this.loadPreview(); }
  protected loadPreview(): void {
    const providerId = this.selectedProvider();
    if (!providerId) return;
    this.loading.set(true); this.problem.set('');
    this.registry.load(providerId, this.search()).subscribe({
      next: (result) => { this.preview.set(result); this.loading.set(false); },
      error: (error) => { this.preview.set(null); this.problem.set(this.errors.normalize(error).title ?? this.i18n.text('errorDescription')); this.loading.set(false); },
    });
  }
  protected toggleViewMode(): void { this.viewMode.update((value) => value === 'comfortable' ? 'compact' : 'comfortable'); }
  protected saveSnapshot(): void { const provider = this.selectedProvider(); if (provider) this.preferences.saveSnapshot(provider, this.viewMode()); }
  protected applySnapshot(providerId: QuickPreviewProviderId, viewMode: CommandCenterViewMode): void { this.selectedProvider.set(providerId); this.viewMode.set(viewMode); this.loadPreview(); }
}
