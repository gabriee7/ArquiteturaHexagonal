using ContratacaoService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContratacaoService.Infrastructure.Data.Configurations
{
    public class ContratacaoConfiguration : IEntityTypeConfiguration<Contratacao>
    {
        public void Configure(EntityTypeBuilder<Contratacao> builder)
        {
            builder.ToTable("Contratacoes");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.CreatedAt)
                .IsRequired();
            builder.Property(c => c.UpdatedAt)
                .IsRequired(false);
            builder.Property(c => c.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false)
                .ValueGeneratedNever();
            builder.Property(c => c.PropostaId)
                .IsRequired();
            builder.Property(c => c.DataContratacao)
                .IsRequired();
            builder.HasQueryFilter(c => !c.IsDeleted);
        }
    }
}
