using Microsoft.AspNetCore.Mvc;

namespace LapTrinh_Web.Controllers;

public sealed class AdminController : Controller
{
    public IActionResult Index() => View();
}
