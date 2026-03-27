import { dashboard } from "@/lib/mock-data";

const badgeStyles: Record<string, string> = {
  red: "bg-red-500/20 text-red-200",
  amber: "bg-amber-500/20 text-amber-100",
  orange: "bg-orange-500/20 text-orange-100",
  green: "bg-emerald-500/20 text-emerald-100"
};

const scoreStyles: Record<string, string> = {
  red: "bg-red-400/25 text-red-50",
  amber: "bg-emerald-400/25 text-emerald-50",
  orange: "bg-orange-400/25 text-orange-50",
  green: "bg-emerald-400/25 text-emerald-50"
};

function MetricCard({
  title,
  value,
  badge,
  badgeTone
}: {
  title: string;
  value: string;
  badge: string;
  badgeTone: string;
}) {
  return (
    <div className="metric-card min-w-0">
      <p className="text-[1.05rem] font-semibold text-slate-100">{title}</p>
      <div className="mt-4 flex items-center gap-3">
        <span className="text-[3.2rem] font-semibold leading-none text-white">{value}</span>
        <span className={`metric-badge ${badgeStyles[badgeTone]}`}>{badge}</span>
      </div>
    </div>
  );
}

export default function DashboardPage() {
  return (
    <div className="space-y-4">
      <section className="grid gap-4 xl:grid-cols-6">
        <MetricCard title="Portfolio Risk Score" value={String(dashboard.portfolioRiskScore)} badge={dashboard.portfolioRiskLabel} badgeTone="red" />
        <MetricCard title="High-Risk Clients" value={String(dashboard.highRiskClients)} badge={dashboard.highRiskLabel} badgeTone="red" />
        <MetricCard title="Projected Churn" value={dashboard.projectedChurn} badge={dashboard.projectedChurnLabel} badgeTone="orange" />
        <MetricCard title="Late Payments" value={String(dashboard.latePayments)} badge={dashboard.latePaymentsLabel} badgeTone="amber" />
        <MetricCard title="Revenue at Risk" value={dashboard.revenueAtRisk} badge={dashboard.revenueAtRiskLabel} badgeTone="red" />
        <div className="metric-card">
          <p className="text-[1.05rem] font-semibold text-slate-100">Top Drivers</p>
          <div className="mt-6 flex h-16 items-end gap-1">
            {dashboard.topDrivers.map((point, index) => (
              <div
                key={`${point}-${index}`}
                className="flex-1 rounded-t-sm bg-gradient-to-t from-sky-500/60 to-slate-200/70"
                style={{ height: `${point}px` }}
              />
            ))}
          </div>
        </div>
      </section>

      <section className="grid gap-4 xl:grid-cols-[1.1fr_0.95fr]">
        <div className="space-y-4">
          <div className="panel overflow-hidden">
            <div className="flex items-center justify-between border-b border-white/10 px-5 py-4">
              <h2 className="dashboard-title">Risk Overview</h2>
              <div className="flex gap-3 text-slate-400">
                <span>↗</span>
                <span>⚙</span>
              </div>
            </div>

            <div className="px-5 py-4">
              <div className="mb-6 flex flex-wrap gap-6 text-sm text-slate-300">
                <span className="flex items-center gap-2">
                  <span className="h-1.5 w-6 rounded-full bg-[#d79764]" />
                  Churn Probability
                </span>
                <span className="flex items-center gap-2">
                  <span className="h-1.5 w-6 rounded-full bg-[#f9a24a]" />
                  Late Payment Risk
                </span>
                <span className="flex items-center gap-2">
                  <span className="h-1.5 w-6 rounded-full bg-[#71d4b0]" />
                  Revenue Exposure
                </span>
              </div>

              <div className="relative h-[230px] overflow-hidden rounded-lg bg-[linear-gradient(180deg,rgba(255,255,255,0.02),rgba(255,255,255,0.01))] px-2 py-4">
                <div className="absolute inset-x-0 top-10 border-t border-dashed border-white/10" />
                <div className="absolute inset-x-0 top-24 border-t border-dashed border-white/10" />
                <div className="absolute inset-x-0 top-40 border-t border-dashed border-white/10" />
                <div className="absolute inset-y-0 left-[14%] border-l border-dashed border-white/10" />
                <div className="absolute inset-y-0 left-[36%] border-l border-dashed border-white/10" />
                <div className="absolute inset-y-0 left-[58%] border-l border-dashed border-white/10" />
                <div className="absolute inset-y-0 left-[81%] border-l border-dashed border-white/10" />

                <div className="absolute inset-x-4 bottom-4 flex items-end gap-2">
                  {dashboard.trend.map((point) => (
                    <div key={point.label} className="flex flex-1 items-end gap-1">
                      <div className="w-2 rounded-t-sm bg-[#71d4b0]/75" style={{ height: `${point.revenue * 2.2}px` }} />
                      <div className="w-2 rounded-t-sm bg-[#f67a52]/85" style={{ height: `${point.churn * 1.05}px` }} />
                      <div className="w-2 rounded-t-sm bg-[#f0a64f]/8" style={{ height: `${point.late * 0.2}px` }} />
                    </div>
                  ))}
                </div>

                <svg className="absolute inset-0 h-full w-full" viewBox="0 0 1000 240" preserveAspectRatio="none" aria-hidden="true">
                  <defs>
                    <linearGradient id="riskFill" x1="0" x2="0" y1="0" y2="1">
                      <stop offset="0%" stopColor="rgba(253, 168, 79, 0.35)" />
                      <stop offset="100%" stopColor="rgba(253, 168, 79, 0.02)" />
                    </linearGradient>
                  </defs>
                  <path
                    d="M0 124 L62 118 L124 108 L186 136 L248 116 L310 104 L372 133 L434 108 L496 90 L558 116 L620 134 L682 96 L744 126 L806 101 L868 88 L1000 92 L1000 240 L0 240 Z"
                    fill="url(#riskFill)"
                  />
                  <path
                    d="M0 124 L62 118 L124 108 L186 136 L248 116 L310 104 L372 133 L434 108 L496 90 L558 116 L620 134 L682 96 L744 126 L806 101 L868 88 L1000 92"
                    fill="none"
                    stroke="#f5a24c"
                    strokeWidth="4"
                    strokeLinecap="round"
                  />
                </svg>
              </div>
            </div>
          </div>

          <div className="panel overflow-hidden">
            <div className="flex items-center justify-between border-b border-white/10 px-5 py-4">
              <h2 className="dashboard-title">Key Alerts</h2>
              <div className="flex gap-3 text-slate-400">
                <span>▱</span>
                <span>⚙</span>
              </div>
            </div>
            <div className="px-5 py-2">
              {dashboard.keyAlerts.map((alert, index) => (
                <div key={alert} className={`flex items-center gap-4 py-4 ${index !== dashboard.keyAlerts.length - 1 ? "border-b border-white/10" : ""}`}>
                  <span className={`h-4 w-4 rounded-full ${index === 2 ? "bg-orange-400" : "bg-red-400"}`} />
                  <p className="text-[1.15rem] text-slate-200">{alert}</p>
                </div>
              ))}
            </div>
          </div>

          <div className="panel px-5 py-4">
            <h2 className="dashboard-title">AI Risk Analysis</h2>
            <p className="mt-5 max-w-4xl text-[1.12rem] leading-8 text-slate-200/90">
              High risk due to 39% drop in usage, 18 days overdue, and 3 critical support tickets reported recently.
            </p>
          </div>
        </div>

        <div className="space-y-4">
          <div className="panel overflow-hidden">
            <div className="flex items-center justify-between border-b border-white/10 px-5 py-4">
              <h2 className="dashboard-title">Top High-Risk Clients</h2>
              <div className="text-2xl text-slate-400">•••</div>
            </div>

            <div className="grid grid-cols-[1.35fr_0.55fr_0.6fr_0.8fr] border-b border-white/10 px-5 py-3 text-lg text-slate-400">
              <div>Client</div>
              <div>Risk Score</div>
              <div>Churn Risk</div>
              <div>Status</div>
            </div>

            {dashboard.highRiskTable.map((client, index) => (
              <div key={client.name} className={`grid grid-cols-[1.35fr_0.55fr_0.6fr_0.8fr] items-center px-5 py-4 ${index !== dashboard.highRiskTable.length - 1 ? "border-b border-white/10" : ""}`}>
                <div className="flex items-center gap-3 text-xl font-medium text-slate-100">
                  <div className="flex h-9 w-9 items-center justify-center rounded-full bg-slate-300/80 text-sm font-bold text-slate-800">
                    {client.name.charAt(0)}
                  </div>
                  {client.name}
                </div>
                <div>
                  <span className={`inline-flex min-w-14 justify-center rounded px-3 py-1 text-xl font-semibold ${scoreStyles[client.tone]}`}>{client.score}</span>
                </div>
                <div>
                  <span className={`metric-badge ${badgeStyles[client.tone]}`}>{client.churn}</span>
                </div>
                <div className="text-lg text-slate-300">{client.status}</div>
              </div>
            ))}
          </div>

          <div className="grid gap-4 lg:grid-cols-[0.95fr_1.05fr]">
            <div className="panel p-5">
              <h2 className="dashboard-title">Churn Prediction</h2>
              <div className="mt-8 flex flex-col items-center">
                <div className="relative flex h-52 w-52 items-center justify-center rounded-full border-[18px] border-orange-500/80 border-t-orange-300 border-l-orange-400 border-r-red-500">
                  <div className="absolute inset-5 rounded-full bg-[#142338]" />
                  <div className="relative text-center">
                    <div className="text-[3.7rem] font-semibold leading-none text-white">{dashboard.churnGauge}%</div>
                    <div className="mt-2 text-lg text-slate-300">Churn Probability</div>
                  </div>
                </div>
                <span className="mt-4 metric-badge bg-orange-500/25 text-orange-50">{dashboard.churnGaugeLabel}</span>
              </div>
            </div>

            <div className="space-y-4">
              <div className="panel p-5">
                <h2 className="dashboard-title">Late Payment Forecast</h2>
                <div className="mt-7 flex items-center gap-4">
                  <span className="text-5xl text-slate-500">➤</span>
                  <div className="text-[3.8rem] font-semibold leading-none text-white">{dashboard.latePaymentForecast}</div>
                  <span className="metric-badge bg-red-500/25 text-red-100">{dashboard.latePaymentForecastLabel}</span>
                </div>
                <div className="mt-8 flex items-center gap-3">
                  <span className="text-xl text-red-300">⌘</span>
                  <div className="text-[3.3rem] font-semibold leading-none text-white">{dashboard.potentialLoss}</div>
                  <span className="metric-badge bg-red-500/20 text-red-200">Potential Loss</span>
                </div>
              </div>

              <div className="panel overflow-hidden">
                <div className="flex items-center justify-between border-b border-white/10 px-5 py-4">
                  <h2 className="dashboard-title">Scenario Simulator</h2>
                  <button className="rounded-lg bg-teal-500/80 px-5 py-2 text-lg font-semibold text-slate-100">Run Simulation</button>
                </div>
                <div className="space-y-4 px-5 py-4">
                  <div className="grid gap-3 xl:grid-cols-3">
                    <div className="rounded-md border border-white/15 bg-[#142338] px-4 py-3 text-lg text-slate-200">
                      Payment Delay: <span className="ml-2">{dashboard.simulator.paymentDelay}</span>
                    </div>
                    <div className="rounded-md border border-white/15 bg-[#142338] px-4 py-3 text-lg text-slate-200">
                      Usage Decline: <span className="ml-2">{dashboard.simulator.usageDecline}</span>
                    </div>
                    <div className="rounded-md border border-white/15 bg-[#142338] px-4 py-3 text-lg text-slate-200">
                      Critical Tickets: <span className="ml-2">{dashboard.simulator.criticalTickets}</span>
                    </div>
                  </div>

                  <div className="grid gap-4 border-t border-white/10 pt-4 lg:grid-cols-[0.42fr_1fr]">
                    <div className="text-[2rem] font-semibold text-slate-100">Outcome:</div>
                    <div className="flex flex-wrap items-center gap-4">
                      <div className="text-[2.15rem] text-slate-100">
                        Risk Score: <span className="font-semibold text-orange-300">{dashboard.simulator.score}</span>
                      </div>
                      <span className="metric-badge bg-red-500/20 text-orange-100">{dashboard.simulator.outcome}</span>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>
    </div>
  );
}
