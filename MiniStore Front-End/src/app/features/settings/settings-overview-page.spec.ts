import { TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { ApiErrorService } from '../../core/api/api-error.service';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { SettingsApiService } from './data/settings-api.service';
import { SettingsOverviewPageComponent } from './settings-overview-page';

describe('SettingsOverviewPageComponent', () => {
  const preferences = { currency: 'JOD', decimalPlaces: 2, quantityDecimalPlaces: 3, dateFormat: 'dd/MM/yyyy', timeZone: 'Asia/Amman', defaultLanguage: 1, rowVersion: 'AQID' };
  const overview = { identity: { tradeName: 'MiniStore', hasLogo: false, customFields: [] }, preferences };
  const api = { getOverview: vi.fn(() => of(overview)), updatePreferences: vi.fn() };
  const workspace = { activeTab: vi.fn(() => ({ id: 'settings::default' })), markDirty: vi.fn() };

  beforeEach(async () => {
    vi.clearAllMocks();
    api.getOverview.mockReturnValue(of(overview));
    await TestBed.configureTestingModule({
      imports: [SettingsOverviewPageComponent],
      providers: [
        { provide: SettingsApiService, useValue: api },
        { provide: WorkspaceService, useValue: workspace },
        { provide: ApiErrorService, useValue: { normalize: (error: unknown) => error } },
      ],
    }).compileComponents();
  });

  it('saves the concurrency token and adopts the refreshed token', () => {
    api.updatePreferences.mockReturnValue(of({ ...preferences, decimalPlaces: 4, rowVersion: 'BAUG' }));
    const component = TestBed.createComponent(SettingsOverviewPageComponent).componentInstance as any;
    component.form.patchValue({ decimalPlaces: 4 }); component.form.markAsDirty(); component.save();
    expect(api.updatePreferences).toHaveBeenCalledWith(expect.objectContaining({ decimalPlaces: 4, rowVersion: 'AQID' }));
    expect(component.form.controls.rowVersion.value).toBe('BAUG');
    expect(component.form.dirty).toBe(false);
    expect(component.saved()).toBe(true);
  });

  it('keeps edited values dirty after a failed save', () => {
    api.updatePreferences.mockReturnValue(throwError(() => ({ status: 409, kind: 'concurrency', detail: 'Reload settings.' })));
    const component = TestBed.createComponent(SettingsOverviewPageComponent).componentInstance as any;
    component.form.patchValue({ timeZone: 'UTC' }); component.form.markAsDirty(); component.save();
    expect(component.form.controls.timeZone.value).toBe('UTC');
    expect(component.form.dirty).toBe(true);
    expect(component.saveProblem()?.kind).toBe('concurrency');
  });
});
