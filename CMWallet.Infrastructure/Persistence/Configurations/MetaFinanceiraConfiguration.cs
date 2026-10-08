using CMWallet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMWallet.Infrastructure.Persistence.Configurations;

public class MetaFinanceiraConfiguration : IEntityTypeConfiguration<MetaFinanceira>
{
    public void Configure(EntityTypeBuilder<MetaFinanceira> entity)
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
    }
}
