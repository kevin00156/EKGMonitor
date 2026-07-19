# EKG Monitor

A data acquisition and persistence pipeline with a pluggable signal source, plus a WinForms UI.

C# / WinForms / .NET Framework 4.7.2, ~4,100 lines. Archived coursework from a graduate
Object-Oriented Programming class. The code is preserved as it was written; this repository
adds only a `.gitignore` and documentation.

## What it does

Six signal generators are sampled on a 10 ms timer. Each sample set is written into
per-lead in-memory queues, and once the buffer reaches a threshold, a background task
drains it to CSV. People and their records are persisted separately as JSON.

Two things worth stating up front:

- The topic and its requirements (six leads, role separation, record management) were set
  by the course. Driving the pipeline from synthetic signals is part of that specification,
  not a shortcut taken during implementation.
- **There is no signal analysis in this project.** The "physiological data" shown on screen
  is random numbers, unrelated to the waveform. See `ENGINEERING_NOTES.md`, issue (a).

So what this code demonstrates is data flow and lifecycle handling, not biosignal processing.

## Architecture

| Layer | Types |
|---|---|
| Signal | `SignalBase` and four subclasses; `SignalFactory` |
| Record | `EKGRecord : RecordBase, IDisposable` |
| Domain | `PersonBase`, `User`, `Doctor`, `MedicalDataContainer` |
| Serialization | Newtonsoft.Json with a custom `DoctorJsonConverter` |
| UI | `PortableEKGMonitor` (UserControl), generic dialogs `PersonRegisterForm<T>` / `SelectPersonForm<T>` |

```
Windows.Forms.Timer (10 ms, UI thread)
  -> SignalBase.Next() x6
  -> EKGRecord.AddData()      lock(_dataLock), enqueue
  -> at threshold: Task.Run -> lock(_fileLock) -> lock(_dataLock) snapshot
                            -> release data lock -> write CSV
```

`RealSignal` accepts an injected `delegate double SignalProvider(double currentTime)`
as a seam for a real acquisition source. That seam is **not currently used**: nothing in
the codebase constructs a `RealSignal`. The system runs without hardware because all six
leads are driven by synthetic generators, not because of this injection point.

## Concurrency model

Single producer: `AddData` has exactly one call site, driven by a `System.Windows.Forms.Timer`,
so production stays on the UI thread. Sample writes happen on the thread pool.
`SaveBufferedDataToFileAsync` takes `_fileLock` first, then holds `_dataLock` only long enough
to swap the queues out, and performs file I/O after releasing the data lock.

Details, and five known defects in this design, are in
[`ENGINEERING_NOTES.md`](ENGINEERING_NOTES.md).

## Building

Requires Windows and Visual Studio 2022. Dependencies use the legacy packages.config format,
and `packages/` is not tracked, so restore before opening the solution:

```
nuget restore FinalProjectBy113327014.sln
```

`dotnet restore` and `msbuild -t:restore` will not work here: those serve PackageReference.
The csproj imports four `.targets` files from under `packages/` and fails to load if they
are absent.

Key dependencies: ScottPlot 5.0.55 (plotting), SkiaSharp 3.119.0, Newtonsoft.Json 13.0.3.

**Build status:** compiles with 0 warnings and 0 errors. It was also compiled on Linux
with the .NET SDK, which required working around three toolchain issues without touching
the project files; that procedure is documented in `ENGINEERING_NOTES.md`. Compilation only
proves the source type-checks. The output is a WinForms executable, so its runtime behaviour
was not verified outside Windows.

## Scope

All code is my own work. Plotting is handled by ScottPlot, a third-party library, and is not
my work; the same applies to the other NuGet dependencies (SkiaSharp, HarfBuzzSharp,
Newtonsoft.Json, OpenTK).

Project and assembly names are kept as originally submitted.
