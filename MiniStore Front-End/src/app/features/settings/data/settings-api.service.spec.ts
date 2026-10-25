import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { SettingsApiService } from './settings-api.service';

describe('SettingsApiService', () => {
  it('loads identity and preferences as one overview', () => { TestBed.configureTestingModule({ providers: [SettingsApiService, provideHttpClient(), provideHttpClientTesting()] }); const service = TestBed.inject(SettingsApiService); const http = TestBed.inject(HttpTestingController); service.getOverview().subscribe(value => expect(value.identity.tradeName).toBe('MiniStore')); http.expectOne('/api/v1/settings/organization-identity').flush({ tradeName: 'MiniStore', hasLogo: false, customFields: [] }); http.expectOne('/api/v1/settings/preferences').flush({ currency: 'JOD', decimalPlaces: 2, quantityDecimalPlaces: 3, dateFormat: 'dd/MM/yyyy', timeZone: 'Asia/Amman', defaultLanguage: 1 }); http.verify(); });
  it('updates preferences with the concurrency token', () => { TestBed.configureTestingModule({ providers: [SettingsApiService, provideHttpClient(), provideHttpClientTesting()] }); const service = TestBed.inject(SettingsApiService); const http = TestBed.inject(HttpTestingController); const value = { currency: 'JOD', decimalPlaces: 2, quantityDecimalPlaces: 3, dateFormat: 'dd/MM/yyyy', timeZone: 'Asia/Amman', defaultLanguage: 1, rowVersion: 'AQID' }; service.updatePreferences(value).subscribe(result => expect(result.rowVersion).toBe('BAUG')); const request = http.expectOne('/api/v1/settings/preferences'); expect(request.request.method).toBe('PUT'); expect(request.request.body.rowVersion).toBe('AQID'); request.flush({ ...value, rowVersion: 'BAUG' }); http.verify(); });
});
