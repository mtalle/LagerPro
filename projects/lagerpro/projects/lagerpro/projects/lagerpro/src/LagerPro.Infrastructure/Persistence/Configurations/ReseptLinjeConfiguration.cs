using LagerPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LagerPro.Infrastructure.Persistence.Configurations;

public class ReseptLinjeConfiguration : IEntityTypeConfiguration<ReseptLinje>
{
    public void Configure(EntityTypeBuilder<ReseptLinje> builder)
    {
        builder.ToTable("ReseptLinjer");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Enhet).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Kommentar).HasMaxLength(1000);
        builder.Property(x => x.Mengde).HasColumnType("decimal(18,3)");

        builder.HasOne(x => x.Resept)
            .WithMany(x => x.Linjer)
            .HasForeignKey(x => x.ReseptId);

        builder.HasOne(x => x.Ravare)
            .WithMany(x => x.ReseptLinjer)
            .HasForeignKey(x => x.RavareId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
