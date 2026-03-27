// ---Made By Destiny7 Softwares---
import { chromium, devices } from "playwright";
import path from "node:path";
import { fileURLToPath } from "node:url";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const root = path.resolve(__dirname, "..", "..");
const outputDir = path.join(root, "docs", "screenshots");
const baseUrl = process.env.README_CAPTURE_BASE_URL ?? "http://127.0.0.1:3000";

const pages = [
  { route: "/dashboard", file: "dashboard.png" },
  { route: "/login", file: "login.png" },
  { route: "/customers", file: "customers.png" }
];

const browser = await chromium.launch({ headless: true });
const context = await browser.newContext({
  ...devices["Desktop Chrome"],
  viewport: { width: 1600, height: 1000 },
  deviceScaleFactor: 1
});

for (const pageDef of pages) {
  const page = await context.newPage();
  const url = `${baseUrl}${pageDef.route}`;
  console.log(`Capturing ${url}`);
  await page.goto(url, { waitUntil: "networkidle" });
  await page.screenshot({
    path: path.join(outputDir, pageDef.file),
    fullPage: true
  });
  await page.close();
}

await browser.close();
console.log("README screenshot assets refreshed.");
