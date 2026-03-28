# LagerPro — MVP-plan

## Visjon
**For matindustrien** — sporbarheit, lagerstyring og produksjonsplanlegging i éin applikasjon.

**Kjerneverdi:** Live lager med full sporing frå råvare til ferdig produkt.

**Langtsiktig:** Versjon 1 → éin solid applikasjon, bygd på generisk architektur.

---

## Mål
Få en første brukertestbar MVP av LagerPro så fort som mulig.

## Backend-status (2026-03-28) ✅
**Ferdig:** Artikler, Kunder, Leverandører, Resepter, Mottak, Produksjon, Levering, Lager, Sporbarhet
**Tester:** 82 passing
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

#### Fase 2: Nye sider + organisering
- [ ] **Artikklar** → vis alle + rediger (Varelager)
- [ ] **Innstillingar** → Kunder + Leverandører (CRUD)
- [ ] **Resepter** → liste + legg til + rediger

#### Fase 3: Avansert
- [ ] **Sporing** → spor artikkel/lotje via `/api/traceability/lot/{lotNr}`
- [ ] **Dashboard** → enkel oversikt (artiklar, lager, produksjon)
- [ ] **Rapporter** → enkle nedlastningar

---

## Brukarroller & Mobil-oppleving

### Kjerneprinsipp
**Brukarvennleg høgt opp** — appen skal vere enkel for alle, spesielt dei på gulvet.

### Brukartypen
| Rolle | Tilgang | Grensesnitt |
|-------|--------|-------------|
| **Admin** | Alt + brukarstyring | Web (full) |
| **Varemottak** | Berre mottak | Mobil-vennleg (eitt skjerm) |
| **Produksjon** | Berre produksjon | Mobil-vennleg |
| **Levering** | Berre levering | Mobil-vennleg |

### Admin sine oppgåver
- Opprette/redigere brukarar
- Velje kva kvar brukar har tilgang til
- Definere rollene

### De på gulvet (mobil)
- **EITT skjermbilde** — berre den funksjonen dei treng
- **Éin-handta** — store knappar, lite tekst
- **Kun det dei treng** — zero distraksjon

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
