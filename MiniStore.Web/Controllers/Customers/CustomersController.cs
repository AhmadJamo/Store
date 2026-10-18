using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MiniStore.Application.Dtos.Customers;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;
namespace MiniStore.Web.Controllers;
public sealed class CustomersController(CustomerService service, IStringLocalizer<SharedResource> localizer) : Controller
{
    [HttpGet, PermissionAuthorize("Customers.View")]
    public async Task<IActionResult> Index([FromQuery] CustomerListQueryDto query) => View(await service.GetPageAsync(query));

    [HttpGet, PermissionAuthorize("Customers.View")]
    public async Task<IActionResult> Details(int id) => await service.GetDetailsAsync(id) is { } model ? View(model) : NotFound();

    [HttpGet, PermissionAuthorize("Customers.Create")]
    public async Task<IActionResult> Create() => View(await service.GetCreatePageAsync());

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("Customers.Create")]
    public async Task<IActionResult> Create(CustomerFormDto form)
    {
        if (!ModelState.IsValid) return View(await service.GetCreatePageAsync(form));
        try { var id = await service.CreateAsync(form); TempData["Success"] = "Customer created successfully."; return RedirectToAction(nameof(Details), new { id }); }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        { ModelState.AddModelError(string.Empty, localizer[exception.Message]); return View(await service.GetCreatePageAsync(form)); }
    }

    [HttpGet, PermissionAuthorize("Customers.Edit")]
    public async Task<IActionResult> Edit(int id) => await service.GetEditPageAsync(id) is { } model ? View(model) : NotFound();

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("Customers.Edit")]
    public async Task<IActionResult> Edit(CustomerFormDto form)
    {
        if (!ModelState.IsValid) return View(await service.GetEditPageAsync(form.Id, form) ?? await service.GetCreatePageAsync(form));
        try { await service.UpdateAsync(form); TempData["Success"] = "Customer updated successfully."; return RedirectToAction(nameof(Details), new { id = form.Id }); }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        { ModelState.AddModelError(string.Empty, localizer[exception.Message]); return View(await service.GetEditPageAsync(form.Id, form) ?? await service.GetCreatePageAsync(form)); }
    }

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("Customers.Archive")]
    public async Task<IActionResult> Archive(CustomerStatusCommandDto command) => await ChangeStatus(command, true);

    [HttpPost, ValidateAntiForgeryToken, PermissionAuthorize("Customers.Archive")]
    public async Task<IActionResult> Restore(CustomerStatusCommandDto command) => await ChangeStatus(command, false);

    [HttpPost,ValidateAntiForgeryToken,PermissionAuthorize("Customers.Edit")]
    public async Task<IActionResult> SaveContact(CustomerContactCommandDto command)=>await SaveChild(command.CustomerId,()=>service.SaveContactAsync(command),"Customer contact saved successfully.");
    [HttpPost,ValidateAntiForgeryToken,PermissionAuthorize("Customers.Edit")]
    public async Task<IActionResult> SaveAddress(CustomerAddressCommandDto command)=>await SaveChild(command.CustomerId,()=>service.SaveAddressAsync(command),"Customer address saved successfully.");
    [HttpPost,ValidateAntiForgeryToken,PermissionAuthorize("Customers.Archive")]
    public async Task<IActionResult> ArchiveContact(CustomerChildStatusCommandDto command)=>await SaveChild(command.CustomerId,()=>service.ChangeContactStatusAsync(command,true),"Customer contact archived successfully.");
    [HttpPost,ValidateAntiForgeryToken,PermissionAuthorize("Customers.Archive")]
    public async Task<IActionResult> RestoreContact(CustomerChildStatusCommandDto command)=>await SaveChild(command.CustomerId,()=>service.ChangeContactStatusAsync(command,false),"Customer contact restored successfully.");
    [HttpPost,ValidateAntiForgeryToken,PermissionAuthorize("Customers.Archive")]
    public async Task<IActionResult> ArchiveAddress(CustomerChildStatusCommandDto command)=>await SaveChild(command.CustomerId,()=>service.ChangeAddressStatusAsync(command,true),"Customer address archived successfully.");
    [HttpPost,ValidateAntiForgeryToken,PermissionAuthorize("Customers.Archive")]
    public async Task<IActionResult> RestoreAddress(CustomerChildStatusCommandDto command)=>await SaveChild(command.CustomerId,()=>service.ChangeAddressStatusAsync(command,false),"Customer address restored successfully.");

    private async Task<IActionResult> SaveChild(int customerId,Func<Task> action,string success)
    {
        if(!ModelState.IsValid){TempData["NotificationType"]="error";TempData["NotificationMessage"]=localizer["Please correct the entered customer information."].Value;return RedirectToAction(nameof(Details),new{id=customerId});}
        try{await action();TempData["Success"]=success;}
        catch(Exception exception) when(exception is ArgumentException or InvalidOperationException){TempData["NotificationType"]="error";TempData["NotificationMessage"]=localizer[exception.Message].Value;}
        return RedirectToAction(nameof(Details),new{id=customerId});
    }

    private async Task<IActionResult> ChangeStatus(CustomerStatusCommandDto command, bool archive)
    {
        try { if (archive) await service.ArchiveAsync(command); else await service.RestoreAsync(command); TempData["Success"] = archive ? "Customer archived successfully." : "Customer restored successfully."; }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException) { TempData["NotificationType"] = "error"; TempData["NotificationMessage"] = localizer[exception.Message].Value; }
        return RedirectToAction(nameof(Details), new { id = command.Id });
    }
}
