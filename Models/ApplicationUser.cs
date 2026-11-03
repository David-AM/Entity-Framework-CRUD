using Microsoft.AspNetCore.Identity;

namespace EF_CRUD.Models;

public sealed class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
}
