import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {
  CompanyOnboardingPage,
  CompanyRegistrationPage,
  CompanyRegistrationResult,
  CompleteCompanyOnboardingRequest,
  RegisterCompanyRequest,
} from './onboarding.models';

@Injectable({ providedIn: 'root' })
export class OnboardingApiService {
  constructor(private readonly http: HttpClient) {}

  getRegistrationPage(): Observable<CompanyRegistrationPage> {
    return this.http.get<CompanyRegistrationPage>('/api/v1/registration');
  }

  register(request: RegisterCompanyRequest): Observable<CompanyRegistrationResult> {
    return this.http.post<CompanyRegistrationResult>('/api/v1/registration', request);
  }

  getCompanyOnboarding(): Observable<CompanyOnboardingPage> {
    return this.http.get<CompanyOnboardingPage>('/api/v1/company-onboarding');
  }

  complete(request: CompleteCompanyOnboardingRequest): Observable<void> {
    return this.http.post<void>('/api/v1/company-onboarding/complete', request);
  }

  skip(): Observable<void> {
    return this.http.post<void>('/api/v1/company-onboarding/skip', {});
  }
}
