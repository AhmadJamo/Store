using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using Microsoft.Extensions.Localization;using MiniStore.Application.DTOs.Purchases;using MiniStore.Application.Services;using MiniStore.Web.Authorization;using MiniStore.Web.Middleware.Api;
namespace MiniStore.Web.Controllers.Api.V1;
[ApiController,Authorize,Route("api/v1/supplier-credit-notes")]
public sealed class SupplierCreditNotesApiController(SupplierCreditNoteService service,IStringLocalizer<SharedResource> localizer):ControllerBase{
[HttpGet,PermissionAuthorize("SupplierCreditNotes.View")]public async Task<ActionResult<List<SupplierCreditNoteDto>>>GetAll()=>Ok(await service.GetAllAsync());
[HttpGet("{id:int}"),PermissionAuthorize("SupplierCreditNotes.View")]public async Task<ActionResult<SupplierCreditNoteDto>>Get(int id){var x=await service.GetAsync(id);return x is null?NotFound():Ok(x);}
[HttpGet("create-form"),PermissionAuthorize("SupplierCreditNotes.Create")]public async Task<IActionResult>GetCreateForm(int vendorBillId){var x=await service.GetCreatePageAsync(vendorBillId);return string.IsNullOrWhiteSpace(x.VendorBillNumber)?NotFound():Ok(x);}
[HttpPost,ValidateAntiForgeryToken,PermissionAuthorize("SupplierCreditNotes.Create")]public async Task<IActionResult>Create(CreateSupplierCreditNoteDto input){try{return Ok(new{id=await service.CreateAndPostAsync(input)});}catch(Exception e)when(e is ArgumentException or InvalidOperationException){ModelState.AddModelError(string.Empty,localizer[e.Message]);return InternalApiProblemFactory.Validation(HttpContext,ModelState,localizer["One or more validation errors occurred."],InternalApiProblemCodes.Validation);}}
}
