using System.ComponentModel.DataAnnotations;

namespace EF_CRUD.Models;

public sealed class Supplier
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
