import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { SupplierCommunicationsApiService } from './supplier-communications-api.service';

describe('SupplierCommunicationsApiService', () => {
  it('covers prepare, detail, document history and activity operations', () => {
    TestBed.configureTestingModule({
      providers: [
        SupplierCommunicationsApiService,
        provideHttpClient(),
        provideHttpClientTesting(),
      ],
    });
    const api = TestBed.inject(SupplierCommunicationsApiService),
      http = TestBed.inject(HttpTestingController);
    api
      .prepare({ documentType: 2, documentId: 4, supplierId: 8, preferredLanguage: 'ar-JO' })
      .subscribe();
    expect(http.expectOne('/api/v1/supplier-communications').request.method).toBe('POST');
    api.get(3).subscribe();
    expect(http.expectOne('/api/v1/supplier-communications/3').request.method).toBe('GET');
    api.byDocument(2, 4).subscribe();
    expect(http.expectOne('/api/v1/supplier-communications/document/2/4').request.method).toBe(
      'GET',
    );
    api.record(3, { communicationId: 3, status: 2 }).subscribe();
    expect(http.expectOne('/api/v1/supplier-communications/3/activities').request.method).toBe(
      'POST',
    );
    http.verify();
  });
});
