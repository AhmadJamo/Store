using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.Dtos.Accounting;
using MiniStore.Application.Services;
namespace MiniStore.Web.Controllers;
[Authorize(Roles = "Admin")]
public class AccountsController(AccountService service) : Controller
{
    public async Task<IActionResult> Index() => View(await service.GetAllAsync());
    [HttpGet] public async Task<IActionResult> Create() { ViewBag.Accounts = await service.GetAllAsync(); return View(new CreateAccountDto()); }
    [HttpPost] public async Task<IActionResult> Create(CreateAccountDto dto) { if (!ModelState.IsValid) { ViewBag.Accounts = await service.GetAllAsync(); return View(dto); } try { await service.CreateAsync(dto); return RedirectToAction(nameof(Index)); } catch (Exception ex) { ModelState.AddModelError(string.Empty, ex.Message); ViewBag.Accounts = await service.GetAllAsync(); return View(dto); } }
}
