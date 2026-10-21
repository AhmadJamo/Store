using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.Dtos.Sales;
using MiniStore.Application.Services;
using MiniStore.Domain.Entities;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers;

public sealed class SalesOrdersController(SalesOrderService service,SalesOrderReservationService reservationService,CustomerInvoiceService invoiceService,
    SalesOrderApprovalService approvalService,IStringLocalizer<SharedResource> localizer):Controller
{
    [HttpGet,PermissionAuthorize("SalesOrders.View")]
    public async Task<IActionResult> Index(SalesOrderStatus? status,string? search)
    {ViewBag.Status=status;ViewBag.Search=search;return View(await service.GetAllAsync(status,search));}

    [HttpGet,PermissionAuthorize("SalesOrders.View")]
    public async Task<IActionResult> Details(int id){var dto=await service.GetAsync(id);if(dto is null)return NotFound();dto.Invoice=await invoiceService.GetProgressAsync(id);return View(dto);}

    [HttpPost,ValidateAntiForgeryToken,PermissionAuthorize("SalesOrders.Create")]
    public async Task<IActionResult> CreateFromQuotation(ConvertSalesQuotationDto dto)
    {
        try{var id=await service.CreateFromQuotationAsync(dto);Notify("success","Sales order created from accepted quotation.");return RedirectToAction(nameof(Details),new{id});}
        catch(Exception ex)when(ex is ArgumentException or InvalidOperationException){Notify("danger",ex.Message);return RedirectToAction("Details","SalesQuotations",new{id=dto.QuotationId});}
    }

    [HttpPost,ValidateAntiForgeryToken,PermissionAuthorize("SalesOrders.Confirm")]
    public async Task<IActionResult> Confirm(SalesOrderStatusCommandDto dto)
    {
        try
        {
            var outcome=await service.ConfirmAsync(dto);
            Notify(outcome==SalesOrderConfirmationOutcome.Confirmed?"success":outcome==SalesOrderConfirmationOutcome.PendingApproval?"warning":"danger",
                outcome==SalesOrderConfirmationOutcome.Confirmed?"Sales order confirmed.":outcome==SalesOrderConfirmationOutcome.PendingApproval?"Sales order submitted for approval.":"Sales order confirmation was blocked by a configured control.");
        }
        catch(Exception ex)when(ex is ArgumentException or InvalidOperationException){Notify("danger",ex.Message);}
        return RedirectToAction(nameof(Details),new{id=dto.Id});
    }
    [HttpPost,ValidateAntiForgeryToken,PermissionAuthorize("SalesOrders.Cancel")]
    public Task<IActionResult> Cancel(SalesOrderStatusCommandDto dto)=>Change(dto,service.CancelAsync,"Sales order cancelled.");

    [HttpPost,ValidateAntiForgeryToken,PermissionAuthorize("SalesOrders.ApproveException")]
    public Task<IActionResult> ApproveException(SalesOrderApprovalDecisionDto dto)=>Decide(dto,true);

    [HttpPost,ValidateAntiForgeryToken,PermissionAuthorize("SalesOrders.ApproveException")]
    public Task<IActionResult> RejectException(SalesOrderApprovalDecisionDto dto)=>Decide(dto,false);

    [HttpGet,PermissionAuthorize("SalesOrders.View")]
    public async Task<IActionResult> Reserve(int id)
    {var dto=await reservationService.GetPageAsync(id);return dto is null?NotFound():View(dto);}

    [HttpPost,ValidateAntiForgeryToken,PermissionAuthorize("SalesOrders.Reserve")]
    public async Task<IActionResult> Reserve(CreateSalesOrderReservationDto dto)
    {
        try{await reservationService.ReserveAsync(dto);Notify("success","Sales order inventory reserved.");}
        catch(Exception ex)when(ex is ArgumentException or InvalidOperationException){Notify("danger",ex.Message);}
        return RedirectToAction(nameof(Reserve),new{id=dto.SalesOrderId});
    }

    [HttpPost,ValidateAntiForgeryToken,PermissionAuthorize("SalesOrders.ReleaseReservation")]
    public async Task<IActionResult> ReleaseReservation(ReleaseSalesOrderReservationDto dto)
    {
        try{await reservationService.ReleaseAsync(dto);Notify("success","Sales order inventory reservation released.");}
        catch(Exception ex)when(ex is ArgumentException or InvalidOperationException){Notify("danger",ex.Message);}
        return RedirectToAction(nameof(Reserve),new{id=dto.SalesOrderId});
    }

    private async Task<IActionResult> Change(SalesOrderStatusCommandDto dto,Func<SalesOrderStatusCommandDto,Task> command,string success)
    {try{await command(dto);Notify("success",success);}catch(Exception ex)when(ex is ArgumentException or InvalidOperationException){Notify("danger",ex.Message);}return RedirectToAction(nameof(Details),new{id=dto.Id});}
    private async Task<IActionResult> Decide(SalesOrderApprovalDecisionDto dto,bool approve)
    {try{var status=await approvalService.DecideAsync(dto,approve);Notify(approve?"success":"warning",!approve?"Sales order approval rejected.":status==SalesOrderApprovalStatus.Approved?"Sales order approved and confirmed.":"Sales order approval step completed.");}catch(Exception ex)when(ex is ArgumentException or InvalidOperationException or UnauthorizedAccessException){Notify("danger",ex.Message);}return RedirectToAction(nameof(Details),new{id=dto.SalesOrderId});}
    private void Notify(string type,string key){TempData["NotificationType"]=type;TempData["NotificationMessage"]=localizer[key].Value;}
}
