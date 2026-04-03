# MEMORY.md

## People
- Martin (dumelidum) is the human I help.
- Martin speaks Norwegian and prefers Norwegian unless he switches language.
- Martin lives in Ålfoten, Bremanger and works in Svelgen, Bremanger.
- Martin's timezone is Europe/Oslo.

## LagerPro Focus Areas
- Sub-agents → Frontend (Blazor/Razor Pages)
- Steve (main agent) → Unit testing, code review, backend
- All sub-agents must commit every 30 min, build+test must pass

## LagerPro Tech Stack
- .NET 8, Clean Architecture, EF Core + SQL Server
- Backend: src/LagerPro.Api, Application, Domain, Infrastructure, Contracts
- Frontend: frontend/ (Next.js, started by sub-agent)
- Tests: tests/ (xUnit + Moq)

## LagerPro Status (2026-03-29)
- **Backend build:** ✅ (0 errors, 0 warnings)
- **Tester:** 117 grønne ✅ (opp fra 112)
- **GlobalExceptionHandlerMiddleware:** Fanger alle unntak, returnerer konsistent JSON
- **CORS:** Konfigurerbar via appsettings.json `Cors:AllowedOrigins`
- **Alle controller-feilmeldinger:** Norsk (ble ikke funnet, ikke "not found")
- **Health endpoint:** Returnerer timestamp i tillegg til status
- **Frontend:** Next.js, 12 routes, bygger OK
- **Migrations:** `20260329092643_InitialCreate` (SQL Server-kompatibel)
- **MVP: Funksjonelt komplett!** Kjerner: Artikler, Mottak, Lager, Produksjon, Levering, Resepter, Sporing

### Nye features (2026-03-29)
- Manuell lagerjustering via `POST /inventory/juster` med transaksjonslogg
- `DeleteMottak` + `DeleteLevering` endepunkt
- `FerdigmeldPrefill` + `FerdigmeldLinje` typer i frontend
- DeleteMottak/DeleteLevering handlers + repo-metoder

### Oppdatert 2026-03-29
- Alle controller-feilmeldinger → norsk
- GlobalExceptionHandlerMiddleware lagt til
- CORS nå konfigurerbar via appsettings.json
- Helsesjekk `/health` → viser timestamp

## LagerPro Tech Stack
- Repository interface mismatch (ILager, IMottak, ILevering, IProduksjonsOrdre, IArtikkel)
- Lagt til manglende repository-metoder i interfaces og implementasjoner
- Lagt til IUnitOfWork i DI
- Lagt til alle repositories i DI (ikke bare IArtikkelRepository)
- Kallet AddApplication() i Program.cs (manglede)
- InventoryController: stub → ekte implementasjon med GetAllLagerBeholdningHandler
- ReceiptsController: stub → ekte implementasjon
- ShippingController: stub → ekte implementasjon (CreateLevering med riktige feltnavn)
- LeveringDto: lagt til Linjer-felt + Kunde/Artikkel includes i repository
- GetAllLagerBeholdningHandler: fikset nullable warning
- DeleteArticleHandler: lagt til await
- Articles.razor: async-task uten await → void
- Current main project: LagerPro.
- I am the main programmer responsible for LagerPro.
- I take ownership of all programming for the app unless Martin કહે otherwise.
- Goal: get an MVP ready as fast as possible.
- Priority now: LagerPro development.
- I have broad autonomy on the app until Martin says otherwise.

## Operating preferences
- Be direct, practical, and proactive.
- Take ownership of tasks and suggest concrete next steps.
- Include useful reminders, deadlines, and prioritization when giving briefings.

## Morning brief template
- Project status: what moved since last time, what is blocked, and the next milestone.
- Today’s top priority: the single most important task for LagerPro.
- Technical focus: what I should work on in code/config/docs right now.
- Risks/blockers: anything that could delay the MVP.
- Human context: travel, work, and weather considerations for Ålfoten/Svelgen.
- Suggested deadline: one near-term deadline for the next concrete deliverable.

## 2026-03-28 - Aktiv utviklingsdag

### LagerPro framgang:
- Backend: 82 tester, 0 errors ✅
- Frontend: Blazor + Next.js, 10+ sider
- Sporbarheits-API: 4 endepunkt (lot, artikkel, batch, kunde)
- Produksjonsflyt: Resepter, Ordre, Plukkliste, Ferdigmelding
- Varelager/Varetelling: Filter, Søk, Manuell justering
- Brukarroller: Admin, Varemottak, Produksjon, Levering

### Martin sin visjon:
- Målgruppe: Matindustrien
- Kjernefunksjon: Live lager + full sporing
- Brukarroller med RBAC
- Sentral Sporingsmodul (portal for all sporing)

### Infrastruktur:
- Docker SQL Server på port 14333
- Connection string: Server=localhost,14333;Database=LagerProDb;User Id=sa;Password=LagerPro123!
- Sub-agent cron: kvar time via OpenClaw
- Deploy: DigitalOcean $8/mnd planlagt

### Produksjons-server (DigitalOcean)
- **IP:** 167.99.195.94
- **Brukar:** root
- **Passord:** TalleraaS123m
- **Frontend:** /root/LagerPro/frontend (npm run dev -- -p 3000)
- **Backend:** /root/LagerPro (dotnet run --urls "http://0.0.0.0:5000")
- **Ved opplasting:** Kopier filer direkte med scp/ssh, restart deretter

### Martin sin hardware:
- Lenovo laptop (Ryzen 5 7520U, 16GB RAM, Radeon 610M)
- Windows 11

## 2026-03-29 — LagerPro MVP arbeidsøkt

### Hva ble gjort
- **Backend**: Manifold DI-registreringer manglet i Application/DependencyInjection (GetLagerBeholdningByArtikkelHandler, GetLagerBeholdningByLotNrHandler, JusterLagerHandler, UpdateMottakLinjeGodkjenningHandler, GetFerdigmeldPrefillHandler, CreateMottakHandler). Alle er nå registrert.
- **Ny feature**: Manuell lagerjustering via `POST /inventory/juster` med transaksjonslogg
- **Test-fiks**: `DeleteArticleHandler` gjør soft delete (Aktiv=false), testen var feil
- **3 nye tester** for JusterLagerHandler (85 tester totalt, alle grønne)
- **Frontend**: Juster-knapp + modal på lager-siden

### Git-historikk
- `579e83c` — test-fiks + nye tester + DI
- `1ffe2ff` — manuell lagerjustering backend + frontend (committed under tidligere jobb)
- Begge pushes til origin/main ✅

### Kjent stand
- Alle 85 tester grønne
- Backend bygger med 0 warnings, 0 errors
- Frontend bygger OK (Next.js)
