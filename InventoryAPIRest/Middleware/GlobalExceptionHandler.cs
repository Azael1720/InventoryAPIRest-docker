using InventoryAPIRest.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace InventoryAPIRest.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly IProblemDetailsService _problemDetails;
        private readonly ILogger<GlobalExceptionHandler> _logger;
        public GlobalExceptionHandler(
            IProblemDetailsService problemDetails,
            ILogger<GlobalExceptionHandler> logger)
        {
            _problemDetails = problemDetails;
            _logger = logger;
        }
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var (status, title) = exception switch
            {
                NotFoundException => (StatusCodes.Status404NotFound, "Recurso no encontrado"),
                ConflictException => (StatusCodes.Status409Conflict, "Conflicto con las reglas de negocio"),
                _ => (StatusCodes.Status500InternalServerError, "Error interno del servidor")
            };
            if (status == StatusCodes.Status500InternalServerError)
                _logger.LogError(exception, "Error no controlado en {Method} {Path}",
                    httpContext.Request.Method, httpContext.Request.Path);
            else
                _logger.LogWarning("{Title}: {Message}", title, exception.Message);

            var problem = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = status == StatusCodes.Status500InternalServerError
                    ? "Ocurrió un error inesperado."
                    : exception.Message,
                Instance = httpContext.Request.Path
            };
            problem.Extensions["traceId"] = httpContext.TraceIdentifier;

            httpContext.Response.StatusCode = status;

            return await _problemDetails.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = problem
            });
        }
    }
}
