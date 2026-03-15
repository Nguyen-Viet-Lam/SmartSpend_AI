using LapTrinh_Web.Contracts.Requests.Budgets;
using LapTrinh_Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LapTrinh_Web.Controllers.Api;

[ApiController]
[Route("api/budgets")]
public sealed class BudgetsController(IBudgetService budgetService) : ControllerBase
{
    [HttpGet("progress")]
    public IActionResult GetMonthlyProgress([FromQuery] int year, [FromQuery] int month)
    {
        _ = budgetService;
        _ = year;
        _ = month;
        return StatusCode(StatusCodes.Status501NotImplemented);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateBudgetRequest request)
    {
        _ = budgetService;
        _ = request;
        return StatusCode(StatusCodes.Status501NotImplemented);
    }

    [HttpPut("{budgetId:guid}")]
    public IActionResult Update([FromRoute] Guid budgetId, [FromBody] UpdateBudgetRequest request)
    {
        _ = budgetService;
        _ = budgetId;
        _ = request;
        return StatusCode(StatusCodes.Status501NotImplemented);
    }
}