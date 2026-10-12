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

        // Excluir conta ou categoria com transações é bloqueado pelo serviço; as FKs garantem o mesmo no banco.
        entity.HasOne(t => t.Conta)
            .WithMany(c => c.Transacoes)
            .HasForeignKey(t => t.ContaId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(t => t.Categoria)
            .WithMany(c => c.Transacoes)
            .HasForeignKey(t => t.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        // Listagem por conta ordenada por data, e filtros/resumos por período.
        entity.HasIndex(t => new { t.ContaId, t.Data });
        entity.HasIndex(t => t.Data);
    }
}
