using LagerPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LagerPro.Infrastructure.Persistence.Configurations;

public class LeveringLinjeConfiguration : IEntityTypeConfiguration<LeveringLinje>
{
    public void Configure(EntityTypeBuilder<LeveringLinje> builder)
    {
        builder.ToTable("LeveringLinjer");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.LotNr).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Enhet).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Kommentar).HasMaxLength(1000);
        builder.Property(x => x.Mengde).HasColumnType("decimal(18,3)");

        builder.HasOne(x => x.Levering)
            .WithMany(x => x.Linjer)
            .HasForeignKey(x => x.LeveringId);

        builder.HasOne(x => x.Artikkel)
            .WithMany(x => x.LeveringLinjer)
            .HasForeignKey(x => x.ArtikkelId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
