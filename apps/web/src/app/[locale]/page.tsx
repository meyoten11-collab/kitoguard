import { ChevronRight } from "lucide-react";
import { useTranslations } from "next-intl";
import { setRequestLocale } from "next-intl/server";

import { BrandPills } from "@/components/brand-pills";
import { BrandShield } from "@/components/brand-shield";
import { FooterCredit } from "@/components/footer-credit";
import { getDirection, isLocale, localeLabels, locales } from "@/i18n/config";
import { Link } from "@/i18n/navigation";

export default async function Home({
  params,
}: {
  params: Promise<{ locale: string }>;
}) {
  const { locale } = await params;
  setRequestLocale(locale);

  return <HomeContent locale={locale} />;
}

function HomeContent({ locale }: { locale: string }) {
  const t = useTranslations();
  const direction = isLocale(locale) ? getDirection(locale) : "ltr";

  return (
    <div className="flex min-h-dvh flex-col">
      <main
        className="flex flex-1 flex-col items-center justify-center gap-8 px-6 py-12 text-center"
        id="main"
      >
        <BrandShield className="h-16 w-auto" title={t("brand.shieldAlt")} />
        <div className="space-y-2">
          <h1 className="text-3xl font-bold text-brand-700 dark:text-brand-300 sm:text-4xl">
            {t("brand.name")}
          </h1>
          <p className="text-base text-zinc-600 dark:text-zinc-300 sm:text-lg">
            {t("brand.tagline")}
          </p>
          <p className="text-sm text-zinc-500 dark:text-zinc-400">
            {t("brand.subTagline")}
          </p>
        </div>

        <BrandPills />

        <section
          aria-labelledby="welcome-heading"
          className="max-w-2xl space-y-3 rounded-lg border border-zinc-200 bg-white/70 p-6 text-start shadow-sm dark:border-zinc-800 dark:bg-zinc-900/60"
        >
          <h2 id="welcome-heading" className="text-lg font-semibold">
            {t("home.welcomeTitle")}
          </h2>
          <p className="text-sm text-zinc-600 dark:text-zinc-300">
            {t("home.welcomeBody")}
          </p>
          <dl className="grid grid-cols-2 gap-2 pt-3 text-xs text-zinc-500 dark:text-zinc-400">
            <dt>{t("home.currentLocale")}</dt>
            <dd data-testid="current-locale">{locale}</dd>
            <dt>{t("home.currentDirection")}</dt>
            <dd data-testid="current-direction">{direction}</dd>
          </dl>
        </section>

        <nav aria-label={t("home.switchLocale")} className="flex flex-wrap items-center justify-center gap-2 text-sm">
          {locales.map((l) => {
            const isCurrent = l === locale;
            return (
              <Link
                key={l}
                href="/"
                locale={l}
                aria-current={isCurrent ? "page" : undefined}
                className={
                  "inline-flex items-center gap-1 rounded-md border px-3 py-1.5 transition-colors " +
                  (isCurrent
                    ? "border-brand bg-brand text-white"
                    : "border-zinc-300 hover:border-brand hover:text-brand-700 dark:border-zinc-700 dark:hover:text-brand-300")
                }
              >
                {localeLabels[l]}
                <ChevronRight className="dir-flip h-4 w-4" aria-hidden="true" />
              </Link>
            );
          })}
        </nav>

        <Link
          href="/dashboard"
          className="inline-flex items-center gap-2 rounded-md bg-brand px-4 py-2 text-sm font-semibold text-white transition-colors hover:bg-brand-600"
          data-testid="open-dashboard-preview"
        >
          {t("home.openDashboard")}
          <ChevronRight className="dir-flip h-4 w-4" aria-hidden="true" />
        </Link>
      </main>
      <FooterCredit />
    </div>
  );
}
