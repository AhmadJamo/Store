import { TestBed } from '@angular/core/testing';
import { DataTableComponent } from './data-table/data-table';
import { FeedbackStateComponent } from './feedback-state/feedback-state';
import { PageHeaderComponent } from './page-header/page-header';
import { PagerComponent } from './pager/pager';
import { FilterBarComponent } from './filter-bar/filter-bar';
import { SectionHeaderComponent } from './section-header/section-header';
import { SummaryStripComponent } from './summary-strip/summary-strip';
import { WorkflowFooterComponent } from './workflow-footer/workflow-footer';

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
});
