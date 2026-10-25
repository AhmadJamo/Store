using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniStore.Application.DTOs.Products;
using MiniStore.Application.Services;
using MiniStore.Web.Authorization;

namespace MiniStore.Web.Controllers.Api.V1;

[ApiController]
[Authorize]
[Route("api/v1/products")]
public sealed class ProductsApiController(ProductService products) : ControllerBase
{
    [HttpGet]
    [PermissionAuthorize("Products.View")]
    public async Task<ActionResult<ProductListPageDto>> GetPage([FromQuery] ProductListQueryDto query) =>
        Ok(await products.SearchAsync(query));

    [HttpGet("{id:int}")]
    [PermissionAuthorize("Products.View")]
    public async Task<ActionResult<ProductDto>> Get(int id)
    {
        var product = await products.GetByIdAsync(id);
        return product is null ? NotFound() : Ok(product);
    }
}
