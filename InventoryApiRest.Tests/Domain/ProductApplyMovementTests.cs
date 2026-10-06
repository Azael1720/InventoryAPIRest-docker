using InventoryAPIRest.Exceptions;
using InventoryAPIRest.Models;
using InventoryAPIRest.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryApiRest.Tests.Domain
{
    public class ProductApplyMovementTests
    {
        private static Product CreateProduct(int stock = 10, bool isActive = true) => new()
        {
            Id = 1,
            Name = "Teclado",
            Code = "TEC-001",
            Price = 899.90m,
            Stock = stock,
            IsActive = isActive,
            CategoryId = 1
        };

        [Fact]
        public void Sale_ReducesStock_AndRecordsBeforeAfterAndUnitPrice()
        {
            var product = CreateProduct(stock: 10);

            var movement = product.ApplyMovement(MovementType.Sale, 3, "Venta");

            Assert.Equal(7, product.Stock);
            Assert.Equal(MovementType.Sale, movement.Type);
            Assert.Equal(3, movement.Quantity);
            Assert.Equal(10, movement.StockBefore);
            Assert.Equal(7, movement.StockAfter);
            Assert.Equal(899.90m, movement.UnitPrice);
            Assert.Equal("Venta", movement.Reason);
        }

        [Fact]
        public void StockIn_AddsStock_WithoutUnitPrice()
        {
            var product = CreateProduct(stock: 10);

            var movement = product.ApplyMovement(MovementType.StockIn, 5, null);

            Assert.Equal(15, product.Stock);
            Assert.Equal(10, movement.StockBefore);
            Assert.Equal(15, movement.StockAfter);
            Assert.Null(movement.UnitPrice);
        }

        [Fact]
        public void StockOut_SubtractsStock()
        {
            var product = CreateProduct(stock: 10);

            var movement = product.ApplyMovement(MovementType.StockOut, 4, null);

            Assert.Equal(6, product.Stock);
            Assert.Equal(6, movement.StockAfter);
        }

        [Fact]
        public void Sale_OfAllTheStock_LeavesZero()
        {
            var product = CreateProduct(stock: 5);

            product.ApplyMovement(MovementType.Sale, 5, null);

            Assert.Equal(0, product.Stock);
        }

        [Theory]
        [InlineData(MovementType.Sale)]
        [InlineData(MovementType.StockOut)]
        public void WithInsufficientStock_Throws_AndKeepsStock(MovementType type)
        {
            var product = CreateProduct(stock: 2);

            var ex = Assert.Throws<InsufficientStockException>(
                () => product.ApplyMovement(type, 5, null));

            Assert.Equal(2, ex.Available);
            Assert.Equal(5, ex.Requested);
            Assert.Equal(2, product.Stock);
            Assert.Null(product.UpdatedAt);
        }

        [Fact]
        public void Sale_OnInactiveProduct_Throws_AndKeepsStock()
        {
            var product = CreateProduct(stock: 10, isActive: false);

            Assert.Throws<InactiveProductException>(
                () => product.ApplyMovement(MovementType.Sale, 1, null));

            Assert.Equal(10, product.Stock);
        }

        [Theory]
        [InlineData(MovementType.StockIn)]
        [InlineData(MovementType.StockOut)]
        public void StockIn_AndStockOut_AreAllowed_OnInactiveProduct(MovementType type)
        {
            var product = CreateProduct(stock: 10, isActive: false);

            var movement = product.ApplyMovement(type, 1, null);

            Assert.NotNull(movement);
        }

        [Fact]
        public void ApplyMovement_SetsUpdatedAt_AndMovementDate()
        {
            var product = CreateProduct();

            var movement = product.ApplyMovement(MovementType.StockIn, 1, null);

            Assert.NotNull(product.UpdatedAt);
            Assert.Equal(product.UpdatedAt, movement.CreatedAt);
        }
    }
}
