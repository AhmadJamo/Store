using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.Saas;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Web.Controllers;

[Authorize]
public class HomeController(ICompanyOnboardingService onboarding, ITenantContext tenantContext) : Controller
{
    public async Task<IActionResult> Index()
    {
        if (tenantContext.TenantId is int tenantId && !await onboarding.IsResolvedAsync(tenantId))
            return RedirectToAction("Index", "CompanyOnboarding");
        return View();
    }
}
