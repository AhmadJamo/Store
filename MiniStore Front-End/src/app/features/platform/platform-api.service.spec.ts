import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { PlatformApiService } from './platform-api.service';

describe('PlatformApiService', () => {
  it('uses the isolated platform session endpoints', () => {
    TestBed.configureTestingModule({
      providers: [PlatformApiService, provideHttpClient(), provideHttpClientTesting()],
    });
    const api = TestBed.inject(PlatformApiService);
    const http = TestBed.inject(HttpTestingController);
    api.login({ username: 'operator', password: 'secret' }).subscribe();
    expect(http.expectOne('/api/v1/platform/login').request.method).toBe('POST');
    api.session().subscribe();
    expect(http.expectOne('/api/v1/platform/session').request.method).toBe('GET');
    api.overview().subscribe();
    expect(http.expectOne('/api/v1/platform/overview').request.method).toBe('GET');
    api.logout().subscribe();
    expect(http.expectOne('/api/v1/platform/logout').request.method).toBe('POST');
    http.verify();
  });
});
