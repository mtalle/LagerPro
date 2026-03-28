using LagerPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LagerPro.Infrastructure.Persistence.Configurations;

public class LagerBeholdningConfiguration : IEntityTypeConfiguration<LagerBeholdning>
{
    public void Configure(EntityTypeBuilder<LagerBeholdning> builder)
    {
        builder.ToTable("LagerBeholdninger");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.LotNr).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Enhet).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Lokasjon).HasMaxLength(100);
        builder.Property(x => x.Mengde).HasColumnType("decimal(18,3)");

        builder.HasIndex(x => new { x.ArtikkelId, x.LotNr }).IsUnique();

        builder.HasOne(x => x.Artikkel)
            .WithMany(x => x.LagerBeholdninger)
            .HasForeignKey(x => x.ArtikkelId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
