import Link from "next/link";
import { customers } from "@/lib/mock-data";

export default function CustomersPage() {
  return (
    <div className="space-y-6">
      <div>
        <p className="text-sm uppercase tracking-[0.3em] text-emerald-300/70">Customers</p>
        <h2 className="mt-2 text-4xl font-semibold">Risk-ranked customer portfolio</h2>
      </div>
      <div className="glass overflow-hidden">
        <table className="w-full text-left">
          <thead className="border-b border-white/10 text-sm text-slate-400">
            <tr>
              <th className="px-6 py-4">Customer</th>
              <th className="px-6 py-4">Segment</th>
              <th className="px-6 py-4">Industry</th>
              <th className="px-6 py-4">Risk</th>
              <th className="px-6 py-4">Revenue</th>
            </tr>
          </thead>
          <tbody>
            {customers.map((customer) => (
              <tr key={customer.id} className="border-b border-white/5">
                <td className="px-6 py-4">
                  <Link href={`/customers/${customer.id}`} className="font-medium text-white">
                    {customer.name}
                  </Link>
                  <p className="text-sm text-slate-400">{customer.country}</p>
                </td>
                <td className="px-6 py-4">{customer.segment}</td>
                <td className="px-6 py-4">{customer.industry}</td>
                <td className="px-6 py-4">
                  <span className="rounded-full bg-emerald-400/10 px-3 py-1 text-sm text-emerald-300">{customer.risk}%</span>
                </td>
                <td className="px-6 py-4">{customer.revenue}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
