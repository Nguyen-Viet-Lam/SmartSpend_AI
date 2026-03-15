using Microsoft.AspNetCore.Mvc;

namespace LapTrinh_Web.Controllers;

public sealed class DashboardController : Controller
{
    public IActionResult Index() => View();
}