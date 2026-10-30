import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { LocalizationService } from './localization.service';

describe('LocalizationService', () => {
  beforeEach(() => {
    localStorage.clear();
    TestBed.resetTestingModule();
  });

  it('changes document language only after the server culture cookie succeeds', () => {
    TestBed.configureTestingModule({ providers: [provideHttpClient(), provideHttpClientTesting()] });
    const service = TestBed.inject(LocalizationService);
    const http = TestBed.inject(HttpTestingController);
    expect(document.documentElement.lang).toBe('ar-JO');
    expect(document.documentElement.dir).toBe('rtl');

    service.toggle();
    expect(document.documentElement.lang).toBe('ar-JO');
    const request = http.expectOne('/api/v1/culture');
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual({ culture: 'en-US' });
    request.flush(null);
    expect(document.documentElement.lang).toBe('en-US');
    expect(document.documentElement.dir).toBe('ltr');
    expect(localStorage.getItem('ministore.ui.locale')).toBe('en-US');
    http.verify();
  });

  it('keeps the visible locale unchanged when server synchronization fails', () => {
    TestBed.configureTestingModule({ providers: [provideHttpClient(), provideHttpClientTesting()] });
    const service = TestBed.inject(LocalizationService);
    const http = TestBed.inject(HttpTestingController);
    service.toggle();
    http.expectOne('/api/v1/culture').flush({}, { status: 400, statusText: 'Bad Request' });
    expect(service.locale()).toBe('ar-JO');
    expect(service.switchError()).toBe(true);
  });

  it('treats a supported session culture as authoritative', () => {
    localStorage.setItem('ministore.ui.locale', 'en-US');
    TestBed.configureTestingModule({ providers: [provideHttpClient(), provideHttpClientTesting()] });
    const service = TestBed.inject(LocalizationService);
    service.synchronize('ar-JO');
    expect(service.locale()).toBe('ar-JO');
    expect(document.documentElement.dir).toBe('rtl');
  });
});
