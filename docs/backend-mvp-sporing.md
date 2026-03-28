# LagerPro — Backend MVP: Sporbarheit & Kvalitetssikring

## Visjon
**For matindustrien** — sporbarheit, lagerstyring og produksjonsplanlegging i éin applikasjon.

**Kjerneverdi:** Live lager med full sporing frå råvare til ferdig produkt.

---

## 🔴 Kritisk: Sentral Sporingsmodul

**Éin portal for all sporing** — kan nås frå kvar som helst i appen.

### Kva du kan søkja etter
- [ ] **Lot-nummer** → full historikk for ein lot
- [ ] **Artikkelnummer** → alle lot for ein artikkel
- [ ] **Produksjonsordre / Batch-nummer** → produksjonsdetaljar + råvarer
- [ ] **Kundenamn** → alle leveringar til ein kunde
- [ ] **Dato-periode** → alle transaksjonar i ein periode

### Kva du får ut
- [ ] **Heile historikken** til varen
- [ ] **Alle transaksjonar** knytt til lot/artikkel/batch/kunde
- [ ] **Kven, kva, når, kvifor** — komplett trail

### Tilgjengeleg frå
- [ ] Artikkelliste → klikk på lot-nr
- [ ] Produksjonsordre → klikk på batch
- [ ] Levering → klikk på kunde
- [ ] Kunde → vis alle leveringar
- [ ] Varetelling → vis avvik

### Teknisk
- **Éin sannheitskjelde** — LagerTransaksjon tabellen
- Alle andre modulane SKAL logga til LagerTransaksjon
- Lenkje til kjelda (MottakId, ProduksjonsOrdreId, LeveringId, etc.)

---

## 🟠 Kritisk: Produksjonsflyt
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

## 🟡 Kritisk: Varelager / Artikklar

### Hovudside (Artikkelliste)
- [ ] **Vis alle artikklar** → komplett liste med artikkelnavn, artikkelnr, kategori, lagerstatus
- [ ] **Filter** → på kategori, på lager, ikkje på lager
- [ ] **Søk** → på navn, artikkelnummer, lot-nummer

### Artikkeldetaljer (klikk på artikkel)
- [ ] **Oversikt over alle lot-numre** + lagerstatus for kvar lot
- [ ] Viser ALLE lot, òg dei med 0 i lager
- [ ] Klikk på lot-nr → går vidare til Sporingsmodulen for den lot

### Varetelling
- [ ] **Start varetelling** → Send til alle brukarar eller bestemte brukarar
- [ ] **Telleliste** → Éin linje per lot-nummer per artikkel
  - Linje viser: lagertall (no) + ny status (ved telling)
  - Linjer med 0 i antal visast IKKJE, men kan leggjast til
  - Kan leggja til ekstra linje (ny vare eller vare som har 0)
- [ ] **Ferdigmelding av telling** → Lager justerast automatisk basert på telling
- [ ] **Avviksrapport** → kva vart telt vs kva som var i systemet

### Manuell justering
- [ ] Velje ein eller fleire linjer
- [ ] Endre antall ELLER lot-nummer
- [ ] Med grunn/kommentar

### Historikk-fane (på kvar artikkel)
- [ ] **Alle transaksjonar** lagra: varetelling, justering, mottak, levering, produksjon
- [ ] Kven gjorde kva + tidspunkt
- [ ] Éin samla historikk per artikkel

---

## 🟢 Viktig: Andre modul

### 📥 Varemottak (Mottak)
- [ ] Mottakshandel → Leverandør, dato, referanse, kommentar
- [ ] Linjer → Artikkel + lot-nr + mengde + eining
- [ ] Kvalitetskontroll → Temperatur, holdbarheit (Best For Dato)
- [ ] Godkjenning → Lager oppdatert VED godkjenning, ikkje før
- [ ] Avvik → Merk som "Avvik" med grunn
- [ ] **Logger til LagerTransaksjon** → Kven, kva, når, kvifor

### 🚚 Levering
- [ ] Leveringsordre → Kunde, dato, referanse, fraktbrev
- [ ] Plukk → System viser kva som skal plukkast
- [ ] Linjer → Artikkel + lot-nr + mengde (automatisk frå lager)
- [ ] Kvalitetskontroll → Sending logga med kven + når
- [ ] **Logger til LagerTransaksjon** → Kven, kva, når, kvifor

### 👥 Kunder & Leverandører
- [ ] Kunder → Navn, orgnr, kontakt, adresse, e-post
- [ ] Leverandører → Navn, orgnr, kontakt, adresse, produktkategori
- [ ] Aktiv/Inaktiv → Kan deaktiverast utan å slette
- [ ] Historikk → Kva kunden har kjøpt / kva leverandøren har levert

### Brukarar & Tilgang
- [ ] Brukarar → Navn, e-post, passord, rolge
- [ ] Roller → Admin, Varemottak, Produksjon, Levering
- [ ] Tilgangskontroll → Kva kvar brukar ser og kan gjere
- [ ] Aktivitet → Kven logga inn sist, kva dei gjorde

---

## 🟡 Viktig: Kvalitetssikring
- [x] **Input-validering** → alle requests sjekka for gyldige verdiar
- [x] **Feilhandsaming** → logg + 400 Bad Request
- [x] **Lager-konsistens** → nei negative tal ved trekking
- [x] **Transaksjonssikring** → atomiske operasjonar (UnitOfWork m/ transactions)
- [ ] **Brukarrettar (RBAC)** → admin vel kva kvar brukar ser

### 🟢 Bra å ha
- [ ] **Min/max lageralarm** → flagg når beholdning < minimum
- [ ] **Batch-nummer autogenerering** → LOT-YYYYMMDD-NNN format

---

## Brukarroller & Mobil-oppleving

### Brukartypen
| Rolle | Tilgang | Grensesnitt |
|-------|---------|-------------|
| **Admin** | Alt + brukarstyring | Web (full) |
| **Varemottak** | Berre mottak | Mobil-vennleg (én knapp) |
| **Produksjon** | Berre produksjon | Mobil-vennleg |
| **Levering** | Berre levering | Mobil-vennleg |

### De på gulvet (mobil)
- **ÉITT skjermbilde** — berre den funksjonen dei treng
- **Éin-handta** — store knappar, lite tekst
- **Offline-støtte** (framtidig) — kan registrera utan nett

---

## Teknisk oversyn

### LagerTransaksjon tabellen (sentral)
```
Alle modulane loggar hit:
- Mottak (Type=Mottak, MottakId=X)
- Produksjon (Type=ProduksjonInn/ProduksjonUttak, ProduksjonsOrdreId=X)
- Levering (Type=Levering, LeveringId=X)
- Varetelling (Type=Varetelling, VaretellingId=X)
- Justering (Type=Justering, JusteringId=X)

Felles felt:
- Id, ArtikkelId, LotNr, Mengde, BeholdningEtter
- Kilde, KildeId, Type, KundeId, LeverandørId
- Kommentar, UtfortAv, Tidspunkt
```

### Sporings-API
| Metode | Endepunkt | Beskriving |
|--------|-----------|-------------|
| GET | `/api/traceability/lot/{lotNr}` | Alle transaksjonar for ein lot |
| GET | `/api/traceability/artikkel/{artikkelId}` | Alle lot for ein artikkel |
| GET | `/api/traceability/batch/{batchNr}` | Produksjonsbatch med detaljar |
| GET | `/api/traceability/kunde/{kundeId}` | Alle leveringar til ein kunde |
| GET | `/api/traceability/sok?q={query}` | Fritekst-søk på lot, artikkel, batch, kunde |

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
