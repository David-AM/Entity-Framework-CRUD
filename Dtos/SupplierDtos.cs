namespace EF_CRUD.Dtos;

public sealed class SupplierDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public sealed class SupplierRequestDto
{
    public string Name { get; set; } = string.Empty;
}
