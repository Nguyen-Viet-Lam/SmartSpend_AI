using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartSpendAI.Models.Dtos.Reports;
using SmartSpendAI.Security;
using SmartSpendAI.Services.Reports;

namespace SmartSpendAI.Controllers
{
    [Authorize(Policy = AppPolicies.UserOrAdmin)]
    [Route("api/[controller]")]
    public class ReportsController : ApiControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("weekly")]
        public async Task<ActionResult<ReportPeriodSummaryResponse>> GetWeekly(CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId is null)
            {
                return Unauthorized();
            }

            var response = await _reportService.GetWeeklySummaryAsync(userId.Value, cancellationToken);
            return Ok(response);
        }

        [HttpGet("monthly")]
        public async Task<ActionResult<ReportPeriodSummaryResponse>> GetMonthly(
            [FromQuery] DateTime? month,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId is null)
            {
                return Unauthorized();
            }

            var response = await _reportService.GetMonthlySummaryAsync(userId.Value, month, cancellationToken);
            return Ok(response);
        }

        [HttpGet("email-history")]
        public async Task<ActionResult<IReadOnlyList<ReportEmailHistoryResponse>>> GetEmailHistory(
            [FromQuery] int take = 20,
            CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            if (userId is null)
            {
                return Unauthorized();
            }

            var response = await _reportService.GetEmailHistoryAsync(userId.Value, take, cancellationToken);
            return Ok(response);
        }
    }
}
