using LapTrinh_Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LapTrinh_Web.Controllers.Api;

[ApiController]
[Route("api/reports")]
public sealed class ReportsController(IReportService reportService) : ControllerBase
{
    [HttpGet("weekly-summary")]
    public IActionResult WeeklySummary()
    {
        _ = reportService;
        return StatusCode(StatusCodes.Status501NotImplemented);
    }

    [HttpGet("export-excel")]
    public IActionResult ExportExcel([FromQuery] DateTime? fromUtc, [FromQuery] DateTime? toUtc)
    {
        _ = reportService;
        _ = fromUtc;
        _ = toUtc;
        return StatusCode(StatusCodes.Status501NotImplemented);
    }
}