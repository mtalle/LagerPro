# Learnings Log

Captured learnings, corrections, and discoveries. Review before major tasks.

---

## [LRN-20260326-001] feature_request

**Logged**: 2026-03-26T12:55:00Z
**Priority**: medium
**Status**: promoted
**Area**: config

### Summary
Installed self-improving-agent skill from GitHub repository and verified its structure.

### Details
User asked to download a skill at `~/.openclaw/skills/self-improving-agent`. The local path did not exist, so the repository was cloned from `https://github.com/peterskoett/self-improving-agent.git` into the workspace for inspection. The repo includes `SKILL.md`, `.learnings/` templates, `references/openclaw-integration.md`, and scripts.

### Suggested Action
Install the skill under the OpenClaw skills directory if the environment expects skill auto-loading, or keep the repo available for copying the relevant files into the proper location.

### Metadata
- Source: user_feedback
- Related Files: /home/ubuntu/.openclaw/workspace/.tmp-self-improving-agent/SKILL.md
- Tags: openclaw, skill, self-improvement
- Pattern-Key: skill.installation
- Recurrence-Count: 1
- First-Seen: 2026-03-26
- Last-Seen: 2026-03-26

---
