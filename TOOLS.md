# TOOLS.md - Local Notes

Skills define _how_ tools work. This file is for _your_ specifics — the stuff that's unique to your setup.

## What Goes Here

Things like:

- Camera names and locations
- SSH hosts and aliases
- Preferred voices for TTS
- Speaker/room names
- Device nicknames
- Anything environment-specific

## Examples

```markdown
### Cameras

- living-room → Main area, 180° wide angle
- front-door → Entrance, motion-triggered

### SSH

- home-server → 192.168.1.100, user: admin

### TTS

- Preferred voice: "Nova" (warm, slightly British)
- Default speaker: Kitchen HomePod
```

## Why Separate?

Skills are shared. Your setup is yours. Keeping them apart means you can update skills without losing your notes, and share skills without leaking your infrastructure.

---

## Discord Webhook

- Steve cron-notifikasjoner → `https://discord.com/api/webhooks/1487127580350877759/dfqIhCSvWtIJWTLxU2m0q7BfPz1gYL4ge1hYUCUzSlETCrOcZmhdOgopJFOdW1J7vBlz`
- Brukes av: lagerpro-cron.sh, morgenbrief.sh, dagsrapport.sh, tommekalender-reminder.sh

## Cron Jobs

- `0 */2 * * *` → LagerPro status hver 2. time
- `0 6 * * *` → Morgenbrief kl. 06:00
- `0 8 * * *` → Dagsrapport kl. 08:00
- `0 18 * * 2` → Tømmekalender påminnelse tirsdag kl. 18:00
- Scripts: `/home/ubuntu/.openclaw/workspace/reports/`
