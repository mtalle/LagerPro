# HEARTBEAT.md

## Hovedregel
Hver gang en heartbeat poll kommer inn:
- Sjekk om det er noe viktig som trenger oppmerksomhet
- Følg instruksjonene i meldingen
- Hvis ingenting trenger handling, svar HEARTBEAT_OK

## Kodeoppgaver med Groq-sub-agent
- Når Martin gir kodingsoppgaver: Spawn en sub-agent med Groq-modellen (hvis mulig) til å skrive koden.
- Sub-agenten skal IKKE pushe til Git direkte.
- Jeg (Steve) leser gjennom koden, justerer om nødvendig, og pusher deretter til Git.
- Jeg oppdaterer også serveren (pull + restart) etter push.
- Pass på at layout.tsx ikke overskrives feilaktig.

## LagerPro
- OpenClaw sin innebygde cron håndterer LagerPro-jobs hver time
- Cron job ID: f5a82f23-deb0-42fc-a553-2260c8fc96b0 (Kjører på Groq)
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
