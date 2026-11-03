namespace EF_CRUD.Dtos;

public sealed class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public sealed class CategoryRequestDto
{
    public string Name { get; set; } = string.Empty;
}
