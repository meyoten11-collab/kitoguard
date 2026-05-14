"use client";

import { useQuery } from "@tanstack/react-query";
import { formatDistanceToNowStrict } from "date-fns";
import { ArrowRight, Info, ShieldAlert, ShieldCheck, ShieldX } from "lucide-react";
import { useTranslations } from "next-intl";

import { Card, CardBody, CardHeader, CardTitle } from "@/components/ui/card";
import { Link } from "@/i18n/navigation";
import { cn } from "@/lib/utils";
import type { LogRow, LogSeverity } from "@/lib/types/dashboard";

const SEVERITY_META: Record<
  LogSeverity,
  { icon: typeof Info; tone: string; ringTone: string }
> = {
  info: {
    icon: Info,
    tone: "text-sky-600 dark:text-sky-300",
    ringTone: "bg-sky-50 dark:bg-sky-500/15",
  },
  success: {
    icon: ShieldCheck,
    tone: "text-emerald-600 dark:text-emerald-300",
    ringTone: "bg-emerald-50 dark:bg-emerald-500/15",
  },
  warn: {
    icon: ShieldAlert,
    tone: "text-amber-600 dark:text-amber-300",
    ringTone: "bg-amber-50 dark:bg-amber-500/15",
  },
  error: {
    icon: ShieldX,
    tone: "text-red-600 dark:text-red-300",
    ringTone: "bg-red-50 dark:bg-red-500/15",
  },
};

async function fetchLogs(limit: number): Promise<LogRow[]> {
  const res = await fetch(`/api/v1/logs/recent?limit=${limit}`, { cache: "no-store" });
  if (!res.ok) throw new Error("logs_load_failed");
  return (await res.json()) as LogRow[];
}

export function LogsTimeline({ initial, limit = 5 }: { initial: LogRow[]; limit?: number }) {
  const t = useTranslations("dashboard.logs");
  const tSeverity = (key: LogSeverity) => t(key);

  const { data } = useQuery({
    queryKey: ["logs-recent", limit],
    queryFn: () => fetchLogs(limit),
    initialData: initial,
    refetchInterval: 10_000,
  });

  const rows = data ?? initial;

  return (
    <Card className="flex h-full flex-col">
      <CardHeader>
        <CardTitle>{t("title")}</CardTitle>
        <Link
          href="/system-logs"
          className="inline-flex items-center gap-1 text-xs font-semibold text-brand-700 hover:text-brand dark:text-brand-300"
        >
          <span>{t("viewAll")}</span>
          <ArrowRight aria-hidden="true" className="dir-flip h-3.5 w-3.5" />
        </Link>
      </CardHeader>
      <CardBody className="flex-1 p-0">
        <ol className="divide-y divide-zinc-200 dark:divide-zinc-800">
          {rows.map((row) => {
            const meta = SEVERITY_META[row.severity];
            const Icon = meta.icon;
            return (
              <li key={row.id} className="flex items-start gap-3 px-5 py-3">
                <span
                  className={cn(
                    "mt-0.5 flex h-7 w-7 shrink-0 items-center justify-center rounded-full",
                    meta.ringTone,
                  )}
                >
                  <Icon aria-hidden="true" className={cn("h-3.5 w-3.5", meta.tone)} />
                </span>
                <div className="min-w-0 flex-1">
                  <div className="flex items-baseline justify-between gap-3">
                    <span className={cn("text-xs font-semibold uppercase tracking-wide", meta.tone)}>
                      {tSeverity(row.severity)}
                    </span>
                    <time className="text-xs text-zinc-500 dark:text-zinc-400" dateTime={row.ts}>
                      {formatDistanceToNowStrict(new Date(row.ts), { addSuffix: true })}
                    </time>
                  </div>
                  <p className="truncate text-sm text-zinc-800 dark:text-zinc-100">{row.message}</p>
                  <p className="text-xs text-zinc-500 dark:text-zinc-400">{row.source}</p>
                </div>
              </li>
            );
          })}
        </ol>
      </CardBody>
    </Card>
  );
}
