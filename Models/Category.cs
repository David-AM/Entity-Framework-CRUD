using System.ComponentModel.DataAnnotations;

namespace EF_CRUD.Models;

public sealed class Category
{
    public int Id { get; set; }

    [Required]
    [MaxLength(80)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
