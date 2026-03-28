#!/usr/bin/env bash
set -euo pipefail
cd /home/ubuntu/.openclaw/workspace
WEBHOOK="https://discord.com/api/webhooks/1487127580350877759/dfqIhCSvWtIJWTLxU2m0q7BfPz1gYL4ge1hYUCUzSlETCrOcZmhdOgopJFOdW1J7vBlz"

MSG="📋 Morgenbrief - LagerPro | $(date -u '+%Y-%m-%d %H:%M UTC')

Fokus: Fullfore artikkel-CRUD, deretter mottak -> lager -> produksjon -> levering
Tech: .NET 8, EF Core, Clean Architecture"

curl -s -H "Content-Type: application/json" \
  -d "{\"content\": \"${MSG}\"}" \
  "${WEBHOOK}" >> /dev/null 2>&1 || true
