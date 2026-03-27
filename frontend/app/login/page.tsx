import Link from "next/link";

export default function LoginPage() {
  return (
    <main className="shell flex min-h-screen items-center">
      <section className="grid w-full gap-8 lg:grid-cols-[1.2fr_0.8fr]">
        <div className="rounded-[2rem] border border-white/10 bg-white/5 p-10 backdrop-blur-xl">
          <p className="text-sm uppercase tracking-[0.4em] text-emerald-300/80">Sentinel Finance AI</p>
          <h1 className="mt-4 max-w-2xl text-6xl font-semibold leading-tight">Financial risk intelligence for modern revenue operations.</h1>
          <p className="mt-6 max-w-xl text-lg text-slate-300">
            Predict churn, payment delays, and revenue exposure with premium executive visibility.
          </p>
        </div>
        <div className="glass p-8">
          <h2 className="text-2xl font-semibold">Executive access</h2>
          <div className="mt-6 space-y-4">
            <input className="w-full rounded-2xl border border-white/10 bg-slate-900/60 px-4 py-3" placeholder="name@company.com" />
            <input className="w-full rounded-2xl border border-white/10 bg-slate-900/60 px-4 py-3" placeholder="Password" type="password" />
            <Link href="/dashboard" className="block rounded-2xl bg-emerald-400 px-4 py-3 text-center font-semibold text-slate-950">
              Sign in to dashboard
            </Link>
          </div>
        </div>
      </section>
    </main>
  );
}
