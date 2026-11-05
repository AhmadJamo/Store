import { DOCUMENT } from '@angular/common';
import { ChangeDetectionStrategy, Component, DestroyRef, HostListener, computed, inject, OnInit, signal } from '@angular/core';
import { NavigationEnd, Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { filter } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { LocalizationService } from '../../core/localization/localization.service';
import { ContentWidthPreference, DensityPreference, SidebarPreference, ThemePreference, UiPreferencesService } from '../../core/preferences/ui-preferences.service';
import { WorkspaceProfile, WorkspaceProfileService } from '../../core/preferences/workspace-profile.service';
import { NavigationGroupId, NavigationRegistryService } from '../../workspace/navigation/navigation-registry.service';
import { NavigationItem, WorkspaceTab } from '../../workspace/state/workspace.models';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { AccountApiService } from '../../core/api/account-api.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { NavigationGroupStateService } from '../../workspace/navigation/navigation-group-state.service';
import { NavigationLocalizationService } from '../../workspace/navigation/navigation-localization.service';
import { ShellNavigationLocalizationService } from './shell-navigation-localization.service';

@Component({
  selector: 'app-shell',
  imports: [RouterLink, RouterLinkActive, RouterOutlet],
  templateUrl: './app-shell.html',
  styleUrl: './app-shell.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AppShell implements OnInit {
  protected readonly navigationI18n = inject(NavigationLocalizationService);
  protected readonly i18n = inject(LocalizationService);
  protected readonly workspace = inject(WorkspaceService);
  protected readonly preferences = inject(UiPreferencesService);
  protected readonly navigation = inject(NavigationRegistryService);
  protected readonly navigationGroups = inject(NavigationGroupStateService);
  protected readonly shellI18n = inject(ShellNavigationLocalizationService);
  protected readonly workspaceProfile = inject(WorkspaceProfileService);
  protected readonly session = inject(ApiSessionService);
  protected readonly signingOut = signal(false);
  protected readonly signOutFailed = signal(false);
  protected readonly searchQuery = signal('');
  protected readonly searchOpen = signal(false);
  protected readonly searchResults = computed(() => {
    const query = this.searchQuery().trim().toLocaleLowerCase(this.i18n.locale());
    if (!query) return [];
    return this.navigation.visibleItems().filter(item => {
      const label = this.navigationI18n.text(item.labelKey).toLocaleLowerCase(this.i18n.locale());
      const group = this.shellI18n.group(this.navigation.groupForItem(item)).toLocaleLowerCase(this.i18n.locale());
      return label.includes(query) || group.includes(query);
    }).slice(0, 8);
  });
  protected readonly recentDestinations = computed(() => {
    const seen = new Set<string>();
    return this.workspace.recentlyClosed().flatMap(tab => {
      const item = this.navigation.findById(tab.navigationId);
      if (!item || !this.navigation.canAccess(item) || seen.has(item.id)) return [];
      seen.add(item.id);
      return [item];
    }).slice(0, 5);
  });
  protected readonly displayedSearchItems = computed(() => this.searchQuery().trim() ? this.searchResults() : this.recentDestinations());
  private readonly account = inject(AccountApiService);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);
  private readonly document = inject(DOCUMENT);
  private dialogOpener: HTMLElement | null = null;
  protected readonly currentRoute = signal('/');

  ngOnInit(): void {
    this.currentRoute.set(this.router.url);
    this.workspace.syncRoute(this.router.url);
    this.navigationGroups.ensureOpen(this.navigation.groupForRoute(this.router.url));
    this.router.events.pipe(
      filter((event): event is NavigationEnd => event instanceof NavigationEnd),
      takeUntilDestroyed(this.destroyRef),
    ).subscribe((event) => {
      this.currentRoute.set(event.urlAfterRedirects);
      this.workspace.syncRoute(event.urlAfterRedirects);
      this.navigationGroups.ensureOpen(this.navigation.groupForRoute(event.urlAfterRedirects));
      this.focusMain();
    });
  }

  protected currentNavigationItem(): NavigationItem | undefined { return this.navigation.findByRoute(this.currentRoute()); }

  protected onNavigationGroupToggle(groupId: NavigationGroupId, event: Event): void {
    this.navigationGroups.setOpen(groupId, (event.currentTarget as HTMLDetailsElement).open);
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

  protected updateSearch(value: string): void {
    this.searchQuery.set(value.slice(0, 80));
    this.searchOpen.set(true);
  }

  protected navigateSearch(item: NavigationItem): void {
    const targetId = `${item.id}::default`;
    if (!this.confirmLeave(targetId)) return;
    const tab = this.workspace.open(item);
    if (tab) void this.router.navigateByUrl(tab.route);
    this.closeSearch();
  }

  protected onSearchKeydown(event: KeyboardEvent): void {
    if (event.key === 'Escape') {
      event.preventDefault();
      this.closeSearch();
      return;
    }
    if (event.key === 'ArrowDown') {
      event.preventDefault();
      this.document.querySelector<HTMLElement>('.search-results button')?.focus();
      return;
    }
    if (event.key === 'Enter') {
      const first = this.displayedSearchItems()[0];
      if (first) {
        event.preventDefault();
        this.navigateSearch(first);
      }
    }
  }

  protected onSearchFocusOut(event: FocusEvent): void {
    const container = event.currentTarget as HTMLElement;
    if (!container.contains(event.relatedTarget as Node | null)) this.searchOpen.set(false);
  }

  @HostListener('document:keydown', ['$event'])
  protected onGlobalKeydown(event: KeyboardEvent): void {
    const target = event.target as HTMLElement | null;
    const typing = !!target && ['INPUT', 'TEXTAREA', 'SELECT'].includes(target.tagName);
    const shortcut = (event.ctrlKey || event.metaKey) && event.key.toLocaleLowerCase() === 'k';
    if (!shortcut && !(event.key === '/' && !typing)) return;
    event.preventDefault();
    this.searchOpen.set(true);
    queueMicrotask(() => this.document.getElementById('workspace-command-search')?.focus());
  }

  private closeSearch(): void {
    this.searchOpen.set(false);
    this.searchQuery.set('');
  }

  protected skipToMain(event: Event): void {
    event.preventDefault();
    this.focusMain();
  }

  protected activate(tab: WorkspaceTab): void {
    if (!this.confirmLeave(tab.id)) return;
    this.workspace.activate(tab.id);
    void this.router.navigateByUrl(tab.route);
  }

  protected close(event: Event, tab: WorkspaceTab): void {
    event.stopPropagation();
    const result = this.workspace.requestClose(tab.id);
    if (result === 'confirmation-required') this.openDialogFrom(event);
    if (result === 'closed') this.navigateToActive();
  }

  protected confirmClose(): void { this.workspace.confirmClose(); this.navigateToActive(); this.restoreDialogFocus(); }
  protected cancelClose(): void { this.workspace.cancelClose(); this.restoreDialogFocus(); }
  protected closeOthers(event: Event, tab: WorkspaceTab): void { if (this.workspace.closeOthers(tab.id) === 'confirmation-required') this.openDialogFrom(event); }
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

  protected onDialogKeydown(event: KeyboardEvent): void {
    if (event.key === 'Escape') { event.preventDefault(); this.cancelClose(); return; }
    if (event.key !== 'Tab') return;
    const buttons = [...this.document.querySelectorAll<HTMLElement>('.confirm-dialog button:not(:disabled)')];
    if (!buttons.length) return;
    const index = buttons.indexOf(this.document.activeElement as HTMLElement);
    const next = event.shiftKey ? (index <= 0 ? buttons.length - 1 : index - 1) : (index >= buttons.length - 1 ? 0 : index + 1);
    event.preventDefault(); buttons[next].focus();
  }

  private confirmHistoryMove(): boolean { return !this.workspace.activeTab()?.dirty || globalThis.confirm(this.i18n.text('unsavedDescription')); }
  private confirmLeave(targetId: string): boolean { return !this.workspace.requiresLeaveConfirmation(targetId) || globalThis.confirm(this.i18n.text('unsavedDescription')); }
  private navigateToActive(): void { const active = this.workspace.activeTab(); void this.router.navigateByUrl(active?.route ?? '/'); }
  private focusMain(): void { queueMicrotask(() => this.document.getElementById('main-content')?.focus()); }
  private openDialogFrom(event: Event): void { this.dialogOpener = event.currentTarget as HTMLElement; queueMicrotask(() => this.document.querySelector<HTMLElement>('.confirm-dialog button')?.focus()); }
  private restoreDialogFocus(): void { const opener = this.dialogOpener; this.dialogOpener = null; queueMicrotask(() => opener?.isConnected && opener.focus()); }

  protected setTheme(value: string): void { this.preferences.setTheme(value as ThemePreference); }
  protected setDensity(value: string): void { this.preferences.setDensity(value as DensityPreference); }
  protected setSidebar(value: string): void { this.preferences.setSidebar(value as SidebarPreference); }
  protected setContentWidth(value: string): void { this.preferences.setContentWidth(value as ContentWidthPreference); }
  protected setWorkspaceProfile(value: string): void { this.workspaceProfile.setProfile(value as WorkspaceProfile); }

  protected signOut(): void {
    if (this.signingOut()) return;
    if (this.workspace.tabs().some(tab => tab.dirty) && !globalThis.confirm(this.i18n.text('unsavedDescription'))) return;
    this.signingOut.set(true);
    this.signOutFailed.set(false);
    this.account.logout().subscribe({
      next: () => {
        this.session.clear();
        this.signingOut.set(false);
        void this.router.navigate(['/login']);
      },
      error: () => {
        this.signingOut.set(false);
        this.signOutFailed.set(true);
      },
    });
  }
}
