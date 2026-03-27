# LagerPro cron runbook

Purpose: run a quick health/status check every 2 hours and report the result to Martin.

## What it should do
- Check git status
- Build the solution
- Run tests
- Read the MVP plan and identify the current next step
- Send Martin a short status message after every run

## Recommended message format
- Build: green/red
- Tests: green/red
- Current MVP focus: <one line>
- Next step: <one line>
- Notes: <anything important>

## Current limitation
The local environment does not have `crontab`, so a real 2-hour cron job cannot be installed here yet.
Use an external scheduler or a host-level cron service when available.
