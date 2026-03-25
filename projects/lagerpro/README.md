# 🏭 LagerPro — Sporbarhetssystem for matprodusenter

## Konsept
Lagerstyring og sporbarhet fra råvare til kunde for små/mellomstore matprodusenter.
Full LOT-sporing gjennom hele verdikjeden.

## Flyt
1. **Mottak** — Råvarer inn (manuelt/strekkode). Vare, antall/vekt, leverandør, dato, LOT, avvik.
2. **Lager** — Organisert med artikkelnummer. Flere linjer per vare ved ulike LOT-nummer.
3. **Produksjon** — Reseptbasert. Råvarer plukkes fra lager → ferdigvare. Nytt LOT genereres, råvare-LOT kobles til ferdigvare-LOT. Plukkliste, ferdigmelding, overstyring mulig.
4. **Levering** — Ferdigvare til kunde. Fjernes fra lager. Full sporbarhet: kunde ↔ ferdigvare-LOT ↔ råvare-LOT.

## Hvorfor dette har verdi
- Sporbarhet er **lovpålagt** (Mattilsynet, HACCP)
- Mange småbedrifter bruker Excel/papir
- Store systemer (Visma, SAP) er for dyre/komplekse
- Martin har **førstehånds bransjeerfaring**

## Nåværende status
- Prototype bygget i Base44: https://lager-pro-6a1e4d2f.base44.app
- Funker som demo, men ikke salgbart (Base44-begrensninger)
- Strekkodeleser delvis implementert

## Plan
- [ ] Velge tech-stack for "skikkelig" versjon
- [ ] Definere MVP-scope
- [x] Sette opp prosjektstruktur
- [ ] Bygge backend (C#/.NET)
- [ ] Bygge frontend
- [ ] Teste med reelle data
- [ ] Pilotbruker

## Prosjektstruktur
Prosjektet er klargjort for en typisk .NET-løsning med lagdeling:

- `src/LagerPro.Domain` — kjerneobjekter, regler og enums
- `src/LagerPro.Application` — use cases / applikasjonslogikk
- `src/LagerPro.Infrastructure` — database og integrasjoner
- `src/LagerPro.Api` — API-lag
- `src/LagerPro.Contracts` — DTO-er og request/response-modeller
- `tests/` — tester
- `docs/` — arkitektur og domene-notater
- `LagerPro.sln` — solution-fil
- `Directory.Build.props` — felles .NET-settings

## SQL / database
Prosjektet er satt opp med tanke på SQL Server + Entity Framework Core:

- `LagerProDbContext` er opprettet
- `Microsoft.EntityFrameworkCore.SqlServer` er lagt inn i `Infrastructure`
- `appsettings.json` har `DefaultConnection`
- `Persistence/Configurations/` er utvidet for sentrale entiteter
- `DesignTimeDbContextFactory` er lagt inn for migreringer
- `Persistence/Migrations/` er opprettet for senere migreringer

## API / CRUD
Det finnes nå et første API-skjelett med CRUD-starter for artikler:

- `GET /api/articles`
- `GET /api/articles/{id}`
- `POST /api/articles`
- `PUT /api/articles/{id}`
- `DELETE /api/articles/{id}`
- `GET /api/traceability/lot/{lotNr}`

## Frontend
Det er også opprettet en enkel frontend-struktur i `frontend/` for senere UI-arbeid.

Se også:
- `docs/architecture/folder-structure.md`
- `docs/api/mvp-endpoints.md`
- `docs/domain/sql-plan.md`

---
Opprettet: 2026-03-24
