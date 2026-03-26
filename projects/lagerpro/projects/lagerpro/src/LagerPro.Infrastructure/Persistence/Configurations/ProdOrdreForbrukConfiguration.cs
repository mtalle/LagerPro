using LagerPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LagerPro.Infrastructure.Persistence.Configurations;

public class ProdOrdreForbrukConfiguration : IEntityTypeConfiguration<ProdOrdreForbruk>
{
    public void Configure(EntityTypeBuilder<ProdOrdreForbruk> builder)
    {
        builder.ToTable("ProdOrdreForbruk");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.LotNr).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Enhet).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Kommentar).HasMaxLength(1000);
        builder.Property(x => x.MengdeBrukt).HasColumnType("decimal(18,3)");

        builder.HasOne(x => x.ProduksjonsOrdre)
            .WithMany(x => x.Forbruk)
            .HasForeignKey(x => x.ProdOrdreId);

        builder.HasOne(x => x.Artikkel)
            .WithMany(x => x.ProdOrdreForbruk)
            .HasForeignKey(x => x.ArtikkelId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
