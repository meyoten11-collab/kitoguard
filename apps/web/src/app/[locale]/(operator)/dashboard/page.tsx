import {
  Activity,
  CalendarRange,
  Layers,
  ServerCog,
  ShieldAlert,
  Users,
} from "lucide-react";
import { getTranslations, setRequestLocale } from "next-intl/server";

import { ControlCard } from "@/components/dashboard/control-card";
import { EventsList } from "@/components/dashboard/events-list";
import { FeatureRibbon } from "@/components/dashboard/feature-ribbon";
import { IconMenuPreview } from "@/components/dashboard/icon-menu-preview";
import { InfoListCard } from "@/components/dashboard/info-list-card";
import { KpiCard } from "@/components/dashboard/kpi-card";
import { LiveChart } from "@/components/dashboard/live-chart";
import { LogsTimeline } from "@/components/dashboard/logs-timeline";
import { PluginManagerTable } from "@/components/dashboard/plugin-manager-table";
import { RankingTable } from "@/components/dashboard/ranking-table";
import { StatusListCard } from "@/components/dashboard/status-list-card";
import { SystemResources } from "@/components/dashboard/system-resources";
import { UniqueTracker } from "@/components/dashboard/unique-tracker";
import {
  getActiveEvents,
  getDashboardSummary,
  getFilterStatus,
  getIconMenu,
  getMetricsSeed,
  getPlugins,
  getRecentLogs,
  getServerInfo,
  getSystemResources,
  getTopPlayers,
  getUniques,
} from "@/lib/mock-data";
import { getFilterRuntime } from "@/lib/mock-state";

function formatUptime(seconds: number): string {
  const days = Math.floor(seconds / 86_400);
  const hours = Math.floor((seconds % 86_400) / 3600);
  return `${days}d ${hours.toString().padStart(2, "0")}h`;
}

function formatDelta(delta: number): string {
  const sign = delta > 0 ? "+" : delta < 0 ? "−" : "±";
  return `${sign}${Math.abs(delta)} (24h)`;
}

export default async function DashboardPage({
  params,
}: {
  params: Promise<{ locale: string }>;
}) {
  const { locale } = await params;
  setRequestLocale(locale);

  const [summary, baseStatus, info, logs, ranking, events, plugins, uniques, iconMenu, systemSnap, metricsSeed] = await Promise.all([
    Promise.resolve(getDashboardSummary()),
    Promise.resolve(getFilterStatus()),
    Promise.resolve(getServerInfo()),
    Promise.resolve(getRecentLogs(5)),
    Promise.resolve(getTopPlayers(5)),
    Promise.resolve(getActiveEvents()),
    Promise.resolve(getPlugins(5)),
    Promise.resolve(getUniques(6)),
    Promise.resolve(getIconMenu()),
    Promise.resolve(getSystemResources()),
    Promise.resolve(getMetricsSeed(60)),
  ]);

  const runtime = getFilterRuntime();
  const status = {
    ...baseStatus,
    filterRunning: runtime.filterRunning,
    lastChangedAt: runtime.lastChangedAt,
    lastChangedBy: runtime.lastChangedBy,
  };

  const t = await getTranslations("dashboard");

  return (
    <div className="space-y-6">
      <header className="flex flex-col gap-1">
        <h1 className="text-2xl font-bold tracking-tight text-zinc-900 dark:text-zinc-50">{t("title")}</h1>
        <p className="text-sm text-zinc-600 dark:text-zinc-300">{t("subtitle")}</p>
      </header>

      <FeatureRibbon />

      <section
        aria-label={t("kpi.onlinePlayers")}
        className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-6"
      >
        <KpiCard
          accent="blue"
          icon={Users}
          label={t("kpi.onlinePlayers")}
          value={summary.onlinePlayers.toLocaleString()}
          delta={formatDelta(summary.onlinePlayersDelta24h)}
          href="/players"
          footerLabel={t("kpi.onlinePlayersFooter")}
        />
        <KpiCard
          accent="green"
          icon={Activity}
          label={t("kpi.uptime")}
          value={`${summary.uptimePercent.toFixed(2)}%`}
          delta={formatUptime(summary.uptimeSeconds)}
          href="/server-status"
          footerLabel={t("kpi.uptimeFooter")}
        />
        <KpiCard
          accent="orange"
          icon={CalendarRange}
          label={t("kpi.activeEvents")}
          value={summary.activeEvents}
          href="/event-settings"
          footerLabel={t("kpi.activeEventsFooter")}
        />
        <KpiCard
          accent="purple"
          icon={Layers}
          label={t("kpi.activePlugins")}
          value={summary.activePlugins}
          href="/plugin-manager"
          footerLabel={t("kpi.activePluginsFooter")}
        />
        <KpiCard
          accent="teal"
          icon={ServerCog}
          label={t("kpi.serversOnline")}
          value={`${summary.serversOnline}/${summary.serversTotal}`}
          href="/server-status"
          footerLabel={t("kpi.serversOnlineFooter")}
        />
        <KpiCard
          accent="pink"
          icon={ShieldAlert}
          label={t("kpi.activeAttacks")}
          value={summary.activeAttacks}
          href="/security-dashboard"
          footerLabel={t("kpi.activeAttacksFooter")}
        />
      </section>

      <section
        aria-label={t("control.title")}
        className="grid grid-cols-1 gap-4 lg:grid-cols-3"
      >
        <ControlCard initial={status} />
        <StatusListCard initial={status} />
        <InfoListCard info={info} />
      </section>

      <section
        aria-label={t("liveChart.title")}
        className="grid grid-cols-1 gap-4 lg:grid-cols-12"
      >
        <div className="lg:col-span-8">
          <LiveChart seed={metricsSeed} />
        </div>
        <div className="lg:col-span-4">
          <SystemResources initial={systemSnap} />
        </div>
      </section>

      <section
        aria-label={t("logs.title")}
        className="grid grid-cols-1 gap-4 lg:grid-cols-3"
      >
        <LogsTimeline initial={logs} limit={5} />
        <RankingTable rows={ranking} />
        <EventsList rows={events} />
      </section>

      <section
        aria-label={t("plugins.title")}
        className="grid grid-cols-1 gap-4 lg:grid-cols-3"
      >
        <PluginManagerTable initial={plugins} limit={5} />
        <IconMenuPreview layout={iconMenu} />
        <UniqueTracker initial={uniques} />
      </section>

    </div>
  );
}
