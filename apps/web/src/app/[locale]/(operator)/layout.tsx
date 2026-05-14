import { LayoutDashboard } from "lucide-react";
import { useTranslations } from "next-intl";
import { setRequestLocale } from "next-intl/server";
import type { ReactNode } from "react";

import { BrandShield } from "@/components/brand-shield";
import { FooterCredit } from "@/components/footer-credit";
import { Link } from "@/i18n/navigation";
import { QueryProvider } from "@/lib/query-client";

/**
 * TEMPORARY operator-route layout for W2. The full §C.1 AppShell
 * (sidebar / topbar / user menu / breadcrumb / theme switcher / RBAC nav
 * filter) belongs to W1 and is owned by a sibling Devin session. This thin
 * shim renders the dashboard and any other W2+ operator pages standalone
 * until the real AppShell drops in — it intentionally re-uses the brand
 * components shipped in W1.0 (BrandShield, FooterCredit) so the look
 * matches once the real shell takes over.
 */
export default async function OperatorLayout({
  children,
  params,
}: {
  children: ReactNode;
  params: Promise<{ locale: string }>;
}) {
  const { locale } = await params;
  setRequestLocale(locale);

  return (
    <QueryProvider>
      <div className="flex min-h-dvh flex-col bg-zinc-100 dark:bg-zinc-950">
        <ShellHeader />
        <main id="main" className="mx-auto w-full max-w-[1440px] flex-1 px-6 py-6">
          {children}
        </main>
        <FooterCredit />
      </div>
    </QueryProvider>
  );
}

function ShellHeader() {
  const t = useTranslations();
  return (
    <header className="border-b border-zinc-200/80 bg-white/80 px-6 py-3 shadow-sm backdrop-blur dark:border-zinc-800/80 dark:bg-zinc-950/80">
      <div className="mx-auto flex max-w-[1440px] items-center justify-between gap-4">
        <div className="flex items-center gap-3">
          <BrandShield className="h-8 w-auto" title={t("brand.shieldAlt")} />
          <div className="leading-tight">
            <p className="text-sm font-bold tracking-tight text-brand-700 dark:text-brand-300">
              {t("brand.name")}
            </p>
            <p className="text-[11px] text-zinc-500 dark:text-zinc-400">{t("brand.tagline")}</p>
          </div>
        </div>
        <nav aria-label={t("nav.dashboard")} className="hidden items-center gap-2 text-sm sm:flex">
          <Link
            href="/dashboard"
            className="inline-flex items-center gap-1.5 rounded-md bg-brand/10 px-3 py-1.5 font-semibold text-brand-700 dark:text-brand-300"
          >
            <LayoutDashboard aria-hidden="true" className="h-4 w-4" />
            {t("nav.dashboard")}
          </Link>
          <span
            className="rounded-md border border-dashed border-zinc-300 px-3 py-1.5 text-xs uppercase tracking-wide text-zinc-500 dark:border-zinc-700"
            title={t("shell.appshellPlaceholderBody")}
          >
            {t("shell.appshellPlaceholderTitle")}
          </span>
        </nav>
      </div>
    </header>
  );
}
