import { TestBed } from '@angular/core/testing';
import { DataTableComponent } from './data-table/data-table';
import { FeedbackStateComponent } from './feedback-state/feedback-state';
import { PageHeaderComponent } from './page-header/page-header';
import { PagerComponent } from './pager/pager';
import { FilterBarComponent } from './filter-bar/filter-bar';
import { SectionHeaderComponent } from './section-header/section-header';
import { SummaryStripComponent } from './summary-strip/summary-strip';
import { WorkflowFooterComponent } from './workflow-footer/workflow-footer';
import { FieldCueComponent } from '../forms/field-cue/field-cue';
import { FormSectionComponent } from '../forms/form-section/form-section';
import { DetailSectionComponent } from './detail-section/detail-section';
import { WorkflowProgressComponent } from './workflow-progress/workflow-progress';

describe('shared UI primitives', () => {
  it('renders page metadata without owning business actions', async () => {
    await TestBed.configureTestingModule({ imports: [PageHeaderComponent] }).compileComponents();
    const fixture = TestBed.createComponent(PageHeaderComponent);
    fixture.componentRef.setInput('title', 'Customers');
    fixture.componentRef.setInput('description', 'Directory');
    fixture.detectChanges();
    expect(fixture.nativeElement.textContent).toContain('Customers');
    expect(fixture.nativeElement.textContent).toContain('Directory');
  });

  it('emits retry from an error feedback state', async () => {
    await TestBed.configureTestingModule({ imports: [FeedbackStateComponent] }).compileComponents();
    const fixture = TestBed.createComponent(FeedbackStateComponent);
    fixture.componentRef.setInput('kind', 'error');
    fixture.componentRef.setInput('title', 'Failed');
    fixture.componentRef.setInput('retryLabel', 'Retry');
    const retry = vi.fn();
    fixture.componentInstance.retry.subscribe(retry);
    fixture.detectChanges();
    fixture.nativeElement.querySelector('button').click();
    expect(retry).toHaveBeenCalledOnce();
    expect(fixture.nativeElement.querySelector('[role="alert"]')).toBeTruthy();
  });

  it('keeps paging decisions with the consuming feature', async () => {
    await TestBed.configureTestingModule({ imports: [PagerComponent] }).compileComponents();
    const fixture = TestBed.createComponent(PagerComponent);
    fixture.componentRef.setInput('page', 2);
    fixture.componentRef.setInput('totalPages', 4);
    fixture.componentRef.setInput('totalCount', 80);
    fixture.componentRef.setInput('totalLabel', 'Total');
    fixture.componentRef.setInput('previousLabel', 'Previous');
    fixture.componentRef.setInput('nextLabel', 'Next');
    const changes: number[] = [];
    fixture.componentInstance.pageChange.subscribe((page) => changes.push(page));
    fixture.detectChanges();
    fixture.nativeElement.querySelectorAll('button')[1].click();
    expect(changes).toEqual([3]);
  });

  it('gives projected tables an accessible named region', async () => {
    await TestBed.configureTestingModule({ imports: [DataTableComponent] }).compileComponents();
    const fixture = TestBed.createComponent(DataTableComponent);
    fixture.componentRef.setInput('label', 'Suppliers');
    fixture.detectChanges();
    expect(fixture.nativeElement.querySelector('[role="region"]').getAttribute('aria-label')).toBe('Suppliers');
  });

  it('renders typed columns and emits server-side sort intent without reordering rows', async () => {
    await TestBed.configureTestingModule({ imports: [DataTableComponent] }).compileComponents();
    const fixture = TestBed.createComponent(DataTableComponent);
    const rows = [{ id: 1, name: 'Zulu', amount: 2 }, { id: 2, name: 'Alpha', amount: 1 }];
    fixture.componentRef.setInput('label', 'Products');
    fixture.componentRef.setInput('columns', [
      { key: 'name', header: 'Name', sortable: true },
      { key: 'amount', header: 'Amount', kind: 'number' },
    ]);
    fixture.componentRef.setInput('rows', rows);
    const changes: unknown[] = [];
    fixture.componentInstance.sortChange.subscribe(value => changes.push(value));
    fixture.detectChanges();

    fixture.nativeElement.querySelector('.sort-button').click();
    expect(changes).toEqual([{ key: 'name', direction: 'asc' }]);
    expect(rows.map(row => row.name)).toEqual(['Zulu', 'Alpha']);
    expect(fixture.nativeElement.querySelector('tbody .column-number').textContent).toContain('2');
  });

  it('renders bounded loading, error and empty table states', async () => {
    await TestBed.configureTestingModule({ imports: [DataTableComponent] }).compileComponents();
    const fixture = TestBed.createComponent(DataTableComponent);
    fixture.componentRef.setInput('label', 'Requests');
    fixture.componentRef.setInput('columns', [{ key: 'number', header: 'Number' }]);
    fixture.componentRef.setInput('rows', []);
    fixture.componentRef.setInput('emptyTitle', 'No requests');
    fixture.detectChanges();
    expect(fixture.nativeElement.querySelector('.table-state.empty').textContent).toContain('No requests');

    fixture.componentRef.setInput('loading', true);
    fixture.componentRef.setInput('loadingTitle', 'Loading');
    fixture.detectChanges();
    expect(fixture.nativeElement.querySelector('[role="status"]').textContent).toContain('Loading');

    fixture.componentRef.setInput('loading', false);
    fixture.componentRef.setInput('error', 'Failed');
    fixture.componentRef.setInput('retryLabel', 'Retry');
    fixture.detectChanges();
    expect(fixture.nativeElement.querySelector('[role="alert"]').textContent).toContain('Failed');
  });

  it('gives reusable page architecture regions accessible names', async () => {
    await TestBed.configureTestingModule({
      imports: [FilterBarComponent, SectionHeaderComponent, SummaryStripComponent, WorkflowFooterComponent],
    }).compileComponents();

    const filters = TestBed.createComponent(FilterBarComponent);
    filters.componentRef.setInput('label', 'Product filters');
    filters.detectChanges();
    expect(filters.nativeElement.querySelector('[role="search"]').getAttribute('aria-label')).toBe('Product filters');

    const section = TestBed.createComponent(SectionHeaderComponent);
    section.componentRef.setInput('title', 'Request lines');
    section.detectChanges();
    expect(section.nativeElement.querySelector('h2').textContent).toContain('Request lines');

    const summary = TestBed.createComponent(SummaryStripComponent);
    summary.componentRef.setInput('label', 'Transfer summary');
    summary.componentRef.setInput('items', [{ label: 'Status', value: 'Draft', tone: 'warning' }]);
    summary.detectChanges();
    expect(summary.nativeElement.querySelector('section').getAttribute('aria-label')).toBe('Transfer summary');
    expect(summary.nativeElement.querySelector('article').classList).toContain('warning');

    const footer = TestBed.createComponent(WorkflowFooterComponent);
    footer.componentRef.setInput('label', 'Workflow actions');
    footer.detectChanges();
    expect(footer.nativeElement.querySelector('footer').getAttribute('aria-label')).toBe('Workflow actions');
  });

  it('renders reusable form sections and localized field cues', async () => {
    await TestBed.configureTestingModule({ imports: [FormSectionComponent, FieldCueComponent] }).compileComponents();

    const section = TestBed.createComponent(FormSectionComponent);
    section.componentRef.setInput('title', 'Request details');
    section.componentRef.setInput('description', 'Complete the required fields');
    section.detectChanges();
    expect(section.nativeElement.querySelector('fieldset legend').textContent).toContain('Request details');
    expect(section.nativeElement.querySelector('.section-description').textContent).toContain('required fields');

    const cue = TestBed.createComponent(FieldCueComponent);
    cue.componentRef.setInput('label', 'Warehouse');
    cue.componentRef.setInput('required', true);
    cue.componentRef.setInput('requiredText', 'Required');
    cue.componentRef.setInput('optionalText', 'Optional');
    cue.detectChanges();
    expect(cue.nativeElement.textContent).toContain('Warehouse');
    expect(cue.nativeElement.querySelector('small.required').textContent).toContain('Required');
  });

  it('renders consistent detail sections with optional actions', async () => {
    await TestBed.configureTestingModule({ imports: [DetailSectionComponent] }).compileComponents();
    const fixture = TestBed.createComponent(DetailSectionComponent);
    fixture.componentRef.setInput('title', 'Commercial data');
    fixture.detectChanges();
    expect(fixture.nativeElement.querySelector('section.detail-section')).toBeTruthy();
    expect(fixture.nativeElement.querySelector('h2').textContent).toContain('Commercial data');
  });

  it('renders server-derived workflow stages and current guidance accessibly', async () => {
    await TestBed.configureTestingModule({ imports: [WorkflowProgressComponent] }).compileComponents();
    const fixture = TestBed.createComponent(WorkflowProgressComponent);
    fixture.componentRef.setInput('label', 'Workflow progress');
    fixture.componentRef.setInput('steps', [
      { label: 'Draft', state: 'complete' },
      { label: 'Submitted', state: 'current' },
      { label: 'Approved', state: 'upcoming' },
    ]);
    fixture.componentRef.setInput('guidance', 'Approval is required before posting.');
    fixture.detectChanges();
    expect(fixture.nativeElement.querySelector('section').getAttribute('aria-label')).toBe('Workflow progress');
    expect(fixture.nativeElement.querySelector('[aria-current="step"]').textContent).toContain('Submitted');
    expect(fixture.nativeElement.textContent).toContain('Approval is required');
  });
});
