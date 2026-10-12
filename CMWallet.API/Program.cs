using CMWallet.API.Middleware;
using CMWallet.Application;
using CMWallet.Infrastructure;
using CMWallet.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Scalar.AspNetCore;

namespace CMWallet.API
{
    public class Program
    {
        private const string PoliticaCors = "Frontend";

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddProblemDetails();
            builder.Services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                    options.InvalidModelStateResponseFactory = RespostaDeValidacao);
            builder.Services.AddOpenApi();

            var origensPermitidas = builder.Configuration.GetSection("Cors:OrigensPermitidas").Get<string[]>() ?? [];
            builder.Services.AddCors(options =>
                options.AddPolicy(PoliticaCors, policy => policy
                    .WithOrigins(origensPermitidas)
                    .AllowAnyHeader()
                    .AllowAnyMethod()));

            builder.Services.AddApplication();
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddHealthChecks().AddDbContextCheck<ApplicationDbContext>();

            var app = builder.Build();

            if (app.Configuration.GetValue<bool>("Database:AplicarMigrationsAoIniciar"))
            {
                app.Services.AplicarMigrations();
            }

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference(options => options.WithTitle("CMWallet API"));
            }

            app.UseHttpsRedirection();
            app.UseCors(PoliticaCors);

            app.MapControllers();
            app.MapHealthChecks("/health");

            app.Run();
        }

        /// <summary>
        /// Mantém o <see cref="ValidationProblemDetails"/> padrão (com <c>errors</c> por campo), mas com
        /// título em português e a primeira mensagem em <c>detail</c>, para o cliente exibir diretamente.
        /// </summary>
        private static IActionResult RespostaDeValidacao(ActionContext context)
        {
            // Chaves "$..." vêm do System.Text.Json: corpo malformado ou valor de tipo incompatível,
            // com mensagens técnicas em inglês que não servem para o usuário.
            var jsonInvalido = context.ModelState.Keys.Any(chave => chave.StartsWith('$'));

            var primeiraMensagem = jsonInvalido
                ? "O corpo da requisição não é um JSON válido ou contém um campo com tipo incorreto."
                : context.ModelState.Values
                    .SelectMany(entrada => entrada.Errors)
                    .Select(erro => erro.ErrorMessage)
                    .FirstOrDefault(mensagem => !string.IsNullOrWhiteSpace(mensagem));

            var problema = context.HttpContext.RequestServices
                .GetRequiredService<ProblemDetailsFactory>()
                .CreateValidationProblemDetails(
                    context.HttpContext,
                    context.ModelState,
                    StatusCodes.Status400BadRequest,
                    title: "Dados inválidos",
                    detail: primeiraMensagem ?? "Verifique os dados enviados.");

            return new BadRequestObjectResult(problema)
            {
                ContentTypes = { "application/problem+json" }
            };
        }
    }
}
