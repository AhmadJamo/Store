using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MiniStore.Domain.Interfaces;
using MiniStore.Web.Authorization;
using MiniStore.Application.Services;
using MiniStore.Domain.Entities;

namespace MiniStore.Web.Areas.Platform.Controllers;

[Area("Platform")]
[Route("platform/account")]
public class PlatformAccountController(
    UserManager<IdentityUser> users,
    SignInManager<IdentityUser> signInManager,
    ISaasRepository saas,
    ISecurityAuditEventWriter securityAudit,
    IAuditRequestContext auditRequest) : Controller
{
    [AllowAnonymous, HttpGet("login")]
    public IActionResult Login(
        string? returnUrl = null,
        bool rateLimited = false,
        int? retryAfterSeconds = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        ViewBag.RateLimitMinutes = rateLimited
            ? (int?)Math.Max(1, (int)Math.Ceiling(Math.Clamp(retryAfterSeconds ?? 60, 1, 86400) / 60d))
            : null;
        return View();
    }

    [AllowAnonymous, HttpPost("login"), ValidateAntiForgeryToken, Microsoft.AspNetCore.RateLimiting.EnableRateLimiting("login")]
    public async Task<IActionResult> Login(string username, string password, string? returnUrl = null)
    {
        var user = await users.FindByNameAsync(username);
        var op = user is null ? null : await saas.GetOperatorAsync(user.Id);
        var result = user is null || op is null
            ? Microsoft.AspNetCore.Identity.SignInResult.Failed
            : await signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);
        if (!result.Succeeded)
        {
            await securityAudit.RecordAsync(new IndependentAuditEventWriteRequest
            {
                Scope = AuditEventScope.Platform,
                Category = AuditEventCategory.Security,
                Severity = result.IsLockedOut ? AuditEventSeverity.Critical : AuditEventSeverity.Warning,
                Outcome = AuditEventOutcome.Denied,
                EventType = SemanticAuditEventTypes.PlatformLoginFailed,
                Action = "Login",
                ActorType = AuditActorType.External,
                ActorId = "anonymous",
                SourceChannel = AuditSourceChannel.Platform,
                CorrelationId = auditRequest.CorrelationId,
                ResourceType = "PlatformSession",
                ResourceId = "login",
                ReasonCode = result.IsLockedOut ? "AccountLocked" : "InvalidCredentials",
                RequestPathTemplate = auditRequest.RequestPathTemplate,
                SourceIpMasked = auditRequest.SourceIpMasked,
                UserAgentHash = auditRequest.UserAgentHash
            }, CancellationToken.None);
            ModelState.AddModelError("", "Invalid platform credentials.");
            return View();
        }
        var identity = new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, user!.Id), new Claim(ClaimTypes.Name, user.UserName!), new Claim(ClaimTypes.Role, op!.Role.ToString())], PlatformAuthentication.Scheme);
        await HttpContext.SignInAsync(PlatformAuthentication.Scheme, new ClaimsPrincipal(identity));
        return LocalRedirect(Url.IsLocalUrl(returnUrl) ? returnUrl! : "/platform");
    }

    [Authorize(AuthenticationSchemes = PlatformAuthentication.Scheme), HttpPost("logout"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout() { await HttpContext.SignOutAsync(PlatformAuthentication.Scheme); return RedirectToAction(nameof(Login)); }
}
