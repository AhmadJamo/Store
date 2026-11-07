import { DatePipe, DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormArray, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { ApiSessionService } from '../../core/api/api-session.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { PageContextService } from '../../shared/state/page-context.service';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { DataTableComponent } from '../../shared/ui/data-table/data-table';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { RecurringJournalApiService } from './recurring-journal-api.service';
import { RecurringJournalLocalizationService } from './recurring-journal-localization.service';
import { RecurringJournalPage, RecurringJournalTemplate, SaveRecurringJournalTemplate } from './recurring-journal.models';

@Component({selector:'app-recurring-journal-page',imports:[DatePipe,DecimalPipe,ReactiveFormsModule,RouterLink,PageHeaderComponent,DataTableComponent,FeedbackStateComponent,ApiRecoveryComponent],templateUrl:'./recurring-journal-page.html',styleUrl:'./recurring-journal-page.scss',changeDetection:ChangeDetectionStrategy.OnPush})
export class RecurringJournalPageComponent {
  protected readonly i18n=inject(RecurringJournalLocalizationService); protected readonly session=inject(ApiSessionService);
  private readonly api=inject(RecurringJournalApiService); private readonly errors=inject(ApiErrorService); private readonly fb=inject(FormBuilder); private readonly context=inject(PageContextService); private readonly workspace=inject(WorkspaceService); private readonly destroy=inject(DestroyRef);
  protected readonly loading=signal(true); protected readonly saving=signal(false); protected readonly failure=signal<NormalizedApiProblem|null>(null); protected readonly problem=signal<NormalizedApiProblem|null>(null); protected readonly page=signal<RecurringJournalPage|null>(null); protected readonly editingId=signal(0); protected readonly success=signal(''); protected readonly generatedId=signal<number|null>(null);
  private requestKey=crypto.randomUUID();
  protected readonly form=this.fb.group({name:['',[Validators.required,Validators.maxLength(150)]],description:['',[Validators.required,Validators.maxLength(500)]],frequency:[1,[Validators.required]],startDate:[new Date().toISOString().slice(0,10),Validators.required],endDate:[''],rowVersion:[''],lines:this.fb.array([])});
  constructor(){this.form.valueChanges.pipe(takeUntilDestroyed(this.destroy)).subscribe(()=>{this.persistDraft();this.markDirty();});this.load();}
  get lines(){return this.form.controls.lines as FormArray;}
  canDeactivate(){return !this.form.dirty||globalThis.confirm(this.i18n.text('unsaved'));}
  protected addLine(value:any={}){this.lines.push(this.fb.group({accountId:[value.accountId??0,Validators.min(1)],branchId:value.branchId??null,warehouseId:value.warehouseId??null,debit:[value.debit??0,Validators.min(0)],credit:[value.credit??0,Validators.min(0)],description:[value.description??null,Validators.maxLength(500)]}));}
  protected removeLine(index:number){if(this.lines.length>2)this.lines.removeAt(index);}
  protected total(kind:'debit'|'credit'){return this.lines.controls.reduce((sum,row)=>sum+Number(row.get(kind)?.value??0),0);}
  protected balanced(){return this.lines.length>=2&&this.total('debit')>0&&this.total('debit')===this.total('credit');}
  protected edit(item:RecurringJournalTemplate){this.editingId.set(item.id);this.lines.clear();this.form.patchValue({name:item.name,description:item.description,frequency:item.frequency,startDate:item.startDate.slice(0,10),endDate:item.endDate?.slice(0,10)??'',rowVersion:item.rowVersion},{emitEvent:false});item.lines.forEach(line=>this.addLine(line));this.form.markAsPristine();this.markDirty();}
  protected reset(){this.editingId.set(0);this.lines.clear();this.form.reset({name:'',description:'',frequency:1,startDate:new Date().toISOString().slice(0,10),endDate:'',rowVersion:''},{emitEvent:false});this.addLine();this.addLine();this.form.markAsPristine();this.context.clearDraft('recurring-journal-template');this.markDirty();}
  protected save(){if(this.form.invalid||!this.balanced()||this.saving()) {this.form.markAllAsTouched();return;}this.saving.set(true);this.problem.set(null);const raw=this.form.getRawValue();const value={...raw,endDate:raw.endDate||null,rowVersion:raw.rowVersion||null} as SaveRecurringJournalTemplate;const request=this.editingId()?this.api.update(this.editingId(),value):this.api.create(value);request.subscribe({next:()=>{this.success.set(this.i18n.text(this.editingId()?'updated':'created'));this.context.clearDraft('recurring-journal-template');this.form.markAsPristine();this.load(true);},error:error=>this.failed(error)});}
  protected toggle(item:RecurringJournalTemplate){if(item.isActive&&!globalThis.confirm(this.i18n.text('confirmPause')))return;this.saving.set(true);this.api.setState(item.id,!item.isActive,item.rowVersion).subscribe({next:()=>this.load(true),error:error=>this.failed(error)});}
  protected run(item:RecurringJournalTemplate){if(!globalThis.confirm(this.i18n.text('confirmRun')))return;this.saving.set(true);this.problem.set(null);this.api.run(item.id,item.rowVersion,this.requestKey).subscribe({next:value=>{this.generatedId.set(value.journalEntryId);this.success.set(this.i18n.text('runSuccess'));this.requestKey=crypto.randomUUID();this.load(true);},error:error=>this.failed(error)});}
  protected retry(){this.load();}
  private load(keepMessage=false){this.loading.set(true);this.failure.set(null);this.api.getPage().subscribe({next:value=>{this.page.set(value);this.loading.set(false);this.saving.set(false);if(!this.lines.length)this.restoreDraft();if(this.editingId()){const current=value.templates.find(x=>x.id===this.editingId());if(current)this.edit(current);else this.reset();}if(!keepMessage)this.success.set('');},error:error=>{this.failure.set(this.errors.normalize(error));this.loading.set(false);this.saving.set(false);}});}
  private restoreDraft(){const draft=this.context.loadDraft<any>('recurring-journal-template');if(draft){this.form.patchValue(draft,{emitEvent:false});for(const line of draft.lines??[{},{}])this.addLine(line);}else{this.addLine();this.addLine();}this.form.markAsPristine();this.markDirty();}
  private persistDraft(){if(!this.page()||this.editingId())return;if(this.form.dirty)this.context.saveDraft('recurring-journal-template',this.form.getRawValue());else this.context.clearDraft('recurring-journal-template');}
  private failed(error:unknown){this.problem.set(this.errors.normalize(error));this.saving.set(false);}
  private markDirty(){const tab=this.workspace.activeTab();if(tab)this.workspace.markDirty(tab.id,this.form.dirty);}
}
