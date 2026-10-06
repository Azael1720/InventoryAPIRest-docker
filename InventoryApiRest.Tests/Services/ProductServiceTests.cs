using InventoryAPIRest.DTOs;
using InventoryAPIRest.Exceptions;
using InventoryAPIRest.Models;
using InventoryAPIRest.Models.Enums;
using InventoryAPIRest.Repositories.Interfaces;
using InventoryAPIRest.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryApiRest.Tests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _products = new();
        private readonly Mock<ICategoryRepository> _categories = new();
        private readonly ProductService _sut;
        private Product? _saved;

        public ProductServiceTests() => _sut = new ProductService(_products.Object, _categories.Object);

        private static CreateProductDto NewProductDto(int stock = 10) => new()
        {
            Name = "Teclado",
            Code = "TEC-001",
            Price = 899.90m,
            Stock = stock,
            CategoryId = 1
        };

        private static UpdateProductDto NewUpdateDto() => new()
        {
            Name = "Teclado Pro",
            Code = "TEC-002",
            Price = 999m,
            CategoryId = 2,
            IsActive = true
        };

        private static Product ExistingProduct(int stock = 10) => new()
        {
            Id = 1,
            Name = "Teclado",
            Code = "TEC-001",
            Price = 899.90m,
            Stock = stock,
            IsActive = true,
            CategoryId = 1
        };

        // Moq no admite argumentos opcionales en expresiones: se pasa excludeId explícito
        private void SetupCategoryAndCode(bool categoryExists = true, bool codeExists = false)
        {
            _categories.Setup(c => c.ExistsAsync(It.IsAny<int>())).ReturnsAsync(categoryExists);
            _products.Setup(p => p.CodeExistsAsync(It.IsAny<string>(), It.IsAny<int?>())).ReturnsAsync(codeExists);
        }

        private void SetupAdd()
        {
            _products.Setup(p => p.AddAsync(It.IsAny<Product>()))
                .Callback<Product>(p => _saved = p)
                .ReturnsAsync((Product p) => { p.Id = 10; return p; });
            _products.Setup(p => p.GetByIdAsync(10)).ReturnsAsync(() => _saved);
        }

        [Fact]
        public async Task CreateAsync_WhenCategoryDoesNotExist_Throws_AndDoesNotSave()
        {
            SetupCategoryAndCode(categoryExists: false);

            await Assert.ThrowsAsync<CategoryNotFoundException>(() => _sut.CreateAsync(NewProductDto()));

            _products.Verify(p => p.AddAsync(It.IsAny<Product>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_WhenCodeIsDuplicated_Throws_AndDoesNotSave()
        {
            SetupCategoryAndCode(codeExists: true);

            await Assert.ThrowsAsync<DuplicateProductCodeException>(() => _sut.CreateAsync(NewProductDto()));

            _products.Verify(p => p.AddAsync(It.IsAny<Product>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_WithInitialStock_SavesProductWithInitialMovement()
        {
            SetupCategoryAndCode();
            SetupAdd();

            var result = await _sut.CreateAsync(NewProductDto(stock: 10));

            Assert.Equal(10, result.Id);
            Assert.NotNull(_saved);
            var movement = Assert.Single(_saved!.Movements);
            Assert.Equal(MovementType.StockIn, movement.Type);
            Assert.Equal(10, movement.Quantity);
            Assert.Equal(0, movement.StockBefore);
            Assert.Equal(10, movement.StockAfter);
        }

        [Fact]
        public async Task CreateAsync_WithoutInitialStock_SavesProductWithoutMovement()
        {
            SetupCategoryAndCode();
            SetupAdd();

            await _sut.CreateAsync(NewProductDto(stock: 0));

            Assert.Empty(_saved!.Movements);
        }

        [Fact]
        public async Task UpdateAsync_WhenProductDoesNotExist_ReturnsFalse()
        {
            _products.Setup(p => p.GetByIdAsync(99)).ReturnsAsync((Product?)null);

            var result = await _sut.UpdateAsync(99, NewUpdateDto());

            Assert.False(result);
            _products.Verify(p => p.UpdateAsync(It.IsAny<Product>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_WhenCategoryDoesNotExist_Throws()
        {
            _products.Setup(p => p.GetByIdAsync(1)).ReturnsAsync(ExistingProduct());
            SetupCategoryAndCode(categoryExists: false);

            await Assert.ThrowsAsync<CategoryNotFoundException>(() => _sut.UpdateAsync(1, NewUpdateDto()));
        }

        [Fact]
        public async Task UpdateAsync_WhenCodeBelongsToAnotherProduct_Throws()
        {
            _products.Setup(p => p.GetByIdAsync(1)).ReturnsAsync(ExistingProduct());
            _categories.Setup(c => c.ExistsAsync(It.IsAny<int>())).ReturnsAsync(true);
            _products.Setup(p => p.CodeExistsAsync("TEC-002", 1)).ReturnsAsync(true);

            await Assert.ThrowsAsync<DuplicateProductCodeException>(() => _sut.UpdateAsync(1, NewUpdateDto()));
        }

        [Fact]
        public async Task UpdateAsync_WithValidData_UpdatesFields_AndKeepsStock()
        {
            var product = ExistingProduct(stock: 10);
            _products.Setup(p => p.GetByIdAsync(1)).ReturnsAsync(product);
            SetupCategoryAndCode();

            var result = await _sut.UpdateAsync(1, NewUpdateDto());

            Assert.True(result);
            Assert.Equal("Teclado Pro", product.Name);
            Assert.Equal(999m, product.Price);
            Assert.Equal(10, product.Stock);
            Assert.NotNull(product.UpdatedAt);
            _products.Verify(p => p.UpdateAsync(product), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenProductDoesNotExist_ReturnsFalse()
        {
            _products.Setup(p => p.GetByIdAsync(99)).ReturnsAsync((Product?)null);

            Assert.False(await _sut.DeleteAsync(99));
        }

        [Fact]
        public async Task DeleteAsync_DeactivatesProduct_InsteadOfDeleting()
        {
            var product = ExistingProduct();
            _products.Setup(p => p.GetByIdAsync(1)).ReturnsAsync(product);

            var result = await _sut.DeleteAsync(1);

            Assert.True(result);
            Assert.False(product.IsActive);
            _products.Verify(p => p.UpdateAsync(product), Times.Once);
            _products.Verify(p => p.DeleteAsync(It.IsAny<Product>()), Times.Never);
        }
    }
}
