import type { Config } from "tailwindcss";

const config: Config = {
  content: ["./src/**/*.{ts,tsx,mdx}"],
  theme: {
    extend: {
      colors: {
        brand: {
          DEFAULT: "#E53935",
          50: "#FDECEC",
          100: "#FBD0D0",
          200: "#F7A2A2",
          300: "#F27474",
          400: "#ED4747",
          500: "#E53935",
          600: "#C62828",
          700: "#A01F1F",
          800: "#7A1717",
          900: "#560F0F",
        },
      },
      fontFamily: {
        sans: ["var(--font-sans)", "system-ui", "sans-serif"],
      },
    },
  },
  plugins: [],
};

export default config;
