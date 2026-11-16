import { TestBed } from '@angular/core/testing';
import { BranchContextService } from './branch-context.service';

describe('BranchContextService', () => {
  const session: any = { tenant: { id: 7 }, user: { id: 'u1' } };
  const branches = [
    { id: 1, code: 'MAIN', name: 'Main', isActive: true },
    { id: 2, code: 'OUT', name: 'Outlet', isActive: true },
    { id: 3, code: 'OLD', name: 'Old', isActive: false },
  ];
  beforeEach(() => { localStorage.clear(); TestBed.configureTestingModule({}); });

  it('restores only an active server-authorized branch in the tenant and user scope', () => {
    localStorage.setItem('ministore.branch-context.v1:7:u1', '2');
    const service = TestBed.inject(BranchContextService);
    service.initialize(session, branches);
    expect(service.selectedId()).toBe(2);
    expect(service.choices().map(x => x.id)).toEqual([1, 2]);
    expect(service.select(3)).toBe(false);
  });

  it('does not reuse a stale or foreign choice', () => {
    localStorage.setItem('ministore.branch-context.v1:7:u1', '99');
    const service = TestBed.inject(BranchContextService);
    service.initialize(session, branches);
    expect(service.selectedId()).toBeNull();
    expect(localStorage.getItem('ministore.branch-context.v1:7:u1')).toBeNull();
  });
});
