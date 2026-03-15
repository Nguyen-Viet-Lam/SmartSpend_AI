using Microsoft.AspNetCore.Mvc;

namespace LapTrinh_Web.Controllers;

public sealed class TransactionsController : Controller
{
    public IActionResult Index() => View();
}