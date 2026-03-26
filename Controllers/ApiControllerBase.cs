using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SmartSpendAI.Security;

namespace SmartSpendAI.Controllers
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
            return AppRoles.IsAdmin(User.FindFirstValue(ClaimTypes.Role));
        }
    }
}
