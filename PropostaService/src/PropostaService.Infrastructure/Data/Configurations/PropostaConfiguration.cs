using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropostaService.Domain.Entities;

namespace PropostaService.Infrastructure.Data.Configurations
{
    public class PropostaConfiguration : IEntityTypeConfiguration<Proposta>
    {
        public void Configure(EntityTypeBuilder<Proposta> builder)
        {
            builder.ToTable("Propostas");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.CreatedAt)
                .IsRequired();
            builder.Property(p => p.UpdatedAt)
                .IsRequired(false);
            builder.Property(p => p.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);
            builder.Property(p => p.NomeSegurado)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(p => p.Valor)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            builder.Property(p => p.Status)
                .IsRequired();
            builder.HasQueryFilter(p => !p.IsDeleted);
        }
    }
}
