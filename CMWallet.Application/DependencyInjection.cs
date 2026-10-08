using CMWallet.Application.Interfaces;
using CMWallet.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CMWallet.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);

        services.AddScoped<IContaService, ContaService>();
        services.AddScoped<ICategoriaService, CategoriaService>();
        services.AddScoped<ITransacaoService, TransacaoService>();
        services.AddScoped<IMetaService, MetaService>();

        return services;
    }
}
