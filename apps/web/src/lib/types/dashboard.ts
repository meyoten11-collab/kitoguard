/**
 * Shared dashboard API types. Mirrors the §F.2 REST contract so the live
 * backend (W3+) can swap in without changing the components that consume
 * these shapes.
 */

export type FilterServiceState = "online" | "offline" | "degraded";

export interface DashboardSummary {
  onlinePlayers: number;
  onlinePlayersDelta24h: number;
  uptimeSeconds: number;
  uptimePercent: number;
  activeEvents: number;
  activePlugins: number;
  serversOnline: number;
  serversTotal: number;
  activeAttacks: number;
  generatedAt: string;
}

export interface FilterStatus {
  gateway: FilterServiceState;
  agent: FilterServiceState;
  download: FilterServiceState;
  filterRunning: boolean;
  lastChangedAt: string;
  lastChangedBy: string;
}

export interface ServerInfo {
  currentTime: string;
  serverLoadPercent: number;
  totalConnections: number;
  peakConnections24h: number;
  shardName: string;
  shardRegion: string;
}

export type LogSeverity = "info" | "success" | "warn" | "error";

export interface LogRow {
  id: string;
  ts: string;
  severity: LogSeverity;
  source: string;
  message: string;
}

export interface RankingRow {
  rank: number;
  charId: number;
  name: string;
  level: number;
  kills: number;
  guildTag: string | null;
}

export type EventState = "live" | "scheduled" | "ended";

export interface EventRow {
  id: string;
  nameKey: string;
  state: EventState;
  startsAt: string;
  endsAt: string;
  registered: number;
  capacity: number;
}

export interface PluginRow {
  id: string;
  name: string;
  version: string;
  author: string;
  enabled: boolean;
  serverType: "Gateway" | "Agent" | "Download" | "None";
  description: string;
}

export interface SystemResourcesSnapshot {
  cpuPercent: number;
  ramPercent: number;
  diskPercent: number;
  networkMbps: number;
}

export type UniqueStatus = "alive" | "dead";

export interface UniqueRow {
  id: string;
  nameKey: string;
  status: UniqueStatus;
  location: string;
  respawnAt: string | null;
  updatedAt: string;
}

export interface IconMenuTile {
  slotIndex: number;
  iconId: number | null;
  mediaPath: string | null;
  label: string | null;
  type: 1 | 2 | 3 | null;
}

export interface IconMenuLayout {
  rows: 4;
  cols: 4;
  tiles: IconMenuTile[];
}

export interface MetricsTick {
  ts: string;
  players: number;
  connections: number;
  packetsPerSec: number;
  cpuPercent: number;
  ramPercent: number;
  diskPercent: number;
  networkMbps: number;
}
