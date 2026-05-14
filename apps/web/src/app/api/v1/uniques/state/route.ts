import { NextResponse } from "next/server";

import { getUniques } from "@/lib/mock-data";

export const dynamic = "force-dynamic";

export function GET() {
  return NextResponse.json(getUniques());
}
