#!/usr/bin/env bash
set -euo pipefail
WEBHOOK="https://discord.com/api/webhooks/1487127580350877759/dfqIhCSvWtIJWTLxU2m0q7BfPz1gYL4ge1hYUCUzSlETCrOcZmhdOgopJFOdW1J7vBlz"
curl -s -H "Content-Type: application/json" \
  -d '{"content": "Steve: heartbeat check"}' \
  "${WEBHOOK}" >> /dev/null 2>&1 || true
