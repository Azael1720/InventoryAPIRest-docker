using InventoryAPIRest.Abstractions;
using InventoryAPIRest.Controllers;
using InventoryAPIRest.DTOs;
using InventoryAPIRest.Features.Products;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace InventoryApiRest.Tests
{
    public class ProductsControllerTests
    {
        private readonly Mock<IDispatcher> _mockDispatcher;
        private readonly ProductsController _controller;

        public ProductsControllerTests()
        {
            _mockDispatcher = new Mock<IDispatcher>();
            _controller = new ProductsController(_mockDispatcher.Object);
        }

        [Fact]
        public async Task GetAll_Products()
        {
            var falsesProducts = new List<ProductDto>
            {
                new ProductDto { Id = 1, Name = "Laptop", Code = "LAP01", Price = 1200m },
                new ProductDto { Id = 2, Name = "Mouse", Code = "MOU02", Price = 25m }
            };

            _mockDispatcher
                .Setup(d => d.QueryAsync(It.IsAny<GetProductsQuery>()))
                .ReturnsAsync(falsesProducts);

            var result = await _controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedList = Assert.IsAssignableFrom<IEnumerable<ProductDto>>(okResult.Value);
            Assert.Equal(2, ((List<ProductDto>)returnedList).Count);
        }

        [Fact]
        public async Task GetById_IfNotFound()
        {
            int inexistentId = 50;
            _mockDispatcher
                .Setup(d => d.QueryAsync(It.IsAny<GetProductByIdQuery>()))
                .ReturnsAsync((ProductDto)null!);

            var resultado = await _controller.GetById(inexistentId);

            Assert.IsType<NotFoundResult>(resultado.Result);
        }

        [Fact]
        public async Task Create_Product()
        {
            var dto = new CreateProductDto { Name = "Teclado", Code = "TEC03", Price = 45m };
            var createdProduct = new ProductDto { Id = 3, Name = "Teclado", Code = "TEC03", Price = 45m };

            _mockDispatcher
                .Setup(d => d.SendAsync(It.IsAny<CreateProductCommand>()))
                .ReturnsAsync(createdProduct);

            var result = await _controller.Create(dto);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal("GetById", createdResult.ActionName);
            Assert.NotNull(createdResult.Value);
        }

        [Fact]
        public async Task Update_IfNotExistsBadRequest()
        {
            int id = 10;
            var dto = new UpdateProductDto { Name = "Laptop Modificada" };
            _mockDispatcher
                .Setup(d => d.SendAsync(It.IsAny<UpdateProductCommand>()))
                .ThrowsAsync(new KeyNotFoundException("La categoría especificada no existe."));

            var result = await _controller.Update(id, dto);

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
