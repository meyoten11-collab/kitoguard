import { cn } from "@/lib/utils";
import type { FilterServiceState } from "@/lib/types/dashboard";

const TONES: Record<FilterServiceState, string> = {
  online: "bg-emerald-500 shadow-[0_0_0_3px_rgba(16,185,129,0.18)]",
  degraded: "bg-amber-500 shadow-[0_0_0_3px_rgba(245,158,11,0.18)]",
  offline: "bg-zinc-400 shadow-[0_0_0_3px_rgba(161,161,170,0.18)]",
};

export function StatusDot({
  state,
  size = "md",
  className,
}: {
  state: FilterServiceState;
  size?: "sm" | "md";
  className?: string;
}) {
  return (
    <span
      aria-hidden="true"
      className={cn(
        "inline-block rounded-full",
        size === "sm" ? "h-2 w-2" : "h-2.5 w-2.5",
        TONES[state],
        className,
      )}
    />
  );
}
