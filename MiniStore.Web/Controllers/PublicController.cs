using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.Saas;

namespace MiniStore.Web.Controllers;

[AllowAnonymous]
public class PublicController(PlatformSaasService service) : Controller
{
    [HttpGet("/")]
    public IActionResult Index() => View();
    [HttpGet("/pricing")]
    public async Task<IActionResult> Pricing() => View(await service.GetPublicPlansAsync());
}
