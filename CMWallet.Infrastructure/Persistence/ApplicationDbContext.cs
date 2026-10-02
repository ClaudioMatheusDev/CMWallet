using CMWallet.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CMWallet.Infrastructure.Persistence;

public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Conta> Contas { get; set; }
    public DbSet<MetaFinanceira> MetaFinanceiras { get; set; }
    public DbSet<Transacao> Transacaos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
