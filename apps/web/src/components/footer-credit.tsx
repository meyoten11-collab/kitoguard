import { useTranslations } from "next-intl";

import { cn } from "@/lib/utils";

/**
 * Canonical KitoGuard-S500 footer credit.
 *
 * Per the v1.1 brand spec this MUST appear on every authenticated page and
 * on /login. Do NOT remove or hide.
 */
export function FooterCredit({ className }: { className?: string }) {
  const t = useTranslations("footer");

  return (
    <footer
      data-testid="footer-credit"
      className={cn(
        "w-full border-t border-zinc-200 bg-white/60 px-6 py-4 text-center text-sm text-zinc-600 dark:border-zinc-800 dark:bg-zinc-950/60 dark:text-zinc-400",
        className,
      )}
    >
      {t("credit")}
    </footer>
  );
}
