import { NextResponse } from "next/server";

import { getDashboardSummary } from "@/lib/mock-data";

export const dynamic = "force-dynamic";

export function GET() {
  return NextResponse.json(getDashboardSummary());
}
