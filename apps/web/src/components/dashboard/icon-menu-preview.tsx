import { ArrowRight, Image as ImageIcon } from "lucide-react";
import { useTranslations } from "next-intl";

import { Card, CardBody, CardHeader, CardTitle } from "@/components/ui/card";
import { Link } from "@/i18n/navigation";
import type { IconMenuLayout } from "@/lib/types/dashboard";
import { cn } from "@/lib/utils";

export function IconMenuPreview({ layout }: { layout: IconMenuLayout }) {
  const t = useTranslations("dashboard.iconMenu");

  return (
    <Card className="flex h-full flex-col">
      <CardHeader>
        <div>
          <CardTitle>{t("title")}</CardTitle>
          <p className="mt-1 text-xs text-zinc-500 dark:text-zinc-400">{t("subtitle")}</p>
        </div>
        <Link
          href="/icon-manager"
          className="inline-flex items-center gap-1 text-xs font-semibold text-brand-700 hover:text-brand dark:text-brand-300"
        >
          <span>{t("customize")}</span>
          <ArrowRight aria-hidden="true" className="dir-flip h-3.5 w-3.5" />
        </Link>
      </CardHeader>
      <CardBody className="flex-1">
        <div className="grid grid-cols-4 gap-2">
          {layout.tiles.map((tile) => {
            const filled = tile.iconId != null;
            return (
              <div
                key={tile.slotIndex}
                title={tile.label ?? t("emptySlot")}
                className={cn(
                  "flex aspect-square items-center justify-center rounded-md border text-[10px] font-semibold uppercase tracking-tight",
                  filled
                    ? "border-brand/30 bg-brand/10 text-brand-700 dark:text-brand-300"
                    : "border-dashed border-zinc-200 text-zinc-300 dark:border-zinc-700 dark:text-zinc-600",
                )}
                data-testid={`icon-slot-${tile.slotIndex}`}
                aria-label={tile.label ?? t("emptySlot")}
              >
                {filled ? (
                  <span className="flex flex-col items-center gap-1">
                    <ImageIcon aria-hidden="true" className="h-4 w-4" />
                    <span className="text-[9px]">{tile.label?.slice(0, 5)}</span>
                  </span>
                ) : (
                  <span aria-hidden="true">+</span>
                )}
              </div>
            );
          })}
        </div>
      </CardBody>
    </Card>
  );
}
