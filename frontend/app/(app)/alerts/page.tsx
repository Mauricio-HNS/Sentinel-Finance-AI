import { alerts } from "@/lib/mock-data";

export default function AlertsPage() {
  return (
    <div className="space-y-6">
      <div>
        <p className="text-sm uppercase tracking-[0.3em] text-emerald-300/70">Alert Center</p>
        <h2 className="mt-2 text-4xl font-semibold">Critical account signals</h2>
      </div>
      <div className="grid gap-4">
        {alerts.map((alert) => (
          <div key={alert.id} className="glass flex items-center justify-between p-5">
            <div>
              <p className="text-sm text-slate-400">{alert.type}</p>
              <h3 className="mt-1 text-xl font-semibold">{alert.title}</h3>
              <p className="mt-1 text-slate-300">{alert.customer}</p>
            </div>
            <span className="rounded-full bg-orange-400/10 px-3 py-1 text-sm text-orange-300">{alert.severity}</span>
          </div>
        ))}
      </div>
    </div>
  );
}
