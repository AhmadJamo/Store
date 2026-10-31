import { TestBed } from '@angular/core/testing';
import { ActivatedRoute, provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { ApiErrorService } from '../../core/api/api-error.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { InventoryAdjustmentApiService } from './data/inventory-adjustment-api.service';
import { InventoryAdjustmentDetailsPageComponent } from './inventory-adjustment-details-page';
import { InventoryLocalizationService } from './inventory-localization.service';

describe('InventoryAdjustmentDetailsPageComponent', () => {
  const details: any = { id: 4, number: 'ADJ-4', warehouse: 'Main', location: 'Unassigned', status: 1, isBlindCount: false, reason: 'Cycle', createdAt: '2026-10-27T10:00:00Z', lineCount: 1, lines: [{ productId: 2, productName: 'Coffee', expectedQuantity: 5 }] };
  const api = { getDetails: vi.fn(() => of(details)), count: vi.fn(() => of(void 0)), approve: vi.fn(() => of(void 0)), post: vi.fn(() => of(void 0)), cancel: vi.fn(() => of(void 0)) };
  beforeEach(async () => { vi.clearAllMocks(); api.getDetails.mockReturnValue(of(details)); vi.spyOn(globalThis, 'confirm').mockReturnValue(true); await TestBed.configureTestingModule({ imports: [InventoryAdjustmentDetailsPageComponent], providers: [{ provide: InventoryAdjustmentApiService, useValue: api }, { provide: ActivatedRoute, useValue: { snapshot: { paramMap: { get: () => '4' } } } }, { provide: ApiSessionService, useValue: { can: () => true } }, { provide: ApiErrorService, useValue: { normalize: (error: unknown) => error } }, { provide: WorkspaceService, useValue: { activeTab: () => ({ id: 'adjustment' }), markDirty: vi.fn() } }, { provide: InventoryLocalizationService, useValue: { text: (key: string) => key } }, provideRouter([])] }).compileComponents(); });
  it('records every line and keeps workflow commands separate', () => { const component = TestBed.createComponent(InventoryAdjustmentDetailsPageComponent).componentInstance as any; component.id = 4; component.lines.at(0).controls.countedQuantity.setValue(6); component.recordCount(); expect(api.count).toHaveBeenCalledWith(4, { lines: [{ productId: 2, countedQuantity: 6 }] }); component.approve(); expect(api.approve).toHaveBeenCalledWith(4); });
});
