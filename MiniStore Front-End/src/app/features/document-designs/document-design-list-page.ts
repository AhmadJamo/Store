import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ApiErrorService } from '../../core/api/api-error.service';
import { NormalizedApiProblem } from '../../core/api/api.models';
import { LocalizationService } from '../../core/localization/localization.service';
import { ApiRecoveryComponent } from '../../shared/ui/api-recovery/api-recovery';
import { FeedbackStateComponent } from '../../shared/ui/feedback-state/feedback-state';
import { PageHeaderComponent } from '../../shared/ui/page-header/page-header';
import { DocumentDesignSummary } from './data/document-design.models';
import { DocumentDesignsApiService } from './data/document-designs-api.service';

@Component({ selector:'app-document-design-list-page', imports:[ReactiveFormsModule,RouterLink,ApiRecoveryComponent,FeedbackStateComponent,PageHeaderComponent], templateUrl:'./document-design-list-page.html', styleUrl:'./document-designs.scss', changeDetection:ChangeDetectionStrategy.OnPush })
export class DocumentDesignListPageComponent {
  protected readonly i18n=inject(LocalizationService); private readonly api=inject(DocumentDesignsApiService); private readonly errors=inject(ApiErrorService);
  protected readonly loading=signal(true); protected readonly problem=signal<NormalizedApiProblem|null>(null); protected readonly designs=signal<DocumentDesignSummary[]>([]);
  protected readonly form=inject(FormBuilder).nonNullable.group({search:'',documentType:0,status:0});
  protected readonly types=[1,2,3,4,5,6,7,8,9,10,99];
  constructor(){this.load();}
  protected load():void{const value=this.form.getRawValue();this.loading.set(true);this.problem.set(null);this.api.getAll({search:value.search,documentType:value.documentType||undefined,status:value.status||undefined}).subscribe({next:rows=>{this.designs.set(rows);this.loading.set(false);},error:error=>{this.problem.set(this.errors.normalize(error));this.loading.set(false);}});}
  protected typeName(value:number):string{return this.i18n.text((['','salesQuotationDesign','salesOrderDesign','salesInvoiceDesign','salesReceiptDesign','customerCreditNoteDesign','purchaseOrderDesign','goodsReceiptDesign','vendorBillDesign','supplierPaymentDesign','inventoryTransferDesign'][value]??'customDesign') as 'customDesign');}
  protected statusName(value:number):string{return this.i18n.text((['','draftStatus','active','archived'][value]??'draftStatus') as 'draftStatus');}
}
