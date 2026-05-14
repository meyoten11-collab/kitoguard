"use client";

import { useEffect, useMemo, useRef, useState } from "react";
import { useTranslations } from "next-intl";
import {
  CartesianGrid,
  Legend,
  Line,
  LineChart,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from "recharts";

import { Card, CardBody, CardHeader, CardTitle } from "@/components/ui/card";
import type { MetricsTick } from "@/lib/types/dashboard";

const MAX_POINTS = 60;

export function LiveChart({ seed }: { seed: MetricsTick[] }) {
  const t = useTranslations("dashboard.liveChart");
  const [points, setPoints] = useState<MetricsTick[]>(() => seed.slice(-MAX_POINTS));
  const sourceRef = useRef<EventSource | null>(null);

  useEffect(() => {
    const es = new EventSource("/api/v1/hubs/metrics");
    sourceRef.current = es;

    const handler = (event: MessageEvent) => {
      try {
        const tick = JSON.parse(event.data) as MetricsTick;
        setPoints((prev) => {
          const next = [...prev, tick];
          return next.length > MAX_POINTS ? next.slice(-MAX_POINTS) : next;
        });
      } catch {
        // ignore malformed payloads
      }
    };

    es.addEventListener("tick", handler);
    es.onerror = () => {
      es.close();
    };

    return () => {
      es.removeEventListener("tick", handler);
      es.close();
      sourceRef.current = null;
    };
  }, []);

  const chartData = useMemo(
    () =>
      points.map((p) => ({
        ts: new Date(p.ts).toLocaleTimeString(undefined, { hour: "2-digit", minute: "2-digit" }),
        players: p.players,
        connections: p.connections,
        pps: p.packetsPerSec,
      })),
    [points],
  );

  return (
    <Card className="flex h-full flex-col">
      <CardHeader>
        <div>
          <CardTitle>{t("title")}</CardTitle>
          <p className="mt-1 text-xs text-zinc-500 dark:text-zinc-400">{t("subtitle")}</p>
        </div>
        <span className="inline-flex items-center gap-1.5 rounded-full bg-emerald-50 px-2.5 py-0.5 text-xs font-semibold text-emerald-700 dark:bg-emerald-500/15 dark:text-emerald-300">
          <span className="h-1.5 w-1.5 animate-pulse rounded-full bg-emerald-500" aria-hidden="true" />
          {t("live")}
        </span>
      </CardHeader>
      <CardBody className="flex-1">
        <div className="h-64 w-full">
          <ResponsiveContainer width="100%" height="100%">
            <LineChart data={chartData} margin={{ top: 10, right: 12, left: -12, bottom: 0 }}>
              <CartesianGrid stroke="currentColor" strokeOpacity={0.08} vertical={false} />
              <XAxis dataKey="ts" tick={{ fontSize: 11, fill: "currentColor", fillOpacity: 0.6 }} tickLine={false} axisLine={false} minTickGap={32} />
              <YAxis tick={{ fontSize: 11, fill: "currentColor", fillOpacity: 0.6 }} tickLine={false} axisLine={false} width={40} />
              <Tooltip
                contentStyle={{
                  background: "rgba(24,24,27,0.92)",
                  border: "1px solid rgba(255,255,255,0.08)",
                  borderRadius: "0.5rem",
                  color: "#fafafa",
                  fontSize: "0.75rem",
                }}
                labelStyle={{ color: "rgba(255,255,255,0.7)" }}
              />
              <Legend wrapperStyle={{ fontSize: "0.7rem" }} iconSize={10} />
              <Line type="monotone" dataKey="players" name={t("playersSeries")} stroke="#3B82F6" strokeWidth={2} dot={false} isAnimationActive={false} />
              <Line type="monotone" dataKey="connections" name={t("connectionsSeries")} stroke="#22C55E" strokeWidth={2} dot={false} isAnimationActive={false} />
              <Line type="monotone" dataKey="pps" name={t("ppsSeries")} stroke="#F97316" strokeWidth={2} dot={false} isAnimationActive={false} />
            </LineChart>
          </ResponsiveContainer>
        </div>
      </CardBody>
    </Card>
  );
}
