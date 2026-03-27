export default function UploadPage() {
  return (
    <div className="space-y-6">
      <div>
        <p className="text-sm uppercase tracking-[0.3em] text-emerald-300/70">CSV Upload</p>
        <h2 className="mt-2 text-4xl font-semibold">Ingest customer signals</h2>
      </div>
      <div className="glass p-8">
        <div className="rounded-[2rem] border border-dashed border-white/20 bg-slate-950/40 p-12 text-center">
          <p className="text-lg text-slate-200">Drop dataset here or click to browse</p>
          <p className="mt-2 text-sm text-slate-400">Supports customer, payment, usage and support exports in CSV format.</p>
        </div>
      </div>
    </div>
  );
}
