using LagerPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LagerPro.Infrastructure.Persistence.Configurations;

public class MottakConfiguration : IEntityTypeConfiguration<Mottak>
{
    public void Configure(EntityTypeBuilder<Mottak> builder)
    {
        builder.ToTable("Mottak");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Referanse).HasMaxLength(100);
        builder.Property(x => x.Kommentar).HasMaxLength(1000);
        builder.Property(x => x.MottattAv).HasMaxLength(100);

        builder.HasOne(x => x.Leverandor)
            .WithMany(x => x.Mottak)
            .HasForeignKey(x => x.LeverandorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
