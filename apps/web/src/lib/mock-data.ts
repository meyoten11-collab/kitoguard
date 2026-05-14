/**
 * Deterministic-but-living mock data for the W2 §D.1 dashboard. The W3+
 * real backend will replace these generators; the API shape (see lib/types/
 * dashboard.ts) is the contract.
 */

import type {
  DashboardSummary,
  EventRow,
  FilterStatus,
  IconMenuLayout,
  IconMenuTile,
  LogRow,
  LogSeverity,
  MetricsTick,
  PluginRow,
  RankingRow,
  ServerInfo,
  SystemResourcesSnapshot,
  UniqueRow,
} from "./types/dashboard";

function seedRandom(seed: number): () => number {
  let state = seed >>> 0;
  return () => {
    state = (state * 1664525 + 1013904223) >>> 0;
    return state / 0xffffffff;
  };
}

function bucketSeed(bucketSeconds: number, salt = 0): () => number {
  const bucket = Math.floor(Date.now() / (bucketSeconds * 1000));
  return seedRandom(bucket + salt);
}

function pick<T>(rng: () => number, items: readonly T[]): T {
  return items[Math.floor(rng() * items.length)] ?? items[0];
}

function range(rng: () => number, min: number, max: number): number {
  return Math.floor(min + rng() * (max - min + 1));
}

export function getDashboardSummary(): DashboardSummary {
  const rng = bucketSeed(10);
  const onlinePlayers = range(rng, 1820, 2940);
  return {
    onlinePlayers,
    onlinePlayersDelta24h: range(rng, -60, 220),
    uptimeSeconds: 21 * 86400 + range(rng, 0, 3600 * 6),
    uptimePercent: 99.94 + rng() * 0.05,
    activeEvents: range(rng, 1, 4),
    activePlugins: range(rng, 10, 14),
    serversOnline: 3,
    serversTotal: 3,
    activeAttacks: range(rng, 0, 3),
    generatedAt: new Date().toISOString(),
  };
}

export function getFilterStatus(): FilterStatus {
  const rng = bucketSeed(30);
  const states = ["online", "online", "online", "degraded"] as const;
  return {
    gateway: "online",
    agent: pick(rng, states),
    download: "online",
    filterRunning: true,
    lastChangedAt: new Date(Date.now() - range(rng, 60, 3600) * 1000).toISOString(),
    lastChangedBy: "admin",
  };
}

export function getServerInfo(): ServerInfo {
  const rng = bucketSeed(10);
  const totalConnections = range(rng, 2100, 3400);
  return {
    currentTime: new Date().toISOString(),
    serverLoadPercent: 40 + Math.floor(rng() * 35),
    totalConnections,
    peakConnections24h: totalConnections + range(rng, 100, 480),
    shardName: "Aegis-01",
    shardRegion: "EU-West",
  };
}

const LOG_SOURCES = [
  "GatewayServer",
  "AgentServer",
  "AntiCheat",
  "AutoEvents",
  "PluginHost",
  "AuditLog",
  "ChatFilter",
  "IPRateLimiter",
] as const;

const LOG_TEMPLATES: ReadonlyArray<{ severity: LogSeverity; message: string }> = [
  { severity: "info", message: "Player {player} entered region {region}" },
  { severity: "info", message: "Filter throughput {value} pps" },
  { severity: "success", message: "Event {event} started in {region}" },
  { severity: "success", message: "Plugin {plugin} enabled by {actor}" },
  { severity: "warn", message: "Connection burst from {ip} (rate-limited)" },
  { severity: "warn", message: "Detection rule '{rule}' triggered for {player}" },
  { severity: "error", message: "Auto-ban applied to {player} ({reason})" },
  { severity: "error", message: "Failed login from {ip} (captcha mismatch)" },
];

function fillTemplate(rng: () => number, template: string): string {
  return template
    .replace(/\{player\}/g, () => pick(rng, ["Akinos", "MasterChief", "Ozge", "Karim", "TenshinHan", "Sumi"]))
    .replace(/\{region\}/g, () => pick(rng, ["Jangan", "Hotan", "Constantinople", "Samarkand", "Donwhang"]))
    .replace(/\{event\}/g, () => pick(rng, ["Survival Arena", "CTF", "Job War", "Fortress War"]))
    .replace(/\{plugin\}/g, () => pick(rng, ["AutoNotice", "UniqueTracker", "IconManager", "AntiCheat"]))
    .replace(/\{actor\}/g, () => pick(rng, ["admin", "operator-01", "super"]))
    .replace(/\{ip\}/g, () => `${range(rng, 10, 250)}.${range(rng, 10, 250)}.${range(rng, 10, 250)}.${range(rng, 10, 250)}`)
    .replace(/\{rule\}/g, () => pick(rng, ["AutoPickupSpam", "ImpossibleMovement", "PacketAnomaly", "SkillTimingScript"]))
    .replace(/\{reason\}/g, () => pick(rng, ["bot pattern", "memory tamper", "speed hack"]))
    .replace(/\{value\}/g, () => String(range(rng, 1200, 4800)));
}

