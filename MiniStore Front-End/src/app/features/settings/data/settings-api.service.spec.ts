import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { SettingsApiService } from './settings-api.service';

describe('SettingsApiService', () => {
  it('loads identity and preferences as one overview', () => { TestBed.configureTestingModule({ providers: [SettingsApiService, provideHttpClient(), provideHttpClientTesting()] }); const service = TestBed.inject(SettingsApiService); const http = TestBed.inject(HttpTestingController); service.getOverview().subscribe(value => expect(value.identity.tradeName).toBe('MiniStore')); http.expectOne('/api/v1/settings/organization-identity').flush({ tradeName: 'MiniStore', hasLogo: false, customFields: [] }); http.expectOne('/api/v1/settings/preferences').flush({ currency: 'JOD', decimalPlaces: 2, quantityDecimalPlaces: 3, dateFormat: 'dd/MM/yyyy', timeZone: 'Asia/Amman', defaultLanguage: 1 }); http.verify(); });
});
