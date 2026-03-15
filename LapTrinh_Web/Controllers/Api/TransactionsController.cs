using LapTrinh_Web.Contracts.Requests.Transactions;
using LapTrinh_Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LapTrinh_Web.Controllers.Api;

[ApiController]
[Route("api/transactions")]
public sealed class TransactionsController(ITransactionService transactionService, ICategorySuggestionService categorySuggestionService) : ControllerBase
{
    [HttpGet]
    public IActionResult GetByFilter([FromQuery] FilterTransactionsRequest request)
    {
        _ = transactionService;
        _ = request;
        return StatusCode(StatusCodes.Status501NotImplemented);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateTransactionRequest request)
    {
        _ = transactionService;
        _ = request;
        return StatusCode(StatusCodes.Status501NotImplemented);
    }

    [HttpPut("{transactionId:guid}")]
    public IActionResult Update([FromRoute] Guid transactionId, [FromBody] UpdateTransactionRequest request)
    {
        _ = transactionService;
        _ = transactionId;
        _ = request;
        return StatusCode(StatusCodes.Status501NotImplemented);
    }

    [HttpDelete("{transactionId:guid}")]
    public IActionResult Delete([FromRoute] Guid transactionId)
    {
        _ = transactionService;
        _ = transactionId;
        return StatusCode(StatusCodes.Status501NotImplemented);
    }

    [HttpPost("suggest-category")]
    public IActionResult SuggestCategory([FromBody] string description)
    {
        _ = categorySuggestionService;
        _ = description;
        return StatusCode(StatusCodes.Status501NotImplemented);
    }
}