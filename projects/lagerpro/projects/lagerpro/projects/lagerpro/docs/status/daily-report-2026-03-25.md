# LagerPro – Daily report

Date: 2026-03-25

## Ferdig
- .NET 8 er installert på VPS-en.
- GitHub-repoet `mtalle/LagerPro` er satt opp med SSH deploy key.
- Prosjektet er pushet til `main`.
- Solutionen bygger grønt igjen.
- Prosjektstruktur er på plass for:
  - Domain
  - Application
  - Infrastructure
  - Api
  - Contracts
  - tests
  - frontend
- SQL/EF Core-oppsettet er klargjort:
  - `LagerProDbContext`
  - entity configurations
  - `DesignTimeDbContextFactory`
  - `UnitOfWork`
- Første artikkel-flyt er koblet til application-laget.
- Daglig rapportjobb er satt opp for kl. 20:00 Oslo.

## Gjenstår
- Koble artikkel-modulen helt til ekte database-CRUD i full path.
- Lage første EF Core-migrering.
- Koble SQL Server/Database fullt opp i runtime.
- Fortsette med modulene:
  - Mottak
  - Lager
  - Produksjon
  - Levering
  - Sporbarhet
- Rydde naming konsekvent (Artikkel/Artikel).
- Lage ekte frontend og API-konsumpsjon.

## Mulige utfordringer
- Database-konfigurasjon og migreringer må verifiseres før videre arbeid.
- Noe naming er fortsatt litt blandet, som kan gi små opprydningsoppgaver.
- Discord-levering fra cron-jobben trenger fortsatt stabilisering for vedlegg/melding.

## Neste steg
1. Fullføre databaseknytning for artikkel-modulen.
2. Lage initial migrering.
3. Starte på Mottak/Lager.
4. Standardisere navngiving.
5. Bygge frontend-skjelett videre.

## Kommentar
Fundamentet er på plass, og backend er i gang.
