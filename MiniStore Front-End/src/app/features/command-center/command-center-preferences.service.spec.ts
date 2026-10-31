import { TestBed } from '@angular/core/testing';
import { ApiSessionService } from '../../core/api/api-session.service';
import { NavigationRegistryService } from '../../workspace/navigation/navigation-registry.service';
import { CommandCenterPreferencesService } from './command-center-preferences.service';

describe('CommandCenterPreferencesService', () => {
  beforeEach(() => {
    localStorage.clear();
    TestBed.configureTestingModule({ providers: [
      NavigationRegistryService, CommandCenterPreferencesService,
      { provide: ApiSessionService, useValue: { can: (permission: string) => permission !== 'Suppliers.View' } },
    ] });
  });

  it('persists only allowlisted accessible navigation identifiers', () => {
    const preferences = TestBed.inject(CommandCenterPreferencesService);
    preferences.toggleFavorite('customers');
    preferences.toggleFavorite('suppliers');
    preferences.toggleFavorite('../../admin');
    expect(preferences.favoriteIds()).toEqual(['customers']);
    expect(localStorage.getItem('ministore.ui.command-center.v1')).not.toContain('admin');
  });

  it('restores bounded view metadata without record data or search terms', () => {
    localStorage.setItem('ministore.ui.command-center.v1', JSON.stringify({
      version: 1,
      favorites: ['customers', 'unknown'],
      snapshots: [
        { id: 'customers-compact', providerId: 'customers', viewMode: 'compact', search: 'secret customer' },
        { id: 'unsafe', providerId: 'admin', viewMode: 'compact' },
      ],
    }));
    TestBed.resetTestingModule();
    TestBed.configureTestingModule({ providers: [
      NavigationRegistryService, CommandCenterPreferencesService,
      { provide: ApiSessionService, useValue: { can: () => true } },
    ] });
    const preferences = TestBed.inject(CommandCenterPreferencesService);
    expect(preferences.favoriteIds()).toEqual(['customers']);
    expect(preferences.snapshots()).toEqual([{ id: 'customers-compact', providerId: 'customers', viewMode: 'compact' }]);
  });
});
