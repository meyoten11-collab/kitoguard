import { NextResponse } from "next/server";

import { getFilterStatus } from "@/lib/mock-data";
import { setFilterRunning } from "@/lib/mock-state";

export const dynamic = "force-dynamic";

export function POST() {
  const runtime = setFilterRunning(false);
  const base = getFilterStatus();
  return NextResponse.json({
    ...base,
    filterRunning: runtime.filterRunning,
    lastChangedAt: runtime.lastChangedAt,
    lastChangedBy: runtime.lastChangedBy,
  });
}
