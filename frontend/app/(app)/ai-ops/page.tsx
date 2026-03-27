// ---Made By Destiny7 Softwares---
import { AiOpsControlCenter } from "@/components/ai-ops-control-center";
import { getAiOpsViewModel } from "@/lib/api";

const statusTone: Record<string, string> = {
  true: "bg-emerald-500/20 text-emerald-100 border-emerald-400/25",
  false: "bg-amber-500/20 text-amber-100 border-amber-400/25"
};

function CapabilityCard({
  title,
  value,
  helper,
  active
}: {
  title: string;
  value: string;
  helper: string;
  active: boolean;
}) {
  return (
    <div className="metric-card min-w-0">
      <div className="flex items-start justify-between gap-4">
        <div>
          <p className="text-sm uppercase tracking-[0.24em] text-slate-500">{title}</p>
          <div className="mt-3 text-2xl font-semibold text-slate-100">{value}</div>
          <p className="mt-3 max-w-sm text-sm leading-6 text-slate-400">{helper}</p>
        </div>
        <span className={`rounded-full border px-3 py-1 text-xs font-semibold uppercase tracking-[0.22em] ${statusTone[String(active)]}`}>
          {active ? "Active" : "Standby"}
        </span>
      </div>
    </div>
  );
}

export default async function AiOpsPage() {
  const aiOps = await getAiOpsViewModel();

  return (
    <div className="space-y-4">
      <section className="panel overflow-hidden">
        <div className="flex flex-col gap-6 border-b border-white/10 px-6 py-5 lg:flex-row lg:items-end lg:justify-between">
          <div>
            <p className="text-sm uppercase tracking-[0.28em] text-sky-300/80">AI Native Operations</p>
            <h1 className="mt-3 text-4xl font-semibold text-slate-50">Sentinel AI Ops</h1>
            <p className="mt-4 max-w-4xl text-base leading-7 text-slate-300">
              Monitor the active reasoning layer, explanation mode, retrieval architecture, and eval posture behind the financial risk copilot.
            </p>
          </div>
          <div className="rounded-2xl border border-cyan-400/20 bg-cyan-400/10 px-5 py-4">
            <div className="text-xs uppercase tracking-[0.22em] text-cyan-100/75">Architecture Posture</div>
            <div className="mt-2 text-2xl font-semibold text-cyan-50">Senior / Futuristic</div>
          </div>
        </div>

        <div className="grid gap-4 px-6 py-5 xl:grid-cols-4">
          <CapabilityCard
            title="Copilot Layer"
            value={aiOps.status.copilotMode}
            helper="Structured executive briefings for customer-level financial risk analysis."
            active={aiOps.status.openAiConfigured}
          />
          <CapabilityCard
            title="Explanation Layer"
            value={aiOps.status.explanationMode}
            helper="Narratives shaped for deterministic rendering and executive communication."
            active={aiOps.status.openAiConfigured}
          />
          <CapabilityCard
            title="Retrieval Layer"
            value={aiOps.status.retrievalMode}
            helper="Grounding path for contracts, support intelligence, and operating playbooks."
            active={aiOps.status.vectorStoreConfigured}
          />
          <CapabilityCard
            title="Eval Layer"
            value={aiOps.status.evalMode}
            helper="Smoke-test capability to measure grounding, actionability, and calibration."
            active={aiOps.status.evalSuiteConfigured}
          />
        </div>
      </section>

      <section className="grid gap-4 xl:grid-cols-[1.15fr_0.85fr]">
        <div className="space-y-4">
          <AiOpsControlCenter />

          <div className="panel overflow-hidden">
            <div className="flex items-center justify-between border-b border-white/10 px-5 py-4">
              <h2 className="dashboard-title">Evaluation Trail</h2>
              <span className="rounded-full bg-white/5 px-3 py-1 text-xs uppercase tracking-[0.22em] text-slate-400">{aiOps.evals.length} scenarios</span>
            </div>

            <div className="space-y-3 px-5 py-5">
              {aiOps.evals.map((item) => (
                <div key={`${item.evaluationName}-${item.scenario}`} className="rounded-2xl border border-white/10 bg-white/[0.03] p-4">
                  <div className="flex flex-wrap items-center justify-between gap-3">
                    <div className="text-lg font-semibold text-slate-100">{item.evaluationName}</div>
                    <span className="rounded-full bg-sky-400/15 px-3 py-1 text-xs uppercase tracking-[0.22em] text-sky-200">{item.status}</span>
                  </div>
                  <p className="mt-3 text-sm leading-6 text-slate-300">{item.scenario}</p>
                  <div className="mt-4 grid gap-3 md:grid-cols-3">
                    <div className="rounded-xl border border-white/10 bg-[#122136] p-3">
                      <div className="text-xs uppercase tracking-[0.18em] text-slate-500">Expected</div>
                      <div className="mt-2 text-sm text-slate-100">{item.expectedBehavior}</div>
                    </div>
                    <div className="rounded-xl border border-white/10 bg-[#122136] p-3">
                      <div className="text-xs uppercase tracking-[0.18em] text-slate-500">Scorecard</div>
                      <div className="mt-2 text-sm text-slate-100">{item.scorecard}</div>
                    </div>
                    <div className="rounded-xl border border-white/10 bg-[#122136] p-3">
                      <div className="text-xs uppercase tracking-[0.18em] text-slate-500">Target</div>
                      <div className="mt-2 text-sm text-slate-100">{item.modelTarget}</div>
                      <div className="mt-2 text-xs text-slate-500">{item.updatedAt}</div>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          </div>
        </div>

        <div className="space-y-4">
          <div className="panel p-5">
            <h2 className="dashboard-title">Execution Graph</h2>
            <div className="mt-6 rounded-[1.8rem] border border-white/10 bg-[radial-gradient(circle_at_top,rgba(34,211,238,0.12),transparent_42%),linear-gradient(180deg,#10203a,#0f1a2e)] p-5">
              <div className="grid gap-4">
                <div className="rounded-2xl border border-cyan-400/15 bg-cyan-400/10 p-4">
                  <div className="text-xs uppercase tracking-[0.22em] text-cyan-100/70">Input</div>
                  <div className="mt-2 text-lg font-semibold text-cyan-50">Customer risk question + operational signals</div>
                </div>
                <div className="pl-4 text-cyan-200/60">↓</div>
                <div className="rounded-2xl border border-fuchsia-400/15 bg-fuchsia-400/10 p-4">
                  <div className="text-xs uppercase tracking-[0.22em] text-fuchsia-100/70">Reasoning</div>
                  <div className="mt-2 text-lg font-semibold text-fuchsia-50">{aiOps.status.copilotMode}</div>
                </div>
                <div className="pl-4 text-fuchsia-200/60">↓</div>
                <div className="rounded-2xl border border-amber-400/15 bg-amber-400/10 p-4">
                  <div className="text-xs uppercase tracking-[0.22em] text-amber-100/70">Grounding</div>
                  <div className="mt-2 text-lg font-semibold text-amber-50">{aiOps.status.retrievalMode}</div>
                </div>
                <div className="pl-4 text-amber-200/60">↓</div>
                <div className="rounded-2xl border border-emerald-400/15 bg-emerald-400/10 p-4">
                  <div className="text-xs uppercase tracking-[0.22em] text-emerald-100/70">Assurance</div>
                  <div className="mt-2 text-lg font-semibold text-emerald-50">{aiOps.status.evalMode}</div>
                </div>
              </div>
            </div>
          </div>

          <div className="panel p-5">
            <h2 className="dashboard-title">Power Signals</h2>
            <div className="mt-5 space-y-3">
              {[
                "OpenAI Responses API path with structured JSON outputs",
                "Vector-store-ready retrieval via file_search configuration",
                "Eval smoke-run endpoint for risk copilot quality checks",
                "Clean Architecture orchestration across .NET, FastAPI, and AI services"
              ].map((item) => (
                <div key={item} className="flex items-start gap-3 rounded-xl border border-white/10 bg-white/[0.03] px-4 py-3">
                  <span className="mt-1 h-2.5 w-2.5 rounded-full bg-cyan-300" />
                  <p className="text-sm leading-6 text-slate-200">{item}</p>
                </div>
              ))}
            </div>
          </div>
        </div>
      </section>
    </div>
  );
}
