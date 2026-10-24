import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { WorkCenter } from './work-center.models';
@Injectable({ providedIn: 'root' })
export class WorkCenterApiService { constructor(private readonly http: HttpClient) {} get() { return this.http.get<WorkCenter>('/api/v1/work-center'); } }
