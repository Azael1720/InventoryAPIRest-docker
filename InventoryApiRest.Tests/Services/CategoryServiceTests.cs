using InventoryAPIRest.DTOs;
using InventoryAPIRest.Exceptions;
using InventoryAPIRest.Models;
using InventoryAPIRest.Repositories.Interfaces;
using InventoryAPIRest.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryApiRest.Tests.Services
{
    public class CategoryServiceTests
    {
        private readonly Mock<ICategoryRepository> _repository = new();
        private readonly CategoryService _sut;

        public CategoryServiceTests() => _sut = new CategoryService(_repository.Object);

        private static Category ExistingCategory() => new()
        {
            Id = 3,
            Name = "Periféricos",
            Description = "Accesorios"
        };

        [Fact]
        public async Task GetAllAsync_ReturnsMappedCategories()
        {
            IEnumerable<Category> categories = new List<Category>
        {
            new() { Id = 1, Name = "Periféricos" },
            new() { Id = 2, Name = "Monitores" }
        };
            _repository.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);

            var result = (await _sut.GetAllAsync()).ToList();

            Assert.Equal(2, result.Count);
            Assert.Equal("Monitores", result[1].Name);
        }

        [Fact]
        public async Task GetByIdAsync_WhenMissing_ReturnsNull()
        {
            _repository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Category?)null);

            Assert.Null(await _sut.GetByIdAsync(99));
        }

        [Fact]
        public async Task GetByIdAsync_WhenExists_ReturnsDto()
        {
            _repository.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(ExistingCategory());

            var result = await _sut.GetByIdAsync(3);

            Assert.NotNull(result);
            Assert.Equal("Periféricos", result!.Name);
            Assert.Equal("Accesorios", result.Description);
        }

        [Fact]
        public async Task CreateAsync_SavesCategory_AndReturnsDtoWithId()
        {
            _repository.Setup(r => r.AddAsync(It.IsAny<Category>()))
                .ReturnsAsync((Category c) => { c.Id = 7; return c; });

            var result = await _sut.CreateAsync(new CreateCategoryDto { Name = "Monitores", Description = "Pantallas" });

            Assert.Equal(7, result.Id);
            Assert.Equal("Monitores", result.Name);
            _repository.Verify(r => r.AddAsync(It.Is<Category>(c => c.Name == "Monitores")), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WhenMissing_ReturnsFalse_AndDoesNotSave()
        {
            _repository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Category?)null);

            var result = await _sut.UpdateAsync(99, new UpdateCategoryDto { Name = "Nuevo" });

            Assert.False(result);
            _repository.Verify(r => r.UpdateAsync(It.IsAny<Category>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_WhenExists_ChangesFields()
        {
            var category = ExistingCategory();
            _repository.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(category);

            var result = await _sut.UpdateAsync(3, new UpdateCategoryDto { Name = "Accesorios", Description = "Nueva" });

            Assert.True(result);
            Assert.Equal("Accesorios", category.Name);
            Assert.Equal("Nueva", category.Description);
            _repository.Verify(r => r.UpdateAsync(category), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenMissing_ReturnsFalse()
        {
            _repository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Category?)null);

            Assert.False(await _sut.DeleteAsync(99));
        }

        [Fact]
        public async Task DeleteAsync_WhenCategoryHasProducts_Throws_AndDoesNotDelete()
        {
            _repository.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(ExistingCategory());
            _repository.Setup(r => r.HasProductsAsync(3)).ReturnsAsync(true);

            await Assert.ThrowsAsync<CategoryHasProductsException>(() => _sut.DeleteAsync(3));

            _repository.Verify(r => r.DeleteAsync(It.IsAny<Category>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_WhenCategoryHasNoProducts_DeletesIt()
        {
            var category = ExistingCategory();
            _repository.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(category);
            _repository.Setup(r => r.HasProductsAsync(3)).ReturnsAsync(false);

            var result = await _sut.DeleteAsync(3);

            Assert.True(result);
            _repository.Verify(r => r.DeleteAsync(category), Times.Once);
        }
    }
}
