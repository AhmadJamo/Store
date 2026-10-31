import { TestBed } from '@angular/core/testing';
import { UiPreferencesService } from './ui-preferences.service';

describe('UiPreferencesService', () => {
  beforeEach(() => {
    localStorage.clear();
    document.documentElement.removeAttribute('data-theme');
    document.documentElement.removeAttribute('data-density');
    TestBed.resetTestingModule();
  });

  it('applies and persists explicit theme and density preferences', () => {
    const service = TestBed.inject(UiPreferencesService);

    service.setTheme('dark');
    service.setDensity('compact');
    service.setSidebar('compact');
    service.setContentWidth('contained');

    expect(document.documentElement.dataset['theme']).toBe('dark');
    expect(document.documentElement.dataset['density']).toBe('compact');
    expect(document.documentElement.dataset['sidebar']).toBe('compact');
    expect(document.documentElement.dataset['contentWidth']).toBe('contained');
    expect(localStorage.getItem('ministore.ui.preferences.v1')).toContain('"theme":"dark"');
  });

  it('restores safe defaults and removes persisted preferences on reset', () => {
    const service = TestBed.inject(UiPreferencesService);
    service.setTheme('dark');
    service.setDensity('compact');

    service.reset();

    expect(service.theme()).toBe('system');
    expect(service.density()).toBe('comfortable');
    expect(service.sidebar()).toBe('expanded');
    expect(service.contentWidth()).toBe('fluid');
    expect(document.documentElement.dataset['density']).toBe('comfortable');
    expect(localStorage.getItem('ministore.ui.preferences.v1')).toBeNull();
  });

  it('restores version one safely and ignores invalid layout values', () => {
    localStorage.setItem('ministore.ui.preferences.v1', JSON.stringify({ version: 1, theme: 'dark', density: 'compact', sidebar: '../../admin', contentWidth: 'wide' }));
    const service = TestBed.inject(UiPreferencesService);
    expect(service.theme()).toBe('dark');
    expect(service.density()).toBe('compact');
    expect(service.sidebar()).toBe('expanded');
    expect(service.contentWidth()).toBe('fluid');
  });
});
