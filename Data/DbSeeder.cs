using EF_CRUD.Common;
using EF_CRUD.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EF_CRUD.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var context = services.GetRequiredService<ProductDbContext>();
        await context.Database.EnsureCreatedAsync();

        await SeedIdentityAsync(services);
        await SeedCatalogAsync(context);
    }

    private static async Task SeedIdentityAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        foreach (var role in new[] { Roles.Admin, Roles.User })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        var config = services.GetRequiredService<IConfiguration>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        await SeedUserAsync(
            userManager,
            config["Seed:AdminEmail"],
            config["Seed:AdminPassword"],
            Roles.Admin);

        await SeedUserAsync(
            userManager,
            config["Seed:UserEmail"],
            config["Seed:UserPassword"],
            Roles.User);
    }

    private static async Task SeedUserAsync(
        UserManager<IdentityUser> userManager,
        string? email,
        string? password,
        string role)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password)) return;

        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                throw new InvalidOperationException(
                    $"Could not seed the {role} user: " + string.Join("; ", result.Errors.Select(e => e.Description)));
        }

        if (!await userManager.IsInRoleAsync(user, role))
        {
            var roleResult = await userManager.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded)
                throw new InvalidOperationException(
                    $"Could not assign the {role} role: " + string.Join("; ", roleResult.Errors.Select(e => e.Description)));
        }
    }

    private static async Task SeedCatalogAsync(ProductDbContext context)
    {
        var categoryNames = new[] { "Electronics", "Office", "Tools" };
        var categories = await context.Categories
            .Where(c => categoryNames.Contains(c.Name))
            .ToDictionaryAsync(c => c.Name);

        foreach (var name in categoryNames)
        {
            if (categories.ContainsKey(name)) continue;
            var category = new Category { Name = name };
            context.Categories.Add(category);
            categories[name] = category;
        }

        var supplierNames = new[] { "Logitech", "IKEA", "PT. Serba Ada" };
        var suppliers = await context.Suppliers
            .Where(s => supplierNames.Contains(s.Name))
            .ToDictionaryAsync(s => s.Name);

        foreach (var name in supplierNames)
        {
            if (suppliers.ContainsKey(name)) continue;
            var supplier = new Supplier { Name = name };
            context.Suppliers.Add(supplier);
            suppliers[name] = supplier;
        }

        if (!await context.Products.AnyAsync())
        {
            context.Products.AddRange(
                new Product { Name = "Wireless Mouse", Price = 29.99m, Description = "Ergonomic wireless mouse", Category = categories["Electronics"], Supplier = suppliers["Logitech"] },
                new Product { Name = "Wood Desk", Price = 39.99m, Description = "Wooden desk for an office", Category = categories["Office"], Supplier = suppliers["IKEA"] },
                new Product { Name = "Chainsaw", Price = 129.99m, Description = "Heavy-duty chainsaw", Category = categories["Tools"], Supplier = suppliers["PT. Serba Ada"] });
        }

        await context.SaveChangesAsync();
    }
}
