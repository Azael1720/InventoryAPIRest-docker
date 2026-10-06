using InventoryAPIRest.Abstractions;
using InventoryAPIRest.Controllers;
using InventoryAPIRest.DTOs;
using InventoryAPIRest.Features.Inventory;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace InventoryApiRest.Tests.Controllers
{
    public class InventoryMovementsControllerTest
    {
        private readonly Mock<IDispatcher> _mockDispatcher;
        private readonly InventoryMovementsController _controller;

        public InventoryMovementsControllerTest()
        {
            _mockDispatcher = new Mock<IDispatcher>();
            _controller = new InventoryMovementsController(_mockDispatcher.Object);
        }

        [Fact]
        public async Task AddStock_Product()
        {
            int productId = 1;
            var dto = new StockOperationDto { Quantity = 10, Reason = "Compra de mercancía" };
            var falseMovement = new InventoryMovementDto { Id = 101, ProductId = productId, Quantity = 10, Type = "StockIn" };

            _mockDispatcher
                .Setup(d => d.SendAsync(It.IsAny<AddStockCommand>()))
                .ReturnsAsync(falseMovement);

            var result = await _controller.AddStock(productId, dto);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var value = Assert.IsType<InventoryMovementDto>(okResult.Value);
            Assert.Equal(101, value.Id);
        }

        [Fact]
        public async Task RemoveStock_StockNotEnougthConflict409()
        {
            int productId = 1;
            var dto = new StockOperationDto { Quantity = 500 };

            _mockDispatcher
                .Setup(d => d.SendAsync(It.IsAny<RemoveStockCommand>()))
                .ThrowsAsync(new InvalidOperationException("Stock insuficiente para realizar la operación."));

            var result = await _controller.RemoveStock(productId, dto);

            Assert.IsType<ConflictObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetByProduct_IfProductNotFound404()
        {
            int inexistentProductId = 999;
            _mockDispatcher
                .Setup(d => d.QueryAsync(It.IsAny<GetMovementsByProductQuery>()))
                .ThrowsAsync(new KeyNotFoundException("El producto no existe."));

            var result = await _controller.GetByProduct(inexistentProductId);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetAll_AllMovements()
        {
            var falseMovements = new List<InventoryMovementDto>
            {
                new InventoryMovementDto { Id = 1, ProductId = 1, Quantity = 5 },
                new InventoryMovementDto { Id = 2, ProductId = 2, Quantity = 3 }
            };

            _mockDispatcher
                .Setup(d => d.QueryAsync(It.IsAny<GetMovementsQuery>()))
                .ReturnsAsync(falseMovements);

            var resultado = await _controller.GetAll();

            var okResultado = Assert.IsType<OkObjectResult>(resultado.Result);
            var lista = Assert.IsAssignableFrom<IEnumerable<InventoryMovementDto>>(okResultado.Value);
            Assert.Equal(2, ((List<InventoryMovementDto>)lista).Count);
        }
    }
}
