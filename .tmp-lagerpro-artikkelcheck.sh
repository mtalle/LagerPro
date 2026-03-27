#!/usr/bin/env bash
set -euo pipefail
cd /home/ubuntu/.openclaw/workspace
printf '\n[%s] Article flow check\n' "$(date -u '+%Y-%m-%d %H:%M:%S UTC')" >> reports/lagerpro-cron.log

grep -n "CreatedAtAction\|AddAsync\|SaveChangesAsync\|Enum.Parse" projects/lagerpro/src/LagerPro.Api/Controllers/ArticlesController.cs projects/lagerpro/src/LagerPro.Application/Features/Articles/Commands/CreateArticle/CreateArticleHandler.cs projects/lagerpro/src/LagerPro.Infrastructure/Repositories/ArtikkelRepository.cs >> reports/lagerpro-cron.log || true
