/**
 * Module-scope mutable state for the mocked W2 filter-control endpoints.
 * The real backend (W3+) replaces this with EF Core writes and SignalR
 * broadcasts to the audit hub.
 */

interface FilterRuntimeState {
  filterRunning: boolean;
  lastChangedAt: string;
  lastChangedBy: string;
}

const state: FilterRuntimeState = {
  filterRunning: true,
  lastChangedAt: new Date().toISOString(),
  lastChangedBy: "admin",
};

const enabledPluginIds = new Set<string>([
  "kg.auto-notice",
  "kg.unique-tracker",
  "kg.icon-manager",
  "kg.anti-cheat",
]);

export function getFilterRuntime(): FilterRuntimeState {
  return { ...state };
}

export function setFilterRunning(running: boolean, actor = "admin"): FilterRuntimeState {
  state.filterRunning = running;
  state.lastChangedAt = new Date().toISOString();
  state.lastChangedBy = actor;
  return { ...state };
}

export function isPluginEnabled(id: string, defaultEnabled: boolean): boolean {
  if (enabledPluginIds.has(id)) return true;
  return defaultEnabled && !enabledPluginIds.has(`!${id}`);
}

export function setPluginEnabled(id: string, enabled: boolean): void {
  if (enabled) {
    enabledPluginIds.add(id);
    enabledPluginIds.delete(`!${id}`);
  } else {
    enabledPluginIds.delete(id);
    enabledPluginIds.add(`!${id}`);
  }
}
