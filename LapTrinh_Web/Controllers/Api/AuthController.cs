using System.Security.Claims;
using LapTrinh_Web.Contracts.Requests.Auth;
using LapTrinh_Web.Contracts.Responses.Auth;
using LapTrinh_Web.Services.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LapTrinh_Web.Controllers.Api;

[ApiController]
[Route("api/auth")]
public sealed class AuthApiController(IAuthService authService, ILogger<AuthApiController> logger) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await authService.RegisterAsync(request, cancellationToken);
            return Ok(new
            {
                message = "Dang ky thanh cong. Vui long kiem tra OTP trong email.",
                data = response
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Register failed");
            if (IsDatabaseException(ex))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "Khong ket noi duoc database. Kiem tra SQL Server va chay 'dotnet ef database update'."
                });
            }

            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Loi he thong khi dang ky." });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await authService.LoginAsync(request, cancellationToken);
            if (response.RequiresOtpVerification)
            {
                return Ok(new
                {
                    message = "Tai khoan chua xac thuc. OTP moi da duoc gui vao email.",
                    data = response
                });
            }

            await SignInAsync(response);
            return Ok(new
            {
                message = "Dang nhap thanh cong.",
                data = response
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Login failed");
            if (IsDatabaseException(ex))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "Khong ket noi duoc database. Kiem tra SQL Server va chay 'dotnet ef database update'."
                });
            }

            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Loi he thong khi dang nhap." });
        }
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var isVerified = await authService.VerifyOtpAsync(request, cancellationToken);
            if (!isVerified)
            {
                return BadRequest(new { message = "OTP khong dung hoac da het han." });
            }

            return Ok(new { message = "Xac thuc OTP thanh cong. Ban co the dang nhap." });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Verify OTP failed");
            if (IsDatabaseException(ex))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "Khong ket noi duoc database. Kiem tra SQL Server va chay 'dotnet ef database update'."
                });
            }

            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Loi he thong khi xac thuc OTP." });
        }
    }

    [HttpPost("resend-otp")]
    public async Task<IActionResult> ResendOtp([FromBody] ResendOtpRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await authService.ResendOtpAsync(request, cancellationToken);
            return Ok(new { message = "Da gui lai OTP. Vui long kiem tra email." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Resend OTP failed");
            if (IsDatabaseException(ex))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "Khong ket noi duoc database. Kiem tra SQL Server va chay 'dotnet ef database update'."
                });
            }

            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Loi he thong khi gui lai OTP." });
        }
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok(new { message = "Dang xuat thanh cong." });
    }

    private async Task SignInAsync(AuthResponse response)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, response.UserId.ToString()),
            new(ClaimTypes.Email, response.Email),
            new(ClaimTypes.Role, response.Role.ToString())
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    }

    private static bool IsDatabaseException(Exception ex)
        => ex is SqlException
           || ex is DbUpdateException
           || ex.InnerException is SqlException
           || ex.InnerException is DbUpdateException;
}