export function getRecentLogs(limit = 5): LogRow[] {
  const rng = bucketSeed(15);
  const rows: LogRow[] = [];
  for (let i = 0; i < limit; i += 1) {
    const template = pick(rng, LOG_TEMPLATES);
    rows.push({
      id: `log-${i}-${Math.floor(Date.now() / 15000)}`,
      ts: new Date(Date.now() - i * range(rng, 30_000, 90_000)).toISOString(),
      severity: template.severity,
      source: pick(rng, LOG_SOURCES),
      message: fillTemplate(rng, template.message),
    });
  }
  return rows;
}

const RANKING_NAMES = [
  "Akinos",
  "TenshinHan",
  "MasterChief",
  "Ozge",
  "Karim",
  "Sumi",
  "Sephir",
  "Zylo",
  "Tarkan",
  "Kuro",
];
const RANKING_GUILDS = ["KG", "S500", "Aegis", null, "Vox", "Solo"];

export function getTopPlayers(limit = 5): RankingRow[] {
  const rng = bucketSeed(60);
  return Array.from({ length: limit }, (_, i) => ({
    rank: i + 1,
    charId: 10_000 + i,
    name: RANKING_NAMES[i % RANKING_NAMES.length] ?? `Player${i}`,
    level: range(rng, 110, 130),
    kills: range(rng, 800 - i * 80, 1200 - i * 80),
    guildTag: pick(rng, RANKING_GUILDS),
  }));
}

const EVENT_DEFS: Array<{ id: string; nameKey: string; duration: number }> = [
  { id: "evt-survival", nameKey: "events.survivalArena", duration: 1800 },
  { id: "evt-ctf", nameKey: "events.captureTheFlag", duration: 1500 },
  { id: "evt-jw", nameKey: "events.jobWar", duration: 3600 },
  { id: "evt-ftw", nameKey: "events.fortressWar", duration: 5400 },
];

export function getActiveEvents(): EventRow[] {
  const rng = bucketSeed(30);
  const now = Date.now();
  return EVENT_DEFS.map((def, i) => {
    const offsetMin = (i - 1) * 10;
    const startsAt = new Date(now + offsetMin * 60_000);
    const endsAt = new Date(startsAt.getTime() + def.duration * 1000);
    const state: EventRow["state"] =
      endsAt.getTime() < now ? "ended" : startsAt.getTime() > now ? "scheduled" : "live";
    return {
      id: def.id,
      nameKey: def.nameKey,
      state,
      startsAt: startsAt.toISOString(),
      endsAt: endsAt.toISOString(),
      registered: range(rng, 40, 220),
      capacity: 240,
    };
  });
}

const PLUGIN_DEFS: PluginRow[] = [
  {
    id: "kg.auto-notice",
    name: "AutoNotice",
    version: "1.4.2",
    author: "KitoGuard core",
    enabled: true,
    serverType: "Agent",
    description: "Scheduled in-game notices and Discord fan-out.",
  },
  {
    id: "kg.unique-tracker",
    name: "UniqueTracker",
    version: "2.1.0",
    author: "KitoGuard core",
    enabled: true,
    serverType: "Agent",
    description: "Tracks unique spawns and broadcasts kill events.",
  },
  {
    id: "kg.icon-manager",
    name: "IconManager",
    version: "1.0.6",
    author: "KitoGuard core",
    enabled: true,
    serverType: "Agent",
    description: "Right-side in-game icon menu (16 slots).",
  },
  {
    id: "kg.anti-cheat",
    name: "AntiCheat",
    version: "3.2.1",
    author: "KitoGuard core",
    enabled: true,
    serverType: "Gateway",
    description: "Detection rules + auto-ban worker.",
  },
  {
    id: "kg.event-runner",
    name: "AutoEvents",
    version: "1.7.0",
    author: "KitoGuard core",
    enabled: false,
    serverType: "Agent",
    description: "Survival / CTF / Job War / Fortress War scheduler.",
  },
];

export function getPlugins(limit?: number): PluginRow[] {
  return typeof limit === "number" ? PLUGIN_DEFS.slice(0, limit) : PLUGIN_DEFS;
}

export function getSystemResources(): SystemResourcesSnapshot {
  const rng = bucketSeed(5);
  return {
    cpuPercent: 30 + Math.floor(rng() * 35),
    ramPercent: 55 + Math.floor(rng() * 25),
    diskPercent: 41 + Math.floor(rng() * 6),
    networkMbps: 40 + Math.floor(rng() * 80),
  };
}

