import { ArrowRight, type LucideIcon } from "lucide-react";
import type { ReactNode } from "react";

import { Link } from "@/i18n/navigation";
import { cn } from "@/lib/utils";

export type KpiAccent = "blue" | "green" | "orange" | "purple" | "teal" | "pink";

const ACCENT_GRADIENTS: Record<KpiAccent, string> = {
  blue: "from-[#1E3A8A] to-[#3B82F6]",
  green: "from-[#14532D] to-[#22C55E]",
  orange: "from-[#7C2D12] to-[#F97316]",
  purple: "from-[#581C87] to-[#A855F7]",
  teal: "from-[#0F766E] to-[#06B6D4]",
  pink: "from-[#9F1239] to-[#EC4899]",
};

const ACCENT_RING: Record<KpiAccent, string> = {
  blue: "ring-blue-300/30",
  green: "ring-emerald-300/30",
  orange: "ring-orange-300/30",
  purple: "ring-fuchsia-300/30",
  teal: "ring-cyan-300/30",
  pink: "ring-pink-300/30",
};

export interface KpiCardProps {
  label: string;
  value: string | number;
  delta?: ReactNode;
  icon: LucideIcon;
  accent: KpiAccent;
  href: string;
  footerLabel: string;
  className?: string;
}

export function KpiCard({
  label,
  value,
  delta,
  icon: Icon,
  accent,
  href,
  footerLabel,
  className,
}: KpiCardProps) {
  return (
    <div
      className={cn(
        "relative flex flex-col gap-3 overflow-hidden rounded-xl bg-gradient-to-br p-5 text-white shadow-lg ring-1",
        ACCENT_GRADIENTS[accent],
        ACCENT_RING[accent],
        className,
      )}
    >
      <div className="flex items-start justify-between gap-2">
        <p className="text-xs font-semibold uppercase tracking-wider text-white/80">{label}</p>
        <Icon aria-hidden="true" className="dir-flip h-5 w-5 text-white/70" />
      </div>
      <p className="text-4xl font-extrabold leading-none tracking-tight" data-testid={`kpi-${accent}-value`}>
        {value}
      </p>
      {delta ? <p className="text-xs font-medium text-white/85">{delta}</p> : <p className="text-xs text-white/0">—</p>}
      <Link
        href={href}
        className="mt-auto inline-flex items-center gap-1 text-xs font-semibold text-white/95 hover:text-white"
      >
        <span>{footerLabel}</span>
        <ArrowRight aria-hidden="true" className="dir-flip h-3.5 w-3.5" />
      </Link>
    </div>
  );
}
