using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.Services;
namespace MiniStore.Web.Controllers;
[Authorize(Roles = "Admin")]
public class PaymentMethodsController(PaymentMethodService service, AccountService accounts) : Controller
{
    public async Task<IActionResult> Index() => View(await service.GetAllAsync());
    [HttpGet] public async Task<IActionResult> Create() { ViewBag.Accounts = await accounts.GetAllAsync(); return View(); }
    [HttpPost] public async Task<IActionResult> Create(string name, int accountId)
    {
        try { await service.CreateAsync(name, accountId); return RedirectToAction(nameof(Index)); }
        catch (Exception ex) { ModelState.AddModelError(string.Empty, ex.Message); ViewBag.Accounts = await accounts.GetAllAsync(); return View(); }
    }
}
