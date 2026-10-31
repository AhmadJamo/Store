import { TestBed } from '@angular/core/testing';
import { ApiSessionService } from '../../core/api/api-session.service';
import { NavigationRegistryService } from '../navigation/navigation-registry.service';
import { NavigationItem } from './workspace.models';
import { WorkspaceService } from './workspace.service';

describe('WorkspaceService', () => {
  const home: NavigationItem = { id: 'home', route: '/', labelKey: 'dashboard', icon: '⌂' };
  const design: NavigationItem = { id: 'design-system', route: '/design-system', labelKey: 'designSystem', icon: '◇' };

  beforeEach(() => {
    localStorage.clear();
    TestBed.configureTestingModule({
      providers: [
        NavigationRegistryService,
        WorkspaceService,
        { provide: ApiSessionService, useValue: { can: (permission: string) => permission !== 'Denied' } },
      ],
    });
  });

  it('deduplicates the same navigation context and separates explicit contexts', () => {
    const workspace = TestBed.inject(WorkspaceService);
    workspace.open(design);
    workspace.open(design);
    workspace.open(design, design.route, 'preview-b');

    expect(workspace.tabs()).toHaveLength(2);
    expect(workspace.activeTab()?.contextKey).toBe('preview-b');
  });

  it('keeps independent bounded history for a tab', () => {
    const workspace = TestBed.inject(WorkspaceService);
    const tab = workspace.open(design)!;
    workspace.navigate(tab.id, '/design-system/components');

    expect(workspace.canGoBack()).toBe(true);
    expect(workspace.back()?.route).toBe('/design-system');
    expect(workspace.forward()?.route).toBe('/design-system/components');
  });

  it('requires confirmation before closing a dirty tab', () => {
    const workspace = TestBed.inject(WorkspaceService);
    const tab = workspace.open(home)!;
    workspace.markDirty(tab.id);

    expect(workspace.requestClose(tab.id)).toBe('confirmation-required');
    expect(workspace.tabs()).toHaveLength(1);
    workspace.confirmClose();
    expect(workspace.tabs()).toHaveLength(0);
  });

  it('restores only registered safe navigation state without dirty flags', () => {
    localStorage.setItem('ministore.ui.workspace.v1', JSON.stringify({
      version: 1,
      activeTabId: 'design-system::default',
      tabs: [
        { navigationId: 'design-system', route: '/design-system/components?secret=no', history: [], historyIndex: 0 },
        { navigationId: 'unknown', route: '/admin', history: [], historyIndex: 0 },
      ],
    }));

    const workspace = TestBed.inject(WorkspaceService);
    expect(workspace.tabs()).toHaveLength(1);
    expect(workspace.activeTab()?.route).toBe('/design-system/components');
    expect(workspace.activeTab()?.dirty).toBe(false);
  });

  it('reopens a recently closed safe tab', () => {
    const workspace = TestBed.inject(WorkspaceService);
    const tab = workspace.open(design)!;
    workspace.requestClose(tab.id);

    expect(workspace.reopenLastClosed()?.id).toBe(tab.id);
    expect(workspace.tabs()).toHaveLength(1);
  });

  it('asks once before close-others removes dirty tabs', () => {
    const workspace = TestBed.inject(WorkspaceService);
    const kept = workspace.open(home)!;
    const dirty = workspace.open(design)!;
    workspace.markDirty(dirty.id);

    expect(workspace.closeOthers(kept.id)).toBe('confirmation-required');
    expect(workspace.tabs()).toHaveLength(2);
    workspace.confirmClose();
    expect(workspace.tabs().map((tab) => tab.id)).toEqual([kept.id]);
  });
});
