export function KpiCard({ label, value, caption }: { label: string; value: string; caption: string }) {
  return (
    <div className="glass p-5">
      <p className="text-sm text-slate-400">{label}</p>
      <div className="mt-3 text-3xl font-semibold text-white">{value}</div>
      <p className="mt-2 text-sm text-slate-400">{caption}</p>
    </div>
  );
}
