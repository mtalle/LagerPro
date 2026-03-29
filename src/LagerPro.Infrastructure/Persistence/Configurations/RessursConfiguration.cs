using LagerPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LagerPro.Infrastructure.Persistence.Configurations;

public class RessursConfiguration : IEntityTypeConfiguration<Ressurs>
{
    public void Configure(EntityTypeBuilder<Ressurs> builder)
    {
        builder.ToTable("Ressurser");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Navn).HasMaxLength(100).IsRequired();
        builder.Property(r => r.Beskrivelse).HasMaxLength(255).IsRequired();
    }
}
