import { DOCUMENT } from '@angular/common';
import { ChangeDetectionStrategy, Component, DestroyRef, HostListener, inject, OnInit } from '@angular/core';
import { NavigationEnd, Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { filter } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { LocalizationService } from '../../core/localization/localization.service';
import { ContentWidthPreference, DensityPreference, SidebarPreference, ThemePreference, UiPreferencesService } from '../../core/preferences/ui-preferences.service';
import { WorkspaceProfile, WorkspaceProfileService } from '../../core/preferences/workspace-profile.service';
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
  protected readonly workspaceProfile = inject(WorkspaceProfileService);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);
  private readonly document = inject(DOCUMENT);
  private dialogOpener: HTMLElement | null = null;

  ngOnInit(): void {
    this.workspace.syncRoute(this.router.url);
    this.router.events.pipe(
      filter((event): event is NavigationEnd => event instanceof NavigationEnd),
      takeUntilDestroyed(this.destroyRef),
    ).subscribe((event) => { this.workspace.syncRoute(event.urlAfterRedirects); this.focusMain(); });
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
}
