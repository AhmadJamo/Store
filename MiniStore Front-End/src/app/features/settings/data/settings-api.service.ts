import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { forkJoin } from 'rxjs';
import { GeneralPreferences, OrganizationIdentity } from './settings.models';
@Injectable({ providedIn: 'root' })
export class SettingsApiService { constructor(private readonly http: HttpClient) {} getOverview() { return forkJoin({ identity: this.http.get<OrganizationIdentity>('/api/v1/settings/organization-identity'), preferences: this.http.get<GeneralPreferences>('/api/v1/settings/preferences') }); } }
