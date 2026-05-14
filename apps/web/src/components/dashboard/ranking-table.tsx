import { ArrowRight, Crown } from "lucide-react";
import { useTranslations } from "next-intl";

import { Card, CardBody, CardHeader, CardTitle } from "@/components/ui/card";
import { Link } from "@/i18n/navigation";
import type { RankingRow } from "@/lib/types/dashboard";

const RANK_TONES = [
  "bg-amber-500/20 text-amber-600 dark:text-amber-300",
  "bg-zinc-300/40 text-zinc-700 dark:text-zinc-200",
  "bg-orange-500/20 text-orange-600 dark:text-orange-300",
];

export function RankingTable({ rows }: { rows: RankingRow[] }) {
  const t = useTranslations("dashboard.ranking");

  return (
    <Card className="flex h-full flex-col">
      <CardHeader>
        <CardTitle>{t("title")}</CardTitle>
        <Link
          href="/custom-ranking"
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
              <th scope="col" className="px-5 py-2 text-start font-medium">{t("rank")}</th>
              <th scope="col" className="px-2 py-2 text-start font-medium">{t("player")}</th>
              <th scope="col" className="px-2 py-2 text-end font-medium">{t("level")}</th>
              <th scope="col" className="px-5 py-2 text-end font-medium">{t("kills")}</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-zinc-200 dark:divide-zinc-800">
            {rows.map((row) => (
              <tr key={row.charId} className="hover:bg-zinc-50 dark:hover:bg-zinc-800/40">
                <td className="px-5 py-2">
                  <span
                    className={`inline-flex h-6 min-w-6 items-center justify-center rounded-full px-1.5 text-xs font-semibold ${
                      RANK_TONES[row.rank - 1] ?? "bg-zinc-200/40 text-zinc-600 dark:text-zinc-300"
                    }`}
                  >
                    {row.rank === 1 ? <Crown aria-hidden="true" className="h-3.5 w-3.5" /> : row.rank}
                  </span>
                </td>
                <td className="px-2 py-2 font-medium text-zinc-800 dark:text-zinc-100">
                  <div className="flex items-center gap-2">
                    <span>{row.name}</span>
                    {row.guildTag ? (
                      <span className="rounded bg-zinc-100 px-1.5 py-0.5 text-[10px] uppercase tracking-wide text-zinc-600 dark:bg-zinc-800 dark:text-zinc-300">
                        {row.guildTag}
                      </span>
                    ) : null}
                  </div>
                </td>
                <td className="px-2 py-2 text-end tabular-nums text-zinc-600 dark:text-zinc-300">{row.level}</td>
                <td className="px-5 py-2 text-end font-semibold tabular-nums">{row.kills.toLocaleString()}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </CardBody>
    </Card>
  );
}
