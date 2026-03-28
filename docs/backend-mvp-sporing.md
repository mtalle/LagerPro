# LagerPro — Backend MVP: Sporbarheit & Kvalitetssikring

## Visjon
**For matindustrien** — sporbarheit, lagerstyring og produksjonsplanlegging i éin applikasjon.

**Kjerneverdi:** Live lager med full sporing frå råvare til ferdig produkt.

---

## Fase 1: Backend ferdig (no → 1 uke)

### 🔴 Kritisk: Sporbarheitskjeden
- [x] **Mottak** → råvare med lot-nr, vekt, temperatur, holdbarheit (→ LagerTransaksjon)
- [x] **Produksjon** → råvarer → ferdigvare (kvar batch = ny lot) (→ LagerTransaksjon)
- [x] **Levering** → ferdigvare til kunde med lot-nr (→ LagerTransaksjon)
- [x] **Traceability API** → gitt lot-nr → vis heile historikken (kvar varen har vore)
- [x] **Råvarerapport** → kva råvarer går i kvar ferdigvare-batch (via /batch endpoint)
- [x] **Kundesporing** → kva kunde fekk kvar batch (via /kunde endpoint)

### 🟠 Kritisk: Produksjonsflyt
- [ ] **Resepter** → Admin oppretter resept med ferdigvare + råvarer (artikkel + mengde)
- [ ] **Produksjonsordre** → Admin eller Produksjon oppretter ordre basert på resept
  - OrdreID autogenereres (PROD-YYYYMMDD-NNN)
  - System foreslår antal basert på resept
- [ ] **Plukkliste** → Kan skrivast ut (PDF) med: OrdreID, artikkelnamn, lot-nr, antal
- [ ] **Ferdigmelding** → Kan gjerast av Admin eller Produksjon
  - Automatisk fylt ut frå resept + gjeldande lager
  - **Kan endre:** varer, råvarer, lot-nr, antal ved ferdigmelding
  - Ved ferdigmelding → Lager oppdaterast automatisk
- [ ] **Ordreliste** → Alle produksjonsordrer i liste, klikk for å ferdigmelde

### Produksjonsflyt steg-for-steg
1. Opprette resept (Admin) → ferdigvare + råvarer (artikkel + mengde)
2. Opprette produksjonsordre → vel resept, PROD-YYYYMMDD-NNN, system foreslår antal
3. Plukkliste (valfri) → Artikkelnamn | Lot-nr | Antal | OrdreID synleg
4. Ferdigmelding → endre lot/antal, lager oppdaterast

---

### 🟡 Kritisk: Varelager / Artikklar

#### Hovudside (Artikkelliste)
- [ ] **Vis alle artikklar** → komplett liste med artikkelnavn, artikkelnr, kategori, lagerstatus
- [ ] **Filter** → på kategori, på lager, ikkje på lager
- [ ] **Søk** → på navn, artikkelnummer, lot-nummer

#### Artikkeldetaljer (klikk på artikkel)
- [ ] **Oversikt over alle lot-numre** + lagerstatus for kvar lot
- [ ] Viser ALLE lot, òg dei med 0 i lager
- [ ] Klikk på lot-nr → går vidare til sporingside for den lot

#### Varetelling
- [ ] **Start varetelling** → Send til alle brukarar eller bestemte brukarar
- [ ] **Telleliste** → Éin linje per lot-nummer per artikkel
  - Linje viser: lagertall (no) + ny status (ved telling)
  - Linjer med 0 i antal visast IKKJE, men kan leggjast til
  - Kan leggja til ekstra linje (ny vare eller vare som har 0)
- [ ] **Ferdigmelding av telling** → Lager justerast automatisk basert på telling
- [ ] **Avviksrapport** → kva vart telt vs kva som var i systemet

#### Manuell justering
- [ ] Velje ein eller fleire linjer
- [ ] Endre antall ELLER lot-nummer
- [ ] Med grunn/kommentar

#### Historikk-fane (på kvar artikkel)
- [ ] **Alle transaksjonar** lagra: varetelling, justering, mottak, levering, produksjon
- [ ] Kven gjorde kva + tidspunkt
- [ ] Éin samla historikk per artikkel

