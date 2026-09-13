using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.Services;
namespace MiniStore.Web.Controllers;
[Authorize(Roles="Admin")]
public class BranchesController(BranchService service):Controller { public async Task<IActionResult> Index()=>View(await service.GetAllAsync()); [HttpGet] public IActionResult Create()=>View(); [HttpPost] public async Task<IActionResult> Create(string code,string name){try{await service.CreateAsync(code,name);return RedirectToAction(nameof(Index));}catch(Exception ex){ModelState.AddModelError(string.Empty,ex.Message);return View();}} }
