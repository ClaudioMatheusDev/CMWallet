using CMWallet.API.Middleware;
using CMWallet.Application;
using CMWallet.Application.Interfaces;
using CMWallet.Application.Services;
using CMWallet.Infrastructure;
using CMWallet.Infrastructure.Repositories;

namespace CMWallet.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddApplication();
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddOpenApi();

            builder.Services.AddScoped<IContaService, ContaService>();
            builder.Services.AddScoped<IContaRepository, ContaRepository>();

            builder.Services.AddScoped<ICategoriaService, CategoriaService>();
            builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();

            builder.Services.AddScoped<ITransacaoService, TransacaoService>();
            builder.Services.AddScoped<ITransacaoRepository, TransacaoRepository>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}

