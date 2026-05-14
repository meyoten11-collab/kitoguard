# KitoGuard Build Prompts — Weeks W1.0 through W8

**Author:** Devin AI (build orchestration for `meyoten11-collab/kitoguard`)
**Date:** 2026-05-14
**Version:** 1.1
**Status:** W1.0 SHIPPED. W2–W8 ready for PR opening.

---

## Overview

This document is the **week-by-week breakdown** of the KitoGuard-S500 rebuild. Each week (W) is a self-contained PR targeting a specific slice of the roadmap from `kitoguard_design.md` §6. Week 1 (W1) covers brand + i18n scaffolding. Weeks 2–8 (W2–W8) layer in the dashboard, settings, plugin manager, IP/HWID/Ban tools, anti-cheat, events, and finally system pages + test suite.

### Key principles

1. **Zero file overlap across PRs.** W1 doesn't touch W2's files; W2 doesn't retouch W1's. Merge conflicts are resolved by careful scoping, not by cherry-picking.
2. **Reusable tokens.** `BrandShield`, `BrandPills`, `FooterCredit`, `next-intl` plumbing, and role-gated components are locked in W1. W2+ inherit them without change.
3. **Mock first, real later.** W2–W5 stand up UI + mock endpoints. W6–W8 integrate real DB queries and backfill DB schema.
4. **Draft PRs until design locked.** Every PR opens as **draft** with an **ACCEPTANCE** section. Human reviewer signs off before moving to in-progress/review.
5. **Coordination comments.** Every week's PR includes a link to the next week's skeleton PR (if opened) so the reviewer can see the full roadmap at a glance.

---

## W1.0 — Brand sweep + i18n scaffolding ✅ SHIPPED

**Merge commit:** 93787dbd1bc6ef86e010e244b4f4fcb7fb4fb735
**Files touched:** 32 files added/modified
**Scope:** pnpm workspace root + `apps/web` Next.js scaffold + brand assets + `next-intl` routing + three i18n catalogs (en/ar/tr).

### What shipped

- **`pnpm-workspace.yaml` + root `package.json`** — monorepo structure; .NET projects stay in place (handled by older session).
- **`apps/web` Next.js 15 scaffold** — TypeScript, Tailwind, `lucide-react`, `next-intl`, shadcn/ui skeleton.
- **Brand components** (reused by all W2+ PRs):
  - `BrandShield` — KG logo + brand red `#E53935`.
  - `BrandPills` — _Secure | Fast | Powerful | Scalable_.
  - `FooterCredit` — _© KitoGuard-S500 · Developed by Ahmed Yasser — Owner of This System_.
- **i18n setup** — `next-intl` middleware routes `/` → `/{en|ar|tr}`. Arabic renders `dir="rtl"`. Three JSON catalogs (en/ar/tr) with full UI strings.
- **CI** — new `.github/workflows/web.yml` runs `pnpm web:typecheck` + `pnpm web:lint` + `pnpm web:build`.

### Deliverables checklist

- [x] `pnpm-workspace.yaml` and root build scripts
- [x] `apps/web/package.json` with Next.js 15, Tailwind, `next-intl`, `lucide-react`
- [x] `BrandShield`, `BrandPills`, `FooterCredit` components
- [x] `next-intl` middleware + locale routing
- [x] `apps/web/messages/{en,ar,tr}.json` with landing page strings
- [x] `[locale]/page.tsx` (landing) referencing all brand + i18n features
- [x] `.github/workflows/web.yml` (CI for the web app)
- [x] README.md rebranded to KitoGuard-S500

### Open questions / decisions locked

- **Namespace:** The internal C# namespace `DuckSoup` is canonical for the filter DB; product brand = KitoGuard-S500 (web layer only).
- **Font/colors:** Tailwind defaults + brand red `#E53935` for accents. No custom fonts yet (can add in W1.1).
- **RTL implementation:** `.dir-flip` utility auto-flips directional icons in Arabic; no hardcoded `→` or `←`.

---

## W2 §D.1 — Dashboard end-to-end (mocked)

