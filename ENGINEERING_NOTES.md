# Engineering Notes

A technical read-through of this archived coursework, written roughly a year after the code
itself. Everything below is derived from the source; none of it is a reconstruction of what
I was thinking at the time.

Line references point at the code as committed and are stable, since the code is frozen.

## Concurrency model

### Producer side

`AddData` (`EKGRecord.cs:282`) has exactly one call site in the entire repository:
`PortableEKGMonitor.cs:180`, inside `TimerScanning`. That handler is driven by a
`System.Windows.Forms.Timer` at a 10 ms interval (`PortableEKGMonitor.cs:135-136`).

The timer type matters. `System.Windows.Forms.Timer` marshals its callback onto the UI
message loop, so every call to `AddData` arrives on the same thread. `System.Timers.Timer`
would have fired on a thread-pool thread and invalidated the single-producer assumption that
the rest of this design rests on. The distinction is visible in the API: this timer exposes
`Tick` and an `int Interval`, whereas `System.Timers.Timer` exposes `Elapsed`.

### Consumer side

Three entry points reach the drain path:

| Entry point | Thread | Note |
|---|---|---|
| `AddData` threshold trigger (`:323`) | thread pool | fire-and-forget `Task.Run` |
| `ManualSaveAsync` (`PortableEKGMonitor.cs:195`) | caller's | no call sites currently |
| `Dispose` (`:590`) | UI | see defect (d): this call is a no-op |

So the consumer side is not single-threaded. Mutual exclusion between drains comes entirely
from `_fileLock`.

### Lock ordering

`SaveBufferedDataToFileAsync` takes `_fileLock` first (`:664`), then nests `_dataLock`
(`:680`). Inside the data lock it does three things and nothing else:

1. check whether every queue is empty (`:683`)
2. copy each lead queue and clear the original (`:690-694`)
3. read the current sample count (`:695`)

The data lock is released at `:696`. `_WriteDataToFile` runs at `:702`, outside it. File I/O
therefore never holds the data lock, and the producer can keep enqueueing into the freshly
emptied queues while a write is in flight. `AutoFlush` is on, so each batch reaches disk as
it is written rather than at close.

## Known defects

The first three are my own assessment of the design. The last two I did not notice until
re-reading the code to archive it.

### (a) `_CalculatePhysiologicalData()` is a stub

`EKGRecord.cs:792-800` returns `_random.NextDouble() * 150` for each lead. It never reads the
input signal. The code still carries its original `//TODO`. The six on-screen labels are
therefore unrelated to the displayed waveform.

Signal analysis was outside the scope of the course, so the absence of an implementation is
expected. The placement is not. This method sits inside `EKGRecord` and is invoked from
`_AddDataToPlots` (`:351`), which means every single sample pays the cost of a full recompute.

What I would do now: derived quantities like heart rate or intervals belong in an analysis
component that consumes a window of samples and returns a result, with the caller deciding
when to run it. Acquisition should not be coupled to analysis at all, let alone at per-sample
granularity.

### (b) The auto-flush trigger is a check-then-act

`AddData:320` reads `BufferedDataCount >= BUFFER_THRESHOLD && !IsWritingToFile`. The two
halves fail differently, and it is worth being precise about which is which.

`BufferedDataCount` (`:97-107`) acquires `_dataLock` internally, so the count itself is read
under the lock. The problem is that the lock is released before the comparison and before
the `Task.Run` at `:323`, so the value can be stale by the time it is acted on. This is a
TOCTOU, not a torn read.

`IsWritingToFile` (`:113`) is the genuinely unsynchronized read. `_isWritingToFile` (`:61`)
is not `volatile`, and there is no `volatile` or `Interlocked` anywhere in the repository.

The worst outcome is a wasted task, not corruption. But the reason is worth stating, because
it is not the reason the code appears to assume. Safety comes from `_fileLock` itself, not
from the re-check at `:666`. In fact the `_isWritingToFile` half of that re-check is dead:
every write to the flag (`:672`, `:717`, `:750`, `:787`) happens while `_fileLock` is held,
so any thread that acquires the lock necessarily observes `false`. A redundant task blocks on
the lock, then hits the all-empty check at `:683` and returns having done nothing.

What I would do now: delete the flag and both checks. Either gate dispatch with a single
`Interlocked.CompareExchange`, or move the drain into a dedicated consumer loop behind a
`Channel` or `BlockingCollection` so the producer only posts and never decides.

### (c) `_sampleCount++` sits outside the lock

The increment is at `:290`; the reads are at `:695` and `:773`, both inside `_dataLock`.
Single-producer means no increment is lost, so this looked to me like an inconsistency
without consequences. It has consequences. See (e).

### (d) The final flush in `Dispose` is dead code

`Dispose` sets `_disposed = true` at `:580`, then calls `SaveBufferedDataToFile()` at `:590`.
That method opens with `if (_isReadOnlyMode || _disposed) return;` at `:738`. The condition
is always true by then, so the call is guaranteed to do nothing.

The blast radius is bounded: `AutoFlush` means every batch that reached the threshold is
already on disk. What is lost is the tail, the samples buffered since the last flush, so
fewer than `BUFFER_THRESHOLD` of them.

What I would do now: move `_disposed = true` after the flush, or better, split the state.
"No longer accepting new data" and "resources released" are two different conditions and
should not share a flag. Collapsing them is what let a shutdown-time write be blocked by a
shutdown-time guard.

### (e) (c) shifts the sample numbering in the CSV

The ordering is the problem. `_sampleCount++` at `:290` happens *before* the matching sample
is enqueued, which occurs at `:312` inside the `lock (_dataLock)` opened at `:296`.

If the drain thread acquires `_dataLock` inside that window, it snapshots a count one higher
than the number of samples actually buffered. `_WriteDataToFile` then back-computes the
starting index as `startSampleCount = endSampleCount - dataCount + 1` (`:819`), so every row
in that batch is written with an index one off.

So (c) is not an inconsistency that happens to be harmless. It produces incorrect output,
silently, under a race that a 10 ms producer will eventually hit.

What I would do now: move `_sampleCount++` inside the lock at `:296`. The counter and the
queue contents are one piece of state and have to change together. The fix costs nothing.

## Other observations

Smaller things noticed during the same read-through, recorded for completeness:

- Serial numbers do not survive a restart. `User.cs:16-17` and `Doctor.cs:18-19` declare
  `private static int _serialNum`, which Newtonsoft ignores by default, so the counter resets
  to zero on every launch and re-issues IDs that already exist.
- `log/mock.csv` is truncated on every startup. `PortableEKGMonitor.cs:86` constructs an
  `EKGRecord` against that path in the `Load` handler, before any user is chosen, and the
  constructor opens the file with `append: false`.
- Replaced records are never disposed. The `ekgRecord` setter (`PortableEKGMonitor.cs:44-55`)
  overwrites `_ekgRecord` without disposing the outgoing instance, leaking its `StreamWriter`.
  `NewRecord` and both load paths go through this setter.
- The CSV header is culture-dependent. `_WriteHeader` (`:360`) interpolates the timestamp
  under the current culture, producing values like `2025/5/29 下午 12:55:00`, which
  `_ParseHeader` then has to read back.
- The "rows written" log line always reports zero. `:704` and `:779` evaluate the queue count
  after `_WriteDataToFile` has already dequeued everything.
- There is no test project. The solution contains exactly one project, and no file under the
  tree is a test. Nothing here is covered, including the `RealSignal` seam.
