// ---Made By Destiny7 Softwares---
import { getAlertsViewModel } from "@/lib/api";
import { alertStats } from "@/lib/mock-data";

const severityTone: Record<string, string> = {
  Critical: "bg-red-500/20 text-red-100",
  High: "bg-orange-500/20 text-orange-100",
  Warning: "bg-amber-500/20 text-amber-100"
};

export default async function AlertsPage() {
  const alerts = await getAlertsViewModel();

  return (
    <div className="space-y-4">
      <div className="grid gap-4 xl:grid-cols-[1.1fr_0.9fr]">
        <div className="panel px-6 py-5">
          <p className="text-sm uppercase tracking-[0.3em] text-sky-300/80">Alert Center</p>
          <h2 className="mt-3 text-5xl font-semibold text-white">Critical account signals</h2>
          <p className="mt-4 max-w-3xl text-lg leading-8 text-slate-300">
            Track billing pressure, churn signals, renewal watchlists, and support escalations in one operational queue.
          </p>
        </div>
        <div className="grid gap-4 md:grid-cols-3">
          {alertStats.map((stat) => (
            <div key={stat.label} className="metric-card">
              <p className="text-sm text-slate-400">{stat.label}</p>
              <div className="mt-4 text-5xl font-semibold text-white">{stat.value}</div>
            </div>
          ))}
        </div>
      </div>

      <div className="panel overflow-hidden">
        <div className="flex flex-wrap items-center justify-between gap-4 border-b border-white/10 px-6 py-4">
          <div className="flex flex-wrap gap-3">
            <div className="rounded-lg border border-white/10 bg-[#142338] px-4 py-2 text-sm text-slate-300">Severity: All</div>
            <div className="rounded-lg border border-white/10 bg-[#142338] px-4 py-2 text-sm text-slate-300">Status: Open</div>
            <div className="rounded-lg border border-white/10 bg-[#142338] px-4 py-2 text-sm text-slate-300">Type: All</div>
          </div>
        </div>

        <div className="grid grid-cols-[0.8fr_1.5fr_1fr_0.8fr] border-b border-white/10 bg-[#142338] px-6 py-4 text-sm uppercase tracking-[0.2em] text-slate-400">
          <div>Severity</div>
          <div>Alert</div>
          <div>Customer</div>
          <div>Status</div>
        </div>

        {alerts.map((alert, index) => (
          <div key={alert.id} className={`grid grid-cols-[0.8fr_1.5fr_1fr_0.8fr] items-center px-6 py-5 ${index !== alerts.length - 1 ? "border-b border-white/10" : ""}`}>
            <div>
              <span className={`metric-badge ${severityTone[alert.severity]}`}>{alert.severity}</span>
            </div>
            <div>
              <div className="text-lg font-semibold text-white">{alert.title}</div>
              <div className="mt-1 text-sm text-slate-400">{alert.type}</div>
            </div>
            <div className="text-slate-200">{alert.customer}</div>
            <div>
              <span className="metric-badge bg-slate-500/20 text-slate-100">{alert.status}</span>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