**PR scope:** `apps/web/src/components/{KpiCard,ControlCard,StatusListCard,InfoListCard,LiveChart,LogsTimeline,RankingTable,EventsList,PluginManagerTable,SystemResources,UniqueTracker,IconMenuPreview}`, `/[locale]/dashboard`, `/api/v1/*` mock endpoints.

### Components to build

#### W2 §C.2 — KpiCard (6 variants)

Gradient background card with large metric + trend arrow. Six instances on the dashboard:

1. **Players** (blue gradient) — online player count + 24h trend
2. **Uptime** (green gradient) — server uptime % + last downtime
3. **Events** (orange gradient) — active events count + next event ETA
4. **Plugins** (purple gradient) — loaded plugin count + health status
5. **Servers** (teal gradient) — proxied services count + all-running status
6. **Attacks** (pink gradient) — detections in last hour + severity distribution

Each card has:
- Large metric number (font-size: 2.5rem, bold)
- Trend line (arrow + %, green up / red down)
- Subtle background gradient (variant-specific colors)
- Hover state (lift + shadow)

Accessibility: ARIA labels, high-contrast text.

#### W2 §C.3 — ControlCard / StatusListCard / InfoListCard primitives

Three layout components, heavily reused:

**ControlCard**
- Title, optional icon, optional action button (top-right)
- Bordered card (light border in light mode, darker in dark)
- Padding: `p-6`
- Used by: Server Status cards, Plugin cards

**StatusListCard**
- List of items with status badges (inline `<badge variant="success|warning|error" />`), timestamps
- Each row is a `<div className="flex justify-between items-center py-2 border-b last:border-b-0" />`
- Used by: Live Sessions, Service Status, Active Events

**InfoListCard**
- Bordered list of key-value pairs (left = label, right = value)
- Values can be text, badges, or mini-sparklines
- Used by: System Resources, Server Info, Event Details

#### W2 §C.4 — LiveChart (Recharts multi-series, SignalR-stubbed)

A multi-series line chart showing real-time metrics over the last 1 hour:
- X-axis: time (5-min intervals)
- Y-axes: Packets/sec, CPU %, RAM %, Detections/min (4 series, each a different color)
- Data source: mock `/api/v1/hubs/metrics` (Server-Sent Events stub; real SignalR will replace in production)
- Auto-refresh: every 5 seconds from the mock stream
- Recharts version 2.x with `ResponsiveContainer`

#### W2 §C.5 — LogsTimeline (compact variant for dashboard — full virtualized list goes in W2 dashboard slot)

Compact vertical timeline of the 10 most recent audit/detection events:
- Each row: timestamp, action type (badge color), description (truncated)
- Icons (via `lucide-react`): `CheckCircle` (success), `AlertCircle` (warning), `XCircle` (error), `Eye` (observed)
- Timestamps in the operator's locale (from next-intl)
- Click row → navigate to full logs page (not in W2; defer to W3)

#### W2 §C.6 — RankingTable (Top Players 24h)

Simple table showing top 10 players by kills in the last 24 hours:
- Columns: Rank (1–10), Character Name, Kills, Deaths, K/D Ratio, Last Kill
- Sortable (client-side for mocked data; server-side later)
- Bordered, striped rows

#### W2 §C.7 — EventsList (active events with countdown)

List of active events (mocked: 2–3 example events):
- Event name, start/end time, countdown to end (MM:SS if <1 hour, else HH:MM)
- Status badge: `Running`, `Starting Soon`, `Ended`
- Small info icon hover-tooltip showing event description

#### W2 §C.8 — PluginManagerTable (compact for dashboard)

Minimal table of active plugins (name, version, status):
- Columns: Plugin Name, Version, Status (enabled/disabled badge), Action (disable button)
- 5 row max (truncated with "view more" link)
- Each plugin row has a `>` chevron that expands to show: Hooks (list of opcodes), Last Error (if any), Memory (stub value)

#### W2 §C.9 — SystemResources (CPU/RAM/Disk bars)

