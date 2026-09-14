using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.Saas;
using MiniStore.Domain.Entities;

namespace MiniStore.Web.Controllers;

[Authorize]
[Route("subscription")]
public class SubscriptionController(PlatformSaasService plans, IBillingCheckoutService checkout) : Controller
{
    [HttpGet("")] public async Task<IActionResult> Index() => View(await plans.GetPublicPlansAsync());
    [HttpPost("checkout"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(int planId, BillingCycle cycle, string? promotionCode)
    {
        try { return View(await checkout.CreateAsync(planId, cycle, promotionCode)); }
        catch (InvalidOperationException ex) { TempData["NotificationType"]="error"; TempData["NotificationMessage"]=ex.Message; return RedirectToAction(nameof(Index)); }
    }
}
