using LapTrinh_Web.Contracts.Requests.Admin;
using LapTrinh_Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LapTrinh_Web.Controllers.Api;

[ApiController]
[Route("api/admin/users")]
public sealed class AdminUsersController(IAdminService adminService) : ControllerBase
{
    [HttpGet]
    public IActionResult GetUsers()
    {
        _ = adminService;
        return StatusCode(StatusCodes.Status501NotImplemented);
    }

    [HttpPut("{userId:guid}/status")]
    public IActionResult UpdateUserStatus([FromRoute] Guid userId, [FromBody] UpdateUserStatusRequest request)
    {
        _ = adminService;
        _ = userId;
        _ = request;
        return StatusCode(StatusCodes.Status501NotImplemented);
    }
}