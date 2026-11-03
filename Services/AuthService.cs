using EF_CRUD.Common;
using EF_CRUD.Dtos;
using EF_CRUD.Models;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace EF_CRUD.Services;

public sealed class AuthService(
    UserManager<IdentityUser> userManager,
    ITokenService tokenService,
    IValidator<RegisterDto> registerValidator,
    IValidator<LoginDto> loginValidator) : IAuthService
{
    private const string InvalidCredentials = "Invalid email or password.";

    public async Task<ServiceResult<AuthResponseDto>> RegisterAsync(RegisterDto request)
    {
        var validation = await registerValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return ServiceResult<AuthResponseDto>.Validation(validation.ToErrorDictionary());

        var email = request.Email.Trim();
        if (await userManager.FindByEmailAsync(email) is not null)
            return ServiceResult<AuthResponseDto>.Conflict("An account with this email already exists.");

        var user = new IdentityUser
        {
            UserName = email,
            Email = email
        };

        var created = await userManager.CreateAsync(user, request.Password);
        if (!created.Succeeded)
            return IdentityFailure(created.Errors);

        var inRole = await userManager.AddToRoleAsync(user, Roles.User);
        if (!inRole.Succeeded)
            return IdentityFailure(inRole.Errors);
            
        return ServiceResult<AuthResponseDto>.Created(await tokenService.CreateTokenAsync(user));
    }

    public async Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginDto request)
    {
        var validation = await loginValidator.ValidateAsync(request);
        if (!validation.IsValid)
            return ServiceResult<AuthResponseDto>.Validation(validation.ToErrorDictionary());

        var user = await userManager.FindByEmailAsync(request.Email.Trim());
        if (user is null)
            return ServiceResult<AuthResponseDto>.Unauthorized(InvalidCredentials);

        if (await userManager.IsLockedOutAsync(user))
            return ServiceResult<AuthResponseDto>.Unauthorized("This account is temporarily locked. Try again later.");

        if (!await userManager.CheckPasswordAsync(user, request.Password))
        {
            await userManager.AccessFailedAsync(user); // counts toward lockout
            return ServiceResult<AuthResponseDto>.Unauthorized(InvalidCredentials);
        }

        await userManager.ResetAccessFailedCountAsync(user);
        return ServiceResult<AuthResponseDto>.Success(await tokenService.CreateTokenAsync(user));
    }

    private static ServiceResult<AuthResponseDto> IdentityFailure(IEnumerable<IdentityError> errors)
    {
        var grouped = errors
            .GroupBy(e => KeyFor(e.Code))
            .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());

        return ServiceResult<AuthResponseDto>.Validation(grouped);
    }

    private static string KeyFor(string code)
    {
        if (code.StartsWith("Password", StringComparison.Ordinal)) return "Password";
        if (code.Contains("Email", StringComparison.Ordinal) || code.Contains("UserName", StringComparison.Ordinal)) return "Email";
        return "Identity";
    }
}
