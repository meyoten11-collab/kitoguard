import "./globals.css";

import type { ReactNode } from "react";

/**
 * Root layout is intentionally minimal because next-intl + the `[locale]`
 * segment owns the real `<html>`/`<body>` rendering (so we can set
 * `lang` / `dir` per-locale). This file just passes children through.
 */
export default function RootLayout({ children }: { children: ReactNode }) {
  return children;
}
