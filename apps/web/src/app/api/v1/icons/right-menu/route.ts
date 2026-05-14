import { NextResponse } from "next/server";

import { getIconMenu } from "@/lib/mock-data";

export const dynamic = "force-dynamic";

export function GET() {
  return NextResponse.json(getIconMenu());
}
