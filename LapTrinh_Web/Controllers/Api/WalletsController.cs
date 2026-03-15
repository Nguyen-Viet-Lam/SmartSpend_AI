using LapTrinh_Web.Contracts.Requests.Wallets;
using LapTrinh_Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LapTrinh_Web.Controllers.Api;

[ApiController]
[Route("api/wallets")]
public sealed class WalletsController(IWalletService walletService) : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll()
    {
        _ = walletService;
        return StatusCode(StatusCodes.Status501NotImplemented);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateWalletRequest request)
    {
        _ = walletService;
        _ = request;
        return StatusCode(StatusCodes.Status501NotImplemented);
    }

    [HttpPut("{walletId:guid}")]
    public IActionResult Update([FromRoute] Guid walletId, [FromBody] UpdateWalletRequest request)
    {
        _ = walletService;
        _ = walletId;
        _ = request;
        return StatusCode(StatusCodes.Status501NotImplemented);
    }

    [HttpDelete("{walletId:guid}")]
    public IActionResult Delete([FromRoute] Guid walletId)
    {
        _ = walletService;
        _ = walletId;
        return StatusCode(StatusCodes.Status501NotImplemented);
    }
}