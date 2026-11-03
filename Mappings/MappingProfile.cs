using AutoMapper;
using EF_CRUD.Dtos;
using EF_CRUD.Models;

namespace EF_CRUD.Mappings;

public sealed class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Entity -> response DTO
        CreateMap<Product, ProductDto>()
            .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name))
            .ForMember(d => d.SupplierName, o => o.MapFrom(s => s.Supplier.Name));

        // Request DTO -> entity (also used to copy values onto an existing tracked entity when updating)
        CreateMap<ProductRequestDto, Product>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Category, o => o.Ignore())
            .ForMember(d => d.Supplier, o => o.Ignore());

        CreateMap<Category, CategoryDto>();
        CreateMap<CategoryRequestDto, Category>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Products, o => o.Ignore());

        CreateMap<Supplier, SupplierDto>();
        CreateMap<SupplierRequestDto, Supplier>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Products, o => o.Ignore());
    }
}
