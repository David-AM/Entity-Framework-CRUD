using AutoMapper;
using EF_CRUD.Common;
using EF_CRUD.Dtos;
using EF_CRUD.Models;
using EF_CRUD.Repositories;
using FluentValidation;

namespace EF_CRUD.Services;

public sealed class ProductService(
    IProductRepository products,
    ICategoryRepository categories,
    ISupplierRepository suppliers,
    IMapper mapper,
    IValidator<ProductRequestDto> validator) : IProductService
{
    public async Task<ServiceResult<IReadOnlyList<ProductDto>>> GetAllAsync()
    {
        var items = await products.GetAllWithDetailsAsync();
        return ServiceResult<IReadOnlyList<ProductDto>>.Success(mapper.Map<List<ProductDto>>(items));
    }

    public async Task<ServiceResult<ProductDto>> GetByIdAsync(int id)
    {
        var product = await products.GetByIdWithDetailsAsync(id);
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

        products.Add(product);
        await products.SaveChangesAsync();

        // Reload with Category and Supplier so the response contains their names.
        var created = await products.GetByIdWithDetailsAsync(product.Id);
        return ServiceResult<ProductDto>.Created(mapper.Map<ProductDto>(created!));
    }

    public async Task<ServiceResult<ProductDto>> UpdateAsync(int id, ProductRequestDto request)
    {
        var product = await products.GetByIdAsync(id);
        if (product is null)
            return ServiceResult<ProductDto>.NotFound($"Product {id} was not found.");

        var failure = await ValidateAsync(request);
        if (failure is not null) return failure;

        mapper.Map(request, product); // copies the request values onto the tracked entity
        product.Name = product.Name.Trim();
        await products.SaveChangesAsync();

        var updated = await products.GetByIdWithDetailsAsync(id);
        return ServiceResult<ProductDto>.Success(mapper.Map<ProductDto>(updated!));
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var product = await products.GetByIdAsync(id);
        if (product is null)
            return ServiceResult.NotFound($"Product {id} was not found.");

        products.Remove(product);
        await products.SaveChangesAsync();
        return ServiceResult.Success();
    }

    // Shape rules come from FluentValidation. Rules that need the database (do the category and supplier exist?) live here.
    private async Task<ServiceResult<ProductDto>?> ValidateAsync(ProductRequestDto request)
    {
        var result = await validator.ValidateAsync(request);
        var errors = result.ToErrorDictionary()
        .ToDictionary(x => x.Key, x => x.Value);

        if (request.CategoryId > 0 &&
            !await categories.ExistsAsync(request.CategoryId))
        {
            errors[nameof(request.CategoryId)] =
                [$"Category {request.CategoryId} does not exist."];
        }

        if (request.SupplierId > 0 &&
            !await suppliers.ExistsAsync(request.SupplierId))
        {
            errors[nameof(request.SupplierId)] =
                [$"Supplier {request.SupplierId} does not exist."];
        }

        return errors.Count > 0
            ? ServiceResult<ProductDto>.Validation(errors)
            : null;
    }
}
