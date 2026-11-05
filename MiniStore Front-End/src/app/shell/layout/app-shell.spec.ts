import { Component, signal } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { LocalizationService } from '../../core/localization/localization.service';
import { UiPreferencesService } from '../../core/preferences/ui-preferences.service';
import { NavigationRegistryService } from '../../workspace/navigation/navigation-registry.service';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { AppShell } from './app-shell';
import { AccountApiService } from '../../core/api/account-api.service';
import { ApiSessionService } from '../../core/api/api-session.service';

@Component({ template: '' }) class EmptyRoute {}

describe('AppShell accessibility', () => {
  const pending = signal<any>(null);
  const permittedItems = signal<any[]>([]);
  const workspace = {
    tabs: signal<any[]>([]), activeTab: signal<any>(null), recentlyClosed: signal<any[]>([]), pendingCloseTab: pending,
    canGoBack: signal(false), canGoForward: signal(false), syncRoute: vi.fn(), cancelClose: vi.fn(() => pending.set(null)),
    confirmClose: vi.fn(), requiresLeaveConfirmation: vi.fn(() => false), open: vi.fn(), activate: vi.fn(), requestClose: vi.fn(),
    closeOthers: vi.fn(), reopenLastClosed: vi.fn(), back: vi.fn(), forward: vi.fn(),
  };
  let fixture: ComponentFixture<AppShell>;

  beforeEach(async () => {
    vi.clearAllMocks(); pending.set(null);
    permittedItems.set([{ id: 'customers', route: '/customers', labelKey: 'customersTitle', icon: 'C' }]);
    await TestBed.configureTestingModule({
      imports: [AppShell],
      providers: [
        provideRouter([{ path: '', component: EmptyRoute }]),
        { provide: WorkspaceService, useValue: workspace },
        { provide: NavigationRegistryService, useValue: { visibleItems: permittedItems, visibleGroups: signal([]), findByRoute: vi.fn(), findById: vi.fn(), canAccess: vi.fn(() => true), groupForRoute: vi.fn(), groupForItem: vi.fn(() => 'partners') } },
        { provide: LocalizationService, useValue: { locale: signal('en-US'), direction: signal('ltr'), text: (key: string) => key, toggle: vi.fn(), switching: signal(false), switchError: signal(false) } },
        { provide: UiPreferencesService, useValue: { theme: signal('system'), density: signal('comfortable'), sidebar: signal('expanded'), contentWidth: signal('fluid'), reset: vi.fn(), setTheme: vi.fn(), setDensity: vi.fn(), setSidebar: vi.fn(), setContentWidth: vi.fn() } },
        { provide: ApiSessionService, useValue: { session: signal(null), clear: vi.fn() } },
        { provide: AccountApiService, useValue: { logout: vi.fn() } },
      ],
    }).compileComponents();
    fixture = TestBed.createComponent(AppShell); fixture.detectChanges();
  });

  it('provides a skip link and a programmatically focusable main region', async () => {
    const skip = fixture.nativeElement.querySelector('.skip-link') as HTMLAnchorElement;
    const main = fixture.nativeElement.querySelector('#main-content') as HTMLElement;
    expect(skip.getAttribute('href')).toBe('#main-content');
    expect(main.tabIndex).toBe(-1);
    skip.click(); await Promise.resolve();
    expect(document.activeElement).toBe(main);
  });

  it('keeps dialog focus inside and closes it with Escape', async () => {
    pending.set({ titleKey: 'customersTitle' }); fixture.detectChanges();
    const buttons = fixture.nativeElement.querySelectorAll('.confirm-dialog button') as NodeListOf<HTMLButtonElement>;
    buttons[buttons.length - 1].focus();
    (fixture.componentInstance as any).onDialogKeydown(new KeyboardEvent('keydown', { key: 'Tab', bubbles: true, cancelable: true }));
    expect(document.activeElement).toBe(buttons[0]);
    (fixture.componentInstance as any).onDialogKeydown(new KeyboardEvent('keydown', { key: 'Escape', bubbles: true, cancelable: true }));
    expect(workspace.cancelClose).toHaveBeenCalled();
  });

  it('searches only permission-filtered destinations and opens them through the workspace', () => {
    (fixture.componentInstance as any).updateSearch('customers');
    fixture.detectChanges();
    const results = fixture.nativeElement.querySelectorAll('.search-results button') as NodeListOf<HTMLButtonElement>;
    expect(results.length).toBe(1);
    expect(results[0].textContent).toContain('customersTitle');
    expect(fixture.nativeElement.textContent).not.toContain('settings');
    results[0].click();
    expect(workspace.open).toHaveBeenCalledWith(permittedItems()[0]);
  });

  it('focuses command search with Ctrl+K without persisting the query', async () => {
    const event = new KeyboardEvent('keydown', { key: 'k', ctrlKey: true, bubbles: true, cancelable: true });
    (fixture.componentInstance as any).onGlobalKeydown(event);
    await Promise.resolve();
    expect(document.activeElement?.id).toBe('workspace-command-search');
    expect(localStorage.getItem('ministore.ui.command-search')).toBeNull();
  });
});
