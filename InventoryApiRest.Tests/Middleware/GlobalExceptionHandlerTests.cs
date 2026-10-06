using InventoryAPIRest.Exceptions;
using InventoryAPIRest.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryApiRest.Tests.Middleware
{
    public class GlobalExceptionHandlerTests
    {
        private static async Task<(int Status, ProblemDetails Problem)> HandleAsync(Exception exception)
        {
            ProblemDetailsContext? captured = null;

            var service = new Mock<IProblemDetailsService>();
            service.Setup(s => s.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
                .Callback<ProblemDetailsContext>(context => captured = context)
                .ReturnsAsync(true);

            var handler = new GlobalExceptionHandler(service.Object, NullLogger<GlobalExceptionHandler>.Instance);
            var httpContext = new DefaultHttpContext();

            var handled = await handler.TryHandleAsync(httpContext, exception, CancellationToken.None);

            Assert.True(handled);
            Assert.NotNull(captured);
            return (httpContext.Response.StatusCode, captured!.ProblemDetails);
        }

        [Fact]
        public async Task NotFoundException_Returns404_WithDomainMessage()
        {
            var (status, problem) = await HandleAsync(new ProductNotFoundException(5));

            Assert.Equal(404, status);
            Assert.Equal(404, problem.Status);
            Assert.Equal("El producto con Id 5 no existe.", problem.Detail);
        }

        [Fact]
        public async Task ConflictException_Returns409_WithDomainMessage()
        {
            var (status, problem) = await HandleAsync(new InsufficientStockException(2, 5));

            Assert.Equal(409, status);
            Assert.Contains("Stock insuficiente", problem.Detail);
        }

        [Fact]
        public async Task UnexpectedException_Returns500_WithoutExposingTheOriginalMessage()
        {
            var (status, problem) = await HandleAsync(new InvalidOperationException("secreto: cadena de conexión"));

            Assert.Equal(500, status);
            Assert.Equal("Ocurrió un error inesperado.", problem.Detail);
            Assert.DoesNotContain("secreto", problem.Detail);
        }

        [Fact]
        public async Task Response_IncludesTraceId()
        {
            var (_, problem) = await HandleAsync(new ProductNotFoundException(1));

            Assert.True(problem.Extensions.ContainsKey("traceId"));
        }
    }
}
