import { useTranslations } from "next-intl";

import { cn } from "@/lib/utils";

/**
 * Canonical pills: "Secure | Fast | Powerful | Scalable".
 */
export function BrandPills({ className }: { className?: string }) {
  const t = useTranslations("pills");
  const items = [t("secure"), t("fast"), t("powerful"), t("scalable")];

  return (
    <ul
      className={cn(
        "flex flex-wrap items-center gap-2 text-xs font-medium",
        className,
      )}
    >
      {items.map((label) => (
        <li
          key={label}
          className="rounded-full border border-brand/30 bg-brand/5 px-3 py-1 text-brand-700 dark:border-brand/50 dark:bg-brand/10 dark:text-brand-200"
        >
          {label}
        </li>
      ))}
    </ul>
  );
}
