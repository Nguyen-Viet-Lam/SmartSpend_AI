using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_Project.Models;

namespace Web_Project.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class AlertsController : ApiControllerBase
    {
        private readonly AppDbContext _dbContext;

        public AlertsController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAlerts(CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId is null)
            {
                return Unauthorized();
            }

            var alerts = await _dbContext.BudgetAlerts
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .Take(20)
                .ToListAsync(cancellationToken);

            return Ok(alerts.Select(x => new
            {
                alertId = x.BudgetAlertId,
                x.Message,
                x.Level,
                x.IsRead,
                x.CreatedAt
            }));
        }

        [HttpPost("{id:int}/read")]
        public async Task<IActionResult> MarkRead(int id, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId is null)
            {
                return Unauthorized();
            }

            var alert = await _dbContext.BudgetAlerts.FirstOrDefaultAsync(x => x.BudgetAlertId == id && x.UserId == userId, cancellationToken);
            if (alert is null)
            {
                return NotFound(new { message = "Khong tim thay canh bao." });
            }

            alert.IsRead = true;
            await _dbContext.SaveChangesAsync(cancellationToken);
            return Ok(new { message = "Da danh dau da doc." });
        }
    }
}
