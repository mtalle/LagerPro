#!/usr/bin/env bash
set -euo pipefail
cd /home/ubuntu/.openclaw/workspace
WEBHOOK="https://discord.com/api/webhooks/1487127580350877759/dfqIhCSvWtIJWTLxU2m0q7BfPz1gYL4ge1hYUCUzSlETCrOcZmhdOgopJFOdW1J7vBlz"

printf "\n[%s] LagerPro cron run\n" "$(date -u '+%Y-%m-%d %H:%M:%S UTC')" >> reports/lagerpro-cron.log
echo "[1/4] git status" >> reports/lagerpro-cron.log
git status --short >> reports/lagerpro-cron.log || true
echo "[2/4] build" >> reports/lagerpro-cron.log
if dotnet build LagerPro.sln >> reports/lagerpro-cron.log 2>&1; then
  echo "[3/4] test" >> reports/lagerpro-cron.log
  dotnet test LagerPro.sln --no-build >> reports/lagerpro-cron.log 2>&1
  BUILD_STATUS="OK"
  EMOJI="✅"
else
  echo "[build failed]" >> reports/lagerpro-cron.log
  BUILD_STATUS="FAILED"
  EMOJI="❌"
fi
echo "[4/4] mvp plan pointer" >> reports/lagerpro-cron.log
sed -n "1,60p" projects/lagerpro/docs/mvp-plan.md >> reports/lagerpro-cron.log 2>&1

curl -s -H "Content-Type: application/json" \
  -d "{\"content\": \"🔄 LagerPro status ${EMOJI} | Build: ${BUILD_STATUS} | $(date -u '+%Y-%m-%d %H:%M UTC')\n📁 Logg: https://github.com/mtalle/LagerPro/blob/main/reports/lagerpro-cron.log\"}" \
  "${WEBHOOK}" >> /dev/null 2>&1 || true
