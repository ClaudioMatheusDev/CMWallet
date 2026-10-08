using CMWallet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMWallet.Infrastructure.Persistence.Configurations;

public class CategoriaConfiguration : IEntityTypeConfiguration<Categoria>
{
    public void Configure(EntityTypeBuilder<Categoria> entity)
    {
        entity.HasKey(c => c.CategoriaId);

        entity.Property(c => c.Nome)
            .IsRequired()
            .HasMaxLength(200);

        entity.Property(c => c.Tipo)
            .IsRequired();

        entity.HasMany(c => c.Transacoes)
            .WithOne(t => t.Categoria)
            .HasForeignKey(t => t.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
