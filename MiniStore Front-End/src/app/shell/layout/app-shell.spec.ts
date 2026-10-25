import { Component, signal } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { LocalizationService } from '../../core/localization/localization.service';
import { UiPreferencesService } from '../../core/preferences/ui-preferences.service';
import { NavigationRegistryService } from '../../workspace/navigation/navigation-registry.service';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { AppShell } from './app-shell';

@Component({ template: '' }) class EmptyRoute {}

describe('AppShell accessibility', () => {
  const pending = signal<any>(null);
  const workspace = {
    tabs: signal<any[]>([]), activeTab: signal<any>(null), recentlyClosed: signal<any[]>([]), pendingCloseTab: pending,
    canGoBack: signal(false), canGoForward: signal(false), syncRoute: vi.fn(), cancelClose: vi.fn(() => pending.set(null)),
    confirmClose: vi.fn(), requiresLeaveConfirmation: vi.fn(() => false), open: vi.fn(), activate: vi.fn(), requestClose: vi.fn(),
    closeOthers: vi.fn(), reopenLastClosed: vi.fn(), back: vi.fn(), forward: vi.fn(),
  };
  let fixture: ComponentFixture<AppShell>;

  beforeEach(async () => {
    vi.clearAllMocks(); pending.set(null);
    await TestBed.configureTestingModule({
      imports: [AppShell],
      providers: [
        provideRouter([{ path: '', component: EmptyRoute }]),
        { provide: WorkspaceService, useValue: workspace },
        { provide: NavigationRegistryService, useValue: { visibleItems: signal([]) } },
        { provide: LocalizationService, useValue: { direction: signal('ltr'), text: (key: string) => key, toggle: vi.fn() } },
        { provide: UiPreferencesService, useValue: { theme: signal('system'), density: signal('comfortable'), sidebar: signal('expanded'), contentWidth: signal('fluid'), reset: vi.fn(), setTheme: vi.fn(), setDensity: vi.fn(), setSidebar: vi.fn(), setContentWidth: vi.fn() } },
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
});
