using EF_CRUD.Dtos;
using Microsoft.AspNetCore.Identity;

namespace EF_CRUD.Services;

public interface ITokenService
{
    Task<AuthResponseDto> CreateTokenAsync(IdentityUser user);
}
