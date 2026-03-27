import type { Config } from "tailwindcss";

export default {
  content: ["./app/**/*.{ts,tsx}", "./components/**/*.{ts,tsx}", "./lib/**/*.{ts,tsx}"],
  theme: {
    extend: {
      colors: {
        ink: "#09121f",
        panel: "#0f1c2f",
        card: "#14233a",
        accent: "#22c55e",
        signal: "#3b82f6"
      },
      boxShadow: {
        halo: "0 24px 80px rgba(15, 23, 42, 0.45)"
      }
    }
  },
  plugins: []
} satisfies Config;
