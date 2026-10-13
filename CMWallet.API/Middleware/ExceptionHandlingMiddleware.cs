using CMWallet.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CMWallet.API.Middleware;

/// <summary>
/// Converte exceções em respostas <see cref="ProblemDetails"/> (RFC 9457), o mesmo formato
/// usado pelos erros de validação de modelo. A mensagem para o usuário vai em <c>detail</c>.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private const int ViolacaoDeChaveEstrangeira = 547;
    private const int ViolacaoDeIndiceUnico = 2601;
    private const int ViolacaoDeChaveUnica = 2627;

    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IProblemDetailsService problemDetailsService)
    {
        try
        {
            await _next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            // O cliente desistiu da requisição; não há a quem responder.
            _logger.LogDebug("Requisição {Metodo} {Caminho} cancelada pelo cliente.", context.Request.Method, context.Request.Path);
        }
        catch (Exception ex) when (!context.Response.HasStarted)
        {
            var (status, titulo, detalhe) = Classificar(ex);

            if (status >= StatusCodes.Status500InternalServerError)
            {
                _logger.LogError(ex, "Erro inesperado em {Metodo} {Caminho}.", context.Request.Method, context.Request.Path);
            }
            else
            {
                _logger.LogInformation("{Titulo} em {Metodo} {Caminho}: {Detalhe}", titulo, context.Request.Method, context.Request.Path, detalhe);
            }

            context.Response.StatusCode = status;
            await problemDetailsService.WriteAsync(new ProblemDetailsContext
            {
                HttpContext = context,
                ProblemDetails = new ProblemDetails
                {
                    Status = status,
                    Title = titulo,
                    Detail = detalhe
                }
            });
        }
    }

    private static (int Status, string Titulo, string Detalhe) Classificar(Exception ex) => ex switch
    {
        RegistroNaoEncontradoException => (StatusCodes.Status404NotFound, "Registro não encontrado", ex.Message),
        ValidacaoNegocioException => (StatusCodes.Status400BadRequest, "Operação inválida", ex.Message),
        ConflitoNegocioException => (StatusCodes.Status409Conflict, "Operação não permitida", ex.Message),

        // Salvaguardas para quando uma regra escapar da validação dos serviços.
        DbUpdateException { InnerException: SqlException { Number: ViolacaoDeChaveEstrangeira } } =>
            (StatusCodes.Status409Conflict, "Operação não permitida", "O registro está vinculado a outros dados ou referencia um registro inexistente."),
        DbUpdateException { InnerException: SqlException { Number: ViolacaoDeIndiceUnico or ViolacaoDeChaveUnica } } =>
            (StatusCodes.Status409Conflict, "Registro duplicado", "Já existe um registro com esses dados."),

        _ => (StatusCodes.Status500InternalServerError, "Erro interno", "Ocorreu um erro inesperado no servidor.")
    };
}
