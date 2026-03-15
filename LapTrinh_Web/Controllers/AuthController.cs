using Microsoft.AspNetCore.Mvc;

namespace LapTrinh_Web.Controllers;

public sealed class AuthController : Controller
{
    public IActionResult Login() => View();

    public IActionResult Register() => View();

    public IActionResult Otp() => View();
}