---

### 🟡 Viktig: Kvalitetssikring + Brukarrettar
- [x] **Input-validering** → alle requests sjekka for gyldige verdiar (i handlers)
- [x] **Feilhandsaming** → kva skjer ved ugyldig data? (logg + 400 Bad Request)
- [x] **Lager-konsistens** → sjekk at lager aldri går i minus ved trekking
- [x] **Transaksjonssikring** → atomiske operasjonar (alt eller inkje) — UnitOfWork m/ transactions
- [ ] **Brukarrettar (RBAC)** → admin vel kva kvar bruker ser

### 🟢 Bra å ha
- [ ] **Audit log** → kven oppretta/endrea kva (operatør, tidspunkt)
- [ ] **Batch-nummer autogenerering** → LOT-YYYYMMDD-NNN format
- [ ] **Min/max lageralarm** → flagg når beholdning < minimum

---

## Brukarroller & Mobil-oppleving

### Brukartypen
| Rolle | Tilgang | Grensesnitt |
|-------|--------|-------------|
| **Admin** | Alt + brukarstyring | Web (full) |
| **Varemottak** | Berre mottak | Mobil-vennleg (én knapp) |
| **Produksjon** | Berre produksjon | Mobil-vennleg |
| **Levering** | Berre levering | Mobil-vennleg |

### Admin sine oppgåver
- Opprette/redigere brukarar
- Velja kva kvar brukar har tilgang til
- Definere rollene

### De på gulvet (mobil)
- **KVART eit skjermbilde** — berre den funksjonen dei treng
- **Éin-handta** — store knappar, lite tekst
- **Offline-støtte** (framtidig) — kan registrere utan nett

### Endepunkt for rettar
| Metode | Endepunkt | Beskriving |
|--------|-----------|-------------|
| GET | `/api/brukere` | Alle brukarar |
| POST | `/api/brukere` | Opprett brukar |
| PUT | `/api/brukere/{id}/tilgang` | Oppdater tilgang |
| DELETE | `/api/brukere/{id}` | Deaktiver brukar |

---

## Teknisk oversyn

### Database-flyt
```
Råvare (Mottak) → Lager → Produksjon (Forbruk + Ferdigvare) → Lager → Levering (Kunde)
     ↑                                                               |
     └──────────────── Traceability (alle transaksjonar) ────────────┘
```

### Endepunkt for sporing
| Metode | Endepunkt | Beskriving |
|--------|-----------|-------------|
| GET | `/api/traceability/lot/{lotNr}` | Alle transaksjonar for ein lot |
| GET | `/api/traceability/artikkel/{artikkelId}` | Alle lot for ein artikkel |
| GET | `/api/traceability/batch/{batchNr}` | Produksjonsbatch med detaljar |
| GET | `/api/traceability/kunde/{kundeId}` | Alle leveringar til ein kunde |

### Transaksjonstyper (for matindustrien)
- `Mottak` — råvare motteke (med temperatur, holdbarheit)
- `ProduksjonInn` — ferdigvare produsert
- `ProduksjonUttak` — råvare brukt i produksjon
- `Levering` — ferdigvare sendt til kunde
- `Varetelling` — fysisk telling vs systemtall
- `Justering` — manuell justering (med grunn + autorisering)
- `Korrigering` — manuell justering (med grunn + autorisering)

---

## Test-leverandør: Krav til testmiljø
- SQL Server (Docker eller LocalDB)
- Apiet på http://localhost:5000
- Frontend på http://localhost:5001

---

## Filorganisering
```
GitHub (kode):      github.com/mtalle/LagerPro
OneDrive (dokument): ./onedrive-export/
```

**I GitHub berre kode og nødvendig dokumentasjon.**
**I OneDrive: SQL-plan, Brukarhistorier, arkitekturteikningar, presentasjonar.**

---

## Sub-agent instruks
1. Følg denne plana strict
2. Kvar phase skal ha unit test før push
3. Commit kvar 30. min
4. Bygg + test skal alltid passere før push
5. Oppdater denne fila etter kvart som noko vert ferdig
