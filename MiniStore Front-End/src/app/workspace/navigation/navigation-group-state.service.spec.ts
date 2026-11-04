import { TestBed } from '@angular/core/testing';
import { NavigationGroupStateService } from './navigation-group-state.service';

describe('NavigationGroupStateService', () => {
  beforeEach(() => { localStorage.clear(); TestBed.resetTestingModule(); });

  it('starts compact and persists only allowlisted group identifiers', () => {
    const service = TestBed.inject(NavigationGroupStateService);
    expect(service.isOpen('overview')).toBe(true);
    expect(service.isOpen('inventory')).toBe(false);
    service.setOpen('inventory', true);
    expect(service.isOpen('inventory')).toBe(true);
    expect(localStorage.getItem('ministore.ui.navigation-groups.v1')).toContain('inventory');
  });

  it('drops invalid persisted values and restores a safe default', () => {
    localStorage.setItem('ministore.ui.navigation-groups.v1', JSON.stringify({ version: 1, open: ['../../admin'] }));
    const service = TestBed.inject(NavigationGroupStateService);
    expect(service.isOpen('overview')).toBe(true);
    expect(service.isOpen('administration')).toBe(false);
  });
});
