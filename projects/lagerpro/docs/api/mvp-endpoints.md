# LagerPro — MVP-endepunkter

## Foreslåtte første API-ruter

- `GET /health`
- `GET /api/articles`
- `POST /api/articles`
- `GET /api/receipts`
- `POST /api/receipts`
- `GET /api/inventory`
- `GET /api/production`
- `POST /api/production`
- `GET /api/shipping`
- `POST /api/shipping`

## SQL / persistence

Prosjektet er klargjort for SQL Server via Entity Framework Core:

- `LagerProDbContext`
- `UseSqlServer(...)`
- `ConnectionStrings:DefaultConnection` i `appsettings.json`
- `Persistence/Configurations/` for entity mapping
- `Persistence/Migrations/` er opprettet for senere migreringer
