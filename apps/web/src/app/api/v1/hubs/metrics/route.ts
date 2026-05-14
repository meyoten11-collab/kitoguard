import { generateMetricsTick } from "@/lib/mock-data";

export const dynamic = "force-dynamic";
export const runtime = "nodejs";

const INTERVAL_MS = 3_000;
const encoder = new TextEncoder();

export function GET() {
  const stream = new ReadableStream<Uint8Array>({
    start(controller) {
      let cancelled = false;
      let timer: NodeJS.Timeout | null = null;

      const push = () => {
        if (cancelled) return;
        try {
          const tick = generateMetricsTick();
          const payload = `event: tick\ndata: ${JSON.stringify(tick)}\n\n`;
          controller.enqueue(encoder.encode(payload));
        } catch {
          // ignore enqueue errors after cancel
        }
      };

      // Initial tick + interval. NB: this is a Next.js mock stand-in for the
      // ASP.NET Core SignalR hub `/hubs/metrics` that W3 wires up.
      push();
      timer = setInterval(push, INTERVAL_MS);

      const cleanup = () => {
        if (cancelled) return;
        cancelled = true;
        if (timer) clearInterval(timer);
        try {
          controller.close();
        } catch {
          // already closed
        }
      };

      // Best-effort cleanup if the underlying socket closes. The
      // ReadableStream pull/cancel cycle handles client disconnect, but we
      // also stop the timer here.
      // @ts-expect-error - Node-only signal on the controller
      if (controller.signal) {
        // @ts-expect-error - Node-only signal on the controller
        controller.signal.addEventListener("abort", cleanup);
      }
    },
    cancel() {
      // ReadableStream's cancel() fires on client disconnect.
    },
  });

  return new Response(stream, {
    headers: {
      "Content-Type": "text/event-stream",
      "Cache-Control": "no-cache, no-transform",
      Connection: "keep-alive",
      "X-Accel-Buffering": "no",
    },
  });
}
