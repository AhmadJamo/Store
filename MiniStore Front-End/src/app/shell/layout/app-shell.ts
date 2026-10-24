import { ChangeDetectionStrategy, Component, DestroyRef, HostListener, inject, OnInit } from '@angular/core';
import { NavigationEnd, Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { filter } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { LocalizationService } from '../../core/localization/localization.service';
import { DensityPreference, ThemePreference, UiPreferencesService } from '../../core/preferences/ui-preferences.service';
import { NavigationRegistryService } from '../../workspace/navigation/navigation-registry.service';
import { NavigationItem, WorkspaceTab } from '../../workspace/state/workspace.models';
import { WorkspaceService } from '../../workspace/state/workspace.service';

@Component({
  selector: 'app-shell',
  imports: [RouterLink, RouterLinkActive, RouterOutlet],
  templateUrl: './app-shell.html',
  styleUrl: './app-shell.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AppShell implements OnInit {
  protected readonly i18n = inject(LocalizationService);
  protected readonly workspace = inject(WorkspaceService);
  protected readonly preferences = inject(UiPreferencesService);
  protected readonly navigation = inject(NavigationRegistryService);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  ngOnInit(): void {
    this.workspace.syncRoute(this.router.url);
    this.router.events.pipe(
      filter((event): event is NavigationEnd => event instanceof NavigationEnd),
      takeUntilDestroyed(this.destroyRef),
    ).subscribe((event) => this.workspace.syncRoute(event.urlAfterRedirects));
  }

  @HostListener('window:beforeunload', ['$event'])
  protected protectUnsavedRefresh(event: BeforeUnloadEvent): void {
    if (this.workspace.tabs().some((tab) => tab.dirty)) event.preventDefault();
  }

  protected openNavigation(event: Event, item: NavigationItem): void {
    event.preventDefault();
    const targetId = `${item.id}::default`;
    if (!this.confirmLeave(targetId)) return;
    const tab = this.workspace.open(item);
    if (tab) void this.router.navigateByUrl(tab.route);
  }

  protected activate(tab: WorkspaceTab): void {
    if (!this.confirmLeave(tab.id)) return;
    this.workspace.activate(tab.id);
    void this.router.navigateByUrl(tab.route);
  }

  protected close(event: Event, tab: WorkspaceTab): void {
    event.stopPropagation();
    const result = this.workspace.requestClose(tab.id);
    if (result === 'closed') this.navigateToActive();
  }

  protected confirmClose(): void { this.workspace.confirmClose(); this.navigateToActive(); }
  protected closeOthers(tab: WorkspaceTab): void { this.workspace.closeOthers(tab.id); }
  protected reopen(): void { const tab = this.workspace.reopenLastClosed(); if (tab) void this.router.navigateByUrl(tab.route); }
  protected back(): void { if (this.confirmHistoryMove()) { const tab = this.workspace.back(); if (tab) void this.router.navigateByUrl(tab.route); } }
  protected forward(): void { if (this.confirmHistoryMove()) { const tab = this.workspace.forward(); if (tab) void this.router.navigateByUrl(tab.route); } }

  protected onTabKeydown(event: KeyboardEvent, index: number): void {
    if (!['ArrowLeft', 'ArrowRight', 'Home', 'End'].includes(event.key)) return;
    event.preventDefault();
    const tabs = this.workspace.tabs();
    const rtlOffset = this.i18n.direction() === 'rtl' ? -1 : 1;
    const nextIndex = event.key === 'Home' ? 0 : event.key === 'End' ? tabs.length - 1
      : (index + (event.key === 'ArrowRight' ? rtlOffset : -rtlOffset) + tabs.length) % tabs.length;
    const next = tabs[nextIndex];
    if (next) this.activate(next);
  }

  private confirmHistoryMove(): boolean { return !this.workspace.activeTab()?.dirty || globalThis.confirm(this.i18n.text('unsavedDescription')); }
  private confirmLeave(targetId: string): boolean { return !this.workspace.requiresLeaveConfirmation(targetId) || globalThis.confirm(this.i18n.text('unsavedDescription')); }
  private navigateToActive(): void { const active = this.workspace.activeTab(); void this.router.navigateByUrl(active?.route ?? '/'); }

  protected setTheme(value: string): void { this.preferences.setTheme(value as ThemePreference); }
  protected setDensity(value: string): void { this.preferences.setDensity(value as DensityPreference); }
}
