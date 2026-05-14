import Link from "next/link";

export default function NotFound() {
  return (
    <html lang="en" dir="ltr">
      <body className="flex min-h-dvh items-center justify-center bg-zinc-50 text-zinc-900">
        <main className="space-y-3 text-center">
          <h1 className="text-2xl font-bold">KitoGuard-S500 — 404</h1>
          <p>The page you requested could not be found.</p>
          <Link href="/en" className="text-brand-600 underline">
            Return to KitoGuard-S500
          </Link>
        </main>
      </body>
    </html>
  );
}
