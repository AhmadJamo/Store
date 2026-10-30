import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { LocalizationService } from '../localization/localization.service';
import { ApiSessionService } from './api-session.service';

describe('ApiSessionService culture synchronization', () => {
  beforeEach(() => {
    localStorage.clear();
    TestBed.configureTestingModule({ providers: [provideHttpClient(), provideHttpClientTesting()] });
  });

  it('uses the server session culture and keeps session consumers aligned after a switch', async () => {
    const session = TestBed.inject(ApiSessionService);
    const localization = TestBed.inject(LocalizationService);
    const http = TestBed.inject(HttpTestingController);
    const loading = session.load();
    http.expectOne('/api/v1/session').flush({
      user: { id: 'u1', userName: 'owner', email: null },
      tenant: { id: 7, name: 'Demo', code: 'demo' },
      culture: 'en-US', direction: 'ltr', permissions: [],
    });
    await loading;
    expect(localization.locale()).toBe('en-US');
    expect(session.session()?.culture).toBe('en-US');

    localization.synchronize('ar-JO');
    TestBed.flushEffects();
    expect(session.session()?.culture).toBe('ar-JO');
    expect(session.session()?.direction).toBe('rtl');
    http.verify();
  });
});
