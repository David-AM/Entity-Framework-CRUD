using EF_CRUD.Data;
using EF_CRUD.Models;
using EF_CRUD.Services;
using Microsoft.EntityFrameworkCore;

namespace EF_CRUD;

public static class Program
{
    public static async Task Main()
    {
        var options = new DbContextOptionsBuilder<ProductDbContext>()
            .UseSqlite("Data Source=ProductCatalog.db")
            .Options;

        await using var context = new ProductDbContext(options);
        await context.Database.EnsureCreatedAsync();
        await SeedDatabaseAsync(context);
        await RunMenuAsync(new ProductService(context));
    }

    private static async Task SeedDatabaseAsync(ProductDbContext context)
    {
        var categoryNames = new[] { "Electronics", "Office", "Tools" };
        var categories = new Dictionary<string, EF_CRUD.Models.Category>();

        foreach (var categoryName in categoryNames)
        {
            EF_CRUD.Models.Category? category = await context.Categories.SingleOrDefaultAsync(c => c.Name == categoryName);
            if (category is null)
            {
                category = new EF_CRUD.Models.Category { Name = categoryName };
                context.Categories.Add(category);
            }

            categories[categoryName] = category;
        }
        await context.SaveChangesAsync();
        categories = await context.Categories
            .Where(category => categoryNames.Contains(category.Name))
            .ToDictionaryAsync(category => category.Name);

        var supplierNames = new[] { "Logitech", "IKEA", "PT. Serba Ada" };
        var suppliers = new Dictionary<string, Supplier>();

        foreach (var supplierName in supplierNames)
        {
            var supplier = await context.Suppliers.SingleOrDefaultAsync(s => s.Name == supplierName);
            if (supplier is null)
            {
                supplier = new Supplier { Name = supplierName };
                context.Suppliers.Add(supplier);
            }

            suppliers[supplierName] = supplier;
        }

        await context.SaveChangesAsync();
        suppliers = await context.Suppliers
            .Where(supplier => supplierNames.Contains(supplier.Name))
            .ToDictionaryAsync(supplier => supplier.Name);

        if (!await context.Products.AnyAsync())
        {
            context.Products.AddRange(
                new Product { Id = 1, Name = "Wireless Mouse", Price = 29.99m, Description = "Ergonomic wireless mouse", Category = categories["Electronics"], Supplier = suppliers["Logitech"] },
                new Product { Id = 2, Name = "Wood Desk", Price = 39.99m, Description = "Wooden desk for an office", Category = categories["Office"], Supplier = suppliers["IKEA"] },
                new Product { Id = 3, Name = "Chainsaw", Price = 129.99m, Description = "Heavy-duty chainsaw", Category = categories["Tools"], Supplier = suppliers["PT. Serba Ada"] });

            await context.SaveChangesAsync();
        }
    }

    private static async Task RunMenuAsync(ProductService service)
    {
        while (true)
        {
            Console.WriteLine("\n=== Product CRUD ===");
            Console.WriteLine("1. List products\n2. Get product by ID\n3. Create/Add product\n4. Update product\n5. Delete product\n0. Exit");
            Console.Write("Choose an option: ");

            switch (Console.ReadLine())
            {
                case "1": await ListProductsAsync(service); break;
                case "2": await GetProductAsync(service); break;
                case "3": await CreateProductAsync(service); break;
                case "4": await UpdateProductAsync(service); break;
                case "5": await DeleteProductAsync(service); break;
                case "0": return;
                default: Console.WriteLine("Invalid option."); break;
            }
        }
    }
    private static int ReadInt(string prompt)
    {
        Console.Write(prompt);
        int id;
        while (!int.TryParse(Console.ReadLine(), out id) || id <= 0) Console.Write("Enter a valid positive number: ");
        return id;
    }
    private static async Task ListProductsAsync(ProductService service)
    {
        var products = await service.GetAllAsync();
        if (products.Count == 0) { Console.WriteLine("No products found."); return; }
        foreach (var product in products)
            Console.WriteLine($"{product.Id}: {product.Name} | {product.Price:C} | Category: {product.Category.Name} | Supplier: {product.Supplier.Name}");
    }

    private static async Task GetProductAsync(ProductService service)
    {
        var product = await service.GetByIdAsync(ReadInt("Product ID: "));
        Console.WriteLine(product is null ? "Product not found." : $"{product.Id}: {product.Name} | {product.Price:C} | Category: {product.Category.Name} | Supplier: {product.Supplier.Name}");
    }

    private static async Task CreateProductAsync(ProductService service)
    {
        var product = await service.CreateAsync(ReadProduct());
        Console.WriteLine($"Product created with ID {product.Id}.");
    }

    private static async Task UpdateProductAsync(ProductService service)
    {
        var id = ReadInt("Product ID: ");
        var product = await service.GetByIdAsync(id);
        if (product is null)
        {
            Console.WriteLine("Product not found.");
            return;
        }

        Console.WriteLine("1. Update name");
        Console.WriteLine("2. Update price");
        Console.WriteLine("3. Update description");
        Console.WriteLine("4. Update category");
        Console.WriteLine("5. Update supplier");
        Console.WriteLine("0. Cancel");
        Console.Write("Choose a field: ");

        switch (Console.ReadLine())
        {
            case "1":
                Console.Write("New name: ");
                var name = Console.ReadLine()?.Trim() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine("Name is required.");
                    return;
                }
                product.Name = name;
                break;
            case "2":
                Console.WriteLine("use . to define decimal on price");
                Console.Write("New price: ");
                if (!decimal.TryParse(Console.ReadLine(), out var price) || price < 0)
                {
                    Console.WriteLine("Enter a valid non-negative price.");
                    return;
                }
                product.Price = price;
                break;
            case "3":
                Console.Write("New description: ");
                product.Description = Console.ReadLine()?.Trim();
                break;
            case "4":
        product.CategoryId = ReadInt("New category ID (1=Electronics, 2=Office, 3=Tools): ");
                break;
            case "5":
                product.SupplierId = ReadInt("New supplier ID (1=Logitech, 2=IKEA, 3=PT. Serba Ada): ");
                break;
            case "0":
                return;
            default:
                Console.WriteLine("Invalid option.");
                return;
        }

        await service.UpdateAsync(id, product);
        Console.WriteLine("Product updated.");
    }

    private static async Task DeleteProductAsync(ProductService service)
    {
        var deleted = await service.DeleteAsync(ReadInt("Product ID: "));
        Console.WriteLine(deleted ? "Product deleted." : "Product not found.");
    }

    private static Product ReadProduct()
    {
        Console.Write("Name: ");
        var name = Console.ReadLine()?.Trim() ?? string.Empty;
        while (string.IsNullOrWhiteSpace(name)) { Console.Write("Name is required: "); name = Console.ReadLine()?.Trim() ?? string.Empty; }
        Console.Write("Price: ");
        decimal price;
        while (!decimal.TryParse(Console.ReadLine(), out price) || price < 0) Console.Write("Enter a valid non-negative price: ");
        Console.Write("Description: ");
        var description = Console.ReadLine()?.Trim();
        Console.Write("Category ID (1=Electronics, 2=Office, 3=Tools): ");
        var categoryId = ReadInt("");
        Console.Write("Supplier ID (1=Logitech, 2=IKEA, 3=PT. Serba Ada): ");
        var supplierId = ReadInt("");
        return new Product { Name = name, Price = price, Description = description, CategoryId = categoryId, SupplierId = supplierId };
    }
}
