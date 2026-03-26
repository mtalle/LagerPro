using LagerPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LagerPro.Infrastructure.Persistence.Configurations;

public class LagerTransaksjonConfiguration : IEntityTypeConfiguration<LagerTransaksjon>
{
    public void Configure(EntityTypeBuilder<LagerTransaksjon> builder)
    {
        builder.ToTable("LagerTransaksjoner");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.LotNr).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Kilde).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Kommentar).HasMaxLength(1000);
        builder.Property(x => x.UtfortAv).HasMaxLength(100);
        builder.Property(x => x.Mengde).HasColumnType("decimal(18,3)");
        builder.Property(x => x.BeholdningEtter).HasColumnType("decimal(18,3)");

        builder.HasOne(x => x.Artikkel)
            .WithMany(x => x.LagerTransaksjoner)
            .HasForeignKey(x => x.ArtikkelId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
