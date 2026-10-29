import{DatePipe}from'@angular/common';
import{ChangeDetectionStrategy,Component,inject,signal}from'@angular/core';
import{FormBuilder,ReactiveFormsModule}from'@angular/forms';
import{Router,RouterLink}from'@angular/router';
import{ApiErrorService}from'../../core/api/api-error.service';
import{NormalizedApiProblem}from'../../core/api/api.models';
import{ApiRecoveryComponent}from'../../shared/ui/api-recovery/api-recovery';
import{PlatformApiService}from'./platform-api.service';
import{PlatformAuditLocalizationService}from'./platform-audit-localization.service';
import{PlatformAuditPage,PlatformAuditQuery,PlatformSession}from'./platform.models';
@Component({selector:'app-platform-audit-page',imports:[DatePipe,ReactiveFormsModule,RouterLink,ApiRecoveryComponent],templateUrl:'./platform-audit-page.html',styleUrl:'./platform-pages.scss',changeDetection:ChangeDetectionStrategy.OnPush})
export class PlatformAuditPageComponent{
 protected readonly i18n=inject(PlatformAuditLocalizationService);private readonly api=inject(PlatformApiService);private readonly errors=inject(ApiErrorService);private readonly router=inject(Router);private readonly fb=inject(FormBuilder);
 protected readonly page=signal<PlatformAuditPage|null>(null);protected readonly session=signal<PlatformSession|null>(null);protected readonly problem=signal<NormalizedApiProblem|null>(null);protected readonly busy=signal(false);protected readonly purpose=signal('InternalReview');
 protected readonly form=this.fb.group({from:'',to:'',category:this.fb.control<number|null>(null),outcome:this.fb.control<number|null>(null),eventType:'',actorId:'',resourceType:'',resourceId:'',correlationId:'',page:1,pageSize:25});
 constructor(){this.api.session().subscribe({next:x=>{if(!x.canViewAudit){void this.router.navigateByUrl('/platform-control');return;}this.session.set(x);this.load()},error:()=>void this.router.navigateByUrl('/platform-login')});}
 protected load(){this.problem.set(null);this.api.auditEvents(this.query()).subscribe({next:x=>this.page.set(x),error:e=>this.problem.set(this.errors.normalize(e))});}
 protected search(){this.form.controls.page.setValue(1);this.load();}
 protected reset(){this.form.reset({from:'',to:'',category:null,outcome:null,eventType:'',actorId:'',resourceType:'',resourceId:'',correlationId:'',page:1,pageSize:25});this.load();}
 protected move(delta:number){const p=this.page();if(!p)return;this.form.controls.page.setValue(Math.min(p.totalPages,Math.max(1,(this.form.controls.page.value??1)+delta)));this.load();}
 protected export(){if(this.busy())return;this.busy.set(true);this.problem.set(null);this.api.exportAudit(this.query(),this.purpose()).subscribe({next:blob=>{const url=URL.createObjectURL(blob);const link=document.createElement('a');link.href=url;link.download='platform-audit.csv';link.click();URL.revokeObjectURL(url);this.busy.set(false)},error:e=>{this.problem.set(this.errors.normalize(e));this.busy.set(false)}});}
 private query():PlatformAuditQuery{const v=this.form.getRawValue();return{from:v.from||undefined,to:v.to||undefined,category:v.category,outcome:v.outcome,eventType:v.eventType||undefined,actorId:v.actorId||undefined,resourceType:v.resourceType||undefined,resourceId:v.resourceId||undefined,correlationId:v.correlationId||undefined,page:v.page??1,pageSize:v.pageSize??25};}
}
