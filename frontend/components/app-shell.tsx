// ---Made By Destiny7 Softwares---
import Link from "next/link";
import { Bell, ChevronDown, LayoutDashboard, Shield, UserCircle2 } from "lucide-react";
import type { ReactNode } from "react";
import { dashboardChrome } from "@/lib/mock-data";

const nav = [
  { href: "/dashboard", label: "Dashboard" },
  { href: "/customers", label: "Customers" },
  { href: "/alerts", label: "Alerts" },
  { href: "/ai-ops", label: "AI Ops" },
  { href: "/simulator", label: "Simulator" },
  { href: "/upload", label: "CSV Upload" }
];

export function AppShell({ children }: { children: ReactNode }) {
  return (
    <div className="min-h-screen bg-[radial-gradient(circle_at_top,rgba(67,95,143,0.28),transparent_35%),linear-gradient(180deg,#0b1527_0%,#091322_100%)]">
      <header className="border-b border-white/10 bg-slate-950/20 px-6 py-5 backdrop-blur-xl">
        <div className="mx-auto flex max-w-[1600px] items-center justify-between gap-6">
          <div className="flex items-center gap-4">
            <div className="flex h-10 w-10 items-center justify-center rounded-xl border border-white/15 bg-white/5">
              <Shield className="h-6 w-6 text-slate-100" />
            </div>
            <div className="flex items-center gap-4">
              <h1 className="text-[2rem] font-semibold leading-none text-slate-50">Sentinel Finance AI</h1>
              <div className="hidden h-8 w-px bg-white/10 lg:block" />
              <p className="hidden text-2xl text-slate-300/80 lg:block">{dashboardChrome.productSubtitle}</p>
            </div>
          </div>
          <div className="flex items-center gap-5 text-slate-300">
            <div className="relative">
              <Bell className="h-5 w-5" />
              <span className="absolute -right-1 -top-1 h-3 w-3 rounded-full bg-red-500" />
            </div>
            <div className="flex items-center gap-3">
              <UserCircle2 className="h-9 w-9 rounded-full bg-white/10 p-1 text-slate-100" />
              <span className="text-xl font-medium text-slate-100">{dashboardChrome.userName}</span>
              <ChevronDown className="h-4 w-4" />
            </div>
          </div>
        </div>
      </header>

      <div className="mx-auto flex max-w-[1600px]">
        <aside className="flex w-20 flex-col items-center border-r border-white/10 bg-slate-950/25 py-6 backdrop-blur-xl">
          <div className="mb-6 flex h-12 w-12 items-center justify-center rounded-2xl border border-white/10 bg-white/5">
            <Shield className="h-6 w-6 text-slate-400" />
          </div>
          <nav className="flex w-full flex-1 flex-col items-center gap-4">
            {dashboardChrome.sideNav.map((item) => (
              <div
                key={item.label}
                className={`flex h-12 w-full items-center justify-center border-l-2 ${
                  item.active ? "border-sky-400 bg-sky-400/12 text-sky-300" : "border-transparent text-slate-500"
                }`}
                title={item.label}
              >
                {item.active ? <LayoutDashboard className="h-6 w-6" /> : <span className="text-sm font-semibold">{item.short}</span>}
              </div>
            ))}
          </nav>
          <div className="mb-2 text-3xl leading-none text-slate-500">≡</div>
        </aside>

        <main className="min-w-0 flex-1 px-4 py-4">
          <div className="hidden pb-4 md:flex gap-2">
            {nav.map(({ href, label }) => (
              <Link key={href} href={href} className="rounded-xl border border-white/10 bg-white/5 px-4 py-2 text-sm text-slate-300 transition hover:bg-white/10">
                {label}
              </Link>
            ))}
          </div>
          {children}
        </main>
      </div>
    </div>
  );
}
