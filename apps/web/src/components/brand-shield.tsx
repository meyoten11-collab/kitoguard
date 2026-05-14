import { cn } from "@/lib/utils";

/**
 * KitoGuard-S500 shield mark — "KG" inside a shield, brand red #E53935.
 * Canonical per v1.1 of the spec.
 */
export function BrandShield({
  className,
  title,
}: {
  className?: string;
  title?: string;
}) {
  return (
    <svg
      role="img"
      aria-label={title ?? "KitoGuard-S500"}
      viewBox="0 0 64 72"
      className={cn("h-10 w-auto", className)}
      xmlns="http://www.w3.org/2000/svg"
    >
      <title>{title ?? "KitoGuard-S500"}</title>
      <defs>
        <linearGradient id="kg-shield-fill" x1="0" y1="0" x2="0" y2="1">
          <stop offset="0%" stopColor="#E53935" />
          <stop offset="100%" stopColor="#A01F1F" />
        </linearGradient>
      </defs>
      <path
        d="M32 2 L60 12 V34 C60 50 48 62 32 70 C16 62 4 50 4 34 V12 Z"
        fill="url(#kg-shield-fill)"
        stroke="#7A1717"
        strokeWidth="1.5"
      />
      <text
        x="32"
        y="44"
        textAnchor="middle"
        fontFamily="Inter, system-ui, sans-serif"
        fontSize="22"
        fontWeight="700"
        fill="#ffffff"
        letterSpacing="0.5"
      >
        KG
      </text>
    </svg>
  );
}
