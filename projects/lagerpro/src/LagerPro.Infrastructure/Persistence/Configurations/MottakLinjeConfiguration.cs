using LagerPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LagerPro.Infrastructure.Persistence.Configurations;

public class MottakLinjeConfiguration : IEntityTypeConfiguration<MottakLinje>
{
    public void Configure(EntityTypeBuilder<MottakLinje> builder)
    {
        builder.ToTable("MottakLinjer");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.LotNr).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Enhet).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Strekkode).HasMaxLength(100);
        builder.Property(x => x.Avvik).HasMaxLength(1000);
        builder.Property(x => x.Kommentar).HasMaxLength(1000);
        builder.Property(x => x.Mengde).HasColumnType("decimal(18,3)");
        builder.Property(x => x.Temperatur).HasColumnType("decimal(5,2)");

        builder.HasOne(x => x.Mottak)
            .WithMany(x => x.Linjer)
            .HasForeignKey(x => x.MottakId);

        builder.HasOne(x => x.Artikkel)
            .WithMany(x => x.MottakLinjer)
            .HasForeignKey(x => x.ArtikkelId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
