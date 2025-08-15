import type { Config } from "tailwindcss";

const config: Config = {
  darkMode: "class",
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        primary: "#13417d",
      },
      boxShadow: {
        glow: "0 0 12px rgba(255, 255, 255, 0.4)",
      },
    },
  },
  plugins: [],
};

export default config;
