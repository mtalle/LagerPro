#!/usr/bin/env bash
set -euo pipefail
cd /home/ubuntu/.openclaw/workspace
WEBHOOK="https://discord.com/api/webhooks/1487127580350877759/dfqIhCSvWtIJWTLxU2m0q7BfPz1gYL4ge1hYUCUzSlETCrOcZmhdOgopJFOdW1J7vBlz"

MSG="🗑️ Paminnelse: Tommefrist i morgen! Sett ut dunkene for kl. 07:00"
curl -s -H "Content-Type: application/json" \
  -d "{\"content\": \"${MSG}\"}" \
  "${WEBHOOK}" >> /dev/null 2>&1 || true
