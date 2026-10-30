using System.ComponentModel.DataAnnotations;

namespace EF_CRUD.Models;

public sealed class Product
{
    public int Id { get; set; }

    [Required]
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [Range(0, 1_000_000_000)]
    public decimal Price { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public int CategoryId { get; set; }

    public Category Category { get; set; } = null!;

    public int SupplierId { get; set; }

    public Supplier Supplier { get; set; } = null!;
}
