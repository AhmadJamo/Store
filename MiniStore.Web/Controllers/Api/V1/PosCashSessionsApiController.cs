using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;
using MiniStore.Web.Middleware.Api;
namespace MiniStore.Web.Controllers.Api.V1;
[ApiController,Authorize,Route("api/v1/pos-cash-sessions")]
public sealed class PosCashSessionsApiController(PosCashSessionService service,IStringLocalizer<SharedResource> localizer):ControllerBase
{
    [HttpGet("terminal/{terminalId:int}"),PermissionAuthorize("PosCashSessions.View")]
    public Task<IActionResult> GetPage(int terminalId)=>Run(async()=>Ok(await service.GetPageAsync(terminalId)));
    [HttpPost("policy"),ValidateAntiForgeryToken,PermissionAuthorize("PosCashSessions.Configure")]
    public Task<IActionResult> ConfigurePolicy(PosCashPolicyCommand input)=>Command(()=>service.ConfigurePolicyAsync(input));
    [HttpPost("drawers"),ValidateAntiForgeryToken,PermissionAuthorize("PosCashSessions.Configure")]
    public Task<IActionResult> CreateDrawer(PosCashDrawerCommand input)=>Run(async()=>Ok(new{id=await service.CreateDrawerAsync(input)}));
    [HttpPost("open"),ValidateAntiForgeryToken,PermissionAuthorize("PosCashSessions.Open")]
    public Task<IActionResult> Open(PosCashOpenCommand input)=>Run(async()=>Ok(new{id=await service.OpenAsync(input)}));
    [HttpPost("{id:long}/movements"),ValidateAntiForgeryToken,PermissionAuthorize("PosCashSessions.Move")]
    public Task<IActionResult> Move(long id,PosCashManualMovementCommand input)=>Command(()=>service.RecordManualAsync(id,input));
    [HttpPost("{id:long}/link-sale/{saleId:int}"),ValidateAntiForgeryToken,PermissionAuthorize("PosCashSessions.Move")]
    public Task<IActionResult> LinkSale(long id,int saleId)=>Command(()=>service.LinkSaleAsync(id,saleId));
    [HttpPost("{id:long}/link-return/{returnId:int}"),ValidateAntiForgeryToken,PermissionAuthorize("PosCashSessions.Move")]
    public Task<IActionResult> LinkReturn(long id,int returnId)=>Command(()=>service.LinkReturnAsync(id,returnId));
    [HttpPost("{id:long}/count"),ValidateAntiForgeryToken,PermissionAuthorize("PosCashSessions.Close")]
    public Task<IActionResult> Count(long id,PosCashCountCommand input)=>Command(()=>service.SubmitCountAsync(id,input.DeclaredCash));
    [HttpPost("{id:long}/approve"),ValidateAntiForgeryToken,PermissionAuthorize("PosCashSessions.Approve")]
    public Task<IActionResult> Approve(long id,PosCashApprovalCommand input)=>Command(()=>service.ApproveAsync(id,input));
    private Task<IActionResult> Command(Func<Task> operation)=>Run(async()=>{await operation();return NoContent();});
    private async Task<IActionResult> Run(Func<Task<IActionResult>> operation){try{return await operation();}catch(Exception e)when(e is ArgumentException or InvalidOperationException){ModelState.AddModelError(string.Empty,localizer[e.Message]);return InternalApiProblemFactory.Validation(HttpContext,ModelState,localizer["One or more validation errors occurred."],InternalApiProblemCodes.Validation);}}
}
public sealed record PosCashCountCommand(decimal DeclaredCash);
