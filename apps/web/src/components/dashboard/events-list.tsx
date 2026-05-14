"use client";

import { ArrowRight, CalendarClock, Flag, Sword, Trophy } from "lucide-react";
import { useTranslations } from "next-intl";
import { useEffect, useState } from "react";

import { Card, CardBody, CardHeader, CardTitle } from "@/components/ui/card";
import { Link } from "@/i18n/navigation";
import type { EventRow, EventState } from "@/lib/types/dashboard";
import { cn } from "@/lib/utils";

const EVENT_ICONS: Record<string, typeof Sword> = {
  "events.survivalArena": Sword,
  "events.captureTheFlag": Flag,
  "events.jobWar": Trophy,
  "events.fortressWar": CalendarClock,
};

const STATE_TONE: Record<EventState, string> = {
  live: "bg-emerald-500/15 text-emerald-700 dark:text-emerald-300",
  scheduled: "bg-amber-500/15 text-amber-700 dark:text-amber-300",
  ended: "bg-zinc-500/10 text-zinc-500 dark:text-zinc-400",
};

function formatCountdown(targetMs: number, now: number): string {
  const ms = Math.max(0, targetMs - now);
  const totalSec = Math.floor(ms / 1000);
  const h = Math.floor(totalSec / 3600);
  const m = Math.floor((totalSec % 3600) / 60);
  const s = totalSec % 60;
  if (h > 0) return `${h}h ${m.toString().padStart(2, "0")}m`;
  return `${m.toString().padStart(2, "0")}:${s.toString().padStart(2, "0")}`;
}

export function EventsList({ rows }: { rows: EventRow[] }) {
  const t = useTranslations("dashboard.events");
  const tEvents = useTranslations();
  const [now, setNow] = useState<number>(() => Date.now());

  useEffect(() => {
    const id = window.setInterval(() => setNow(Date.now()), 1000);
    return () => window.clearInterval(id);
  }, []);

  return (
    <Card className="flex h-full flex-col">
      <CardHeader>
        <CardTitle>{t("title")}</CardTitle>
        <Link
          href="/event-settings"
          className="inline-flex items-center gap-1 text-xs font-semibold text-brand-700 hover:text-brand dark:text-brand-300"
        >
          <span>{t("manage")}</span>
          <ArrowRight aria-hidden="true" className="dir-flip h-3.5 w-3.5" />
        </Link>
      </CardHeader>
      <CardBody className="flex-1 space-y-2">
        {rows.map((row) => {
          const Icon = EVENT_ICONS[row.nameKey] ?? CalendarClock;
          const target = row.state === "live" ? new Date(row.endsAt).getTime() : new Date(row.startsAt).getTime();
          const stateLabel = t(row.state);
          return (
            <div
              key={row.id}
              className="flex items-center gap-3 rounded-lg border border-zinc-200 bg-white px-3 py-2 dark:border-zinc-800 dark:bg-zinc-900/60"
            >
              <span className="flex h-9 w-9 items-center justify-center rounded-md bg-brand/10 text-brand-700 dark:text-brand-300">
                <Icon aria-hidden="true" className="h-4 w-4" />
              </span>
              <div className="min-w-0 flex-1">
                <p className="truncate text-sm font-semibold text-zinc-900 dark:text-zinc-50">{tEvents(row.nameKey)}</p>
                <p className="text-xs text-zinc-500 dark:text-zinc-400">
                  {row.registered}/{row.capacity}
                </p>
              </div>
              <div className="flex flex-col items-end gap-1">
                <span className={cn("rounded-full px-2 py-0.5 text-[10px] font-semibold uppercase tracking-wide", STATE_TONE[row.state])}>
                  {stateLabel}
                </span>
                {row.state !== "ended" ? (
                  <span className="font-mono text-xs text-zinc-600 dark:text-zinc-300">
                    {row.state === "live" ? t("endsIn") : t("startsIn")} {formatCountdown(target, now)}
                  </span>
                ) : null}
              </div>
            </div>
          );
        })}
      </CardBody>
    </Card>
  );
}
