using LapTrinh_Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LapTrinh_Web.Controllers.Api;

[ApiController]
[Route("api/profile")]
public sealed class ProfileController(IUserProfileService userProfileService) : ControllerBase
{
    [HttpGet("me")]
    public IActionResult GetMyProfile()
    {
        _ = userProfileService;
        return StatusCode(StatusCodes.Status501NotImplemented);
    }

    [HttpPut("avatar")]
    public IActionResult UpdateAvatar([FromBody] string avatarUrl)
    {
        _ = userProfileService;
        _ = avatarUrl;
        return StatusCode(StatusCodes.Status501NotImplemented);
    }
}