using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrigiBank.Domain.Exceptions;
namespace OrigiBank.Api.ExceptionHandling;
public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (status, title, detail) = exception switch
        {
            NotFoundException e => (StatusCodes.Status404NotFound, "Recurso no encontrado", e.Message),
            InsufficientFundsException e => (StatusCodes.Status400BadRequest, "Fondos insuficientes", e.Message),
            ValidationException e => (StatusCodes.Status400BadRequest, "Solicitud inválida", e.Message),
            ConflictException e => (StatusCodes.Status409Conflict, "Conflicto", e.Message),
            _ => (StatusCodes.Status500InternalServerError, "Error interno", "Ocurrió un error inesperado.")
        };
        if (status == StatusCodes.Status500InternalServerError)
            logger.LogError(exception, "Excepción no controlada");
        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(
            new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = detail,
                Instance = httpContext.Request.Path
            },
            cancellationToken);
        return true;
    }
}