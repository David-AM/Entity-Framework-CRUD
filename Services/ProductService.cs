using AutoMapper;
using EF_CRUD.Common;
using EF_CRUD.Dtos;
using EF_CRUD.Models;
using EF_CRUD.Repositories;
using FluentValidation;

namespace EF_CRUD.Services;

public sealed class ProductService(
    IUnitOfWork uow,
    IMapper mapper,
    IValidator<ProductRequestDto> validator) : IProductService
{
    public async Task<ServiceResult<IReadOnlyList<ProductDto>>> GetAllAsync()
    {
        var products = await uow.Products.GetAllWithDetailsAsync();
        return ServiceResult<IReadOnlyList<ProductDto>>.Success(mapper.Map<List<ProductDto>>(products));
    }

    public async Task<ServiceResult<ProductDto>> GetByIdAsync(int id)
    {
        var product = await uow.Products.GetByIdWithDetailsAsync(id);
        return product is null
            ? ServiceResult<ProductDto>.NotFound($"Product {id} was not found.")
            : ServiceResult<ProductDto>.Success(mapper.Map<ProductDto>(product));
    }

    public async Task<ServiceResult<ProductDto>> CreateAsync(ProductRequestDto request)
    {
        var failure = await ValidateAsync(request);
        if (failure is not null) return failure;

        var product = mapper.Map<Product>(request);
        product.Name = product.Name.Trim();

        uow.Products.Add(product);
        await uow.SaveChangesAsync();

        // Reload with Category and Supplier so the response contains their names.
        var created = await uow.Products.GetByIdWithDetailsAsync(product.Id);
        return ServiceResult<ProductDto>.Created(mapper.Map<ProductDto>(created!));
    }

    public async Task<ServiceResult<ProductDto>> UpdateAsync(int id, ProductRequestDto request)
    {
        var product = await uow.Products.GetByIdAsync(id);
        if (product is null)
            return ServiceResult<ProductDto>.NotFound($"Product {id} was not found.");

        var failure = await ValidateAsync(request);
        if (failure is not null) return failure;

        mapper.Map(request, product); // copies the request values onto the tracked entity
        product.Name = product.Name.Trim();
        await uow.SaveChangesAsync();

        var updated = await uow.Products.GetByIdWithDetailsAsync(id);
        return ServiceResult<ProductDto>.Success(mapper.Map<ProductDto>(updated!));
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var product = await uow.Products.GetByIdAsync(id);
        if (product is null)
            return ServiceResult.NotFound($"Product {id} was not found.");

        uow.Products.Remove(product);
        await uow.SaveChangesAsync();
        return ServiceResult.Success();
    }

    // Shape rules come from FluentValidation. Rules that need the database (do the category and supplier exist?) live here.
    private async Task<ServiceResult<ProductDto>?> ValidateAsync(ProductRequestDto request)
    {
        var result = await validator.ValidateAsync(request);
        if (!result.IsValid)
            return ServiceResult<ProductDto>.Validation(result.ToErrorDictionary());

        var errors = new Dictionary<string, string[]>();
        if (!await uow.Categories.ExistsAsync(request.CategoryId))
            errors[nameof(request.CategoryId)] = [$"Category {request.CategoryId} does not exist."];
        if (!await uow.Suppliers.ExistsAsync(request.SupplierId))
            errors[nameof(request.SupplierId)] = [$"Supplier {request.SupplierId} does not exist."];

        return errors.Count > 0 ? ServiceResult<ProductDto>.Validation(errors) : null;
    }
}
