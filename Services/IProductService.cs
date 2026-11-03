using EF_CRUD.Common;
using EF_CRUD.Dtos;

namespace EF_CRUD.Services;

public interface IProductService
{
    Task<ServiceResult<IReadOnlyList<ProductDto>>> GetAllAsync();
    Task<ServiceResult<ProductDto>> GetByIdAsync(int id);
    Task<ServiceResult<ProductDto>> CreateAsync(ProductRequestDto request);
    Task<ServiceResult<ProductDto>> UpdateAsync(int id, ProductRequestDto request);
    Task<ServiceResult> DeleteAsync(int id);
}
