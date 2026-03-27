// ---Made By Destiny7 Softwares---
"use client";

import { useRef, useState, useTransition } from "react";
import { useRouter } from "next/navigation";
import { uploadCustomerDataset, type CsvUploadResult } from "@/lib/api";

export function UploadWorkbench() {
  const inputRef = useRef<HTMLInputElement | null>(null);
  const router = useRouter();
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [result, setResult] = useState<CsvUploadResult | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [isPending, startTransition] = useTransition();

  const openPicker = () => inputRef.current?.click();

  const onFileChange = (event: { target: HTMLInputElement }) => {
    const file = event.target.files?.[0] ?? null;
    setSelectedFile(file);
    setResult(null);
    setError(null);
  };

  const upload = () => {
    if (!selectedFile) {
      setError("Choose a CSV file before uploading.");
      return;
    }

    startTransition(async () => {
      try {
        setError(null);
        const response = await uploadCustomerDataset(selectedFile);
        setResult(response);
        router.refresh();
      } catch {
        setError("The upload could not be completed. Make sure the API is running.");
      }
    });
  };

  return (
    <div className="panel p-8">
      <input ref={inputRef} type="file" accept=".csv,text/csv" className="hidden" onChange={onFileChange} />

      <div className="rounded-[2rem] border border-dashed border-white/20 bg-[#112036] p-16 text-center">
        <p className="text-2xl font-semibold text-slate-100">Drop dataset here or click to browse</p>
        <p className="mt-3 text-base text-slate-400">Supports customer, payment, usage and support exports in CSV format.</p>
        <div className="mt-8 flex flex-wrap items-center justify-center gap-3">
          <button onClick={openPicker} className="rounded-xl bg-sky-500 px-6 py-3 text-base font-semibold text-white">
            Select file
          </button>
          <button
            onClick={upload}
            disabled={isPending || !selectedFile}
            className="rounded-xl border border-white/10 bg-white/5 px-6 py-3 text-base font-semibold text-slate-100 disabled:cursor-not-allowed disabled:opacity-40"
          >
            {isPending ? "Uploading..." : "Run import"}
          </button>
        </div>

        <div className="mt-6 min-h-6 text-sm text-slate-300">
          {selectedFile ? `Selected: ${selectedFile.name}` : "No file selected yet."}
        </div>
      </div>

      {result ? (
        <div className="mt-4 rounded-2xl border border-emerald-500/30 bg-emerald-500/10 p-5">
          <div className="text-lg font-semibold text-emerald-100">Import completed</div>
          <div className="mt-2 text-slate-200">
            File <span className="font-semibold">{result.fileName}</span> processed with <span className="font-semibold">{result.importedRows}</span> imported rows.
          </div>
        </div>
      ) : null}

      {error ? (
        <div className="mt-4 rounded-2xl border border-red-500/30 bg-red-500/10 p-5 text-red-100">
          {error}
        </div>
      ) : null}
    </div>
  );
}
