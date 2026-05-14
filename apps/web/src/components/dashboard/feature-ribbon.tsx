import { Layers, LayoutGrid, ShieldCheck, Swords, Wand2 } from "lucide-react";
import { useTranslations } from "next-intl";

const ITEMS = [
  { key: "mainFeatures", icon: Swords, accent: "from-[#1E3A8A] to-[#3B82F6]" },
  { key: "securitySystems", icon: ShieldCheck, accent: "from-[#14532D] to-[#22C55E]" },
  { key: "ingameSystems", icon: Wand2, accent: "from-[#581C87] to-[#A855F7]" },
  { key: "pluginArchitecture", icon: Layers, accent: "from-[#0F766E] to-[#06B6D4]" },
  { key: "rightIconMenu", icon: LayoutGrid, accent: "from-[#9F1239] to-[#EC4899]" },
] as const;

export function FeatureRibbon() {
  const t = useTranslations("featureRibbon");

  return (
    <section
      aria-label={t("mainFeatures")}
      className="grid grid-cols-1 gap-3 sm:grid-cols-2 lg:grid-cols-5"
    >
      {ITEMS.map((item) => {
        const Icon = item.icon;
        return (
          <div
            key={item.key}
            className={`relative overflow-hidden rounded-xl bg-gradient-to-br ${item.accent} p-4 text-white shadow-md`}
          >
            <div className="flex items-start gap-3">
              <span className="flex h-9 w-9 shrink-0 items-center justify-center rounded-lg bg-white/15">
                <Icon aria-hidden="true" className="h-4 w-4" />
              </span>
              <div className="min-w-0 space-y-0.5">
                <p className="text-sm font-semibold leading-tight">{t(item.key)}</p>
                <p className="text-[11px] leading-snug text-white/85">{t(`${item.key}Body`)}</p>
              </div>
            </div>
          </div>
        );
      })}
    </section>
  );
}
