// ---Made By Destiny7 Softwares---
import Link from "next/link";

export default function LoginPage() {
  return (
    <main className="min-h-screen bg-[radial-gradient(circle_at_top,rgba(74,113,179,0.22),transparent_35%),linear-gradient(180deg,#0a1322_0%,#09111f_100%)] px-6 py-10">
      <section className="mx-auto grid min-h-[88vh] max-w-[1500px] gap-8 lg:grid-cols-[1.15fr_0.85fr]">
        <div className="panel relative overflow-hidden p-12">
          <div className="absolute -right-24 top-[-40px] h-64 w-64 rounded-full bg-sky-500/10 blur-3xl" />
          <div className="absolute bottom-[-60px] left-[-40px] h-72 w-72 rounded-full bg-teal-500/10 blur-3xl" />
          <p className="text-sm uppercase tracking-[0.4em] text-sky-300/80">Sentinel Finance AI</p>
          <h1 className="mt-5 max-w-3xl text-7xl font-semibold leading-[1.05] text-white">Financial risk intelligence for modern revenue operations.</h1>
          <p className="mt-6 max-w-2xl text-xl leading-9 text-slate-300">
            Predict churn, payment delays, and revenue exposure with premium executive visibility, explainable AI narratives, and enterprise-grade scenario modeling.
          </p>

          <div className="mt-10 grid max-w-4xl gap-4 md:grid-cols-3">
            <div className="metric-card">
              <p className="text-sm text-slate-400">Accounts monitored</p>
              <div className="mt-3 text-5xl font-semibold text-white">3.8K</div>
            </div>
            <div className="metric-card">
              <p className="text-sm text-slate-400">Revenue at risk</p>
              <div className="mt-3 text-5xl font-semibold text-white">$489K</div>
            </div>
            <div className="metric-card">
              <p className="text-sm text-slate-400">Predicted churn</p>
              <div className="mt-3 text-5xl font-semibold text-white">32%</div>
            </div>
          </div>

          <div className="mt-10 rounded-2xl border border-white/10 bg-[#13223a]/90 p-6">
            <p className="text-sm uppercase tracking-[0.25em] text-slate-400">Executive briefing</p>
            <p className="mt-4 text-lg leading-8 text-slate-200">
              Alpha Capital Partners shows elevated churn and payment risk due to sharp platform usage decline, repeated critical support incidents, and recent invoice slippage.
            </p>
          </div>
        </div>

        <div className="panel flex flex-col justify-between p-8">
          <div>
            <p className="text-sm uppercase tracking-[0.3em] text-slate-400">Executive access</p>
            <h2 className="mt-3 text-4xl font-semibold text-white">Sign in to the risk control room</h2>
            <div className="mt-8 space-y-4">
              <input className="w-full rounded-2xl border border-white/10 bg-slate-950/60 px-4 py-4 text-lg text-white" placeholder="name@company.com" />
              <input className="w-full rounded-2xl border border-white/10 bg-slate-950/60 px-4 py-4 text-lg text-white" placeholder="Password" type="password" />
              <Link href="/dashboard" className="block rounded-2xl bg-sky-500 px-4 py-4 text-center text-lg font-semibold text-white">
                Sign in to dashboard
              </Link>
            </div>
          </div>

          <div className="mt-8 rounded-2xl border border-white/10 bg-[#13223a]/80 p-5">
            <p className="text-sm uppercase tracking-[0.25em] text-slate-400">Platform includes</p>
            <div className="mt-4 grid gap-3 text-slate-200">
              <div className="rounded-xl bg-white/5 px-4 py-3">Executive dashboard and portfolio health view</div>
              <div className="rounded-xl bg-white/5 px-4 py-3">Customer dossier with AI-generated explanations</div>
              <div className="rounded-xl bg-white/5 px-4 py-3">Scenario simulator and alert center</div>
            </div>
          </div>
        </div>
      </section>
    </main>
  );
}
