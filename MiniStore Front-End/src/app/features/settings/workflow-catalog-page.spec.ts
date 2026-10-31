import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { ApiErrorService } from '../../core/api/api-error.service';
import { SettingsApiService } from './data/settings-api.service';
import { WorkflowCatalogPageComponent } from './workflow-catalog-page';

describe('WorkflowCatalogPageComponent', () => {
  const steps = [
    { module: 'Sales', number: 1, phase: 'Prepare', title: 'Create', responsibleRole: 'Seller', reviewerRole: 'Manager', instructions: 'Prepare.', controlPrinciple: 'Separate.', completionEvidence: 'Created.', conditional: false, canOpen: true, workUrl: '/Sales/Create' },
    { module: 'Sales', number: 2, phase: 'Review', title: 'Approve', responsibleRole: 'Manager', reviewerRole: 'Owner', instructions: 'Review.', controlPrinciple: 'Approve.', completionEvidence: 'Approved.', conditional: true, canOpen: false, workUrl: null },
    { module: 'Inventory', number: 1, phase: 'Prepare', title: 'Count', responsibleRole: 'Counter', reviewerRole: 'Manager', instructions: 'Count.', controlPrinciple: 'Independent.', completionEvidence: 'Posted.', conditional: false, canOpen: true, workUrl: '/Inventory/Index' },
  ];
  const api = { getWorkflowCatalog: vi.fn(() => of({ moduleName: 'ERP', purpose: 'Guide', steps })) };
  beforeEach(async () => { vi.clearAllMocks(); api.getWorkflowCatalog.mockReturnValue(of({ moduleName: 'ERP', purpose: 'Guide', steps })); await TestBed.configureTestingModule({ imports: [WorkflowCatalogPageComponent], providers: [provideRouter([]), { provide: SettingsApiService, useValue: api }, { provide: ApiErrorService, useValue: { normalize: (error: unknown) => error } }] }).compileComponents(); });
  it('groups server-localized steps by module and phase without inventing work links', () => { const fixture = TestBed.createComponent(WorkflowCatalogPageComponent); fixture.detectChanges(); const component = fixture.componentInstance as any; expect(component.modules(steps)).toEqual(['Sales', 'Inventory']); expect(component.phases(steps, 'Sales')).toEqual(['Prepare', 'Review']); expect(component.phaseSteps(steps, 'Sales', 'Review')[0].canOpen).toBe(false); expect(fixture.nativeElement.querySelectorAll('a[href="/Sales/Create"]').length).toBe(1); expect(fixture.nativeElement.querySelectorAll('a[href="/Sales/Approve"]').length).toBe(0); });
});
