import { HttpErrorResponse, HttpRequest } from '@angular/common/http';
import { TestBed } from '@angular/core/testing';
import { throwError } from 'rxjs';
import { ToastService } from '../../shared/ui/toast/toast.service';
import { operationErrorToastInterceptor } from './operation-error-toast.interceptor';

describe('operationErrorToastInterceptor',()=>{
  const toasts={error:vi.fn()};
  beforeEach(()=>{vi.clearAllMocks();TestBed.configureTestingModule({providers:[{provide:ToastService,useValue:toasts}]});});
  function execute(error:HttpErrorResponse){TestBed.runInInjectionContext(()=>operationErrorToastInterceptor(new HttpRequest('DELETE','/api/v1/warehouses/2'),()=>throwError(()=>error)).subscribe({error:()=>undefined}));}
  it('shows a concise toast for an operation conflict',()=>{execute(new HttpErrorResponse({status:409,error:{title:'Warehouse cannot be deleted.',detail:'Warehouse is in use.',correlationId:'REF-1'}}));expect(toasts.error).toHaveBeenCalledWith('Warehouse cannot be deleted.','Warehouse is in use.','REF-1');});
  it('leaves field validation to the form',()=>{execute(new HttpErrorResponse({status:400,error:{title:'Validation',errors:{Name:['Required']}}}));expect(toasts.error).not.toHaveBeenCalled();});
  it('does not duplicate authentication handling',()=>{execute(new HttpErrorResponse({status:401,error:{title:'Unauthorized'}}));expect(toasts.error).not.toHaveBeenCalled();});
});
