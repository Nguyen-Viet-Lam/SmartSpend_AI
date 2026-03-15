using LapTrinh_Web.Contracts.Requests.Admin;
using LapTrinh_Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LapTrinh_Web.Controllers.Api;

[ApiController]
[Route("api/admin/categories")]
public sealed class AdminCategoriesController(IAdminService adminService) : ControllerBase
{
    [HttpPut("{categoryId:guid}")]
    public IActionResult UpdateCategory([FromRoute] Guid categoryId, [FromBody] UpdateCategoryRequest request)
    {
        _ = adminService;
        _ = categoryId;
        _ = request;
        return StatusCode(StatusCodes.Status501NotImplemented);
    }
}