using EF_CRUD.Common;
using EF_CRUD.Dtos;

namespace EF_CRUD.Services;

public interface ISupplierService
{
    Task<ServiceResult<IReadOnlyList<SupplierDto>>> GetAllAsync();
    Task<ServiceResult<SupplierDto>> GetByIdAsync(int id);
    Task<ServiceResult<SupplierDto>> CreateAsync(SupplierRequestDto request);
    Task<ServiceResult<SupplierDto>> UpdateAsync(int id, SupplierRequestDto request);
    Task<ServiceResult> DeleteAsync(int id);
}
