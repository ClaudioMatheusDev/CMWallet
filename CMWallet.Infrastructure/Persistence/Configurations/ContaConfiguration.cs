using CMWallet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMWallet.Infrastructure.Persistence.Configurations;

public class ContaConfiguration : IEntityTypeConfiguration<Conta>
{
    public void Configure(EntityTypeBuilder<Conta> entity)
    {
        entity.HasKey(c => c.ContaId);

        entity.Property(c => c.Nome)
            .IsRequired()
            .HasMaxLength(200);

        entity.Property(c => c.SaldoInicial)
            .HasColumnType("decimal(18,2)");

        entity.Property(c => c.TipoConta)
            .IsRequired();

        entity.HasMany(c => c.Transacoes)
            .WithOne(t => t.Conta)
            .HasForeignKey(t => t.ContaId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasMany(c => c.MetasFinanceiras)
            .WithOne(m => m.Conta)
            .HasForeignKey(m => m.ContaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
