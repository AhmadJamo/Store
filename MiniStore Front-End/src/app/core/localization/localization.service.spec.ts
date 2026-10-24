import { TestBed } from '@angular/core/testing';
import { LocalizationService } from './localization.service';

describe('LocalizationService', () => {
  beforeEach(() => {
    localStorage.clear();
    TestBed.resetTestingModule();
  });

  it('keeps document language and direction synchronized', () => {
    const service = TestBed.inject(LocalizationService);
    expect(document.documentElement.lang).toBe('ar-JO');
    expect(document.documentElement.dir).toBe('rtl');

    service.toggle();

    expect(document.documentElement.lang).toBe('en-US');
    expect(document.documentElement.dir).toBe('ltr');
    expect(localStorage.getItem('ministore.ui.locale')).toBe('en-US');
  });
});
