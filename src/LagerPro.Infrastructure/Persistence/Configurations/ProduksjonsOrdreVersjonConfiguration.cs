using LagerPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LagerPro.Infrastructure.Persistence.Configurations;

public class ProduksjonsOrdreVersjonConfiguration : IEntityTypeConfiguration<ProduksjonsOrdreVersjon>
{
    public void Configure(EntityTypeBuilder<ProduksjonsOrdreVersjon> builder)
    {
        builder.ToTable("ProduksjonsOrdreVersjoner");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.VersjonsNummer).IsRequired();
        builder.Property(x => x.AntallProdusert).HasPrecision(18, 4);
        builder.Property(x => x.FerdigvareLotNr).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Kommentar).HasMaxLength(1000);
        builder.Property(x => x.UtfortAv).HasMaxLength(200);

        builder.HasOne(x => x.ProduksjonsOrdre)
            .WithMany()
            .HasForeignKey(x => x.ProduksjonsOrdreId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
