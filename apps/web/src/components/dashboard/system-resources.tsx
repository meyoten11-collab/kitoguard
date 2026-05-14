"use client";

import { Cpu, HardDrive, MemoryStick, Wifi } from "lucide-react";
import { useTranslations } from "next-intl";
import { useEffect, useRef, useState } from "react";

import { Card, CardBody, CardHeader, CardTitle } from "@/components/ui/card";
import type { MetricsTick, SystemResourcesSnapshot } from "@/lib/types/dashboard";
import { cn } from "@/lib/utils";

interface Bar {
  key: keyof SystemResourcesSnapshot;
  label: string;
  value: number;
  icon: typeof Cpu;
  unit: string;
  max: number;
  tone: string;
}

function tone(percent: number): string {
  if (percent >= 80) return "bg-red-500";
  if (percent >= 65) return "bg-amber-500";
  return "bg-emerald-500";
}

export function SystemResources({ initial }: { initial: SystemResourcesSnapshot }) {
  const t = useTranslations("dashboard.systemResources");
  const [snap, setSnap] = useState<SystemResourcesSnapshot>(initial);
  const sourceRef = useRef<EventSource | null>(null);

  useEffect(() => {
    const es = new EventSource("/api/v1/hubs/metrics");
    sourceRef.current = es;
    const handler = (event: MessageEvent) => {
      try {
        const tick = JSON.parse(event.data) as MetricsTick;
        setSnap({
          cpuPercent: tick.cpuPercent,
          ramPercent: tick.ramPercent,
          diskPercent: tick.diskPercent,
          networkMbps: tick.networkMbps,
        });
      } catch {
        // ignore
      }
    };
    es.addEventListener("tick", handler);
    es.onerror = () => es.close();
    return () => {
      es.removeEventListener("tick", handler);
      es.close();
    };
  }, []);

  const bars: Bar[] = [
    { key: "cpuPercent", label: t("cpu"), value: snap.cpuPercent, icon: Cpu, unit: "%", max: 100, tone: tone(snap.cpuPercent) },
    { key: "ramPercent", label: t("ram"), value: snap.ramPercent, icon: MemoryStick, unit: "%", max: 100, tone: tone(snap.ramPercent) },
    { key: "diskPercent", label: t("disk"), value: snap.diskPercent, icon: HardDrive, unit: "%", max: 100, tone: tone(snap.diskPercent) },
    {
      key: "networkMbps",
      label: t("network"),
      value: snap.networkMbps,
      icon: Wifi,
      unit: "Mbps",
      max: 1000,
      tone: tone((snap.networkMbps / 1000) * 100),
    },
  ];

  return (
    <Card className="flex h-full flex-col">
      <CardHeader>
        <CardTitle>{t("title")}</CardTitle>
      </CardHeader>
      <CardBody className="flex-1 space-y-3">
        {bars.map((bar) => {
          const Icon = bar.icon;
          const pct = Math.min(100, Math.round((bar.value / bar.max) * 100));
          return (
            <div key={bar.key} className="space-y-1.5">
              <div className="flex items-center justify-between gap-2 text-xs">
                <span className="inline-flex items-center gap-1.5 font-medium text-zinc-700 dark:text-zinc-200">
                  <Icon aria-hidden="true" className="h-3.5 w-3.5 text-zinc-400" />
                  {bar.label}
                </span>
                <span className="font-mono tabular-nums text-zinc-600 dark:text-zinc-300">
                  {bar.value}
                  {bar.unit}
                </span>
              </div>
              <div className="h-1.5 w-full overflow-hidden rounded-full bg-zinc-100 dark:bg-zinc-800">
                <div className={cn("h-full transition-all", bar.tone)} style={{ width: `${pct}%` }} />
              </div>
            </div>
          );
        })}
      </CardBody>
    </Card>
  );
}
