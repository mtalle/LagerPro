using LagerPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LagerPro.Infrastructure.Persistence.Configurations;

public class ProduksjonsOrdreConfiguration : IEntityTypeConfiguration<ProduksjonsOrdre>
{
    public void Configure(EntityTypeBuilder<ProduksjonsOrdre> builder)
    {
        builder.ToTable("ProduksjonsOrdre");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.OrdreNr).HasMaxLength(50).IsRequired();
        builder.Property(x => x.FerdigvareLotNr).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Kommentar).HasMaxLength(1000);
        builder.Property(x => x.UtfortAv).HasMaxLength(100);
        builder.Property(x => x.AntallProdusert).HasColumnType("decimal(18,3)");

        builder.HasIndex(x => x.OrdreNr).IsUnique();

        builder.HasOne(x => x.Resept)
            .WithMany(x => x.ProduksjonsOrdre)
            .HasForeignKey(x => x.ReseptId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
