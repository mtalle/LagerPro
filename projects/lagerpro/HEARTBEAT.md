# HEARTBEAT.md

## Hovedregel
Hver gang en heartbeat poll kommer inn:
- Sjekk om det er noe viktig som trenger oppmerksomhet
- Følg instruksjonene i meldingen
- Hvis ingenting trenger handling, svar HEARTBEAT_OK

## LagerPro
- OpenClaw sin innebygde cron håndterer LagerPro-jobs hver time
- Cron job ID: e7d27d91-1669-462e-a325-1175bb13f4b5
- Sjekk /tmp/lagerpro-task.md for gjeldende jobb-status
- Ved aktiv sub-agent: la den jobbe uforstyrret

## Daglig forbedring
Hver dag kl. 09:00 Europe/Oslo:
- Jobb autonomt på én forbedring som hjelper Martin
- Fokus: automasjon, status, dokumentasjon, repo-hygiene, rapportering, eller verktøy
- Velg noe konkret som gjør en eksisterende arbeidsflyt bedre
- Rapporter kort hva som ble bedre og hvorfor
- LagerPro er topp prioritet

## Hvis du er usikker
- Si ifra heller enn å anta
- Be om avklaring før du gjør noe stort
- Følg eksisterende mønstre i koden
