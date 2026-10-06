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

namespace InventoryApiRest.Tests.Controllers
{
    public class CategoriesControllerTests
    {
        private readonly Mock<IDispatcher> _dispatcher = new();
        private readonly CategoriesController _sut;

        public CategoriesControllerTests() => _sut = new CategoriesController(_dispatcher.Object);

        [Fact]
        public async Task GetAll_ReturnsOk_WithCategories()
        {
            var categories = new List<CategoryDto> { new() { Id = 1, Name = "Periféricos" } };
            _dispatcher.Setup(d => d.QueryAsync(It.IsAny<GetCategoriesQuery>()))
                .ReturnsAsync((IEnumerable<CategoryDto>)categories);

            var result = await _sut.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Same(categories, ok.Value);
        }

        [Fact]
        public async Task GetById_WhenMissing_ReturnsNotFound()
        {
            _dispatcher.Setup(d => d.QueryAsync(It.Is<GetCategoryByIdQuery>(q => q.Id == 99)))
                .ReturnsAsync((CategoryDto?)null);

            var result = await _sut.GetById(99);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_AndSendsTheReceivedDto()
        {
            var dto = new CreateCategoryDto { Name = "Monitores" };
            var created = new CategoryDto { Id = 5, Name = "Monitores" };
            _dispatcher.Setup(d => d.SendAsync(It.IsAny<CreateCategoryCommand>())).ReturnsAsync(created);

            var result = await _sut.Create(dto);

            var response = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(CategoriesController.GetById), response.ActionName);
            Assert.Same(created, response.Value);
            _dispatcher.Verify(
                d => d.SendAsync(It.Is<CreateCategoryCommand>(c => c.Dto == dto)), Times.Once);
        }

        [Theory]
        [InlineData(true, typeof(NoContentResult))]
        [InlineData(false, typeof(NotFoundResult))]
        public async Task Update_MapsResultToStatusCode(bool updated, Type expected)
        {
            _dispatcher.Setup(d => d.SendAsync(It.IsAny<UpdateCategoryCommand>())).ReturnsAsync(updated);

            var result = await _sut.Update(3, new UpdateCategoryDto { Name = "Nuevo" });

            Assert.IsType(expected, result);
            _dispatcher.Verify(
                d => d.SendAsync(It.Is<UpdateCategoryCommand>(c => c.Id == 3)), Times.Once);
        }

        [Theory]
        [InlineData(true, typeof(NoContentResult))]
        [InlineData(false, typeof(NotFoundResult))]
        public async Task Delete_MapsResultToStatusCode(bool deleted, Type expected)
        {
            _dispatcher.Setup(d => d.SendAsync(It.IsAny<DeleteCategoryCommand>())).ReturnsAsync(deleted);

            var result = await _sut.Delete(3);

            Assert.IsType(expected, result);
            _dispatcher.Verify(
                d => d.SendAsync(It.Is<DeleteCategoryCommand>(c => c.Id == 3)), Times.Once);
        }
    }
}
