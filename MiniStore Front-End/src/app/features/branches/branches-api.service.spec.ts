import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { BranchesApiService } from './branches-api.service';

describe('BranchesApiService', () => {
  it('loads the permission-protected branch workspace contract', () => {
    TestBed.configureTestingModule({ providers: [BranchesApiService, provideHttpClient(), provideHttpClientTesting()] });
    const service = TestBed.inject(BranchesApiService);
    const http = TestBed.inject(HttpTestingController);
    service.getWorkspace().subscribe(page => expect(page.branches[0].code).toBe('MAIN'));
    const request = http.expectOne('/api/v1/branches');
    expect(request.request.method).toBe('GET');
    request.flush({ branches: [{ id: 1, code: 'MAIN', name: 'Main', isActive: true, warehouses: [], posTerminals: [] }] });
    service.create({ code: 'NORTH', name: 'North' }).subscribe();
    const create = http.expectOne('/api/v1/branches');
    expect(create.request.method).toBe('POST');
    expect(create.request.body).toEqual({ code: 'NORTH', name: 'North' });
    create.flush({ id: 2, code: 'NORTH', name: 'North', isActive: true });
    service.update(2, { code: 'NORTH-1', name: 'North One' }).subscribe();
    expect(http.expectOne('/api/v1/branches/2').request.method).toBe('PUT');
    service.setActive(2, false).subscribe();
    const status = http.expectOne('/api/v1/branches/2/status');
    expect(status.request.method).toBe('PUT');
    expect(status.request.body).toEqual({ isActive: false });    service.getAccessManagement().subscribe();
    expect(http.expectOne('/api/v1/branches/access-management').request.method).toBe('GET');
    service.replaceUserGrants('user-1', [1, 2]).subscribe();
    const update = http.expectOne('/api/v1/branches/access-management');
    expect(update.request.method).toBe('PUT');
    expect(update.request.body).toEqual({ userId: 'user-1', branchIds: [1, 2] });
    http.verify();
  });
});
