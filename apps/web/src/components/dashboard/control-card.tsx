"use client";

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Pause, Play, RotateCw, ShieldAlert } from "lucide-react";
import { useTranslations } from "next-intl";
import { useState } from "react";

import { Button } from "@/components/ui/button";
import { Card, CardBody, CardFooter, CardHeader, CardTitle } from "@/components/ui/card";
import type { FilterStatus } from "@/lib/types/dashboard";

type Action = "start" | "stop" | "restart";

async function fetchStatus(): Promise<FilterStatus> {
  const res = await fetch("/api/v1/filter/status", { cache: "no-store" });
  if (!res.ok) {
    throw new Error("status_load_failed");
  }
  return (await res.json()) as FilterStatus;
}

async function postControl(action: Action): Promise<FilterStatus> {
  const res = await fetch(`/api/v1/filter/${action}`, { method: "POST" });
  if (!res.ok) {
    throw new Error(`control_${action}_failed`);
  }
  return (await res.json()) as FilterStatus;
}

export function ControlCard({ initial }: { initial: FilterStatus }) {
  const t = useTranslations("dashboard.control");
  const queryClient = useQueryClient();
  const [pendingAction, setPendingAction] = useState<Action | null>(null);

  const { data } = useQuery({
    queryKey: ["filter-status"],
    queryFn: fetchStatus,
    initialData: initial,
    refetchInterval: 8_000,
  });

  const mutate = useMutation({
    mutationFn: postControl,
    onMutate: (action) => setPendingAction(action),
    onSettled: (next) => {
      setPendingAction(null);
      if (next) {
        queryClient.setQueryData(["filter-status"], next);
      }
    },
  });

  const status = data ?? initial;
  const running = status.filterRunning;
  const transitioning = mutate.isPending;

  return (
    <Card>
      <CardHeader>
        <CardTitle>{t("title")}</CardTitle>
        <span className="inline-flex items-center gap-1.5 rounded-full bg-zinc-100 px-2.5 py-0.5 text-xs font-medium text-zinc-700 dark:bg-zinc-800 dark:text-zinc-200">
          <ShieldAlert aria-hidden="true" className="h-3 w-3" />
          {transitioning ? t("transitioning") : running ? t("running") : t("stopped")}
        </span>
      </CardHeader>
      <CardBody className="space-y-3">
        <div className="grid grid-cols-3 gap-2">
          <Button
            type="button"
            variant="success"
            size="md"
            disabled={running || transitioning}
            onClick={() => mutate.mutate("start")}
            aria-busy={pendingAction === "start"}
            data-testid="filter-control-start"
          >
            <Play aria-hidden="true" className="h-4 w-4" />
            <span>{t("start")}</span>
          </Button>
          <Button
            type="button"
            variant="danger"
            size="md"
            disabled={!running || transitioning}
            onClick={() => mutate.mutate("stop")}
            aria-busy={pendingAction === "stop"}
            data-testid="filter-control-stop"
          >
            <Pause aria-hidden="true" className="h-4 w-4" />
            <span>{t("stop")}</span>
          </Button>
          <Button
            type="button"
            variant="outline"
            size="md"
            disabled={transitioning}
            onClick={() => mutate.mutate("restart")}
            aria-busy={pendingAction === "restart"}
            data-testid="filter-control-restart"
          >
            <RotateCw aria-hidden="true" className="dir-flip h-4 w-4" />
            <span>{t("restart")}</span>
          </Button>
        </div>
        <dl className="grid grid-cols-1 gap-1 text-xs text-zinc-600 dark:text-zinc-400">
          <div className="flex justify-between">
            <dt>{t("lastChange")}</dt>
            <dd>{new Date(status.lastChangedAt).toLocaleString()}</dd>
          </div>
        </dl>
      </CardBody>
      <CardFooter>{t("audited")}</CardFooter>
    </Card>
  );
}
