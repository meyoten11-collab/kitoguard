# KitoGuard-S500

> **vSro-iSro R Filter System Guard** — _Premium Web Panel | Filter Management | Premium UI_
>
> **Secure | Fast | Powerful | Scalable**

**Owner / Author:** Ahmed Yasser — Owner of This System

KitoGuard-S500 is a Silkroad Online private-server filter system targeting **vSRO** and
**iSRO-R**. It pairs a hardened .NET 8 / .NET 10 packet filter (originally the **DuckSoup**
filter, now branded KitoGuard-S500 at the product layer) with a Next.js 15 admin panel
("KG-in-shield" brand, brand red `#E53935`).

## Repository layout

```
apps/web/        Next.js 15 (App Router) admin panel — KitoGuard-S500 web UI
API/             .NET API host (will move under apps/api in a follow-up PR)
DuckSoup/        .NET filter runtime (the actual packet filter binary)
Database/        EF Core schema for the DuckSoup filter database
PacketLibrary/   vSRO / iSRO-R packet definitions and handlers
SilkroadSecurityAPI/  Packet crypto / handshake primitives
docs/            Public product docs (e.g. serveraddon-bridge.md)
```

> `DuckSoup` is retained as the **filter database schema name** (per the design spec
> the canonical filter DB is `DuckSoup`). The product/brand layer is **KitoGuard-S500**.

## Languages

The web panel ships with English (default, LTR), Arabic (RTL with mirrored sidebar and
auto-flipped directional icons), and Turkish (LTR) from day one. **No hardcoded UI
strings** — every label lives in `apps/web/messages/{en,ar,tr}.json`.

## Quick start (web panel)

```bash
pnpm install
pnpm web:dev        # http://localhost:3000 — auto-redirects to /en
pnpm web:typecheck
pnpm web:lint
pnpm web:build
```

## Stack

| Layer    | Tech |
| -------- | ---- |
| Frontend | Next.js 15 (App Router) + TypeScript + Tailwind + shadcn/ui + TanStack Query/Table + Recharts + Zustand + react-hook-form + zod + `next-intl` (en/ar/tr, RTL) + lucide-react |
| Backend  | ASP.NET Core 8 minimal APIs + EF Core 8 + SignalR + Serilog + Quartz + McMaster plugins + Watson + SilkroadSecurityAPI |
| Database | MSSQL 2019 — `DuckSoup` (filter DB) + upstream Silkroad DBs (`SRO_VT_SHARD`, `SRO_VT_ACCOUNT`, `SRO_VT_LOG`, `SRO_VT_LOGIN`, `SRO_GAMESERVER`) |
| Auth     | ASP.NET Identity + roles (SuperAdmin / Admin / Operator / Viewer) + optional 2FA + captcha on `/login` |
| Build    | pnpm workspaces (`apps/web`) + dotnet 8 (`apps/api`) |

## Credits & attribution

KitoGuard-S500 builds on the open-source **DuckSoup** filter framework. Original
DuckSoup acknowledgements:

- qqdev — kind words, motivation, Q&A help.
- [Devsome](https://github.com/Devsome/) — kind words, motivation, ideas. [Silkroad Laravel](https://github.com/Devsome/silkroad-laravel).
- [florian0](https://gitlab.com/florian0/) — kind words, motivation, ideas. [SRO_DevKit](https://gitlab.com/florian0/sro_devkit).
- [pushedx](https://www.elitepvpers.com/forum/members/900141-pushedx.html) — original
  [SilkroadSecurityAPI](https://www.elitepvpers.com/forum/sro-coding-corner/1063078-c-silkroadsecurity.html).
- [DaxterSoul](https://www.elitepvpers.com/forum/members/1084164-daxtersoul.html) —
  [SilkroadDocs](https://github.com/DummkopfOfHachtenduden/SilkroadDoc/).
- Chernobyl — PacketHandler idea.

## License

Licensed under the _DON'T BE A DICK PUBLIC LICENSE_. See [`LICENSE.txt`](LICENSE.txt).

---

© KitoGuard-S500 · Developed by Ahmed Yasser — Owner of This System
