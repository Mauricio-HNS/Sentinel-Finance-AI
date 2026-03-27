import { customers } from "@/lib/mock-data";

export default function CustomerDetailsPage({ params }: { params: { id: string } }) {
  const customer = customers.find((item) => item.id === params.id) ?? customers[0];

  return (
    <div className="space-y-6">
      <div className="flex items-start justify-between">
        <div>
          <p className="text-sm uppercase tracking-[0.3em] text-emerald-300/70">Customer Detail</p>
          <h2 className="mt-2 text-4xl font-semibold">{customer.name}</h2>
          <p className="mt-3 max-w-3xl text-slate-300">{customer.summary}</p>
        </div>
        <div className="glass px-5 py-4">
          <p className="text-sm text-slate-400">Overall risk</p>
          <p className="text-3xl font-semibold text-white">{customer.risk}%</p>
        </div>
      </div>
      <section className="grid gap-4 md:grid-cols-3">
        <div className="glass p-5">
          <p className="text-sm text-slate-400">Churn prediction</p>
          <p className="mt-3 text-3xl font-semibold">{customer.churn}%</p>
        </div>
        <div className="glass p-5">
          <p className="text-sm text-slate-400">Late payment risk</p>
          <p className="mt-3 text-3xl font-semibold">{customer.late}%</p>
        </div>
        <div className="glass p-5">
          <p className="text-sm text-slate-400">Plan</p>
          <p className="mt-3 text-3xl font-semibold">{customer.plan}</p>
        </div>
      </section>
      <section className="glass p-6">
        <h3 className="text-xl font-semibold">AI Explanation</h3>
        <p className="mt-4 text-slate-300">
          Customer shows elevated churn and payment risk due to sharp platform usage decline, repeated critical support incidents, recent payment delay, and an active renewal pressure window.
        </p>
      </section>
    </div>
  );
}
