import { TestBed } from '@angular/core/testing';
import { SafeListPreferencesService } from './safe-list-preferences.service';

describe('SafeListPreferencesService', () => {
  beforeEach(() => { localStorage.clear(); TestBed.resetTestingModule(); });
  it('persists only allowlisted non-sensitive customer view fields', () => {
    const service = TestBed.inject(SafeListPreferencesService);
    service.saveCustomers({ includeArchived: true, sortBy: 'status', sortDescending: true, pageSize: 50 });
    expect(service.loadCustomers()).toEqual({ includeArchived: true, sortBy: 'status', sortDescending: true, pageSize: 50 });
    expect(localStorage.getItem('ministore.ui.list-preferences.v1')).not.toContain('search');
  });
  it('normalizes hostile or obsolete values to safe defaults', () => {
    localStorage.setItem('ministore.ui.list-preferences.v1', JSON.stringify({ version: 1, customers: { includeArchived: 'yes', sortBy: '../../admin', sortDescending: 1, pageSize: 999, search: 'private' } }));
    expect(TestBed.inject(SafeListPreferencesService).loadCustomers()).toEqual({ includeArchived: false, sortBy: 'name', sortDescending: false, pageSize: 25 });
  });
});
