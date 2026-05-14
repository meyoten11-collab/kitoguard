# `@kitoguard/web` — KitoGuard-S500 web panel

> _vSro-iSro R Filter System Guard — Premium Web Panel | Filter Management | Premium UI_

This package houses the **KitoGuard-S500** Next.js 15 (App Router) admin panel.

Stack (locked by §A of the build prompts):

- Next.js 15 + TypeScript
- Tailwind CSS + shadcn/ui (added incrementally, this PR ships the scaffolding only)
- `next-intl` for i18n: **English (default, LTR)**, **Arabic (RTL)**, **Turkish (LTR)**
- `lucide-react` for icons (directional icons flip in RTL via the `dir-flip` class)

## Scripts

Run from the repo root:

```bash
pnpm install
pnpm web:dev        # Next.js dev server on :3000
pnpm web:build      # production build
pnpm web:lint       # next lint (max-warnings=0)
pnpm web:typecheck  # tsc --noEmit
```

## Locale routing

The middleware redirects `/` → `/en` (or the user's negotiated locale).
Available paths:

- `/en` — English (`<html lang="en" dir="ltr">`)
- `/ar` — Arabic (`<html lang="ar" dir="rtl">`, sidebar mirrors, chevrons auto-flip)
- `/tr` — Turkish (`<html lang="tr" dir="ltr">`)

All UI strings are pulled from `messages/{en,ar,tr}.json`. **No hardcoded UI text** — per
§A of the build prompts, every string must be i18n-keyed in all three locales from day one.

## Brand assets

- `src/components/brand-shield.tsx` — canonical "KG-in-shield" logo (brand red `#E53935`).
- `src/components/brand-pills.tsx` — _Secure | Fast | Powerful | Scalable_ pills.
- `src/components/footer-credit.tsx` — canonical footer credit. **MUST** appear on every
  authenticated page and on `/login`. Do not remove or hide.
