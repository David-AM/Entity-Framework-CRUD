using EF_CRUD.Common;
using EF_CRUD.Dtos;

namespace EF_CRUD.Services;

public interface IAuthService
{
    Task<ServiceResult<AuthResponseDto>> RegisterAsync(RegisterDto request);
    Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginDto request);
}
