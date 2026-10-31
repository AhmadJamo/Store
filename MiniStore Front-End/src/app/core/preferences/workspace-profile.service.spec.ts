import { TestBed } from '@angular/core/testing';
import { WorkspaceProfileService } from './workspace-profile.service';
describe('WorkspaceProfileService', () => {
  beforeEach(() => { localStorage.clear(); TestBed.resetTestingModule(); });
  it('persists only allowlisted presentation choices', () => { const service = TestBed.inject(WorkspaceProfileService); service.setProfile('operator'); service.setSplitView(true); expect(JSON.parse(localStorage.getItem('ministore.workspace.profile')!)).toEqual({ version: 1, profile: 'operator', splitView: true }); });
  it('drops an injected profile', () => { localStorage.setItem('ministore.workspace.profile', JSON.stringify({ version: 1, profile: 'admin', splitView: true })); const service = TestBed.inject(WorkspaceProfileService); expect(service.profile()).toBe('focused'); });
});
