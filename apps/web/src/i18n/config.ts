/**
 * Canonical KitoGuard-S500 locale configuration.
 * Per spec: English (default, LTR), Arabic (RTL), Turkish (LTR).
 */

export const locales = ["en", "ar", "tr"] as const;
export type Locale = (typeof locales)[number];

export const defaultLocale: Locale = "en";

export const rtlLocales: ReadonlySet<Locale> = new Set(["ar"]);

export function isLocale(value: string | undefined | null): value is Locale {
  return value !== null && value !== undefined && (locales as readonly string[]).includes(value);
}

export function getDirection(locale: Locale): "ltr" | "rtl" {
  return rtlLocales.has(locale) ? "rtl" : "ltr";
}

export const localeLabels: Record<Locale, string> = {
  en: "English",
  ar: "العربية",
  tr: "Türkçe",
};
