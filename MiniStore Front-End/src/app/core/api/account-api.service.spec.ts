import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { AccountApiService } from './account-api.service';

describe('AccountApiService', () => {
  it('uses protected API mutations for login and logout without browser storage', () => {
    TestBed.configureTestingModule({ providers: [AccountApiService, provideHttpClient(), provideHttpClientTesting()] });
    const service = TestBed.inject(AccountApiService);
    const http = TestBed.inject(HttpTestingController);
    service.login({ userName: 'operator', password: 'secret', companyCode: 'amman-cafe' }).subscribe();
    const login = http.expectOne('/api/v1/auth/login');
    expect(login.request.method).toBe('POST');
    expect(login.request.body).toEqual({ userName: 'operator', password: 'secret', companyCode: 'amman-cafe' });
    login.flush({ requiresCompanyOnboarding: false });
    service.logout().subscribe();
    const logout = http.expectOne('/api/v1/auth/logout');
    expect(logout.request.method).toBe('POST');
    logout.flush(null);
    http.verify();
  });
});
