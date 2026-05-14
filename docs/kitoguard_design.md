# KitoGuard — Design Doc

**Author:** Dana (planning notes for `meyoten11-collab/kitoguard`)
**Date:** 2026-05-13
**Status:** Shipped to `docs/kitoguard_design.md`

This is the complete system architecture design. Saved to the kitoguard repo.

---

## Summary

A modern Silkroad Online filter ("guard") is five coordinated channels:

1. **Proxy / packet filter** — TCP MITM with exploit handlers (kitoguard/DuckSoup)
2. **Server-side addon DLL** — In-process game-server commands (not in kitoguard today)
3. **Client-side patch + Media.pk2** — HWID enforcement + custom UI (not in kitoguard today)
4. **Admin panel (web + console)** — Operator UI for bans, events, settings (kitoguard has skeleton)
5. **Filter DB** — Persists operators, services, bans, detections, events, audit (kitoguard has basic schema)

## Reference filters (public features only)

- **MaxiGuard** — web panel, HWID client DLL, live edits without restart
- **Vanguard-R** — per-region bitwise rule flags (EF_PREVENT_ZERK, EF_FORCE_PVP_CAPE, etc.), gateway control, event framework
- **vSROPlus** — team-aware gameplay overrides, instant effects, DB-driven rule execution
- **KGuardEDGE** — multi-user SaaS licensing, live DPS interface, 6-slot rank system, TR+EN dual language

All are essentially the same five-channel pattern with varying UI polish and feature breadth.

---

## Architecture: 5-channel flow diagram

```
┌────────────────────────────────────────────────────────┐
│ ADMIN PANEL (web + console) — React/Vue SPA + WS API  │
│ GM ops │ Bans │ Live Ops │ Events │ Reports │ Audit    │
└───────────────────────┬────────────────────────────────┘
                        │  reads & writes
                        ▼
┌────────────────────────────────────────────────────────┐
│ KITOGUARD FILTER DB (MSSQL)                            │
│ Services, Machines, Rules, Bans, HWID Ledger, Events   │
│ Detection Events, GM Actions, Audit Log                │
└─────┬───────────────────────┬───────────────────────┬──┘
      │                       │                       │
      ▼ reads policy          ▼ invokes actions       ▼ polls queue
┌──────────────────┐  ┌──────────────────┐  ┌───────────────────┐
│ ① PROXY FILTER   │  │ ④ ADMIN BACKEND  │  │ ② ADDON DLL       │
│ (kitoguard)      │  │ (Webserver)      │  │ (GameServer.exe)  │
│                  │  │                  │  │                   │
│ - FakeServer     │  │ JWT auth         │  │ - Polls queue     │
│ - handlers       │  │ - role gates     │  │ - Executes cmds   │
│ - rate limiter   │  │ - static UI      │  │ - Posts events    │
│ - HWID/IP enf.   │  │                  │  │                   │
└──────┬───────────┘  └──────────────────┘  └──────────┬────────┘
       │                                                │
       │ TCP decryption, packet mutation, drops         │ in-process
       │                                                │ memory patches
       ▼                                                ▼
┌──────────────────┐                        ┌────────────────────┐
│ ③ CLIENT PATCH   │                        │ REAL SRO SERVERS   │
│ (guard.dll)      │   normal TCP encrypted │ SR_GameServer.exe  │
│                  │  ◀──────────────────▶  │ + SRO_VT_* DBs     │
│ - HWID id        │                        │                    │
│ - anti-tamper    │                        │                    │
│ - custom UI      │                        │                    │
└──────────────────┘                        └────────────────────┘
```

---

## What kitoguard has today

✅ **Channel 1** (proxy) — strong: packet handlers, exploit protection, rate limiting, session management
✅ **Channel 4 + 5** (panel + DB) — skeleton: JWT auth, webserver commented out, basic schema
❌ **Channel 2** (addon DLL) — not built
❌ **Channel 3** (client patch) — not built

---

## New DB schema (§5 from design doc)

Extends the existing proxy DB with operational tables:

### New tables

| Table | Purpose |
|---|---|
| `RuleSet` | Policies per service (whitelist/blacklist/rate limits) |
| `OpcodeRule` | Opcode disposition (allow/block/observe) |
| `RateLimitPolicy` | Per-opcode + global rate limits, configurable |
| `RegionRule` | Per-region bitwise flags (like Vanguard-R) |
| `HwidLedger` | HWID hash tracking with account JID + session count |
| `IpLedger` | IP tracking with account JID + "is proxy" flag |
| `Ban` | Account / Character / HWID / IP / IP-range bans with expiration |
| `DetectionRule` | Predefined rule catalog (exploit IDs, severity, description) |
| `DetectionEvent` | Audit trail: every detection logged (timestamp, session, severity, disposition) |
| `GmAction` | GM-initiated commands (item delivery, teleport, buff, etc.) |
| `AuditLog` | Every operator action (ban create, rule update, etc.) with operator ID + IP |
| `PacketCapture` (optional) | Raw packet hex for forensics (behind a feature flag) |
| `EventDefinition` / `EventOccurrence` / `EventReward` | Event / battle-pass / daily reward (optional, economy features) |

---

## 6-phase build plan

| Phase | Scope | Effort |
|---|---|---|
| **0** | Stabilize (env config, Serilog, Prometheus) | 1–2w |
| **1** | DB migration + schema (§5) | 1–2w |
| **2** | Detection events + first rule catalog | 1w |
| **3** | Server addon DLL foundation (§2 parity: item/gold/teleport/buff) | 3–4w |
| **4** | Client-side patch + HWID handshake + launcher | 4–6w |
| **5** | Panel polish + event calendar + battle-pass + daily reward | ongoing |

---

## Build prompts (W1.0–W8)

See `kitoguard_build_prompts.md` for the week-by-week PR breakdown.

**Key principle:** Each week is a self-contained PR with:
- **Zero file overlap** — no merge conflicts by careful scoping
- **Draft status** until ACCEPTANCE signed off
- **Mock endpoints** for W2–W5 (real DB integration in W6–W8)
- **Reusable tokens** locked in W1.0 (BrandShield, BrandPills, FooterCredit, i18n routing)

---

## Open questions / decisions

1. **VSRO188 or ISRO_R first?** kitoguard supports both; pick one as v1 focus.
2. **Single-tenant or multi-tenant?** Self-host (simplest) or SaaS (like KGuardEDGE).
3. **Closed-source or open?** Respect DuckSoup's DBAD license; rewrite or relicense if going commercial.
4. **HWID reset policy.** Strict = best anti-multi, but breaks hardware upgrades.
5. **Packet capture default.** Off (privacy + disk), or opt-in via per-char watch flag.

---

## Sources

- MaxiGuard (Turkish vendor): https://maxiguard.org/
- Vanguard-R (ISRO-R files): https://vanguard-r.online/
- vSROPlus (v188 guard): elitepvpers trading threads
- KGuardEDGE (longest-running, multi-user): https://kguardedge.com/
- kitoguard repo: https://github.com/meyoten11-collab/kitoguard (fork of DuckSoup)
- Open-source references:
  - SimplestSilkroadFilter: https://github.com/JellyBitz/SimplestSilkroadFilter
  - vSRO-ServerAddon: https://github.com/JellyBitz/vSRO-ServerAddon
  - sro_devkit: https://gitlab.com/florian0/sro_devkit

---

*This doc was distilled from Dana's kickoff message. Full details (DDL, RegionFlag enum, GmAction action IDs, component specs) are in `kitoguard_build_prompts.md`.*
