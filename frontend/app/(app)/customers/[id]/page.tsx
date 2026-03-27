import Link from "next/link";
import { customers } from "@/lib/mock-data";

const signalTone: Record<string, string> = {
  red: "bg-red-500/20 text-red-100",
  orange: "bg-orange-500/20 text-orange-100",
  amber: "bg-amber-500/20 text-amber-100",
  green: "bg-emerald-500/20 text-emerald-100"
};

export default function CustomerDetailsPage({ params }: { params: { id: string } }) {
  const customer = customers.find((item) => item.id === params.id) ?? customers[0];

  return (
    <div className="space-y-4">
      <div className="flex flex-wrap items-center justify-between gap-4">
        <div>
          <p className="text-sm uppercase tracking-[0.3em] text-sky-300/80">Customer Detail</p>
          <h2 className="mt-3 text-5xl font-semibold text-white">{customer.name}</h2>
          <p className="mt-4 max-w-4xl text-lg leading-8 text-slate-300">{customer.summary}</p>
        </div>
        <Link href="/customers" className="rounded-lg border border-white/10 bg-white/5 px-4 py-3 text-sm font-semibold text-slate-200">
          Back to customers
        </Link>
      </div>

      <section className="grid gap-4 xl:grid-cols-4">
        <div className="metric-card">
          <p className="text-sm text-slate-400">Overall risk</p>
          <div className="mt-4 text-5xl font-semibold text-white">{customer.risk}%</div>
        </div>
        <div className="metric-card">
          <p className="text-sm text-slate-400">Churn prediction</p>
          <div className="mt-4 text-5xl font-semibold text-white">{customer.churn}%</div>
        </div>
        <div className="metric-card">
          <p className="text-sm text-slate-400">Late payment risk</p>
          <div className="mt-4 text-5xl font-semibold text-white">{customer.late}%</div>
        </div>
        <div className="metric-card">
          <p className="text-sm text-slate-400">Monthly revenue</p>
          <div className="mt-4 text-5xl font-semibold text-white">{customer.revenue}</div>
        </div>
      </section>

      <section className="grid gap-4 xl:grid-cols-[1.1fr_0.9fr]">
        <div className="space-y-4">
          <div className="panel p-6">
            <h3 className="dashboard-title">AI Explanation</h3>
            <p className="mt-5 text-lg leading-8 text-slate-200">{customer.aiNarrative}</p>
          </div>

          <div className="panel overflow-hidden">
            <div className="border-b border-white/10 px-6 py-4">
              <h3 className="dashboard-title">Recent payment history</h3>
            </div>
            <div className="px-6 py-4">
              {customer.paymentHistory.map((payment, index) => (
                <div key={payment.month} className={`grid grid-cols-3 items-center py-4 ${index !== customer.paymentHistory.length - 1 ? "border-b border-white/10" : ""}`}>
                  <div className="text-lg font-semibold text-white">{payment.month}</div>
                  <div className="text-slate-300">{payment.amount}</div>
                  <div>
                    <span className={`metric-badge ${payment.status === "Overdue" ? "bg-red-500/20 text-red-100" : "bg-emerald-500/20 text-emerald-100"}`}>
                      {payment.status}
                    </span>
                  </div>
                </div>
              ))}
            </div>
          </div>
        </div>

        <div className="space-y-4">
          <div className="panel p-6">
            <h3 className="dashboard-title">Account profile</h3>
            <div className="mt-5 space-y-4 text-lg text-slate-300">
              <div className="flex items-center justify-between"><span>Segment</span><span className="text-white">{customer.segment}</span></div>
              <div className="flex items-center justify-between"><span>Plan</span><span className="text-white">{customer.plan}</span></div>
              <div className="flex items-center justify-between"><span>Active users</span><span className="text-white">{customer.activeUsers}</span></div>
              <div className="flex items-center justify-between"><span>Usage delta</span><span className="text-white">{customer.usageDelta}</span></div>
              <div className="flex items-center justify-between"><span>Renewal</span><span className="text-white">{customer.contractRenewal}</span></div>
            </div>
          </div>

          <div className="panel p-6">
            <h3 className="dashboard-title">Recent signals</h3>
            <div className="mt-5 space-y-3">
              {customer.recentSignals.map((signal) => (
                <div key={signal.label} className="flex items-center justify-between rounded-lg border border-white/10 bg-[#142338] px-4 py-3">
                  <span className="text-slate-300">{signal.label}</span>
                  <span className={`metric-badge ${signalTone[signal.tone]}`}>{signal.value}</span>
                </div>
              ))}
            </div>
          </div>
        </div>
      </section>
    </div>
  );
}
