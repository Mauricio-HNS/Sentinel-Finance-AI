import { simulatorPresets } from "@/lib/mock-data";

export default function SimulatorPage() {
  return (
    <div className="space-y-4">
      <div className="grid gap-4 xl:grid-cols-[1.05fr_0.95fr]">
        <div className="panel px-6 py-5">
          <p className="text-sm uppercase tracking-[0.3em] text-sky-300/80">Scenario Simulator</p>
          <h2 className="mt-3 text-5xl font-semibold text-white">Pressure-test future account risk</h2>
          <p className="mt-4 max-w-3xl text-lg leading-8 text-slate-300">
            Model what happens when payment delays grow, usage declines deepen, support escalations stack up, or the renewal window tightens.
          </p>
        </div>
        <div className="panel p-6">
          <p className="text-sm text-slate-400">Projected risk score</p>
          <div className="mt-4 text-7xl font-semibold text-white">84.7</div>
          <p className="mt-4 max-w-xl text-lg leading-8 text-slate-300">
            Under this scenario, the account shifts into a critical band driven by payment slippage, adoption loss, and severe support pressure.
          </p>
        </div>
      </div>

      <div className="grid gap-4 xl:grid-cols-[0.95fr_1.05fr]">
        <div className="panel p-6">
          <h3 className="dashboard-title">Scenario inputs</h3>
          <div className="mt-6 space-y-4">
            {simulatorPresets.map((preset) => (
              <div key={preset.label}>
                <div className="mb-2 flex items-center justify-between text-sm text-slate-400">
                  <span>{preset.label}</span>
                  <span>{preset.value}</span>
                </div>
                <div className="h-3 rounded-full bg-[#142338]">
                  <div className="h-3 rounded-full bg-gradient-to-r from-sky-500 to-orange-400" style={{ width: preset.label === "Usage Decline" ? "72%" : preset.label === "Critical Tickets" ? "66%" : "58%" }} />
                </div>
              </div>
            ))}
          </div>
          <button className="mt-8 w-full rounded-xl bg-teal-500 px-5 py-4 text-lg font-semibold text-slate-100">Run simulation</button>
        </div>

        <div className="space-y-4">
          <div className="panel p-6">
            <h3 className="dashboard-title">Outcome narrative</h3>
            <p className="mt-5 text-lg leading-8 text-slate-300">
              Late payment probability accelerates because the outstanding delay exceeds collection tolerance. Churn risk climbs as usage drops below adoption benchmarks and critical tickets remain unresolved.
            </p>
          </div>

          <div className="grid gap-4 md:grid-cols-3">
            <div className="metric-card">
              <p className="text-sm text-slate-400">Churn impact</p>
              <div className="mt-4 text-5xl font-semibold text-white">+18%</div>
            </div>
            <div className="metric-card">
              <p className="text-sm text-slate-400">Payment impact</p>
              <div className="mt-4 text-5xl font-semibold text-white">+23%</div>
            </div>
            <div className="metric-card">
              <p className="text-sm text-slate-400">Revenue at risk</p>
              <div className="mt-4 text-5xl font-semibold text-white">$154K</div>
            </div>
          </div>

          <div className="panel p-6">
            <h3 className="dashboard-title">Recommended actions</h3>
            <div className="mt-5 grid gap-3">
              <div className="rounded-lg border border-white/10 bg-[#142338] px-4 py-3 text-slate-200">Escalate collections outreach within 24 hours</div>
              <div className="rounded-lg border border-white/10 bg-[#142338] px-4 py-3 text-slate-200">Launch executive customer success recovery plan</div>
              <div className="rounded-lg border border-white/10 bg-[#142338] px-4 py-3 text-slate-200">Review renewal concessions only after usage recovery</div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
