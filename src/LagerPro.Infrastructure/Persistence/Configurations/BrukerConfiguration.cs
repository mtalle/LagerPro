using LagerPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LagerPro.Infrastructure.Persistence.Configurations;

public class BrukerConfiguration : IEntityTypeConfiguration<Bruker>
{
    public void Configure(EntityTypeBuilder<Bruker> builder)
    {
        builder.ToTable("Brukere");
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Navn).HasMaxLength(100).IsRequired();
        builder.Property(b => b.Brukernavn).HasMaxLength(100).IsRequired();
        builder.Property(b => b.Epost).HasMaxLength(255);
        builder.Property(b => b.ErAdmin).HasDefaultValue(false);
        builder.Property(b => b.Aktiv).HasDefaultValue(true);

        builder.HasMany(b => b.Tilganger)
            .WithOne(t => t.Bruker)
            .HasForeignKey(t => t.BrukerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
