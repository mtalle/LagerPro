using LagerPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LagerPro.Infrastructure.Persistence.Configurations;

public class LeveringConfiguration : IEntityTypeConfiguration<Levering>
{
    public void Configure(EntityTypeBuilder<Levering> builder)
    {
        builder.ToTable("Leveringer");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Referanse).HasMaxLength(100);
        builder.Property(x => x.FraktBrev).HasMaxLength(100);
        builder.Property(x => x.Kommentar).HasMaxLength(1000);
        builder.Property(x => x.LevertAv).HasMaxLength(100);

        builder.HasOne(x => x.Kunde)
            .WithMany(x => x.Leveringer)
            .HasForeignKey(x => x.KundeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
