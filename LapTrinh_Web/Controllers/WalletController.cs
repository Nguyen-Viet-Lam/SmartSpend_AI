using Microsoft.AspNetCore.Mvc;

namespace LapTrinh_Web.Controllers;

public sealed class WalletController : Controller
{
    public IActionResult Index() => View();
}
