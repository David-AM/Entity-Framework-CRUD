using EF_CRUD.Dtos;
using EF_CRUD.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EF_CRUD.Controllers;

[AllowAnonymous]
public sealed class AuthController(IAuthService authService) : BaseApiController
{
    /// <summary>Creates a user account (role: User) and returns a JWT.</summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto request) =>
        Respond(await authService.RegisterAsync(request));

    /// <summary>Checks the credentials and returns a JWT.</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto request) =>
        Respond(await authService.LoginAsync(request));
}
