using AutoMapper;
using EF_CRUD.Common;
using EF_CRUD.Dtos;
using EF_CRUD.Models;
using EF_CRUD.Repositories;
using FluentValidation;

namespace EF_CRUD.Services;

public sealed class SupplierService(
    ISupplierRepository suppliers,
    IMapper mapper,
    IValidator<SupplierRequestDto> validator) : ISupplierService
{
    public async Task<ServiceResult<IReadOnlyList<SupplierDto>>> GetAllAsync()
    {
        var items = await suppliers.GetAllAsync();
        var dtos = mapper.Map<List<SupplierDto>>(items.OrderBy(s => s.Id));
        return ServiceResult<IReadOnlyList<SupplierDto>>.Success(dtos);
    }

    public async Task<ServiceResult<SupplierDto>> GetByIdAsync(int id)
    {
        var supplier = await suppliers.GetByIdAsync(id);
        return supplier is null
            ? ServiceResult<SupplierDto>.NotFound($"Supplier {id} was not found.")
            : ServiceResult<SupplierDto>.Success(mapper.Map<SupplierDto>(supplier));
    }

    public async Task<ServiceResult<SupplierDto>> CreateAsync(SupplierRequestDto request)
    {
        var validation = await validator.ValidateAsync(request);
        if (!validation.IsValid)
            return ServiceResult<SupplierDto>.Validation(validation.ToErrorDictionary());

        var name = request.Name.Trim();
        if (await suppliers.ExistsByNameAsync(name))
            return ServiceResult<SupplierDto>.Conflict($"A supplier named '{name}' already exists.");

        var supplier = mapper.Map<Supplier>(request);
        supplier.Name = name;

        suppliers.Add(supplier);
        await suppliers.SaveChangesAsync();
        return ServiceResult<SupplierDto>.Created(mapper.Map<SupplierDto>(supplier));
    }

    public async Task<ServiceResult<SupplierDto>> UpdateAsync(int id, SupplierRequestDto request)
    {
        var supplier = await suppliers.GetByIdAsync(id);
        if (supplier is null)
            return ServiceResult<SupplierDto>.NotFound($"Supplier {id} was not found.");

        var validation = await validator.ValidateAsync(request);
        if (!validation.IsValid)
            return ServiceResult<SupplierDto>.Validation(validation.ToErrorDictionary());

        var name = request.Name.Trim();
        if (await suppliers.ExistsByNameAsync(name, excludeId: id))
            return ServiceResult<SupplierDto>.Conflict($"A supplier named '{name}' already exists.");

        mapper.Map(request, supplier);
        supplier.Name = name;
        await suppliers.SaveChangesAsync();
        return ServiceResult<SupplierDto>.Success(mapper.Map<SupplierDto>(supplier));
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var supplier = await suppliers.GetByIdAsync(id);
        if (supplier is null)
            return ServiceResult.NotFound($"Supplier {id} was not found.");

        if (await suppliers.HasProductsAsync(id))
            return ServiceResult.Conflict($"Supplier '{supplier.Name}' still has products. Move or delete them first.");

        suppliers.Remove(supplier);
        await suppliers.SaveChangesAsync();
        return ServiceResult.Success();
    }
}
