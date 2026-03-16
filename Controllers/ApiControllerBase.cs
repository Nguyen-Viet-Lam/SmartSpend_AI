using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Web_Project.Security;

namespace Web_Project.Controllers
{
    [ApiController]
    public abstract class ApiControllerBase : ControllerBase
    {
        protected int? GetCurrentUserId()
        {
            var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(raw, out var userId) ? userId : null;
        }

        protected bool IsSystemAdmin()
        {
            return string.Equals(
                User.FindFirstValue(ClaimTypes.Role),
                AppRoles.SystemAdmin,
                StringComparison.OrdinalIgnoreCase);
        }
    }
}
