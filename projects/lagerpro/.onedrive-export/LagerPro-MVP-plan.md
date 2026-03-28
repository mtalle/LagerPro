# LagerPro — MVP-plan

## Mål
Få en første brukertestbar MVP av LagerPro så fort som mulig.

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
