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
    public DbSet<MetaFinanceira> MetasFinanceiras { get; set; }
    public DbSet<Transacao> Transacoes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Conta>(entity =>
        {
            entity.HasKey(c => c.ContaId);
            entity.Property(c => c.Nome)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(c => c.SaldoInicial)
                .HasColumnType("decimal(18,2)");

            entity.HasMany(c => c.Transacoes)
                .WithOne(t => t.Conta)
                .HasForeignKey(t => t.ContaId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(c => c.MetasFinanceiras)
                .WithOne(m => m.Conta)
                .HasForeignKey(m => m.ContaId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(c => c.CategoriaId);
            entity.Property(c => c.Nome)
                .IsRequired()
                .HasMaxLength(200);

            entity.HasMany(c => c.Transacoes)
                .WithOne(t => t.Categoria)
                .HasForeignKey(t => t.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<MetaFinanceira>(entity =>
        {
            entity.HasKey(m => m.MetaId);
            entity.Property(m => m.ValorMeta)
                .HasColumnType("decimal(18,2)");

            entity.Property(m => m.ValorAtual)
                .HasColumnType("decimal(18,2)");

            entity.Property(m => m.DataMeta)
                .IsRequired();

            entity.HasOne(m => m.Conta)
                .WithMany(c => c.MetasFinanceiras)
                .HasForeignKey(m => m.ContaId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Transacao>(entity =>
        {
            entity.HasKey(t => t.TransacaoId);
            entity.Property(t => t.Descricao)
                .IsRequired()
                .HasMaxLength(300);

            entity.Property(t => t.Valor)
                .HasColumnType("decimal(18,2)");

            entity.Property(t => t.Data)
                .IsRequired();

            entity.Property(t => t.DataCriacao)
                .IsRequired();

            entity.HasOne(t => t.Conta)
                .WithMany(c => c.Transacoes)
                .HasForeignKey(t => t.ContaId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(t => t.Categoria)
                .WithMany(c => c.Transacoes)
                .HasForeignKey(t => t.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
