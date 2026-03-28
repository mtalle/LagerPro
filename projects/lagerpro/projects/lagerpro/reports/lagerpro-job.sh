#!/usr/bin/env bash
set -euo pipefail
cd /home/ubuntu/.openclaw/workspace
WEBHOOK="https://discord.com/api/webhooks/1487127580350877759/dfqIhCSvWtIJWTLxU2m0q7BfPz1gYL4ge1hYUCUzSlETCrOcZmhdOgopJFOdW1J7vBlz"

TIMESTAMP=$(date -u '+%Y-%m-%d %H:%M:%S UTC')

# Write task with timestamp
cat > /tmp/lagerpro-task.md <<TASK
# LagerPro jobb - NY
Timestamp: ${TIMESTAMP}
Alder: fersk (mindre enn 6 timer)

## Oppdrag
Jobb kontinuerlig på LagerPro MVP i 6 timer.

## Fokus (i prioritert rekkefølge):
1. Artikkel-CRUD (GET by id, PUT, DELETE)
2. Database/migrering
3. Mottak-flyt
4. Lageroversikt
5. Produksjon
6. Levering

## Regler
- Commit og push hvert 30. min
- Build + test hver gang
- Hold fokus på MVP
- Ikke gå utenfor scope
TASK

# Notify
curl -s -H "Content-Type: application/json" \
  -d "{\"content\": \"🚀 LagerPro-jobb lagt i kø kl. ${TIMESTAMP} — Steve vil plukke den opp ved neste heartbeat\"}" \
  "${WEBHOOK}" >> /dev/null 2>&1 || true

echo "[${TIMESTAMP}] Job queued" >> /home/ubuntu/.openclaw/workspace/reports/lagerpro-job.log
