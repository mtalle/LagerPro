using LagerPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LagerPro.Infrastructure.Persistence.Configurations;

public class ReseptConfiguration : IEntityTypeConfiguration<Resept>
{
    public void Configure(EntityTypeBuilder<Resept> builder)
    {
        builder.ToTable("Resepter");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Navn).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Beskrivelse).HasMaxLength(1000);
        builder.Property(x => x.Instruksjoner).HasMaxLength(4000);
        builder.Property(x => x.AntallPortjoner).HasColumnType("decimal(18,3)");

        builder.HasOne(x => x.Ferdigvare)
            .WithMany()
            .HasForeignKey(x => x.FerdigvareId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