Three progress bars in a card:
- **CPU:** `color: blue`, usage % label
- **RAM:** `color: green`, free/total label
- **Disk:** `color: orange`, free/total label
- Mocked values (update via `/api/v1/dashboard/system-resources`)
- Refresh every 10 seconds

#### W2 §C.10 — UniqueTracker (floating draggable window)

Optional: floating window (position: fixed, drag-handle at top) showing unique kill milestones this session:
- List of: UniqueID, last kill timestamp, kill count
- Can be dismissed + re-opened from a dashboard widget
- Minimum viable for W2 (full page mode in W7)

#### W2 §C.11 — IconMenuPreview (4×4 right-side icon grid)

Floating grid on the right edge of the dashboard showing quick-access icons:
- 4×4 grid = 16 icons
- Each icon links to a nav route (Server Status, Plugin Manager, Ban List, etc.)
- Drag to reorder (persist order in localStorage)
- Collapsible (hamburger menu to fold/unfold)

#### W2 §B.7 — FeatureRibbon (5-column footer ribbon, dashboard route only)

Footer ribbon (below all dashboard content) with 5 columns:
- **Column 1:** "Quick Links" — Server Control, Event Manager
- **Column 2:** "Documentation" — API Docs, Admin Guide
- **Column 3:** "Support" — Discord, GitHub Issues
- **Column 4:** "Legal" — Privacy, Terms
- **Column 5:** "Settings" — Theme, Language, User Profile

### Pages and layouts

#### W2 §D.1 — /[locale]/dashboard page

Composing all of the above + temporary `AppShell` shim (owned by older session; W2 PR just imports the interface):

```
┌────────────────────────────────────────────────────────┐
│ AppShell (owned by older session in W1.1)              │
│ ┌──────────────────────────────────────────────────────┤
│ │ Header: Logo, Locale Switcher, User Menu             │
│ ├──────────────────────────────────────────────────────┤
│ │ Sidebar: Dashboard, Services, Bans, IP, HWID, ...    │
│ ├─────────────────────────────────┬────────────────────┤
│ │ Main Content                    │ IconMenuPreview    │
│ │ ┌──────────────────────────────┐│ (4x4 grid)         │
│ │ │ [KpiCard x6]                 ││ collapse button    │
│ │ ├──────────────────────────────┤│                    │
│ │ │ [LiveChart] | [LogsTimeline] ││                    │
│ │ ├──────────────────────────────┤│                    │
│ │ │ [RankingTable] [EventsList]  ││                    │
│ │ ├──────────────────────────────┤│                    │
│ │ │ [PluginManagerTable]         ││                    │
│ │ ├──────────────────────────────┤│                    │
│ │ │ [SystemResources]            ││                    │
│ │ │ [UniqueTracker] (draggable)  ││                    │
│ │ └──────────────────────────────┘│                    │
│ │ [FeatureRibbon x5 columns]       │                    │
│ └─────────────────────────────────┴────────────────────┘
└────────────────────────────────────────────────────────┘
```

Layout breakpoints:
- **Mobile (<640px):** Stack all cards vertically; hide IconMenuPreview (float to bottom as bottom sheet instead).
- **Tablet (640–1024px):** KpiCard 2×3 grid; charts full-width.
- **Desktop (>1024px):** KpiCard 3×2 grid; charts side-by-side; IconMenuPreview fixed right.

### Mock API endpoints

All under `/api/v1/*` (Next.js Route Handlers in `apps/web/src/app/api/v1/*`):

