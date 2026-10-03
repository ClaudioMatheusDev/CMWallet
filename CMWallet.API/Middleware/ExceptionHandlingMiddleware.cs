using CMWallet.Application.Exceptions;
using Microsoft.AspNetCore.Http;

namespace CMWallet.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BusinessException ex)
        {
            _logger.LogWarning(ex, "Erro de regra de negócio.");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = ex switch
            {
                RegistroNaoEncontradoException => StatusCodes.Status404NotFound,
                ValidacaoNegocioException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status422UnprocessableEntity
            };

            await context.Response.WriteAsJsonAsync(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno inesperado.");
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new { message = "Ocorreu um erro interno no servidor." });
        }
    }
}
