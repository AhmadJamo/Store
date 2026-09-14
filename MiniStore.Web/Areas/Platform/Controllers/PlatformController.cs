using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.Saas;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Areas.Platform.Controllers;

[Area("Platform")]
[Route("platform")]
[Authorize(AuthenticationSchemes = PlatformAuthentication.Scheme, Policy = PlatformAuthentication.Policy)]
public class PlatformController(PlatformSaasService service, IBillingCheckoutService checkout) : Controller
{
    [HttpGet("")] public async Task<IActionResult> Index() => View(await service.GetPlansAsync());
    [HttpGet("companies")] public async Task<IActionResult> Companies() { ViewBag.Plans = await service.GetPlansAsync(); return View(await service.GetCompaniesAsync()); }
    [HttpPost("companies/update"), ValidateAntiForgeryToken]
    [Authorize(Policy = PlatformAuthentication.ManagementPolicy)]
    public async Task<IActionResult> UpdateCompany(UpdateCompanySubscriptionDto dto) { await service.UpdateCompanySubscriptionAsync(dto); return RedirectToAction(nameof(Companies)); }
    [HttpGet("payments/pending")] public async Task<IActionResult> PendingPayments() => View(await checkout.GetPendingAsync());
    [HttpPost("payments/confirm"), ValidateAntiForgeryToken]
    [Authorize(Policy = PlatformAuthentication.BillingPolicy)]
    public async Task<IActionResult> ConfirmPayment(Guid sessionId, string providerReference) { await checkout.CompleteAsync(sessionId, providerReference); return RedirectToAction(nameof(PendingPayments)); }
    [HttpGet("plans")] public async Task<IActionResult> Plans() => View(await service.GetPlansAsync());
    [HttpGet("plans/create")] public IActionResult CreatePlan() => View();
    [HttpPost("plans/create"), ValidateAntiForgeryToken]
    [Authorize(Policy = PlatformAuthentication.ManagementPolicy)]
    public async Task<IActionResult> CreatePlan(CreatePlanDto dto) { if (!ModelState.IsValid) return View(dto); await service.CreatePlanAsync(dto); return RedirectToAction(nameof(Plans)); }
    [HttpGet("promotions")] public async Task<IActionResult> Promotions() => View(await service.GetPromotionCodesAsync());
    [HttpGet("promotions/create")] public async Task<IActionResult> CreatePromotion() { ViewBag.Plans = await service.GetPlansAsync(); return View(); }
    [HttpPost("promotions/create"), ValidateAntiForgeryToken]
    [Authorize(Policy = PlatformAuthentication.ManagementPolicy)]
    public async Task<IActionResult> CreatePromotion(CreatePromotionCodeDto dto) { if (!ModelState.IsValid) { ViewBag.Plans = await service.GetPlansAsync(); return View(dto); } await service.CreatePromotionCodeAsync(dto); return RedirectToAction(nameof(Promotions)); }
}
