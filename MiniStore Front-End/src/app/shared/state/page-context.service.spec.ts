import { ActivatedRoute, convertToParamMap, Router } from '@angular/router';
import { TestBed } from '@angular/core/testing';
import { BehaviorSubject } from 'rxjs';
import { ApiSessionService } from '../../core/api/api-session.service';
import { PageContextService, pageContextFields } from './page-context.service';

describe('PageContextService', () => {
  const router = { navigate: vi.fn(() => Promise.resolve(true)) };
  const session = { session: vi.fn(() => ({ tenant: { id: 7 }, user: { id: 'user-a' } })) };
  const schema = {
    defaults: { page: 1, search: '', active: undefined as boolean | undefined, compact: true },
    fields: { page: pageContextFields.integer(), search: pageContextFields.text(40), active: pageContextFields.optionalBoolean(), compact: pageContextFields.boolean() },
  };

  beforeEach(() => {
    vi.clearAllMocks();
    localStorage.clear();
    session.session.mockReturnValue({ tenant: { id: 7 }, user: { id: 'user-a' } });
    TestBed.configureTestingModule({
      providers: [
        PageContextService,
        { provide: Router, useValue: router },
        { provide: ApiSessionService, useValue: session },
      ],
    });
  });

  it('reacts to URL back/forward changes and remembers context when a workspace tab returns without query parameters', () => {
    const service = TestBed.inject(PageContextService);
    const params = new BehaviorSubject(convertToParamMap({ search: 'coffee', page: '2' }));
    const values: any[] = [];
    service.watch('products', { queryParamMap: params } as unknown as ActivatedRoute, schema).subscribe(value => values.push(value));
    params.next(convertToParamMap({ search: 'tea', page: '3', compact: 'false' }));
    expect(values.at(-1)).toEqual({ page: 3, search: 'tea', active: undefined, compact: false });
    expect(service.read('products', convertToParamMap({}), schema)).toEqual(values.at(-1));
  });

  it('normalizes allowlisted URL fields and rejects invalid values', () => {
    const service = TestBed.inject(PageContextService);
    expect(service.read('products', convertToParamMap({ page: '2', search: ' coffee ', active: 'true', secret: 'x' }), schema))
      .toEqual({ page: 2, search: 'coffee', active: true, compact: true });
    expect(service.read('other', convertToParamMap({ page: '-9', active: 'maybe' }), schema))
      .toEqual(schema.defaults);
  });

  it('uses tenant and user boundaries for memory context and drafts', () => {
    const service = TestBed.inject(PageContextService);
    service.read('products', convertToParamMap({ search: 'coffee' }), schema);
    service.saveDraft('request-new', { warehouseId: 3 });
    session.session.mockReturnValue({ tenant: { id: 8 }, user: { id: 'user-a' } });
    expect(service.read('products', convertToParamMap({}), schema)).toEqual(schema.defaults);
    expect(service.loadDraft('request-new')).toBeNull();
  });

  it('keeps drafts in memory and writes only normalized query fields', async () => {
    const service = TestBed.inject(PageContextService);
    const route = {} as any;
    service.saveDraft('request-new', { warehouseId: 3, quantities: [2] });
    expect(localStorage.length).toBe(0);
    expect(service.loadDraft('request-new')).toEqual({ warehouseId: 3, quantities: [2] });
    await service.replace('products', route, schema, { page: 3, search: ' tea ', active: false, compact: false });
    expect(router.navigate).toHaveBeenCalledWith([], expect.objectContaining({
      relativeTo: route,
      queryParams: { page: '3', search: 'tea', active: 'false', compact: 'false' },
      replaceUrl: true,
    }));
  });
});
