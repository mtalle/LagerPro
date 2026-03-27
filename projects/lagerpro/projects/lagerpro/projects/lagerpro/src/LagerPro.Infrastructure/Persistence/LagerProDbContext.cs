using LagerPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LagerPro.Infrastructure.Persistence;

public class LagerProDbContext : DbContext
{
    public LagerProDbContext(DbContextOptions<LagerProDbContext> options) : base(options)
    {
    }

    public DbSet<Leverandor> Leverandorer => Set<Leverandor>();
    public DbSet<Artikkel> Artikler => Set<Artikkel>();
    public DbSet<Kunde> Kunder => Set<Kunde>();
    public DbSet<Mottak> Mottak => Set<Mottak>();
    public DbSet<MottakLinje> MottakLinjer => Set<MottakLinje>();
    public DbSet<LagerBeholdning> LagerBeholdninger => Set<LagerBeholdning>();
    public DbSet<LagerTransaksjon> LagerTransaksjoner => Set<LagerTransaksjon>();
    public DbSet<Resept> Resepter => Set<Resept>();
    public DbSet<ReseptLinje> ReseptLinjer => Set<ReseptLinje>();
    public DbSet<ProduksjonsOrdre> ProduksjonsOrdre => Set<ProduksjonsOrdre>();
    public DbSet<ProdOrdreForbruk> ProdOrdreForbruk => Set<ProdOrdreForbruk>();
    public DbSet<Levering> Leveringer => Set<Levering>();
    public DbSet<LeveringLinje> LeveringLinjer => Set<LeveringLinje>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LagerProDbContext).Assembly);
    }
}
