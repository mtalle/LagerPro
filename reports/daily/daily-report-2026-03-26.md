# LagerPro – Daily report

Date: 2026-03-26

## Ferdig
- .NET 8 er installert på VPS-en.
- GitHub-repoet `mtalle/LagerPro` er satt opp med SSH deploy key.
- Prosjektet er pushet til `main`.
- Solutionen bygger grønt igjen.
- Prosjektstruktur er på plass for Domain, Application, Infrastructure, Api, Contracts, tests og frontend.
- SQL/EF Core-oppsettet er klargjort: DbContext, entity configurations, DesignTimeDbContextFactory og UnitOfWork.
- Første artikkel-flyt er koblet til application-laget.
- Daglig rapportjobb er satt opp og feilen er identifisert.

## Gjenstår
- Koble artikkel-modulen helt til ekte database-CRUD i full path.
- Lage første EF Core-migrering.
- Koble SQL Server/Database fullt opp i runtime.
- Fortsette med modulene Mottak, Lager, Produksjon, Levering og Sporbarhet.
- Rydde naming konsekvent (Artikkel/Artikel).
- Lage ekte frontend og API-konsumpsjon.

## Mulige utfordringer
- Database-konfigurasjon og migreringer må verifiseres før videre arbeid.
- Noe naming er fortsatt litt blandet.
- Cron-jobbens levering måtte endres bort fra Discord for å unngå problemer.

## Neste steg
1. Fullføre databaseknytning for artikkel-modulen.
2. Lage initial migrering.
3. Starte på Mottak/Lager.
4. Standardisere navngiving.
5. Bygge frontend-skjelett videre.

## Kommentar
Fundamentet er på plass, og backend er i gang. Rapporten skal nå gå til OneDrive i stedet for Discord.
