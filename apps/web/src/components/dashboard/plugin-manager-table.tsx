"use client";

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { ArrowRight, Power } from "lucide-react";
import { useTranslations } from "next-intl";

import { Button } from "@/components/ui/button";
import { Card, CardBody, CardHeader, CardTitle } from "@/components/ui/card";
import { Link } from "@/i18n/navigation";
import type { PluginRow } from "@/lib/types/dashboard";
import { cn } from "@/lib/utils";

async function fetchPlugins(limit: number): Promise<PluginRow[]> {
  const res = await fetch(`/api/v1/plugins?limit=${limit}`, { cache: "no-store" });
  if (!res.ok) throw new Error("plugins_load_failed");
  return (await res.json()) as PluginRow[];
}

async function togglePlugin(input: { id: string; enabled: boolean }): Promise<PluginRow> {
  const path = input.enabled ? "enable" : "disable";
  const res = await fetch(`/api/v1/plugins/${encodeURIComponent(input.id)}/${path}`, { method: "POST" });
  if (!res.ok) throw new Error("plugin_toggle_failed");
  return (await res.json()) as PluginRow;
}

export function PluginManagerTable({ initial, limit = 5 }: { initial: PluginRow[]; limit?: number }) {
  const t = useTranslations("dashboard.plugins");
  const queryClient = useQueryClient();

  const { data } = useQuery({
    queryKey: ["plugins-list", limit],
    queryFn: () => fetchPlugins(limit),
    initialData: initial,
    refetchInterval: 15_000,
  });

  const mutate = useMutation({
    mutationFn: togglePlugin,
    onSuccess: (next) => {
      const rows = queryClient.getQueryData<PluginRow[]>(["plugins-list", limit]) ?? [];
      queryClient.setQueryData(
        ["plugins-list", limit],
        rows.map((row) => (row.id === next.id ? next : row)),
      );
    },
  });

  const rows = data ?? initial;

  return (
    <Card className="flex h-full flex-col">
      <CardHeader>
        <CardTitle>{t("title")}</CardTitle>
        <Link
          href="/plugin-manager"
          className="inline-flex items-center gap-1 text-xs font-semibold text-brand-700 hover:text-brand dark:text-brand-300"
        >
          <span>{t("viewAll")}</span>
          <ArrowRight aria-hidden="true" className="dir-flip h-3.5 w-3.5" />
        </Link>
      </CardHeader>
      <CardBody className="flex-1 p-0">
        <table className="w-full text-sm">
          <thead className="border-b border-zinc-200 text-xs uppercase text-zinc-500 dark:border-zinc-800 dark:text-zinc-400">
            <tr>
              <th scope="col" className="px-5 py-2 text-start font-medium">{t("name")}</th>
              <th scope="col" className="px-2 py-2 text-start font-medium">{t("version")}</th>
              <th scope="col" className="px-2 py-2 text-start font-medium">{t("status")}</th>
              <th scope="col" className="px-5 py-2 text-end font-medium">{t("actions")}</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-zinc-200 dark:divide-zinc-800">
            {rows.map((row) => (
              <tr key={row.id} className="hover:bg-zinc-50 dark:hover:bg-zinc-800/40">
                <td className="px-5 py-2 font-medium text-zinc-800 dark:text-zinc-100">
                  <div className="flex flex-col">
                    <span>{row.name}</span>
                    <span className="text-xs font-normal text-zinc-500 dark:text-zinc-400">{row.author}</span>
                  </div>
                </td>
                <td className="px-2 py-2 font-mono text-xs text-zinc-600 dark:text-zinc-300">{row.version}</td>
                <td className="px-2 py-2">
                  <span
                    className={cn(
                      "inline-flex items-center gap-1 rounded-full px-2 py-0.5 text-[10px] font-semibold uppercase",
                      row.enabled
                        ? "bg-emerald-500/15 text-emerald-700 dark:text-emerald-300"
                        : "bg-zinc-500/15 text-zinc-500 dark:text-zinc-400",
                    )}
                  >
                    {row.enabled ? t("enabled") : t("disabled")}
                  </span>
                </td>
                <td className="px-5 py-2 text-end">
                  <Button
                    type="button"
                    variant="outline"
                    size="sm"
                    disabled={mutate.isPending}
                    onClick={() => mutate.mutate({ id: row.id, enabled: !row.enabled })}
                  >
                    <Power aria-hidden="true" className="h-3.5 w-3.5" />
                    <span>{row.enabled ? t("toggleOff") : t("toggleOn")}</span>
                  </Button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </CardBody>
    </Card>
  );
}
