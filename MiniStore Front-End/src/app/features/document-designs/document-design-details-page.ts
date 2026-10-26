import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { LocalizationService } from '../../core/localization/localization.service';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { DocumentDesignDetails } from './data/document-design.models';
import { DocumentDesignsApiService } from './data/document-designs-api.service';

@Component({selector:'app-document-design-details-page',imports:[RouterLink,ApiRecoveryComponent,FeedbackStateComponent,PageHeaderComponent],templateUrl:'./document-design-details-page.html',styleUrl:'./document-designs.scss',changeDetection:ChangeDetectionStrategy.OnPush})
export class DocumentDesignDetailsPageComponent{
 protected readonly i18n=inject(LocalizationService);private readonly api=inject(DocumentDesignsApiService);private readonly errors=inject(ApiErrorService);private readonly id=Number(inject(ActivatedRoute).snapshot.paramMap.get('id'));
 protected readonly loading=signal(true);protected readonly saving=signal(false);protected readonly problem=signal<NormalizedApiProblem|null>(null);protected readonly details=signal<DocumentDesignDetails|null>(null);protected readonly message=signal<string|null>(null);
 constructor(){this.load();}protected load():void{this.loading.set(true);this.api.get(this.id).subscribe({next:value=>{this.details.set(value);this.loading.set(false);},error:error=>{this.problem.set(this.errors.normalize(error));this.loading.set(false);}});}
 protected command(action:'activate'|'archive'|'default'):void{const current=this.details();if(!current||this.saving())return;this.saving.set(true);this.problem.set(null);const request=action==='activate'?this.api.activate(this.id,current.rowVersion):action==='archive'?this.api.archive(this.id,current.rowVersion):this.api.setDefault(this.id,current.rowVersion);request.subscribe({next:value=>{this.details.set(value);this.message.set(action);this.saving.set(false);},error:error=>{this.problem.set(this.errors.normalize(error));this.saving.set(false);}});}
 protected statusName(value:number):string{return this.i18n.text((['','draftStatus','active','archived'][value]??'draftStatus') as 'draftStatus');}
}
