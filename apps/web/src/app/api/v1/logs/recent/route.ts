import { type NextRequest, NextResponse } from "next/server";

import { getRecentLogs } from "@/lib/mock-data";

export const dynamic = "force-dynamic";

export function GET(request: NextRequest) {
  const limit = Number(request.nextUrl.searchParams.get("limit") ?? "5");
  const safeLimit = Number.isFinite(limit) ? Math.max(1, Math.min(50, Math.trunc(limit))) : 5;
  return NextResponse.json(getRecentLogs(safeLimit));
}
