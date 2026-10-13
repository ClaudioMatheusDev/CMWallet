using CMWallet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CMWallet.Infrastructure.Persistence;

public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Conta> Contas { get; set; }
    public DbSet<MetaFinanceira> MetasFinanceiras { get; set; }
    public DbSet<Transacao> Transacoes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Datas de negócio usam DateOnly; todo DateTime restante é carimbo de data/hora em UTC.
        configurationBuilder.Properties<DateTime>().HaveConversion<DataHoraUtcConverter>();
    }

    /// <summary>
    /// O SQL Server não guarda o <see cref="DateTimeKind"/>; marcar como UTC na leitura faz a API
    /// serializar com o sufixo "Z", e o cliente converte para o fuso local corretamente.
    /// </summary>
    private sealed class DataHoraUtcConverter : ValueConverter<DateTime, DateTime>
    {
        public DataHoraUtcConverter()
            : base(
                valor => valor.Kind == DateTimeKind.Local ? valor.ToUniversalTime() : valor,
                valor => DateTime.SpecifyKind(valor, DateTimeKind.Utc))
        {
        }
    }
}
