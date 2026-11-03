using EF_CRUD.Common;
using EF_CRUD.Dtos;

namespace EF_CRUD.Services;

public interface ICategoryService
{
    Task<ServiceResult<IReadOnlyList<CategoryDto>>> GetAllAsync();
    Task<ServiceResult<CategoryDto>> GetByIdAsync(int id);
    Task<ServiceResult<CategoryDto>> CreateAsync(CategoryRequestDto request);
    Task<ServiceResult<CategoryDto>> UpdateAsync(int id, CategoryRequestDto request);
    Task<ServiceResult> DeleteAsync(int id);
}
