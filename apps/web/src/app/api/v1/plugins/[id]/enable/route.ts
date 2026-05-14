import { type NextRequest, NextResponse } from "next/server";

import { getPlugins } from "@/lib/mock-data";
import { isPluginEnabled, setPluginEnabled } from "@/lib/mock-state";

export const dynamic = "force-dynamic";

export async function POST(_request: NextRequest, { params }: { params: Promise<{ id: string }> }) {
  const { id } = await params;
  setPluginEnabled(id, true);
  const row = getPlugins().find((p) => p.id === id);
  if (!row) {
    return NextResponse.json({ error: "plugin_not_found" }, { status: 404 });
  }
  return NextResponse.json({ ...row, enabled: isPluginEnabled(row.id, row.enabled) });
}
