# LagerPro - Sporing (Traceability)

## Kva er det?
Ei sporeside der du kan følgje råvarer og produkt frå vareinntak til vareutlevering basert på lot-nummer.

## Flyt
1. Brukar skriv inn lot-nummer (eller vel frå liste)
2. Systemet viser kvar lotten er i systemet:
   - **Inntak** (Mottak → Lager)
   - **Produksjon** (Lager → ProduksjonsOrdre → ny lot)
   - **Utlevering** (Lager → Levering)

## TODO

### Backend
- [ ] `GET /api/traceability/lot/{lotNr}` — allereie eksisterer
- [ ] `GET /api/traceability/artikkel/{artikkelId}` — viser alle lotter for ein artikkel
- [ ] `GET /api/traceability/ordre/{ordreId}` — viser produksjonsflyt for ein ordre
- [ ] Registrer manglande handlers i DI

### Frontend
- [ ] `/sporing` — ny side med lot-nummer søk
- [ ] Visning av noverande beholdning + lokasjon
- [ ] Tidslinje/tabell med alle transaksjonar
- [ ] Knytt "Spor"-knapp til artikler/lager-sider

## Teknisk
- Frontend: `frontend/src/app/sporing/page.tsx`
- Backend: `src/LagerPro.Api/Controllers/TraceabilityController.cs`
- DTOs: `src/LagerPro.Contracts/Dtos/Traceability/`
