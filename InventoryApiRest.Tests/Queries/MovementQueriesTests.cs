using InventoryAPIRest.Exceptions;
using InventoryAPIRest.Features.Inventory;
using InventoryAPIRest.Models;
using InventoryAPIRest.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryApiRest.Tests.Queries
{
    public class MovementQueriesTests : IDisposable
    {
        private readonly SqliteDbFixture _db = new();
        private int _keyboardId;
        private int _mouseId;

        public void Dispose() => _db.Dispose();

        private async Task SeedAsync()
        {
            var category = new Category { Name = "Periféricos" };
            var keyboard = new Product { Name = "Teclado", Code = "TEC-001", Price = 100m, Stock = 7, Category = category };
            var mouse = new Product { Name = "Mouse", Code = "MOU-001", Price = 50m, Stock = 5, Category = category };
            _db.Context.Products.AddRange(keyboard, mouse);
            await _db.Context.SaveChangesAsync();

            _keyboardId = keyboard.Id;
            _mouseId = mouse.Id;

            _db.Context.InventoryMovements.AddRange(
                new InventoryMovement
                {
                    ProductId = keyboard.Id,
                    Type = MovementType.StockIn,
                    Quantity = 10,
                    StockBefore = 0,
                    StockAfter = 10,
                    CreatedAt = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc)
                },
                new InventoryMovement
                {
                    ProductId = keyboard.Id,
                    Type = MovementType.Sale,
                    Quantity = 3,
                    StockBefore = 10,
                    StockAfter = 7,
                    UnitPrice = 100m,
                    CreatedAt = new DateTime(2026, 1, 2, 10, 0, 0, DateTimeKind.Utc)
                },
                new InventoryMovement
                {
                    ProductId = mouse.Id,
                    Type = MovementType.StockIn,
                    Quantity = 5,
                    StockBefore = 0,
                    StockAfter = 5,
                    CreatedAt = new DateTime(2026, 1, 3, 10, 0, 0, DateTimeKind.Utc)
                });
            await _db.Context.SaveChangesAsync();
        }

        [Fact]
        public async Task GetMovements_ReturnsAll_NewestFirst_WithProductData()
        {
            await SeedAsync();
            var handler = new GetMovementsHandler(_db.Context);

            var result = (await handler.HandleAsync(new GetMovementsQuery())).ToList();

            Assert.Equal(3, result.Count);
            Assert.Equal(new[] { "StockIn", "Sale", "StockIn" }, result.Select(m => m.Type));
            Assert.Equal("Mouse", result[0].ProductName);
            Assert.Equal("MOU-001", result[0].ProductCode);
        }

        [Fact]
        public async Task GetMovementsByProduct_ReturnsOnlyThatProduct()
        {
            await SeedAsync();
            var handler = new GetMovementsByProductHandler(_db.Context);

            var result = (await handler.HandleAsync(new GetMovementsByProductQuery(_keyboardId))).ToList();

            Assert.Equal(2, result.Count);
            Assert.All(result, m => Assert.Equal(_keyboardId, m.ProductId));
            Assert.Equal("Sale", result[0].Type);
            Assert.Equal(100m, result[0].UnitPrice);
        }

        [Fact]
        public async Task GetMovementsByProduct_WhenProductDoesNotExist_Throws()
        {
            await SeedAsync();
            var handler = new GetMovementsByProductHandler(_db.Context);

            await Assert.ThrowsAsync<ProductNotFoundException>(
                () => handler.HandleAsync(new GetMovementsByProductQuery(999)));
        }
    }
}