| Endpoint | Method | Response shape | Refresh |
|---|---|---|---|
| `/api/v1/dashboard/summary` | GET | `{ onlinePlayers, uptime%, activeEvents, loadedPlugins, proxiedServices, detectionsLastHour }` | 10s |
| `/api/v1/filter/status` | GET | `{ services: [{id, name, isRunning, uptime}], ...] }` | 30s |
| `/api/v1/server/info` | GET | `{ version, buildTime, cpuUsage%, ramUsage%, diskUsage%, uptime }` | 10s |
| `/api/v1/logs/recent` | GET | `{ events: [{timestamp, action, type, description, severity}] }` | 5s |
| `/api/v1/players/top` | GET | `{ rankings: [{rank, charName, kills, deaths, kdRatio, lastKill}] }` | 60s |
| `/api/v1/events/active` | GET | `{ events: [{id, name, startsAt, endsAt, status, description}] }` | 30s |
| `/api/v1/plugins` | GET | `{ plugins: [{id, name, version, status, hooks[], lastError?}] }` | 60s |
| `/api/v1/icons/right-menu` | GET | `{ icons: [{id, label, route, icon}] }` | never (static) |
| `/api/v1/hubs/metrics` | GET (SSE) | Server-Sent Events stream: `data: {timestamp, packets/s, cpu%, ram%, detections/min}\n\n` | 5s intervals |

### i18n expansion

Add to `messages/{en,ar,tr}.json`:
- Dashboard page title, card titles, chart legends
- Button labels (Theme Toggle, Language Selector, Settings, Logout)
- Empty states ("No active events", "No recent logs")
- Error messages (network error, 404, 500)
- Abbreviations (K for thousand, M for million, %)
- Time formats ("just now", "2 hours ago", "yesterday")

### Acceptance criteria

- [ ] Dashboard loads at `/en/dashboard`, `/ar/dashboard`, `/tr/dashboard`
- [ ] All 6 KpiCards render with correct colors (blue, green, orange, purple, teal, pink)
- [ ] LiveChart animates in with 4 series; SSE `/api/v1/hubs/metrics` mock is callable
- [ ] RankingTable sorts by kills descending
- [ ] LogsTimeline shows 10 items; timestamps localized to user's timezone
- [ ] PluginManagerTable shows 5 plugins max; "view more" link present
- [ ] SystemResources bars update every 10 seconds from mock endpoint
- [ ] IconMenuPreview grid is draggable (reorder persists to localStorage)
- [ ] FeatureRibbon 5 columns render at bottom
- [ ] Dark mode toggle changes all colors correctly (tested with `data-theme="dark"` on root)
- [ ] Arabic layout is RTL; all chevrons auto-flip via `.dir-flip`
- [ ] Turkish text doesn't overflow cards (test long strings)
- [ ] All UI strings are pulled from i18n catalogs (no hardcoded strings)
- [ ] CI passes: `pnpm web:typecheck`, `pnpm web:lint`, `pnpm web:build`

### ACCEPTANCE

**Sign-off required before moving to in-progress:**

- [ ] Visual design of dashboard matches KitoGuard branding (red accents, shield logo)
- [ ] All mock endpoints return data in the expected shapes (can curl to verify)
- [ ] Responsive layout works on 412px (mobile), 768px (tablet), 1920px (desktop)
- [ ] Locales render correctly; Arabic is right-to-left; Turkish text fits
- [ ] No console errors or warnings in the browser
- [ ] Lighthouse score >= 85 (performance, accessibility, best practices)

---

## W3 — Server Status + Filter Settings (3 PRs, staggered)

### W3 §D.3 Server Status detail page

Page: `/[locale]/server-status/{serviceId}`
- Card per service showing: name, status (online/offline), uptime, last started/stopped, packet rate, detections/min
- Per-service signal monitoring (CPU, RAM, specific to that service if available)
- Action buttons: Start, Stop, Restart, Logs
- Historical uptime chart (7-day view)

### W3 §D.10 /filter-settings/general

Form: Shard/Logging/Scheduled settings
- Shard name, player cap, description
- Logging: verbosity, retention days, file size rotation
- Scheduled maintenance: start date, end date, auto-shutdown toggle

### W3 §D.11 /filter-settings/security

Form: Connection limits + rate limits + toggles
- Max connections per IP
- Max accounts per HWID
- Rate limit policy editor (interactive, per-opcode)
- 2FA enforcement toggle
- API key scope restrictions
- Every save audited to `audit_log`

### W3 §D.12 /filter-settings/connection

