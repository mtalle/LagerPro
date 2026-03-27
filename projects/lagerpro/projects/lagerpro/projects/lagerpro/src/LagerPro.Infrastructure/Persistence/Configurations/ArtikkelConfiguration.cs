using LagerPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LagerPro.Infrastructure.Persistence.Configurations;

public class ArtikkelConfiguration : IEntityTypeConfiguration<Artikkel>
{
    public void Configure(EntityTypeBuilder<Artikkel> builder)
    {
        builder.ToTable("Artikler");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.ArtikkelNr).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Navn).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Enhet).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Strekkode).HasMaxLength(100);
        builder.Property(x => x.Kategori).HasMaxLength(100);
        builder.Property(x => x.Innpris).HasColumnType("decimal(18,2)");
        builder.Property(x => x.Utpris).HasColumnType("decimal(18,2)");

        builder.HasIndex(x => x.ArtikkelNr).IsUnique();
    }
}
