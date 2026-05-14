import { type NextRequest, NextResponse } from "next/server";

import { getPlugins } from "@/lib/mock-data";
import { isPluginEnabled } from "@/lib/mock-state";

export const dynamic = "force-dynamic";

export function GET(request: NextRequest) {
  const limit = Number(request.nextUrl.searchParams.get("limit") ?? "5");
  const safeLimit = Number.isFinite(limit) ? Math.max(1, Math.min(50, Math.trunc(limit))) : 5;
  const rows = getPlugins(safeLimit).map((row) => ({
    ...row,
    enabled: isPluginEnabled(row.id, row.enabled),
  }));
  return NextResponse.json(rows);
}
