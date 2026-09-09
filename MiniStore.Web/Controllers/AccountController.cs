using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MiniStore.Web.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<IdentityUser> _signInManager;

    public AccountController(
        SignInManager<IdentityUser> signInManager)
    {
        _signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult Login(
        string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        string username,
        string password,
        string? returnUrl = null)
    {
        if (string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(password))
        {
            ViewBag.Error =
                "Username and password are required.";

            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        var result =
            await _signInManager.PasswordSignInAsync(
                username,
                password,
                isPersistent: false,
                lockoutOnFailure: false);

        if (result.Succeeded)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) &&
                Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(
                "Index",
                "Home");
        }

        ViewBag.Error =
            "Invalid username or password.";

        ViewBag.ReturnUrl = returnUrl;

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();

        return RedirectToAction(
            nameof(Login));
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}