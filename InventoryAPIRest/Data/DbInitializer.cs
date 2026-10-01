using InventoryAPIRest.Models;
using InventoryAPIRest.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace InventoryAPIRest.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(AppDbContext context)
        {
            await context.Database.MigrateAsync();
            if (await context.Categories.AnyAsync())
                return;

            var categories = new List<Category>
            {
                new Category
                {
                    Name = "Computación",
                    Description = "Productos de computación"
                },
                new Category
                {
                    Name = "Accesorios",
                    Description = "Accesorios para computadora"
                },
                new Category
                {
                    Name = "Periféricos",
                    Description = "Periféricos para computadora"
                }
            };

            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();

            var products = new List<Product>
            {
                new Product
                {
                    Name = "Laptop Lenovo",
                    Code = "LAP-LEN-001",
                    Description = "Laptop Lenovo para trabajo y desarrollo",
                    Price = 18500.00m,
                    Stock = 10,
                    IsActive = true,
                    CategoryId = categories[0].Id,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Mouse inalámbrico",
                    Code = "MOU-IN-001",
                    Description = "Mouse inalámbrico para PC",
                    Price = 350.00m,
                    Stock = 25,
                    IsActive = true,
                    CategoryId = categories[1].Id,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Teclado mecánico",
                    Code = "TEC-MEC-001",
                    Description = "Teclado mecánico USB",
                    Price = 850.00m,
                    Stock = 15,
                    IsActive = true,
                    CategoryId = categories[2].Id,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Monitor 24 pulgadas",
                    Code = "MON-24-001",
                    Description = "Monitor Full HD de 24 pulgadas",
                    Price = 3200.00m,
                    Stock = 8,
                    IsActive = true,
                    CategoryId = categories[0].Id,
                    CreatedAt = DateTime.UtcNow
                }
            };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();

            var movements = new List<InventoryMovement>
            {
                new InventoryMovement
                {
                    ProductId = products[0].Id,
                    Type = MovementType.StockIn,
                    Quantity = 10,
                    StockBefore = 0,
                    StockAfter = 10,
                    UnitPrice = 18500.00m,
                    Reason = "Carga inicial de inventario",
                    CreatedAt = DateTime.UtcNow
                },
                new InventoryMovement
                {
                    ProductId = products[1].Id,
                    Type = MovementType.StockIn,
                    Quantity = 25,
                    StockBefore = 0,
                    StockAfter = 25,
                    UnitPrice = 350.00m,
                    Reason = "Carga inicial de inventario",
                    CreatedAt = DateTime.UtcNow
                },
                new InventoryMovement
                {
                    ProductId = products[2].Id,
                    Type = MovementType.StockIn,
                    Quantity = 15,
                    StockBefore = 0,
                    StockAfter = 15,
                    UnitPrice = 850.00m,
                    Reason = "Carga inicial de inventario",
                    CreatedAt = DateTime.UtcNow
                },
                new InventoryMovement
                {
                    ProductId = products[3].Id,
                    Type = MovementType.StockIn,
                    Quantity = 8,
                    StockBefore = 0,
                    StockAfter = 8,
                    UnitPrice = 3200.00m,
                    Reason = "Carga inicial de inventario",
                    CreatedAt = DateTime.UtcNow
                }
            };

            await context.InventoryMovements.AddRangeAsync(movements);
            await context.SaveChangesAsync();
        }
    }
}
