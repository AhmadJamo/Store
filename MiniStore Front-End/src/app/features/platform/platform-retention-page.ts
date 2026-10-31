import{DatePipe}from'@angular/common';
import{ChangeDetectionStrategy,Component,inject,signal}from'@angular/core';
import{FormBuilder,ReactiveFormsModule,Validators}from'@angular/forms';
import{Router,RouterLink}from'@angular/router';
import{Observable}from'rxjs';
import{ApiErrorService}from'../../core/api/api-error.service';
import{NormalizedApiProblem}from'../../core/api/api.models';
import{ApiRecoveryComponent}from'../../shared/ui/api-recovery/api-recovery';
import{PlatformApiService}from'./platform-api.service';
import{PlatformAuditLocalizationService}from'./platform-audit-localization.service';
import{PlatformAuditRetentionStatus}from'./platform.models';
@Component({selector:'app-platform-retention-page',imports:[DatePipe,ReactiveFormsModule,RouterLink,ApiRecoveryComponent],templateUrl:'./platform-retention-page.html',styleUrl:'./platform-pages.scss',changeDetection:ChangeDetectionStrategy.OnPush})
export class PlatformRetentionPageComponent{
 protected readonly i18n=inject(PlatformAuditLocalizationService);private readonly api=inject(PlatformApiService);private readonly errors=inject(ApiErrorService);private readonly router=inject(Router);private readonly fb=inject(FormBuilder);
 protected readonly page=signal<PlatformAuditRetentionStatus|null>(null);protected readonly problem=signal<NormalizedApiProblem|null>(null);protected readonly busy=signal(false);protected readonly notice=signal(false);protected readonly releaseId=signal<number|null>(null);
 protected readonly holdForm=this.fb.group({scope:this.fb.nonNullable.control(2),tenantId:this.fb.control<number|null>(null),from:this.fb.nonNullable.control('',Validators.required),to:this.fb.nonNullable.control('',Validators.required),reason:this.fb.nonNullable.control('',[Validators.required,Validators.maxLength(500)])});
 protected readonly releaseForm=this.fb.nonNullable.group({reason:['',[Validators.required,Validators.maxLength(500)]]});
 constructor(){this.api.session().subscribe({next:x=>{if(!x.canManageRetention){void this.router.navigateByUrl('/platform-control');return;}this.load()},error:()=>void this.router.navigateByUrl('/platform-login')});}
 protected load(){this.problem.set(null);this.api.auditRetention().subscribe({next:x=>this.page.set(x),error:e=>this.problem.set(this.errors.normalize(e))});}
 protected createHold(){if(this.holdForm.invalid)return;const v=this.holdForm.getRawValue();this.run(this.api.createAuditHold({scope:v.scope,tenantId:v.scope===1?v.tenantId:null,fromUtc:new Date(`${v.from}T00:00:00Z`).toISOString(),toUtc:new Date(`${v.to}T23:59:59Z`).toISOString(),reason:v.reason.trim()}),()=>{this.holdForm.reset({scope:2,tenantId:null,from:'',to:'',reason:''});this.holdForm.markAsPristine()});}
 protected chooseRelease(id:number){this.releaseId.set(id);this.releaseForm.reset();}
 protected release(){const id=this.releaseId();if(id===null||this.releaseForm.invalid)return;this.run(this.api.releaseAuditHold(id,this.releaseForm.controls.reason.value.trim()),()=>{this.releaseId.set(null);this.releaseForm.reset()});}
 protected checkpoint(){this.run(this.api.createAuditCheckpoint());}
 protected retain(){if(globalThis.confirm(this.i18n.text('runConfirm')))this.run(this.api.runAuditRetention());}
 private run(call:Observable<unknown>,done:()=>void=()=>{}){if(this.busy())return;this.busy.set(true);this.notice.set(false);this.problem.set(null);call.subscribe({next:()=>{done();this.busy.set(false);this.notice.set(true);this.load()},error:e=>{this.problem.set(this.errors.normalize(e));this.busy.set(false)}});}
}
