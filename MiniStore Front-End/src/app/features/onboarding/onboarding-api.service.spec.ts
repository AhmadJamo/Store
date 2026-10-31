import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { OnboardingApiService } from './onboarding-api.service';

describe('OnboardingApiService', () => {
  it('uses the public registration and protected onboarding API contracts for every mutation', () => {
    TestBed.configureTestingModule({ providers: [OnboardingApiService, provideHttpClient(), provideHttpClientTesting()] });
    const service = TestBed.inject(OnboardingApiService);
    const http = TestBed.inject(HttpTestingController);

    service.getRegistrationPage().subscribe();
    expect(http.expectOne('/api/v1/registration').request.method).toBe('GET');

    const registration = { companyName: 'Cafe', companySlug: 'amman-cafe', ownerUsername: 'owner', ownerEmail: 'owner@example.test', password: 'Secret1!', planId: 2 };
    service.register(registration).subscribe();
    const register = http.expectOne('/api/v1/registration');
    expect(register.request.method).toBe('POST');
    expect(register.request.body).toEqual(registration);
    register.flush({ requiresCompanyOnboarding: true });

    service.getCompanyOnboarding().subscribe();
    expect(http.expectOne('/api/v1/company-onboarding').request.method).toBe('GET');

    const setup = { businessType: 2, countryCode: 'JO', currency: 'JOD', language: 2, fiscalYearStartMonth: 1, isTaxRegistered: true, defaultTaxRate: 16, usePos: true, inventoryControlMode: 3 };
    service.complete(setup).subscribe();
    const complete = http.expectOne('/api/v1/company-onboarding/complete');
    expect(complete.request.method).toBe('POST');
    expect(complete.request.body).toEqual(setup);
    complete.flush(null);

    service.skip().subscribe();
    const skip = http.expectOne('/api/v1/company-onboarding/skip');
    expect(skip.request.method).toBe('POST');
    skip.flush(null);
    http.verify();
  });
});
