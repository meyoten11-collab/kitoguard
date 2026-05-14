"use client";

import { useQuery } from "@tanstack/react-query";
import { useTranslations } from "next-intl";

import { Card, CardBody, CardHeader, CardTitle } from "@/components/ui/card";
import { StatusDot } from "@/components/ui/status-dot";
import type { FilterStatus } from "@/lib/types/dashboard";

async function fetchStatus(): Promise<FilterStatus> {
  const res = await fetch("/api/v1/filter/status", { cache: "no-store" });
  if (!res.ok) throw new Error("status_load_failed");
  return (await res.json()) as FilterStatus;
}

export function StatusListCard({ initial }: { initial: FilterStatus }) {
  const t = useTranslations("dashboard.filterStatus");
  const tc = useTranslations("common");

  const { data } = useQuery({
    queryKey: ["filter-status"],
    queryFn: fetchStatus,
    initialData: initial,
    refetchInterval: 8_000,
  });

  const status = data ?? initial;

  const rows = [
    { label: t("gateway"), state: status.gateway },
    { label: t("agent"), state: status.agent },
    { label: t("download"), state: status.download },
  ] as const;

  return (
    <Card>
      <CardHeader>
        <CardTitle>{t("title")}</CardTitle>
      </CardHeader>
      <CardBody>
        <ul className="space-y-2">
          {rows.map((row) => (
            <li key={row.label} className="flex items-center justify-between gap-3 rounded-md bg-zinc-50 px-3 py-2 dark:bg-zinc-800/40">
              <span className="text-sm font-medium text-zinc-700 dark:text-zinc-200">{row.label}</span>
              <span className="inline-flex items-center gap-2 text-xs font-medium text-zinc-600 dark:text-zinc-300">
                <StatusDot state={row.state} />
                {row.state === "online" ? tc("online") : row.state === "degraded" ? tc("degraded") : tc("offline")}
              </span>
            </li>
          ))}
        </ul>
      </CardBody>
    </Card>
  );
}
