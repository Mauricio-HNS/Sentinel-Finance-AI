export default function SimulatorPage() {
  return (
    <div className="space-y-6">
      <div>
        <p className="text-sm uppercase tracking-[0.3em] text-emerald-300/70">Scenario Simulator</p>
        <h2 className="mt-2 text-4xl font-semibold">Pressure-test future risk</h2>
      </div>
      <div className="grid gap-6 lg:grid-cols-[0.9fr_1.1fr]">
        <div className="glass space-y-4 p-6">
          <input className="w-full rounded-2xl border border-white/10 bg-slate-900/60 px-4 py-3" defaultValue="12" placeholder="Days late" />
          <input className="w-full rounded-2xl border border-white/10 bg-slate-900/60 px-4 py-3" defaultValue="-32" placeholder="Usage variation %" />
          <input className="w-full rounded-2xl border border-white/10 bg-slate-900/60 px-4 py-3" defaultValue="4" placeholder="Critical tickets" />
          <button className="w-full rounded-2xl bg-emerald-400 px-4 py-3 font-semibold text-slate-950">Run simulation</button>
        </div>
        <div className="glass p-6">
          <p className="text-sm text-slate-400">Projected risk score</p>
          <p className="mt-3 text-6xl font-semibold text-white">84.7</p>
          <p className="mt-4 text-slate-300">
            Under this scenario, the customer enters a critical band driven by payment slippage, shrinking platform adoption and a concentration of severe support events.
          </p>
        </div>
      </div>
    </div>
  );
}
