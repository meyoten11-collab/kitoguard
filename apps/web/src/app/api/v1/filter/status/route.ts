import { NextResponse } from "next/server";

import { getFilterStatus } from "@/lib/mock-data";
import { getFilterRuntime } from "@/lib/mock-state";

export const dynamic = "force-dynamic";

export function GET() {
  const base = getFilterStatus();
  const runtime = getFilterRuntime();
  return NextResponse.json({
    ...base,
    filterRunning: runtime.filterRunning,
    lastChangedAt: runtime.lastChangedAt,
    lastChangedBy: runtime.lastChangedBy,
  });
}
