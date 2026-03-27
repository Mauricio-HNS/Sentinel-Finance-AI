// ---Made By Destiny7 Softwares---
"use client";

import { useState, useTransition } from "react";
import { useRouter } from "next/navigation";
import { runRiskCopilotEval, type EvalRunResult } from "@/lib/api";

export function AiOpsControlCenter() {
  const router = useRouter();
  const [result, setResult] = useState<EvalRunResult | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [isPending, startTransition] = useTransition();

  const triggerEval = () => {
    startTransition(async () => {
      try {
        setError(null);
        const response = await runRiskCopilotEval();
        setResult(response);
        router.refresh();
      } catch {
        setError("The eval run could not be started. Confirm the API is online and the AI configuration is available.");
      }
    });
  };

  return (
    <div className="panel overflow-hidden">
      <div className="flex items-center justify-between border-b border-white/10 px-5 py-4">
        <div>
          <h2 className="dashboard-title">AI Ops Control Center</h2>
          <p className="mt-2 text-sm text-slate-400">Trigger a smoke eval run to validate the Sentinel risk copilot against the current architecture layer.</p>
        </div>
        <button
          onClick={triggerEval}
          disabled={isPending}
          className="rounded-xl bg-teal-500/80 px-5 py-3 text-sm font-semibold text-slate-100 transition hover:bg-teal-400 disabled:cursor-not-allowed disabled:opacity-50"
        >
          {isPending ? "Running Eval..." : "Run Risk Copilot Eval"}
        </button>
      </div>

      <div className="px-5 py-5">
        {!result && !error ? (
          <div className="rounded-2xl border border-white/10 bg-white/5 p-4 text-sm text-slate-300">
            No eval run requested in this session yet. This control is wired to the backend AI ops endpoint.
          </div>
        ) : null}

        {result ? (
          <div className="rounded-2xl border border-emerald-500/25 bg-emerald-500/10 p-4">
            <div className="flex flex-wrap items-center gap-3">
              <span className="rounded-full bg-emerald-400/20 px-3 py-1 text-xs font-semibold uppercase tracking-[0.22em] text-emerald-100">
                {result.status}
              </span>
              <span className="text-sm text-slate-300">{result.mode}</span>
            </div>
            <p className="mt-3 text-base text-slate-100">{result.summary}</p>
            <div className="mt-4 grid gap-3 md:grid-cols-3">
              <div className="rounded-xl border border-white/10 bg-[#122136] p-3">
                <div className="text-xs uppercase tracking-[0.18em] text-slate-500">Target</div>
                <div className="mt-2 text-sm text-slate-100">{result.target}</div>
              </div>
              <div className="rounded-xl border border-white/10 bg-[#122136] p-3">
                <div className="text-xs uppercase tracking-[0.18em] text-slate-500">Requested At</div>
                <div className="mt-2 text-sm text-slate-100">{new Date(result.requestedAt).toLocaleString("en-US")}</div>
              </div>
              <div className="rounded-xl border border-white/10 bg-[#122136] p-3">
                <div className="text-xs uppercase tracking-[0.18em] text-slate-500">Run Id</div>
                <div className="mt-2 text-sm text-slate-100">{result.runId ?? "Pending / local mode"}</div>
              </div>
            </div>
          </div>
        ) : null}

        {error ? (
          <div className="rounded-2xl border border-red-500/25 bg-red-500/10 p-4 text-sm text-red-100">{error}</div>
        ) : null}
      </div>
    </div>
  );
}