Form: Bind IP / public+private ports / cert mode
- Local bind IP (for multi-NIC servers)
- Public port (advertised to players)
- Private port (for internal services)
- TLS/SSL toggle + cert path

### W3 §F.4 EF entities + insert wrappers for SRO_GAMESERVER _Instant*Delivery tables

Schema extension:
- `_InstantItemDelivery(ItemDeliveryId, CharName, ItemCode, Count, AtTime)`
- `_InstantGoldDelivery(GoldDeliveryId, CharName, Gold, AtTime)`
- `_InstantBuffDelivery(BuffDeliveryId, CharName, BuffId, Duration, AtTime)`
- `_InstantTeleportDelivery(TeleportDeliveryId, CharName, X, Y, Z, RegionId, AtTime)`
- `_InstantMobSpawnDelivery(SpawnId, RegionId, MobId, X, Y, Z, AtTime)`

EF Core entities + DbSet in the `SRO_GAMESERVER` (or `SRO_VT_SHARD` — confirm with team) context.

---

## W4 — Plugin Manager + SDK

### W4 §D.8 /plugin-manager full page

Page: `/[locale]/plugin-manager`
- Plugin browser (search, filter by category)
- Upload UI (drag-drop .dll file; validate signature)
- Per-plugin details: name, version, author, description, enabled/disabled toggle
- Tabs per plugin: **Logs**, **Settings**, **Routes** (custom endpoints the plugin defines)
- Action buttons: Enable, Disable, Uninstall, View Logs

### W4 §G Plugin SDK

