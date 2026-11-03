using AutoMapper;
using EF_CRUD.Common;
using EF_CRUD.Dtos;
using EF_CRUD.Models;
using EF_CRUD.Repositories;
using FluentValidation;

namespace EF_CRUD.Services;

public sealed class SupplierService(
    IUnitOfWork uow,
    IMapper mapper,
    IValidator<SupplierRequestDto> validator) : ISupplierService
{
    public async Task<ServiceResult<IReadOnlyList<SupplierDto>>> GetAllAsync()
    {
        var suppliers = await uow.Suppliers.GetAllAsync();
        var dtos = mapper.Map<List<SupplierDto>>(suppliers.OrderBy(s => s.Id));
        return ServiceResult<IReadOnlyList<SupplierDto>>.Success(dtos);
    }

    public async Task<ServiceResult<SupplierDto>> GetByIdAsync(int id)
    {
        var supplier = await uow.Suppliers.GetByIdAsync(id);
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
        if (await uow.Suppliers.ExistsByNameAsync(name))
            return ServiceResult<SupplierDto>.Conflict($"A supplier named '{name}' already exists.");

        var supplier = mapper.Map<Supplier>(request);
        supplier.Name = name;

        uow.Suppliers.Add(supplier);
        await uow.SaveChangesAsync();
        return ServiceResult<SupplierDto>.Created(mapper.Map<SupplierDto>(supplier));
    }

    public async Task<ServiceResult<SupplierDto>> UpdateAsync(int id, SupplierRequestDto request)
    {
        var supplier = await uow.Suppliers.GetByIdAsync(id);
        if (supplier is null)
            return ServiceResult<SupplierDto>.NotFound($"Supplier {id} was not found.");

        var validation = await validator.ValidateAsync(request);
        if (!validation.IsValid)
            return ServiceResult<SupplierDto>.Validation(validation.ToErrorDictionary());

        var name = request.Name.Trim();
        if (await uow.Suppliers.ExistsByNameAsync(name, excludeId: id))
            return ServiceResult<SupplierDto>.Conflict($"A supplier named '{name}' already exists.");

        mapper.Map(request, supplier);
        supplier.Name = name;
        await uow.SaveChangesAsync();
        return ServiceResult<SupplierDto>.Success(mapper.Map<SupplierDto>(supplier));
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var supplier = await uow.Suppliers.GetByIdAsync(id);
        if (supplier is null)
            return ServiceResult.NotFound($"Supplier {id} was not found.");

        if (await uow.Suppliers.HasProductsAsync(id))
            return ServiceResult.Conflict($"Supplier '{supplier.Name}' still has products. Move or delete them first.");

        uow.Suppliers.Remove(supplier);
        await uow.SaveChangesAsync();
        return ServiceResult.Success();
    }
}
