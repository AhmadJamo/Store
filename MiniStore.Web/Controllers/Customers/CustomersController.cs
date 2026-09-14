using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;
namespace MiniStore.Web.Controllers;
[PermissionAuthorize("Administration.Access")]
public class CustomersController(CustomerService service, AccountService accounts):Controller
{
    public async Task<IActionResult> Index()=>View(await service.GetAllAsync());
    [HttpGet] public async Task<IActionResult> Create(){ViewBag.Accounts=await accounts.GetAllAsync();return View();}
    [HttpPost] public async Task<IActionResult> Create(string name,int accountId){try{await service.CreateAsync(name,accountId);return RedirectToAction(nameof(Index));}catch(Exception ex){ModelState.AddModelError(string.Empty,ex.Message);ViewBag.Accounts=await accounts.GetAllAsync();return View();}}
}
