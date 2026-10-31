import{DatePipe,DecimalPipe}from'@angular/common';
import{ChangeDetectionStrategy,Component,inject,signal}from'@angular/core';
import{FormBuilder,ReactiveFormsModule,Validators}from'@angular/forms';
import{Router,RouterLink}from'@angular/router';
import{forkJoin}from'rxjs';
import{ApiErrorService}from'../../core/api/api-error.service';
import{NormalizedApiProblem}from'../../core/api/api.models';
import{ApiRecoveryComponent}from'../../shared/ui/api-recovery/api-recovery';
import{PlatformApiService}from'./platform-api.service';
import{PlatformManagementLocalizationService}from'./platform-management-localization.service';
import{PlatformCompany,PlatformManagementPage,PlatformPlan,PlatformSession}from'./platform.models';
@Component({selector:'app-platform-management-page',imports:[DatePipe,DecimalPipe,ReactiveFormsModule,RouterLink,ApiRecoveryComponent],templateUrl:'./platform-management-page.html',styleUrl:'./platform-pages.scss',changeDetection:ChangeDetectionStrategy.OnPush})
export class PlatformManagementPageComponent{
 protected readonly i18n=inject(PlatformManagementLocalizationService);private readonly api=inject(PlatformApiService);private readonly errors=inject(ApiErrorService);private readonly router=inject(Router);private readonly fb=inject(FormBuilder);
 protected readonly page=signal<PlatformManagementPage|null>(null);protected readonly session=signal<PlatformSession|null>(null);protected readonly problem=signal<NormalizedApiProblem|null>(null);protected readonly selectedCompany=signal<PlatformCompany|null>(null);protected readonly busy=signal(false);
 protected readonly companyForm=this.fb.nonNullable.group({tenantId:0,planId:[0,Validators.min(1)],status:2,periodEnd:['',Validators.required],companyIsActive:true});
 protected readonly planForm=this.fb.nonNullable.group({code:['',[Validators.required,Validators.maxLength(50)]],nameEnglish:['',Validators.required],nameArabic:['',Validators.required],descriptionEnglish:'',descriptionArabic:'',monthlyPrice:[0,Validators.min(0)],annualPrice:[0,Validators.min(0)],currency:['USD',[Validators.required,Validators.pattern(/^[A-Za-z]{3}$/)]],maxUsers:[1,Validators.min(1)],maxWarehouses:[1,Validators.min(1)],maxBranches:[1,Validators.min(1)],maxPosTerminals:[1,Validators.min(1)],maxProducts:[1,Validators.min(1)]});
 protected readonly promoForm=this.fb.group({code:this.fb.nonNullable.control('',Validators.required),discountPercentage:this.fb.nonNullable.control(1,[Validators.min(.01),Validators.max(100)]),startsAt:this.fb.nonNullable.control('',Validators.required),endsAt:this.fb.nonNullable.control('',Validators.required),maximumRedemptions:this.fb.control<number|null>(null),planId:this.fb.control<number|null>(null)});
 protected readonly paymentForm=this.fb.nonNullable.group({sessionId:'',providerReference:['',[Validators.required,Validators.maxLength(200)]]});
 constructor(){this.load();}
 canDeactivate(){return![this.companyForm,this.planForm,this.promoForm,this.paymentForm].some(f=>f.dirty)||globalThis.confirm(this.i18n.text('unsaved'));}
 protected load(){this.problem.set(null);forkJoin({session:this.api.session(),page:this.api.management()}).subscribe({next:x=>{this.session.set(x.session);this.page.set(x.page)},error:e=>{if(e.status===401)void this.router.navigateByUrl('/platform-login');else this.problem.set(this.errors.normalize(e))}});}
 protected edit(c:PlatformCompany){this.selectedCompany.set(c);this.companyForm.reset({tenantId:c.tenantId,planId:c.planId??0,status:c.status??2,periodEnd:c.periodEnd?.slice(0,10)??'',companyIsActive:c.isActive});}
 protected updateCompany(){if(this.companyForm.invalid)return;const v=this.companyForm.getRawValue();this.run(()=>this.api.updateCompany({...v,periodEnd:new Date(`${v.periodEnd}T23:59:59`).toISOString()}),()=>{this.selectedCompany.set(null);this.companyForm.markAsPristine()});}
 protected createPlan(){if(this.planForm.invalid)return;this.run(()=>this.api.createPlan(this.planForm.getRawValue()),()=>{this.planForm.reset({currency:'USD',monthlyPrice:0,annualPrice:0,maxUsers:1,maxWarehouses:1,maxBranches:1,maxPosTerminals:1,maxProducts:1});this.planForm.markAsPristine()});}
 protected createPromotion(){if(this.promoForm.invalid)return;const v=this.promoForm.getRawValue();this.run(()=>this.api.createPromotion({...v,startsAt:new Date(v.startsAt).toISOString(),endsAt:new Date(v.endsAt).toISOString()}),()=>{this.promoForm.reset({discountPercentage:1,maximumRedemptions:null,planId:null});this.promoForm.markAsPristine()});}
 protected choosePayment(id:string){this.paymentForm.reset({sessionId:id,providerReference:''});}
 protected confirmPayment(){if(this.paymentForm.invalid)return;const v=this.paymentForm.getRawValue();this.run(()=>this.api.confirmPayment(v.sessionId,v.providerReference.trim()),()=>{this.paymentForm.reset();this.paymentForm.markAsPristine()});}
 protected planName(p:PlatformPlan){return this.i18n.locale.locale()==='ar-JO'?p.nameArabic:p.nameEnglish;}
 private run(call:()=>ReturnType<PlatformApiService['logout']>,done:()=>void){if(this.busy())return;this.busy.set(true);this.problem.set(null);call().subscribe({next:()=>{done();this.busy.set(false);this.load()},error:e=>{this.problem.set(this.errors.normalize(e));this.busy.set(false)}});}
}
