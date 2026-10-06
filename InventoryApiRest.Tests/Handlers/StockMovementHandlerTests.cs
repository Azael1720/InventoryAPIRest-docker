using InventoryAPIRest.Abstractions;
using InventoryAPIRest.DTOs;
using InventoryAPIRest.Exceptions;
using InventoryAPIRest.Features.Inventory;
using InventoryAPIRest.Models;
using InventoryAPIRest.Models.Enums;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryApiRest.Tests.Handlers
{
    public class StockMovementHandlerTests
    {
        private readonly Mock<IProductWriteRepository> _products = new();
        private InventoryMovement? _registered;

        private static Product CreateProduct(int stock = 10, bool isActive = true) => new()
        {
            Id = 1,
            Name = "Teclado",
            Code = "TEC-001",
            Price = 100m,
            Stock = stock,
            IsActive = isActive,
            CategoryId = 1
        };

        private static StockOperationDto Dto(int quantity, string? reason = null) =>
            new() { Quantity = quantity, Reason = reason };

        private void SetupProduct(Product? product) =>
            _products.Setup(p => p.GetByIdAsync(1)).ReturnsAsync(product);

        private void SetupRegister() =>
            _products.Setup(p => p.RegisterMovementAsync(It.IsAny<Product>(), It.IsAny<InventoryMovement>()))
                .Callback<Product, InventoryMovement>((_, movement) => _registered = movement)
                .Returns(Task.CompletedTask);

        private void VerifyNothingRegistered() =>
            _products.Verify(
                p => p.RegisterMovementAsync(It.IsAny<Product>(), It.IsAny<InventoryMovement>()),
                Times.Never);

        [Fact]
        public async Task SellProduct_RegistersSale_AndReturnsDto()
        {
            SetupProduct(CreateProduct(stock: 10));
            SetupRegister();
            var handler = new SellProductHandler(_products.Object);

            var result = await handler.HandleAsync(new SellProductCommand(1, Dto(3, "Venta mostrador")));

            Assert.NotNull(_registered);
            Assert.Equal(MovementType.Sale, _registered!.Type);
            Assert.Equal(3, _registered.Quantity);
            Assert.Equal(1, _registered.ProductId);
            Assert.Equal("Sale", result.Type);
            Assert.Equal(10, result.StockBefore);
            Assert.Equal(7, result.StockAfter);
            Assert.Equal(100m, result.UnitPrice);
        }

        [Fact]
        public async Task SellProduct_WithInsufficientStock_Throws_AndDoesNotRegister()
        {
            SetupProduct(CreateProduct(stock: 2));
            var handler = new SellProductHandler(_products.Object);

            await Assert.ThrowsAsync<InsufficientStockException>(
                () => handler.HandleAsync(new SellProductCommand(1, Dto(5))));

            VerifyNothingRegistered();
        }

        [Fact]
        public async Task SellProduct_OnInactiveProduct_Throws_AndDoesNotRegister()
        {
            SetupProduct(CreateProduct(isActive: false));
            var handler = new SellProductHandler(_products.Object);

            await Assert.ThrowsAsync<InactiveProductException>(
                () => handler.HandleAsync(new SellProductCommand(1, Dto(1))));

            VerifyNothingRegistered();
        }

        [Fact]
        public async Task SellProduct_WhenProductDoesNotExist_Throws_AndDoesNotRegister()
        {
            SetupProduct(null);
            var handler = new SellProductHandler(_products.Object);

            await Assert.ThrowsAsync<ProductNotFoundException>(
                () => handler.HandleAsync(new SellProductCommand(1, Dto(1))));

            VerifyNothingRegistered();
        }

        [Fact]
        public async Task AddStock_RegistersStockIn()
        {
            SetupProduct(CreateProduct(stock: 10));
            SetupRegister();
            var handler = new AddStockHandler(_products.Object);

            var result = await handler.HandleAsync(new AddStockCommand(1, Dto(5, "Compra")));

            Assert.Equal(MovementType.StockIn, _registered!.Type);
            Assert.Equal("StockIn", result.Type);
            Assert.Equal(15, result.StockAfter);
            Assert.Null(result.UnitPrice);
        }

        [Fact]
        public async Task RemoveStock_RegistersStockOut()
        {
            SetupProduct(CreateProduct(stock: 10));
            SetupRegister();
            var handler = new RemoveStockHandler(_products.Object);

            var result = await handler.HandleAsync(new RemoveStockCommand(1, Dto(4, "Merma")));

            Assert.Equal(MovementType.StockOut, _registered!.Type);
            Assert.Equal("StockOut", result.Type);
            Assert.Equal(6, result.StockAfter);
        }

        [Fact]
        public async Task RemoveStock_WithInsufficientStock_Throws_AndDoesNotRegister()
        {
            SetupProduct(CreateProduct(stock: 1));
            var handler = new RemoveStockHandler(_products.Object);

            await Assert.ThrowsAsync<InsufficientStockException>(
                () => handler.HandleAsync(new RemoveStockCommand(1, Dto(3))));

            VerifyNothingRegistered();
        }

        [Fact]
        public async Task WhenStockChangedConcurrently_TheConflictPropagates()
        {
            SetupProduct(CreateProduct(stock: 10));
            _products.Setup(p => p.RegisterMovementAsync(It.IsAny<Product>(), It.IsAny<InventoryMovement>()))
                .ThrowsAsync(new StockConcurrencyException(1));
            var handler = new SellProductHandler(_products.Object);

            await Assert.ThrowsAsync<StockConcurrencyException>(
                () => handler.HandleAsync(new SellProductCommand(1, Dto(1))));
        }
    }
}
