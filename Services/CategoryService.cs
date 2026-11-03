using AutoMapper;
using EF_CRUD.Common;
using EF_CRUD.Dtos;
using EF_CRUD.Models;
using EF_CRUD.Repositories;
using FluentValidation;

namespace EF_CRUD.Services;

public sealed class CategoryService(
    ICategoryRepository categories,
    IMapper mapper,
    IValidator<CategoryRequestDto> validator) : ICategoryService
{
    public async Task<ServiceResult<IReadOnlyList<CategoryDto>>> GetAllAsync()
    {
        var items = await categories.GetAllAsync();
        var dtos = mapper.Map<List<CategoryDto>>(items.OrderBy(c => c.Id));
        return ServiceResult<IReadOnlyList<CategoryDto>>.Success(dtos);
    }

    public async Task<ServiceResult<CategoryDto>> GetByIdAsync(int id)
    {
        var category = await categories.GetByIdAsync(id);
        return category is null
            ? ServiceResult<CategoryDto>.NotFound($"Category {id} was not found.")
            : ServiceResult<CategoryDto>.Success(mapper.Map<CategoryDto>(category));
    }

    public async Task<ServiceResult<CategoryDto>> CreateAsync(CategoryRequestDto request)
    {
        var validation = await validator.ValidateAsync(request);
        if (!validation.IsValid)
            return ServiceResult<CategoryDto>.Validation(validation.ToErrorDictionary());

        var name = request.Name.Trim();
        if (await categories.ExistsByNameAsync(name))
            return ServiceResult<CategoryDto>.Conflict($"A category named '{name}' already exists.");

        var category = mapper.Map<Category>(request);
        category.Name = name;

        categories.Add(category);
        await categories.SaveChangesAsync();
        return ServiceResult<CategoryDto>.Created(mapper.Map<CategoryDto>(category));
    }

    public async Task<ServiceResult<CategoryDto>> UpdateAsync(int id, CategoryRequestDto request)
    {
        var category = await categories.GetByIdAsync(id);
        if (category is null)
            return ServiceResult<CategoryDto>.NotFound($"Category {id} was not found.");

        var validation = await validator.ValidateAsync(request);
        if (!validation.IsValid)
            return ServiceResult<CategoryDto>.Validation(validation.ToErrorDictionary());

        var name = request.Name.Trim();
        if (await categories.ExistsByNameAsync(name, excludeId: id))
            return ServiceResult<CategoryDto>.Conflict($"A category named '{name}' already exists.");

        mapper.Map(request, category);
        category.Name = name;
        await categories.SaveChangesAsync();
        return ServiceResult<CategoryDto>.Success(mapper.Map<CategoryDto>(category));
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var category = await categories.GetByIdAsync(id);
        if (category is null)
            return ServiceResult.NotFound($"Category {id} was not found.");

        if (await categories.HasProductsAsync(id))
            return ServiceResult.Conflict($"Category '{category.Name}' still has products. Move or delete them first.");

        categories.Remove(category);
        await categories.SaveChangesAsync();
        return ServiceResult.Success();
    }
}
