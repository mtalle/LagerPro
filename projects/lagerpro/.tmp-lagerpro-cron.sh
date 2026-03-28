#!/usr/bin/env bash
set -euo pipefail
cd /home/ubuntu/.openclaw/workspace
printf '\n[%s] LagerPro cron run\n' "$(date -u '+%Y-%m-%d %H:%M:%S UTC')" >> reports/lagerpro-cron.log

echo '[1/4] git status' >> reports/lagerpro-cron.log
git status --short >> reports/lagerpro-cron.log || true

echo '[2/4] build' >> reports/lagerpro-cron.log
if dotnet build LagerPro.sln >> reports/lagerpro-cron.log 2>&1; then
  echo '[3/4] test' >> reports/lagerpro-cron.log
  dotnet test LagerPro.sln --no-build >> reports/lagerpro-cron.log 2>&1
else
  echo '[build failed]' >> reports/lagerpro-cron.log
fi

echo '[4/4] mvp plan pointer' >> reports/lagerpro-cron.log
sed -n '1,120p' projects/lagerpro/docs/mvp-plan.md >> reports/lagerpro-cron.log
