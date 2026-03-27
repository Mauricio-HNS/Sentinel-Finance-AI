// ---Made By Destiny7 Softwares---
import { uploadCards } from "@/lib/mock-data";

export default function UploadPage() {
  return (
    <div className="space-y-4">
      <div className="grid gap-4 xl:grid-cols-[1.08fr_0.92fr]">
        <div className="panel px-6 py-5">
          <p className="text-sm uppercase tracking-[0.3em] text-sky-300/80">CSV Upload</p>
          <h2 className="mt-3 text-5xl font-semibold text-white">Ingest customer signals</h2>
          <p className="mt-4 max-w-3xl text-lg leading-8 text-slate-300">
            Load customer, billing, usage, and support exports into the risk engine and prepare the next assessment cycle.
          </p>
        </div>
        <div className="grid gap-4 md:grid-cols-2">
          {uploadCards.map((card) => (
            <div key={card.label} className="metric-card">
              <p className="text-sm text-slate-400">{card.label}</p>
              <div className="mt-4 text-4xl font-semibold text-white">{card.value}</div>
            </div>
          ))}
        </div>
      </div>

      <div className="grid gap-4 xl:grid-cols-[1fr_0.85fr]">
        <div className="panel p-8">
          <div className="rounded-[2rem] border border-dashed border-white/20 bg-[#112036] p-16 text-center">
            <p className="text-2xl font-semibold text-slate-100">Drop dataset here or click to browse</p>
            <p className="mt-3 text-base text-slate-400">Supports customer, payment, usage and support exports in CSV format.</p>
            <button className="mt-8 rounded-xl bg-sky-500 px-6 py-3 text-base font-semibold text-white">Select files</button>
          </div>
        </div>

        <div className="space-y-4">
          <div className="panel p-6">
            <h3 className="dashboard-title">Import checklist</h3>
            <div className="mt-5 grid gap-3">
              <div className="rounded-lg border border-white/10 bg-[#142338] px-4 py-3 text-slate-200">Customer IDs mapped and unique</div>
              <div className="rounded-lg border border-white/10 bg-[#142338] px-4 py-3 text-slate-200">Payment dates normalized to ISO format</div>
              <div className="rounded-lg border border-white/10 bg-[#142338] px-4 py-3 text-slate-200">Usage metrics grouped by monthly reference</div>
              <div className="rounded-lg border border-white/10 bg-[#142338] px-4 py-3 text-slate-200">Support severity levels aligned to platform taxonomy</div>
            </div>
          </div>

          <div className="panel p-6">
            <h3 className="dashboard-title">Processing status</h3>
            <div className="mt-5 space-y-4">
              <div>
                <div className="mb-2 flex items-center justify-between text-sm text-slate-400">
                  <span>Validation</span>
                  <span>100%</span>
                </div>
                <div className="h-3 rounded-full bg-[#142338]"><div className="h-3 w-full rounded-full bg-emerald-500" /></div>
              </div>
              <div>
                <div className="mb-2 flex items-center justify-between text-sm text-slate-400">
                  <span>Transformation</span>
                  <span>74%</span>
                </div>
                <div className="h-3 rounded-full bg-[#142338]"><div className="h-3 w-[74%] rounded-full bg-sky-500" /></div>
              </div>
              <div>
                <div className="mb-2 flex items-center justify-between text-sm text-slate-400">
                  <span>Risk scoring</span>
                  <span>46%</span>
                </div>
                <div className="h-3 rounded-full bg-[#142338]"><div className="h-3 w-[46%] rounded-full bg-orange-400" /></div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
