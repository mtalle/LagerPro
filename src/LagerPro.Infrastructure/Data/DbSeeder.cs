using LagerPro.Domain.Entities;
using LagerPro.Domain.Enums;
using LagerPro.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LagerPro.Infrastructure.Data;

/// <summary>
/// Seeds the database with sample data for development/MVP.
/// Idempotent — safe to run multiple times.
/// </summary>
public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<LagerProDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<LagerProDbContext>>();

        // Ensure database is created (runs migrations if any pending)
        try
        {
            await context.Database.MigrateAsync();
            logger.LogInformation("Database migration applied successfully");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Migration failed, attempting EnsureCreated");
            await context.Database.EnsureCreatedAsync();
        }

        if (await context.Artikler.AnyAsync())
        {
            logger.LogInformation("Database already seeded, skipping");
            return;
        }

        logger.LogInformation("Seeding database with sample data...");

        var now = DateTime.UtcNow;

        // --- Leverandører ---
        var lev1 = new Leverandor { Navn = "Norsk Fisk AS", Kontaktperson = "Ola Nordmann", Telefon = "22334455", Epost = "ola@norskfisk.no", Adresse = "Fiskergata 1", Postnr = "5000", Poststed = "Bergen", OrgNr = "912345678", Aktiv = true, OpprettetDato = now };
        var lev2 = new Leverandor { Navn = "Landbruksvarer DA", Kontaktperson = "Kari Brå", Telefon = "98765432", Epost = "kari@landbruk.no", Adresse = "Gårdsveien 5", Postnr = "6800", Poststed = "Førde", OrgNr = "923456789", Aktiv = true, OpprettetDato = now };
        var lev3 = new Leverandor { Navn = "Pakke & Co", Kontaktperson = "Per Emballasje", Telefon = "55443322", Epost = "per@pakke.no", Adresse = "Industriveien 10", Postnr = "6600", Poststed = "Sunnfjord", OrgNr = "934567890", Aktiv = true, OpprettetDato = now };

        context.Leverandorer.AddRange(lev1, lev2, lev3);
        await context.SaveChangesAsync();

        // --- Artikler ---
        var torskefilet = new Artikkel { ArtikkelNr = "RAV-001", Navn = "Torskefilet", Enhet = "KG", Type = ArtikelType.Ravare, Kategori = "Fisk", Innpris = 89.90m, Utpris = 149.90m, MinBeholdning = 20, Aktiv = true, OpprettetDato = now };
        var laksfilet = new Artikkel { ArtikkelNr = "RAV-002", Navn = "Laksfilet", Enhet = "KG", Type = ArtikelType.Ravare, Kategori = "Fisk", Innpris = 129.90m, Utpris = 219.90m, MinBeholdning = 15, Aktiv = true, OpprettetDato = now };
        var poteter = new Artikkel { ArtikkelNr = "RAV-003", Navn = "Poteter", Enhet = "KG", Type = ArtikelType.Ravare, Kategori = "Grønnsaker", Innpris = 12.00m, Utpris = 24.90m, MinBeholdning = 50, Aktiv = true, OpprettetDato = now };
        var smør = new Artikkel { ArtikkelNr = "RAV-004", Navn = "Smør", Enhet = "KG", Type = ArtikelType.Ravare, Kategori = "Meieriprodukter", Innpris = 45.00m, Utpris = 79.90m, MinBeholdning = 10, Aktiv = true, OpprettetDato = now };
        var hvetemel = new Artikkel { ArtikkelNr = "RAV-005", Navn = "Hvetemel", Enhet = "KG", Type = ArtikelType.Ravare, Kategori = "Tørre varer", Innpris = 8.50m, Utpris = 18.90m, MinBeholdning = 30, Aktiv = true, OpprettetDato = now };
        var salt = new Artikkel { ArtikkelNr = "RAV-006", Navn = "Salt", Enhet = "KG", Type = ArtikelType.Ravare, Kategori = "Krydder", Innpris = 5.00m, Utpris = 15.90m, MinBeholdning = 10, Aktiv = true, OpprettetDato = now };
        var olivenolje = new Artikkel { ArtikkelNr = "RAV-007", Navn = "Olivenolje", Enhet = "L", Type = ArtikelType.Ravare, Kategori = "Oljer", Innpris = 65.00m, Utpris = 119.90m, MinBeholdning = 10, Aktiv = true, OpprettetDato = now };
        var løk = new Artikkel { ArtikkelNr = "RAV-008", Navn = "Løk", Enhet = "KG", Type = ArtikelType.Ravare, Kategori = "Grønnsaker", Innpris = 8.00m, Utpris = 19.90m, MinBeholdning = 25, Aktiv = true, OpprettetDato = now };
        var emb01 = new Artikkel { ArtikkelNr = "EMB-001", Navn = "Fiskekasse (stor)", Enhet = "STK", Type = ArtikelType.Emballasje, Kategori = "Emballasje", Innpris = 25.00m, Utpris = 45.00m, MinBeholdning = 50, Aktiv = true, OpprettetDato = now };
        var emb02 = new Artikkel { ArtikkelNr = "EMB-002", Navn = "Etikett fiskeprodukt", Enhet = "STK", Type = ArtikelType.Emballasje, Kategori = "Emballasje", Innpris = 1.50m, Utpris = 5.00m, MinBeholdning = 200, Aktiv = true, OpprettetDato = now };
        var torskekaker = new Artikkel { ArtikkelNr = "FER-001", Navn = "Torskekaker", Enhet = "KG", Type = ArtikelType.Ferdigvare, Kategori = "Fiskeprodukter", Innpris = 120.00m, Utpris = 199.90m, MinBeholdning = 10, Aktiv = true, OpprettetDato = now };
        var fiskesuppe = new Artikkel { ArtikkelNr = "FER-002", Navn = "Fiskesuppe", Enhet = "L", Type = ArtikelType.Ferdigvare, Kategori = "Fiskeprodukter", Innpris = 75.00m, Utpris = 139.90m, MinBeholdning = 15, Aktiv = true, OpprettetDato = now };

        context.Artikler.AddRange(torskefilet, laksfilet, poteter, smør, hvetemel, salt, olivenolje, løk, emb01, emb02, torskekaker, fiskesuppe);
        await context.SaveChangesAsync();

        // Reload to get generated IDs
        var torskefiletDb = await context.Artikler.FirstAsync(a => a.ArtikkelNr == "RAV-001");
        var laksfiletDb = await context.Artikler.FirstAsync(a => a.ArtikkelNr == "RAV-002");
        var smørDb = await context.Artikler.FirstAsync(a => a.ArtikkelNr == "RAV-004");
        var hvetemelDb = await context.Artikler.FirstAsync(a => a.ArtikkelNr == "RAV-005");
        var saltDb = await context.Artikler.FirstAsync(a => a.ArtikkelNr == "RAV-006");
        var torskekakerDb = await context.Artikler.FirstAsync(a => a.ArtikkelNr == "FER-001");
        var fiskesuppeDb = await context.Artikler.FirstAsync(a => a.ArtikkelNr == "FER-002");

        // --- Kunder ---
        var kun1 = new Kunde { Navn = "Spar Svelgen", Kontaktperson = "Leder", Telefon = "11223344", Epost = "spar@svelgen.no", Adresse = "Storgata 1", Postnr = "6713", Poststed = "Svelgen", OrgNr = "912345678", Aktiv = true, OpprettetDato = now };
        var kun2 = new Kunde { Navn = "Coop Nordfjord", Kontaktperson = "Innkjøpsansvarlig", Telefon = "22334455", Epost = "nordfjord@coop.no", Adresse = "Sentrum 5", Postnr = "6770", Poststed = "Nordfjord", OrgNr = "923456789", Aktiv = true, OpprettetDato = now };
        var kun3 = new Kunde { Navn = "Restaurant Fjord", Kontaktperson = "Kokk", Telefon = "33445566", Epost = "info@fjord.no", Adresse = "Havnegata 2", Postnr = "6700", Poststed = "Florø", Aktiv = true, OpprettetDato = now };
        context.Kunder.AddRange(kun1, kun2, kun3);
        await context.SaveChangesAsync();

        // --- Resepter ---
        // First create empty resept, then add lines (navigation via FK)
        var resept1 = new Resept
        {
            Navn = "Torskekaker",
            Beskrivelse = "Klassiske torskekaker etter norsk oppskrift",
            FerdigvareId = torskekakerDb.Id,
            AntallPortjoner = 10,
            Aktiv = true,
            OpprettetDato = now
        };
        var resept2 = new Resept
        {
            Navn = "Fiskesuppe",
            Beskrivelse = "Kremet fiskesuppe med laks og torsk",
            FerdigvareId = fiskesuppeDb.Id,
            AntallPortjoner = 8,
            Aktiv = true,
            OpprettetDato = now
        };
        context.Resepter.AddRange(resept1, resept2);
        await context.SaveChangesAsync();

        // Reload resept for IDs
        var resept1Db = await context.Resepter.FirstAsync(r => r.Navn == "Torskekaker");
        var resept2Db = await context.Resepter.FirstAsync(r => r.Navn == "Fiskesuppe");

        // ReseptLinjer (ingredient → RavareId)
        var linjer1 = new[]
        {
            new ReseptLinje { ReseptId = resept1Db.Id, RavareId = torskefiletDb.Id, Mengde = 0.8m, Enhet = "KG", Rekkefolge = 1 },
            new ReseptLinje { ReseptId = resept1Db.Id, RavareId = smørDb.Id, Mengde = 0.1m, Enhet = "KG", Rekkefolge = 2 },
            new ReseptLinje { ReseptId = resept1Db.Id, RavareId = hvetemelDb.Id, Mengde = 0.1m, Enhet = "KG", Rekkefolge = 3 },
            new ReseptLinje { ReseptId = resept1Db.Id, RavareId = saltDb.Id, Mengde = 0.01m, Enhet = "KG", Rekkefolge = 4 },
        };
        var linjer2 = new[]
        {
            new ReseptLinje { ReseptId = resept2Db.Id, RavareId = laksfiletDb.Id, Mengde = 0.3m, Enhet = "KG", Rekkefolge = 1 },
            new ReseptLinje { ReseptId = resept2Db.Id, RavareId = torskefiletDb.Id, Mengde = 0.4m, Enhet = "KG", Rekkefolge = 2 },
            new ReseptLinje { ReseptId = resept2Db.Id, RavareId = smørDb.Id, Mengde = 0.1m, Enhet = "KG", Rekkefolge = 3 },
            new ReseptLinje { ReseptId = resept2Db.Id, RavareId = hvetemelDb.Id, Mengde = 0.05m, Enhet = "KG", Rekkefolge = 4 },
        };
        context.ReseptLinjer.AddRange(linjer1.Concat(linjer2));
        await context.SaveChangesAsync();

        // --- Lagerbeholdning (sample stock) ---
        var beholdninger = new List<LagerBeholdning>
        {
            new() { ArtikkelId = torskefiletDb.Id, LotNr = "TF-2026-001", Mengde = 45m, Enhet = "KG", SistOppdatert = now },
            new() { ArtikkelId = laksfiletDb.Id, LotNr = "LAK-2026-001", Mengde = 30m, Enhet = "KG", SistOppdatert = now },
            new() { ArtikkelId = smørDb.Id, LotNr = "SM-2026-001", Mengde = 20m, Enhet = "KG", SistOppdatert = now },
            new() { ArtikkelId = torskefiletDb.Id, LotNr = "TF-2026-002", Mengde = 12m, Enhet = "KG", SistOppdatert = now },
        };
        context.LagerBeholdninger.AddRange(beholdninger);
        await context.SaveChangesAsync();

        // --- Ressurser ---
        var ressurser = new List<Ressurs>
        {
            new() { Id = 1, Navn = "Mottak", Beskrivelse = "Motta varer fra leverandører", OpprettetDato = now },
            new() { Id = 2, Navn = "Artikler", Beskrivelse = "Administrer artikler", OpprettetDato = now },
            new() { Id = 3, Navn = "Lager", Beskrivelse = "Lagerbeholdning og transaksjoner", OpprettetDato = now },
            new() { Id = 4, Navn = "Produksjon", Beskrivelse = "Produksjonsordrer", OpprettetDato = now },
            new() { Id = 5, Navn = "Levering", Beskrivelse = "Leveringer til kunder", OpprettetDato = now },
            new() { Id = 6, Navn = "Resepter", Beskrivelse = "Produksjonsresepter", OpprettetDato = now },
            new() { Id = 7, Navn = "Sporing", Beskrivelse = "Sporing av varer", OpprettetDato = now },
            new() { Id = 8, Navn = "Kunder", Beskrivelse = "Kundeadministrasjon", OpprettetDato = now },
            new() { Id = 9, Navn = "Leverandører", Beskrivelse = "Leverandøradministrasjon", OpprettetDato = now },
        };
        context.Ressurser.AddRange(ressurser);
        await context.SaveChangesAsync();

        // --- Admin bruker (alle tilganger) ---
        var admin = new Bruker
        {
            Navn = "Administrator",
            Brukernavn = "admin",
            Epost = "admin@lagerpro.no",
            ErAdmin = true,
            Aktiv = true,
            OpprettetDato = now
        };
        context.Brukere.Add(admin);
        await context.SaveChangesAsync();

        var alleTilganger = ressurser.Select(r => new BrukerRessursTilgang
        {
            BrukerId = admin.Id,
            RessursId = r.Id,
            OpprettetDato = now
        });
        context.BrukerRessursTilganger.AddRange(alleTilganger);
        await context.SaveChangesAsync();

        logger.LogInformation(
            "Database seeded: {Artikler} articles, {Lev} suppliers, {Kunder} customers, {Resepter} recipes, {Lager} stock items, {Ressurser} resources, {Admin} admin user",
            12, 3, 3, 2, beholdninger.Count, ressurser.Count, 1);
    }
}
