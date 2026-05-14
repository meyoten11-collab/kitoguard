"use client";

import { useQuery } from "@tanstack/react-query";
import { Crosshair, GripVertical, Minus, RefreshCw } from "lucide-react";
import { useTranslations } from "next-intl";
import {
  type CSSProperties,
  type PointerEvent as ReactPointerEvent,
  useCallback,
  useEffect,
  useRef,
  useState,
} from "react";

import { Card, CardBody, CardHeader, CardTitle } from "@/components/ui/card";
import { StatusDot } from "@/components/ui/status-dot";
import type { UniqueRow } from "@/lib/types/dashboard";
import { cn } from "@/lib/utils";

async function fetchUniques(): Promise<UniqueRow[]> {
  const res = await fetch("/api/v1/uniques/state", { cache: "no-store" });
  if (!res.ok) throw new Error("uniques_load_failed");
  return (await res.json()) as UniqueRow[];
}

function formatRespawn(target: string | null, nowMs: number): string {
  if (!target) return "—";
  const ms = Math.max(0, new Date(target).getTime() - nowMs);
  const totalSec = Math.floor(ms / 1000);
  const m = Math.floor(totalSec / 60);
  const s = totalSec % 60;
  return `${m.toString().padStart(2, "0")}:${s.toString().padStart(2, "0")}`;
}

export function UniqueTracker({ initial }: { initial: UniqueRow[] }) {
  const t = useTranslations("dashboard.uniqueTracker");
  const tc = useTranslations("common");
  const tUniques = useTranslations();
  const [collapsed, setCollapsed] = useState(false);
  const [now, setNow] = useState<number>(() => Date.now());
  const [position, setPosition] = useState<{ x: number; y: number } | null>(null);
  const draggingRef = useRef<{ startX: number; startY: number; originX: number; originY: number } | null>(null);

  useEffect(() => {
    const id = window.setInterval(() => setNow(Date.now()), 1000);
    return () => window.clearInterval(id);
  }, []);

  const { data, refetch, isFetching } = useQuery({
    queryKey: ["uniques-state"],
    queryFn: fetchUniques,
    initialData: initial,
    refetchInterval: 12_000,
  });

  const handlePointerDown = useCallback((event: ReactPointerEvent<HTMLDivElement>) => {
    const rect = event.currentTarget.getBoundingClientRect();
    draggingRef.current = {
      startX: event.clientX,
      startY: event.clientY,
      originX: position?.x ?? rect.left,
      originY: position?.y ?? rect.top,
    };
    event.currentTarget.setPointerCapture(event.pointerId);
  }, [position]);

  const handlePointerMove = useCallback((event: ReactPointerEvent<HTMLDivElement>) => {
    const drag = draggingRef.current;
    if (!drag) return;
    const dx = event.clientX - drag.startX;
    const dy = event.clientY - drag.startY;
    setPosition({ x: drag.originX + dx, y: drag.originY + dy });
  }, []);

  const handlePointerUp = useCallback((event: ReactPointerEvent<HTMLDivElement>) => {
    draggingRef.current = null;
    event.currentTarget.releasePointerCapture(event.pointerId);
  }, []);

  const rows = data ?? initial;

  const wrapperStyle: CSSProperties = position
    ? { position: "fixed", top: position.y, left: position.x, zIndex: 30 }
    : { position: "relative" };

  return (
    <Card className="flex h-full flex-col" style={wrapperStyle}>
      <CardHeader>
        <div
          className="flex w-full cursor-grab items-start justify-between gap-3 select-none active:cursor-grabbing"
          onPointerDown={handlePointerDown}
          onPointerMove={handlePointerMove}
          onPointerUp={handlePointerUp}
          onPointerCancel={handlePointerUp}
        >
          <div className="flex items-start gap-2">
            <GripVertical aria-hidden="true" className="mt-0.5 h-4 w-4 text-zinc-400" />
            <div>
              <CardTitle>{t("title")}</CardTitle>
              <p className="mt-0.5 text-xs text-zinc-500 dark:text-zinc-400">{t("subtitle")}</p>
            </div>
          </div>
          <div className="flex items-center gap-1">
            <button
              type="button"
              aria-label={t("refresh")}
              onClick={() => {
                void refetch();
              }}
              className="rounded-md p-1.5 text-zinc-500 hover:bg-zinc-100 hover:text-brand-700 dark:hover:bg-zinc-800 dark:hover:text-brand-300"
            >
              <RefreshCw aria-hidden="true" className={cn("h-3.5 w-3.5", isFetching && "animate-spin")} />
            </button>
            <button
              type="button"
              aria-label={collapsed ? tc("manage") : tc("loading")}
              onClick={() => setCollapsed((prev) => !prev)}
              className="rounded-md p-1.5 text-zinc-500 hover:bg-zinc-100 hover:text-brand-700 dark:hover:bg-zinc-800 dark:hover:text-brand-300"
            >
              <Minus aria-hidden="true" className="h-3.5 w-3.5" />
            </button>
          </div>
        </div>
      </CardHeader>
      {!collapsed ? (
        <CardBody className="flex-1 space-y-1.5">
          {rows.map((row) => (
            <div
              key={row.id}
              className="flex items-center justify-between gap-3 rounded-md border border-zinc-200/60 bg-white px-3 py-2 text-xs dark:border-zinc-800/60 dark:bg-zinc-900/40"
            >
              <div className="flex items-center gap-2">
                <Crosshair aria-hidden="true" className="h-3.5 w-3.5 text-brand-700 dark:text-brand-300" />
                <div>
                  <p className="text-sm font-medium text-zinc-800 dark:text-zinc-100">{tUniques(row.nameKey)}</p>
                  <p className="text-[11px] text-zinc-500 dark:text-zinc-400">{row.location}</p>
                </div>
              </div>
              <div className="flex flex-col items-end gap-0.5">
                <span className="inline-flex items-center gap-1">
                  <StatusDot state={row.status === "alive" ? "online" : "offline"} size="sm" />
                  <span className="text-[11px] uppercase tracking-wide text-zinc-600 dark:text-zinc-300">
                    {row.status === "alive" ? t("alive") : t("dead")}
                  </span>
                </span>
                {row.status === "dead" ? (
                  <span className="font-mono text-[11px] text-zinc-500 dark:text-zinc-400">
                    {t("respawnIn")} {formatRespawn(row.respawnAt, now)}
                  </span>
                ) : null}
              </div>
            </div>
          ))}
        </CardBody>
      ) : null}
    </Card>
  );
}
