using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.Saas;
using MiniStore.Domain.Interfaces;

namespace MiniStore.Web.Controllers;

[Authorize]
[Route("onboarding")]
public class CompanyOnboardingController(
    ICompanyOnboardingService onboarding,
    ITenantContext tenantContext,
    UserManager<IdentityUser> users) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var model = await onboarding.GetAsync(RequireTenant());
        if (model.Status != Domain.Entities.CompanyOnboardingStatus.Pending)
            return RedirectToAction("Index", "Home");
        return View(model);
    }

    [HttpPost("complete")]
    public async Task<IActionResult> Complete(CompleteCompanyOnboardingCommand command)
    {
        try
        {
            await onboarding.CompleteAsync(RequireTenant(), RequireUser(), command);
            TempData["Success"] = "Company setup completed successfully.";
            return RedirectToAction("Index", "Home");
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            var model = await onboarding.GetAsync(RequireTenant());
            return View("Index", model);
        }
    }

    [HttpPost("skip")]
    public async Task<IActionResult> Skip()
    {
        await onboarding.SkipAsync(RequireTenant(), RequireUser());
        TempData["Success"] = "Setup skipped. You can configure the company from Settings.";
        return RedirectToAction("Index", "Home");
    }

    private int RequireTenant() => tenantContext.TenantId
        ?? throw new InvalidOperationException("An active company is required.");

    private string RequireUser() => users.GetUserId(User)
        ?? throw new InvalidOperationException("A signed-in user is required.");
}
