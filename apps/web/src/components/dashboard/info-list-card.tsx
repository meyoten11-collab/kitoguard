import { useTranslations } from "next-intl";

import { Card, CardBody, CardHeader, CardTitle } from "@/components/ui/card";
import type { ServerInfo } from "@/lib/types/dashboard";

export function InfoListCard({ info }: { info: ServerInfo }) {
  const t = useTranslations("dashboard.serverInfo");

  const rows = [
    { label: t("currentTime"), value: new Date(info.currentTime).toLocaleString() },
    { label: t("serverLoad"), value: `${info.serverLoadPercent}%` },
    { label: t("totalConnections"), value: info.totalConnections.toLocaleString() },
    { label: t("peak24h"), value: info.peakConnections24h.toLocaleString() },
  ];

  return (
    <Card>
      <CardHeader>
        <CardTitle>{t("title")}</CardTitle>
        <span className="text-xs text-zinc-500">{info.shardName} · {info.shardRegion}</span>
      </CardHeader>
      <CardBody>
        <dl className="space-y-2 text-sm">
          {rows.map((row) => (
            <div key={row.label} className="flex items-baseline justify-between gap-3">
              <dt className="text-zinc-500 dark:text-zinc-400">{row.label}</dt>
              <dd className="font-medium tabular-nums text-zinc-900 dark:text-zinc-50">{row.value}</dd>
            </div>
          ))}
        </dl>
      </CardBody>
    </Card>
  );
}
