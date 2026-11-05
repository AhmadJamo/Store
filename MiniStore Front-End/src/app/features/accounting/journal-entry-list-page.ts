import { DatePipe, DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { ApiErrorService } from '../../core/api/api-error.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { PageContextSchema, PageContextService, pageContextFields } from '../../shared/state/page-context.service';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { DataTableComponent } from '../../shared/ui/data-table/data-table';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { PagerComponent } from '../../shared/ui/pager/pager';
import { JournalEntryApiService } from './journal-entry-api.service';
import { JournalEntryLocalizationService } from './journal-entry-localization.service';
import { JournalEntryListPage, JournalEntryListQuery } from './journal-entry.models';

@Component({selector:'app-journal-entry-list-page',imports:[RouterLink,DatePipe,DecimalPipe,PageHeaderComponent,DataTableComponent,FeedbackStateComponent,ApiRecoveryComponent,PagerComponent],templateUrl:'./journal-entry-list-page.html',styleUrl:'./journal-entry-list-page.scss',changeDetection:ChangeDetectionStrategy.OnPush})
export class JournalEntryListPageComponent {
  protected readonly i18n=inject(JournalEntryLocalizationService); private readonly api=inject(JournalEntryApiService); private readonly errors=inject(ApiErrorService); private readonly route=inject(ActivatedRoute); private readonly context=inject(PageContextService); private readonly destroy=inject(DestroyRef); private readonly searchChanges=new Subject<string>();
  protected readonly result=signal<JournalEntryListPage|null>(null); protected readonly loading=signal(true); protected readonly problem=signal<NormalizedApiProblem|null>(null);
  private readonly schema:PageContextSchema<JournalEntryListQuery>={defaults:{search:'',fromDate:'',toDate:'',status:'all',page:1,pageSize:25},fields:{search:pageContextFields.text(),fromDate:pageContextFields.text(10),toDate:pageContextFields.text(10),status:pageContextFields.choice(['all','1','2','3'] as const),page:pageContextFields.integer(),pageSize:pageContextFields.numberChoice([10,25,50,100] as const)}};
  protected query:JournalEntryListQuery={...this.schema.defaults};
  constructor(){this.searchChanges.pipe(debounceTime(300),distinctUntilChanged(),takeUntilDestroyed(this.destroy)).subscribe(search=>this.update({...this.query,search,page:1}));this.context.watch('journal-entry-list',this.route,this.schema).pipe(takeUntilDestroyed(this.destroy)).subscribe(query=>{this.query=query;this.load();});}
  protected search(value:string){this.searchChanges.next(value.trim());} protected filter(patch:Partial<JournalEntryListQuery>){this.update({...this.query,...patch,page:1});} protected move(page:number){if(page>=1&&page<=(this.result()?.totalPages??1))this.update({...this.query,page});}
  protected load(){this.loading.set(true);this.problem.set(null);this.api.getPage(this.query).subscribe({next:value=>{this.result.set(value);this.loading.set(false);},error:error=>{this.problem.set(this.errors.normalize(error));this.loading.set(false);}});}
  private update(query:JournalEntryListQuery){void this.context.replace('journal-entry-list',this.route,this.schema,query);}
}