const UNIQUE_DEFS: Array<{ id: string; nameKey: string; location: string }> = [
  { id: "u-tg", nameKey: "uniques.tigerGirl", location: "Tibet" },
  { id: "u-cerb", nameKey: "uniques.cerberus", location: "Karakorum" },
  { id: "u-ivy", nameKey: "uniques.captainIvy", location: "Donwhang" },
  { id: "u-uru", nameKey: "uniques.uruchi", location: "Constantinople" },
  { id: "u-isy", nameKey: "uniques.isyutaru", location: "Hotan" },
  { id: "u-ly", nameKey: "uniques.lordYarkan", location: "Yarkand" },
  { id: "u-ds", nameKey: "uniques.demonShaitan", location: "Roc Mountain" },
  { id: "u-by", nameKey: "uniques.bonelordYuno", location: "Samarkand" },
  { id: "u-roc", nameKey: "uniques.roc", location: "Roc Mountain" },
  { id: "u-wk", nameKey: "uniques.whiteKnight", location: "Constantinople" },
];

export function getUniques(limit = 6): UniqueRow[] {
  const rng = bucketSeed(45);
  const now = Date.now();
  return UNIQUE_DEFS.slice(0, limit).map((def, i) => {
    const alive = rng() > 0.4;
    return {
      id: def.id,
      nameKey: def.nameKey,
      status: alive ? "alive" : "dead",
      location: def.location,
      respawnAt: alive ? null : new Date(now + range(rng, 60, 1800) * 1000).toISOString(),
      updatedAt: new Date(now - i * 60_000).toISOString(),
    };
  });
}

const ICON_TILE_DEFS: ReadonlyArray<{ label: string; mediaPath: string; type: IconMenuTile["type"] }> = [
  { label: "Storage", mediaPath: "icon\\xi_warehouse.ddj", type: 1 },
  { label: "Stall", mediaPath: "icon\\xi_stall.ddj", type: 1 },
  { label: "Auction", mediaPath: "icon\\xi_auction.ddj", type: 1 },
  { label: "Mailbox", mediaPath: "icon\\xi_mail.ddj", type: 1 },
  { label: "Guild", mediaPath: "icon\\xi_guild.ddj", type: 1 },
  { label: "Trade", mediaPath: "icon\\xi_trade.ddj", type: 1 },
  { label: "Reward", mediaPath: "icon\\xi_reward.ddj", type: 2 },
  { label: "Event", mediaPath: "icon\\xi_event.ddj", type: 2 },
  { label: "Ranking", mediaPath: "icon\\xi_ranking.ddj", type: 2 },
  { label: "Premium", mediaPath: "icon\\xi_premium.ddj", type: 2 },
  { label: "Shop", mediaPath: "icon\\xi_shop.ddj", type: 1 },
  { label: "VIP", mediaPath: "icon\\xi_vip.ddj", type: 3 },
];

export function getIconMenu(): IconMenuLayout {
  const tiles: IconMenuTile[] = [];
  for (let slot = 0; slot < 16; slot += 1) {
    const def = ICON_TILE_DEFS[slot];
    if (def) {
      tiles.push({
        slotIndex: slot,
        iconId: 1000 + slot,
        mediaPath: def.mediaPath,
        label: def.label,
        type: def.type,
      });
    } else {
      tiles.push({ slotIndex: slot, iconId: null, mediaPath: null, label: null, type: null });
    }
  }
  return { rows: 4, cols: 4, tiles };
}

export function generateMetricsTick(now = Date.now()): MetricsTick {
  const rng = seedRandom(Math.floor(now / 5000));
  return {
    ts: new Date(now).toISOString(),
    players: 1900 + Math.floor(rng() * 600),
    connections: 2200 + Math.floor(rng() * 700),
    packetsPerSec: 4500 + Math.floor(rng() * 2200),
    cpuPercent: 35 + Math.floor(rng() * 30),
    ramPercent: 60 + Math.floor(rng() * 20),
    diskPercent: 42 + Math.floor(rng() * 4),
    networkMbps: 45 + Math.floor(rng() * 80),
  };
}

/**
 * 60 ticks @ 1-minute spacing for the LiveChart seed series. The SignalR-style
 * SSE stream appends one new tick every few seconds on top of this.
 */
export function getMetricsSeed(points = 60): MetricsTick[] {
  const out: MetricsTick[] = [];
  const now = Date.now();
  for (let i = points - 1; i >= 0; i -= 1) {
    out.push(generateMetricsTick(now - i * 60_000));
  }
  return out;
}
