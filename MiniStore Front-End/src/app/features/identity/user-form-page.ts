import { ChangeDetectionStrategy, Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { LocalizationService } from '../../core/localization/localization.service';
import { SafeFormShortcutsDirective } from '../../shared/forms/safe-form-shortcuts.directive';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { WorkspaceService } from '../../workspace/state/workspace.service';
import { IdentityApiService } from './data/identity-api.service';
import { RoleOption } from './data/identity.models';

@Component({ selector: 'app-user-form-page', imports: [ReactiveFormsModule, RouterLink, SafeFormShortcutsDirective, ApiRecoveryComponent, FeedbackStateComponent, PageHeaderComponent], templateUrl: './user-form-page.html', styleUrl: './identity-pages.scss', changeDetection: ChangeDetectionStrategy.OnPush })
export class UserFormPageComponent {
  protected readonly i18n = inject(LocalizationService); private readonly api=inject(IdentityApiService); private readonly errors=inject(ApiErrorService); private readonly router=inject(Router); private readonly workspace=inject(WorkspaceService); private readonly destroyRef=inject(DestroyRef);
  protected readonly roles=signal<readonly RoleOption[]>([]); protected readonly loading=signal(true); protected readonly saving=signal(false); protected readonly problem=signal<NormalizedApiProblem|null>(null);
  protected readonly form=inject(FormBuilder).nonNullable.group({userName:['',[Validators.required,Validators.maxLength(100)]],email:['',[Validators.required,Validators.email,Validators.maxLength(256)]],password:['',[Validators.required,Validators.maxLength(1024)]],roleId:[0,[Validators.required,Validators.min(1)]]});
  constructor(){this.api.getUserCreatePage().subscribe({next:value=>{this.roles.set(value.roles);this.loading.set(false);},error:error=>{this.problem.set(this.errors.normalize(error));this.loading.set(false);}});this.form.valueChanges.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(()=>this.syncDirty());}
  canDeactivate():boolean{return !this.form.dirty||globalThis.confirm(this.i18n.text('unsavedDescription'));}
  protected save():void{if(this.form.invalid||this.saving()){this.form.markAllAsTouched();return;}this.saving.set(true);this.problem.set(null);const value=this.form.getRawValue();this.api.createUser({...value,userName:value.userName.trim(),email:value.email.trim()}).subscribe({next:()=>{this.form.markAsPristine();this.syncDirty();void this.router.navigate(['/users']);},error:error=>{this.problem.set(this.errors.normalize(error));this.form.controls.password.reset('');this.saving.set(false);}});}
  private syncDirty():void{const tab=this.workspace.activeTab();if(tab)this.workspace.markDirty(tab.id,this.form.dirty);}
}
