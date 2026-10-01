using InventoryAPIRest.Abstractions;
using InventoryAPIRest.Controllers;
using InventoryAPIRest.DTOs;
using InventoryAPIRest.Features.Categories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace InventoryApiRest.Tests
{
    public class CategoriesControllerTests
    {
        private readonly Mock<IDispatcher> _mockDispatcher;
        private readonly CategoriesController _controller;

        public CategoriesControllerTests()
        {
            _mockDispatcher = new Mock<IDispatcher>();
            _controller = new CategoriesController(_mockDispatcher.Object);
        }

        [Fact]
        public async Task GetAll_Categories()
        {
            var categoriesFalses = new List<CategoryDto>
            {
                new CategoryDto { Id = 1, Name = "Electrónica", Description = "Dispositivos" },
                new CategoryDto { Id = 2, Name = "Hogar", Description = "Muebles" }
            };

            _mockDispatcher
                .Setup(d => d.QueryAsync(It.IsAny<GetCategoriesQuery>()))
                .ReturnsAsync(categoriesFalses);

            var result = await _controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedList = Assert.IsAssignableFrom<IEnumerable<CategoryDto>>(okResult.Value);
            Assert.Equal(2, ((List<CategoryDto>)returnedList).Count);
        }

        [Fact]
        public async Task GetById_IfCategoryNotFound()
        {
            int inexistentId = 99;

            _mockDispatcher
                .Setup(d => d.QueryAsync(It.IsAny<GetCategoryByIdQuery>()))
                .ReturnsAsync((CategoryDto)null!);

            var result = await _controller.GetById(inexistentId);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Create_NewCategory()
        {
            var dto = new CreateCategoryDto { Name = "Electronica", Description = "Categoria para articulos electricos y electronicos" };
            var createdCategory = new CategoryDto {Name = "Electronica", Description = "Categoria para articulos electricos y electronicos" };

            _mockDispatcher
                .Setup(d => d.SendAsync(It.IsAny<CreateCategoryCommand>()))
                .ReturnsAsync(createdCategory);

            var result = await _controller.Create(dto);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal("GetById", createdResult.ActionName);
            Assert.NotNull(createdResult.Value);
        }

        [Fact]
        public async Task Delete_WithConflict409()
        {
            int asociatedIdProduct = 90;

            _mockDispatcher
                .Setup(d => d.SendAsync(It.IsAny<DeleteCategoryCommand>()))
                .ThrowsAsync(new InvalidOperationException("No se puede eliminar porque tiene productos."));

            var result = await _controller.Delete(asociatedIdProduct);

            Assert.IsType<ConflictObjectResult>(result);
        }
    }
}
