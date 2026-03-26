using LagerPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LagerPro.Infrastructure.Persistence.Configurations;

public class KundeConfiguration : IEntityTypeConfiguration<Kunde>
{
    public void Configure(EntityTypeBuilder<Kunde> builder)
    {
        builder.ToTable("Kunder");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Navn).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Kontaktperson).HasMaxLength(200);
        builder.Property(x => x.Telefon).HasMaxLength(50);
        builder.Property(x => x.Epost).HasMaxLength(200);
        builder.Property(x => x.Adresse).HasMaxLength(300);
        builder.Property(x => x.Postnr).HasMaxLength(20);
        builder.Property(x => x.Poststed).HasMaxLength(100);
        builder.Property(x => x.OrgNr).HasMaxLength(50);
        builder.Property(x => x.Kommentar).HasMaxLength(1000);
    }
}
