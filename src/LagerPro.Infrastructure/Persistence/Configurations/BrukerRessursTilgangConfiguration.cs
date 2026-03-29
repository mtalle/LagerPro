using LagerPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LagerPro.Infrastructure.Persistence.Configurations;

public class BrukerRessursTilgangConfiguration : IEntityTypeConfiguration<BrukerRessursTilgang>
{
    public void Configure(EntityTypeBuilder<BrukerRessursTilgang> builder)
    {
        builder.ToTable("BrukerRessursTilganger");
        builder.HasKey(t => t.Id);

        builder.HasIndex(t => new { t.BrukerId, t.RessursId }).IsUnique();

        builder.HasOne(t => t.Bruker)
            .WithMany(b => b.Tilganger)
            .HasForeignKey(t => t.BrukerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.Ressurs)
            .WithMany(r => r.BrukerTilganger)
            .HasForeignKey(t => t.RessursId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