`KitoGuard.PluginSDK` NuGet package (C#):
- `IPlugin` contract (methods: `OnLoad()`, `OnUnload()`, `OnPacketIn()`, `OnPacketOut()`)
- `IPluginManager` provided by runtime
- `AssemblyLoadContext` isolation per plugin
- `PluginManifest.json` (name, version, author, hooks)
- Signed-manifest verification (HMAC-SHA256 signature check)
- `TestKit` (mock IPluginManager, mock packets for unit testing)

### W4 §E AutoNotice plugin (first-party, open-source example)

Port from `ds-plugins/AutoNotice` (if available in public repo) or build a simple version:
- Polls `_AutoNotice` table (SRO_VT_SHARD)
- Pushes notices to Discord webhook (configurable URL per notice type)
- Example notice types: server down, maintenance, event start, emergency
- Fan-out: Discord channel per notice type

---

## W5 — IP/HWID/Ban Tools + Anti-Cheat + Auto-Ban

### W5 §D.17 /tools/ip-management

Tabs:
- **Whitelist** — Add/remove IP ranges (CIDR notation)
- **Blacklist** — Add/remove banned IPs
- **Active Blocks** — Real-time blocked IP attempts (last 24h)
- **Netcafe Limits** — Max concurrent sessions per netcafe subnet (geo-detect or manual CIDR ranges)

### W5 §D.18 /tools/hwid-management

Tabs:
- **Blacklist** — Add/remove banned HWIDs
- **Netcafe HWID Limits** — Max concurrent sessions per HWID in netcafe ranges

### W5 §D.19 /tools/ban-management

Tabs:
- **Active Bans** — Paginated list of active bans (account, character, HWID, IP), with lift button + lift reason form
- **History** — All lifted bans (with lift date, who lifted it, reason)
- **Quick Ban** — Form to create a new ban (scope dropdown, target text, reason, expiration)

### W5 §E.16 Anti-Cheat — DetectionRule seed catalog

Seed data into `DetectionRule` table:
- `VSRO188.EXPLOIT.ZERK_INVIS` (severity 3 = disconnect)
- `VSRO188.EXPLOIT.CHARNAME_MODIFY` (severity 3)
- `VSRO188.ANOMALY.OUT_OF_STATE` (severity 1 = warn, observe-only by default)
- `RATE.GLOBAL_FLOOD` (severity 2 = block)
- `HWID.MULTI_ACCOUNT_EXCEED` (severity 2 = block)
- ... and others

### W5 §E.18 Auto-Ban worker

Background service (scheduled task or event handler):
- Watches for `DetectionEvent` rows where `Severity >= 2` (block level)
- Groups by account/HWID/IP (configurable thresholds: e.g., 5+ events in 1 hour)
- Creates a `Ban` row automatically
- Inserts `GmAction` (ActionId = auto-ban marker) for audit trail
- Sends notification to admin channel in Discord

---

## W6 — Icon Manager + Title Manager

### W6 §E.1 /icon-manager

Pages:
- **Icon Catalog** — CRUD icons (upload .dds, set name/id, preview)
- **Character Icon Binding** — Per-character assign icon (from catalog)
- **16-slot preview** — Show how 16 slots look on a character model (static image with overlay, or placeholder)

### W6 §E.2 /title-manager + /title-manager/grants

Pages:
- **Title Catalog** — CRUD titles (name, color, effect, rarity)
- **Grants** — Assign title to character, set expiration date
- **On-save hook** — EXEC `_UpdateVipUser` stored proc in SRO database (Joymax convention)

---

## W7 — Events Module + Unique Tracker + Fortress War

### W7 §D.13 /filter-settings/event

Form: Per-event cron/duration/registration/level/prizes/regions over `_OnEvent<X>*` tables

### W7 §E.3 AutoEvents plugin

State machine: Starting/Running/Ending
- Survival, CTF, JobWar, LMS, Drunk, HorseRace, LuckyStall, XSMB
- Polls `_OnEvent*` tables, emits commands to `_GameServerEvents`

### W7 §E.4 /survival-arena

Pages: Live, History, Settings
- SignalR hub for live event state

### W7 §E.5 /ctf

Pages: Live, History, Settings
- CTF team-score widget
- Push-to-addon-ini button

### W7 §E.6 /unique-tracker

Full page: Live kills, history table, LogUniqueKills integration, per-unique reward settings

### W7 §E.12 /fortress-war

Pages: Schedule, Active, Registration, Rewards
- `_OnEventFortressWar*` tables
- DISABLE_*_FW_REG toggle flags

---

## W8 — System Pages + QA Suite + Docs

### W8 §D.21 /system/backup-restore

Manual + scheduled SQL backups + restore UI

### W8 §D.22 /system/auto-tasks

Quartz cron rows: cleanup / rankings / event counters / pcap prune

### W8 §D.23 /system/system-logs

Serilog file/SQL sink viewer

### W8 §D.24 /system/api-webhooks

ApiKey CRUD + IP allowlist; Webhook CRUD + HMAC secret + retry

### W8 §H.1 Playwright visual-regression suite

Every NAV route x [dark, light] x [1920x1080, mobile 412x915] x [en, ar, tr]

### W8 §H.2 Playwright e2e smoke

Login, start-stop-restart, RBAC denial, plugin enable-disable, IP blacklist, announcement

### W8 §H.3 k6 + bombardier load test

`/dashboard/summary` p95<80ms@50RPS
`/hubs/metrics` 1000 subs

### W8 §H.4 Security checklist

SQLi, JWT-tampering, 2FA-bypass, CSRF, SSRF, path-traversal, RBAC, plugin sandbox

### W8 docs polish

Architecture diagrams, plugin author guide, operator runbook

---

## Deferred to v1.1

- §E.8 Damage Meter
- §D.20 Packet Monitor advanced replay
- §E.19 Memory Protection toggles
- C++ server-addon DLL (can be W3–W4 if you have C++ devs on standby)
- Client-side guard DLL (can be W4–W5 if you have Win32 devs on standby)

---

## Coordination

**PR coordination links:**
- W1.0 (merged) → W2 (draft PR opens with this doc)
- W2 → W3 (3 staggered PRs)
- W3 → W4
- W4 → W5
- W5 → W6
- W6 → W7
- W7 → W8

**Every PR:**
1. Opens as **draft**
2. Includes an **ACCEPTANCE** section with checkboxes
3. Links to the *next* week's PR skeleton (if opened)
4. Includes a **Coordination Comment** summarizing the week's scope + link to `kitoguard_build_prompts.md` in docs

---

*End of build prompts.*
