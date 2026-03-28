# LagerPro — MVP-plan

## Mål
Få en første brukertestbar MVP av LagerPro så fort som mulig.

## Backend-status (2026-03-28) ✅
**Ferdig:** Artikler, Kunder, Leverandører, Resepter, Mottak, Produksjon, Levering, Lager, Sporbarhet
**Tester:** 73+ passing
**Alle API-endepunkt på plass og pushet til GitHub**

---

## Frontend MVP — Nytt fokusområde

### Prioritert rekkefølge (sub-agent oppdrag)

#### Fase 1: Fiks eksisterande sider
- [ ] **Lageroversikt** → Bruk `/api/lager`
- [ ] **Mottak** → Bruk `/api/mottak`
- [ ] **Produksjon** → Bruk `/api/produksjon`
- [ ] **Levering** → Bruk `/api/levering`
- [ ] Alle knappar og skjema må fungere (les, lag, oppdater)

#### Fase 2: Nye sider
- [ ] **Kunder** → liste + legg til + rediger
- [ ] **Leverandører** → liste + legg til + rediger
- [ ] **Resepter** → liste + legg til + rediger
- [ ] **Artiklar** → rediger eksisterande (berre visning idag)

#### Fase 3: Avansert
- [ ] **Sporing** → spor artikkel/lotje via `/api/traceability/lot/{lotNr}`
- [ ] **Rediger artiklar** → full edit-skjema
- [ ] **Dashboard** → enkel oversikt (artiklar, lager, produksjon)
- [ ] **Rapporter** → enkle nedlastningar

---

## MVP-scope
Kun det som trengs for å teste kjerneflyten:
- Registrere artikkel
- Registrere mottak
- Se lagerbeholdning
- Opprette enkel produksjon
- Registrere enkel levering
- Grunnleggende sporbarhet på lot
- Enkel frontend som kan brukes uten friksjon

## Tidsestimat
- Intern grov demo: 3–5 arbeidsdager
- Brukertestbar MVP: 1–2 uker
- Mer stabil første pilot: 2–4 uker

## Prioritert rekkefølge
1. Få database og CRUD for artikkel helt stabilt
2. Lage migrering og runtime-kobling
3. Bygge mottak-flyten
4. Lage lager-visning
5. Bygge produksjon-flyten
6. Bygge levering-flyten
7. Koble sporbarhet på lot
8. Lage enkel frontend som faktisk kan testes

## Dag-for-dag forslag
### Dag 1
- Verifisere databasekobling og migreringer
- Fikse artikkel-CRUD helt ende-til-ende

### Dag 2
- Mottak-modul på plass
- Første lageroppdatering fra mottak

### Dag 3
- Lageroversikt
- Enkel visning av beholdning per artikkel/lot

### Dag 4
- Produksjon: opprette ordre og trekke råvarer

### Dag 5
- Levering: registrere uttak og koble mot kunde

### Dag 6
- Sporbarhet: lot-kjede fra levering tilbake til råvare

### Dag 7
- Frontend-skjermbilder og enkel testflyt
- Rydding, feilretting og demo-klargjøring

## Risikopunkter
- Database/migrering kan bremse resten hvis den ikke sitter
- Sporbarhet kan vokse fort hvis scope ikke holdes stramt
- Frontend kan trekke tid hvis den blir for ambisiøs

## Beslutning
Start med funksjon fremfor penhet. MVP skal kunne brukes, ikke imponere.
