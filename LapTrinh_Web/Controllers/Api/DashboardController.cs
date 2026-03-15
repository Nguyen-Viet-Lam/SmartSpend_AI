using LapTrinh_Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LapTrinh_Web.Controllers.Api;

[ApiController]
[Route("api/dashboard")]
public sealed class DashboardController(IForecastService forecastService) : ControllerBase
{
    [HttpGet("summary")]
    public IActionResult GetSummary()
    {
        _ = forecastService;
        return StatusCode(StatusCodes.Status501NotImplemented);
    }

    [HttpGet("forecast")]
    public IActionResult GetForecast([FromQuery] int year, [FromQuery] int month)
    {
        _ = forecastService;
        _ = year;
        _ = month;
        return StatusCode(StatusCodes.Status501NotImplemented);
    }
}