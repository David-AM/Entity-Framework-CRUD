using EF_CRUD.Dtos;
using EF_CRUD.Models;

namespace EF_CRUD.Services;

public interface ITokenService
{
    Task<AuthResponseDto> CreateTokenAsync(ApplicationUser user);
}
