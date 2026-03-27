import Link from "next/link";
import { getCustomersViewModel } from "@/lib/api";

const toneClasses: Record<string, string> = {
  Healthy: "bg-emerald-500/20 text-emerald-100",
  "Payment Overdue": "bg-red-500/20 text-red-100",
  "Renewal Watch": "bg-orange-500/20 text-orange-100"
};

export default async function CustomersPage() {
  const customers = await getCustomersViewModel();

  return (
    <div className="space-y-4">
      <div className="grid gap-4 xl:grid-cols-[1.1fr_0.9fr]">
        <div className="panel px-6 py-5">
          <p className="text-sm uppercase tracking-[0.3em] text-sky-300/80">Customers</p>
          <h2 className="mt-3 text-5xl font-semibold text-white">Risk-ranked customer portfolio</h2>
          <p className="mt-4 max-w-3xl text-lg leading-8 text-slate-300">
            Explore customers by account health, payment behavior, adoption trend, and renewal timing in one enterprise-grade portfolio view.
          </p>
        </div>

        <div className="grid gap-4 md:grid-cols-3">
          <div className="metric-card">
            <p className="text-sm text-slate-400">Monitored customers</p>
            <div className="mt-4 text-5xl font-semibold text-white">3,842</div>
          </div>
          <div className="metric-card">
            <p className="text-sm text-slate-400">High-risk cluster</p>
            <div className="mt-4 text-5xl font-semibold text-white">318</div>
          </div>
          <div className="metric-card">
            <p className="text-sm text-slate-400">Revenue exposed</p>
            <div className="mt-4 text-5xl font-semibold text-white">$4.8M</div>
          </div>
        </div>
      </div>

      <div className="panel overflow-hidden">
        <div className="flex flex-wrap items-center justify-between gap-4 border-b border-white/10 px-6 py-4">
          <div className="flex flex-wrap gap-3">
            <div className="rounded-lg border border-white/10 bg-[#142338] px-4 py-2 text-sm text-slate-300">Segment: All</div>
            <div className="rounded-lg border border-white/10 bg-[#142338] px-4 py-2 text-sm text-slate-300">Risk: Descending</div>
            <div className="rounded-lg border border-white/10 bg-[#142338] px-4 py-2 text-sm text-slate-300">Status: Active</div>
          </div>
          <div className="w-full max-w-sm rounded-lg border border-white/10 bg-[#142338] px-4 py-2 text-sm text-slate-500">
            Search customers, segments or industries
          </div>
        </div>

        <div className="thin-scrollbar overflow-x-auto">
          <table className="min-w-full text-left">
            <thead className="bg-[#142338] text-sm uppercase tracking-[0.2em] text-slate-400">
              <tr>
                <th className="px-6 py-4">Customer</th>
                <th className="px-6 py-4">Segment</th>
                <th className="px-6 py-4">Health</th>
                <th className="px-6 py-4">Usage</th>
                <th className="px-6 py-4">Renewal</th>
                <th className="px-6 py-4">Revenue</th>
                <th className="px-6 py-4">Action</th>
              </tr>
            </thead>
            <tbody>
              {customers.map((customer, index) => (
                <tr key={customer.id} className={index !== customers.length - 1 ? "border-b border-white/10" : ""}>
                  <td className="px-6 py-5">
                    <div className="flex items-center gap-4">
                      <div className="flex h-11 w-11 items-center justify-center rounded-full bg-slate-200/80 text-sm font-bold text-slate-800">
                        {customer.name.charAt(0)}
                      </div>
                      <div>
                        <p className="text-lg font-semibold text-white">{customer.name}</p>
                        <p className="text-sm text-slate-400">{customer.countryFlag} · {customer.industry}</p>
                      </div>
                    </div>
                  </td>
                  <td className="px-6 py-5 text-slate-200">{customer.segment}</td>
                  <td className="px-6 py-5">
                    <div className="flex items-center gap-3">
                      <span className="inline-flex min-w-14 justify-center rounded-md bg-slate-500/20 px-3 py-1 text-lg font-semibold text-white">
                        {customer.risk}
                      </span>
                      <span className={`metric-badge ${toneClasses[customer.status]}`}>{customer.status}</span>
                    </div>
                  </td>
                  <td className="px-6 py-5 text-slate-200">{customer.activeUsers} active · {customer.usageDelta}</td>
                  <td className="px-6 py-5 text-slate-200">{customer.contractRenewal}</td>
                  <td className="px-6 py-5 text-slate-200">{customer.revenue}</td>
                  <td className="px-6 py-5">
                    <Link href={`/customers/${customer.id}`} className="rounded-lg bg-sky-500/20 px-4 py-2 text-sm font-semibold text-sky-200">
                      Open dossier
                    </Link>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}
