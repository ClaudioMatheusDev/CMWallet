using CMWallet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMWallet.Infrastructure.Persistence.Configurations;

public class TransacaoConfiguration : IEntityTypeConfiguration<Transacao>
{
    public void Configure(EntityTypeBuilder<Transacao> entity)
    {
        entity.HasKey(t => t.TransacaoId);

        entity.Property(t => t.Descricao)
            .IsRequired()
            .HasMaxLength(300);

        entity.Property(t => t.Valor)
            .HasColumnType("decimal(18,2)");

        entity.Property(t => t.Data)
            .IsRequired();

        entity.Property(t => t.Tipo)
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
    }
}
