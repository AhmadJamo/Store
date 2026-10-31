using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.Saas;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController]
[AllowAnonymous]
[Route("api/v1/public/plans")]
public sealed class PublicPlansApiController(PlatformSaasService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PlanCardDto>>> Get() =>
        Ok(await service.GetPublicPlansAsync());
}
