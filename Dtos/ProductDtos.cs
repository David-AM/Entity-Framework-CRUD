namespace EF_CRUD.Dtos;

/// <summary>What the API returns for a product.</summary>
public sealed class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
}

/// <summary>What the client sends to create or update a product (same shape for both).</summary>
public sealed class ProductRequestDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public int SupplierId { get; set; }
}
