# LagerPro — Backend MVP: Sporbarhet & Kvalitetssikring

## Mål
Komplett backend for heil sporbarheitskjede frå råvare til ferdigvare til kunde.

## Fase 1: Backend ferdig (no → 1 uke)

### 🔴 Kritisk: Sporbarheitskjeden
- [ ] **Mottak** → råvare med lot-nr, vekt, temperatur, holdbarheit
- [ ] **Produksjon** → råvarer → ferdigvare (kvar batch = ny lot)
- [ ] **Levering** → ferdigvare til kunde med lot-nr
- [ ] **Traceability API** → gitt lot-nr → vis heile historikken (kvar varen har vore)
- [ ] **Råvarerapport** → kva råvarer går i kvar ferdigvare-batch
- [ ] **Kundesporing** → kva kunde fekk kvar batch

### 🟡 Viktig: Kvalitetssikring
- [ ] **Input-validering** → alle requests sjekka for gyldige verdiar
- [ ] **Feilhandsaming** → kva skjer ved ugyldig data? (logg + 400 Bad Request)
- [ ] **Lager-konsistens** → sjekk at lager aldri går i minus ved trekking
- [ ] **Transaksjonssikring** → atomiske operasjonar (alt eller inkje)

### 🟢 Bra å ha
- [ ] **Audit log** → kven oppretta/endrea kva (operatør, tidspunkt)
- [ ] **Batch-nummer autogenerering** → LOT-YYYYMMDD-NNN format
- [ ] **Min/max lageralarm** → flagg når beholdning < minimum

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

### Transaksjonstyper
- `Mottak` — råvare motteke
- `ProduksjonInn` — ferdigvare produsert
- `ProduksjonUttak` — råvare brukt i produksjon
- `Levering` — ferdigvare sendt til kunde
- `Korrigering` — manuell justering (med grunn)

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
