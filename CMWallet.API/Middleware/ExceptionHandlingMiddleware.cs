using CMWallet.Application.Exceptions;
using Microsoft.EntityFrameworkCore;

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

            var statusCode = ex switch
            {
                RegistroNaoEncontradoException => StatusCodes.Status404NotFound,
                ValidacaoNegocioException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status422UnprocessableEntity
            };

            await EscreverRespostaAsync(context, statusCode, ex.Message);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogWarning(ex, "Violação de integridade ao salvar no banco.");

            await EscreverRespostaAsync(
                context,
                StatusCodes.Status409Conflict,
                "A operação viola uma restrição de integridade (registro em uso ou referência inválida).");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno inesperado.");

            await EscreverRespostaAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "Ocorreu um erro interno no servidor.");
        }
    }

    private static Task EscreverRespostaAsync(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        return context.Response.WriteAsJsonAsync(new { message });
    }
}
