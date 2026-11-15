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
    http.verify();
  });
});
