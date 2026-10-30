# Architecture: shared Rust engine, WinUI presentation

Status: agreed direction, 2026-10-30. The interface and packaging are planned;
no native DLL or C# client has been implemented by this document.
[Product scope](../PRODUCT.md) and the
[implementation plan](plans/synapsecommander.md) are separate authorities.

## Process and source ownership

SynapseCommander is one C#/WinUI 3 application process. It loads an
application-local Rust DLL. There is no additional engine executable,
background host, Windows service or new file-operation pipe.

gozler-ui continues to use the same Rust implementation as crates in its own
process. Both applications run operations with their own user token. They have
separate queues and do not claim to coordinate disk scheduling across processes.

The existing indexing service retains indexing/search responsibility and its
existing trust contract. It gains no copy, move or delete commands.

```text
goz repository: one engine source, one engine test suite
    shared Rust modules
        +--> gozler-ui: direct Rust calls
        +--> native DLL exports --> SynapseCommander: C# / WinUI 3

both engine instances --> filesystem, using their application's token
shared Rust index client --> existing compatible indexing service
```

Initially keep the shared implementation and native exports in the goz
repository, currently `C:\SynoSoftware\TinyTorrent2\3rdParty\goz`.
Publish a versioned native artifact and contract from there. SynapseCommander
references a pinned release; it does not vendor crates, copy algorithms, import
goz history or build against a developer's absolute checkout path.
Separate products may ship different pinned versions; engine fixes are made
once, then adopted through normal dependency updates.

## Responsibility map

| Owner | Responsibilities |
| --- | --- |
| Shared Rust file operations | Preparation, validation, conflicts, destination rules, copy/move/delete/rename/new folder, synchronization, scheduling, progress, reports, recovery and speed settings |
| Shared Rust index client/query code | Existing query syntax, request construction, protocol/trust checks, snapshots, matching metadata, search/export semantics and service diagnostics |
| Shared Rust folder/viewer work | Reusable folder enumeration/watch logic, file identity and size work; viewer reads, decoding and byte navigation |
| Native exports | ABI validation, ownership of handles/buffers, request/result translation and delivery; no second engine |
| C# engine client | Managed lifetimes, asynchronous calls, native-to-managed translation and delivery to presentation owners |
| C# presentation | Windows, panels/tabs, navigation history, focus/selection, command routing, dialogs, localized text, view state and accessibility |
| C# Windows integration | Clipboard and drag/drop, shell launch, app activation, notification area and global shortcuts |
| C# persistence | SynapseCommander settings and window/tab state; engine recovery records remain Rust-owned |

Some reusable code still lives inside `gozler-ui::core` or its platform modules.
Move that implementation to an appropriate shared Rust owner when exposing it;
update gozler-ui to use the new owner and remove the superseded path in the
same change. Do not port query parsing, viewer decoding or filesystem engines
to C# to get an early screen working. Purely visual state and WinUI mechanics
belong in the new UI and need not reproduce Rust UI models.

Future archive, batch-rename and checksum algorithms belong to a shared Rust
owner as their features land. External tools retain their own implementation.
The UI gathers intent and presents outcomes.

## Native interface

Expose a small C ABI over the existing Rust owners; Rust type layout is not the
public contract. Start with file operations, prove a real C# consumer, then
extend the same contract for search, listings and viewers.

The first contract defines these requirements before implementation:

- Explicit ABI version/capabilities and structured status codes. Check
  compatibility before opening a session or accepting mutations.
- Opaque handles for sessions, operations, result snapshots and viewers as
  needed. Managed SafeHandle ownership never exposes Rust references or
  allocator assumptions.
- Explicit fixed-width scalar/enum representations, calling convention,
  structure sizes, pointer/length pairs and allocator-matched release functions.
  Use length-delimited UTF-16 for Windows paths so unpaired surrogates remain
  representable; keep path, display text and shell command text distinct.
- Native functions validate lengths and ranges and return typed failures.
  Rust panics must never unwind across the ABI; define the containment/fatal
  policy rather than presenting a corrupted engine as a successful call.
- One bounded asynchronous delivery mechanism with batched state and progress.
  Terminal states and failures remain observable even when intermediate
  progress is coalesced. A callback never enters WinUI or calls back into a
  locked engine.
- Snapshot/page access for large results and reports, rather than one native
  call per displayed cell or a full managed copy of millions of rows.
- Explicit cancellation, stale-request identities, result release and shutdown.
  A cancelled observation does not imply an accepted filesystem mutation was
  rolled back.

Finalize the concrete exports, payload representation and delivery mechanism in
stage 1 using the first operation slice. Avoid a generic RPC framework or
serializing every internal Rust structure as the public contract.
Transport/layout compatibility tests supplement the shared engine tests.

Use .NET [native interop guidance](https://learn.microsoft.com/en-us/dotnet/standard/native-interop/best-practices),
including LibraryImport where suitable and SafeHandle for native resources.
[P/Invoke](https://learn.microsoft.com/en-us/dotnet/standard/native-interop/pinvoke)
is the supported managed/native call mechanism. The responsibilities above are
project design decisions, not claims that .NET provides an engine abstraction.

## Responsiveness and state

Rust retains its existing worker/scheduler ownership. C# schedules calls that
can block away from the UI thread and applies only current results through the
window dispatcher. Panel/tab/query identities reject late deliveries after
navigation, replacement or disposal.

Engine state is authoritative for operations. Presentation may keep a snapshot
for rendering, not an independent operation state machine. The UI requests
pause/cancel/reorder and displays the engine's resulting state.

Commands capture their origin and targets once. A later active-panel change
cannot redirect an accepted action. File actions use captured entry identities;
text controls retain native editing semantics. Large selections, filtering,
sorting and payload disposal have bounded UI work.

Reuse one command owner across menus, keys, buttons and context menus. Keep
availability and shortcuts consistent while respecting control focus.
Use native WinUI controls and theme resources. Reuse TinyTorrent2's
`Syno.TableView` for tables; validate its integration against realistic
folder/search sizes instead of building a parallel table implementation.
Custom control work requires a demonstrated gap; inherited Win32 paint
approvals do not define WinUI architecture.

Settings presentation belongs to the reusable C# `Syno.Settings` library, whose
[contract](../lib/Settings/docs/contract.md) owns declarations, layout and commit
mechanics. Hosts supply content, typed bindings and persistence. Ordinary
preferences apply individually; coherent values use transaction forms; complex
work such as speed testing uses a registered workflow in the shared shell.
This UI library is separate from the Rust engine DLL. Its current development
[plan](plans/settings-component.md) precedes main application integration.

## Lifetime, persistence and recovery

One engine session owns the application's operation queue. Panel/tab/viewer
lifetime is independent of accepted operations. Closing a tab or search view
releases observations and cancels its reads, not unrelated transfers.

Restore engine recovery state before enabling new mutations. Rust writes
recovery records; C# writes presentation settings. SynapseCommander uses its
own data directory and never opens gozler-ui's live queue as its own. If extra
processes are enabled, establish exclusive ownership or isolated recovery
stores before allowing mutations; two processes must never resume one record.

Closing behavior is explicit:

- Hiding to the notification area, when enabled, keeps the same application
  process and engine running.
- Actual Exit during work offers to remain open or stop safely and exit.
  Engine work cannot continue after process termination.
- Shutdown rejects new requests, invalidates UI deliveries, requests engine
  cancellation and completes required persistence off the UI thread. Preserve
  the engine's guarantees about unfinished outputs and interrupted work.
- Dispose delivery subscriptions before releasing their targets. Keep the
  DLL loaded while any native handle, callback or engine thread can use it.
  Finalizers are a fallback, not a blocking shutdown path.
- Session end and process crashes may interrupt writes; atomic recovery state
  and truthful next-start reporting matter more than promising completion.

The exact stop protocol must follow the current engine, including copy
cancellation and unfinished-output semantics; an interop wrapper must not
silently alter them.

## Dependency and release contract

Ship the pinned DLL and matching managed contract with the application, resolved
from its controlled install location. Record engine revision, ABI version,
architecture and checksums in release metadata. Retain required license notices.
Do not search PATH or the current folder for a substitute DLL.

The initial vertical slice targets x64. Choose supported Windows/.NET/Windows
App SDK versions and packaging from verified toolchain/platform requirements in
the foundation stage. Each additional architecture needs its matching artifact.

Missing/incompatible engine artifacts give an actionable startup error before
mutations. Missing/incompatible indexing service affects indexed features;
ordinary folder browsing and file operations remain useful. Shared helpers
must tolerate index absence rather than introducing an implicit service
requirement for copy/delete.

Testing covers the shared engine once in its own repository, ABI lifetimes and
compatibility through the real DLL, and C# presentation behavior separately.
Measure representative operations through direct Rust and DLL consumers before
claiming parity of speed or memory use. A shared implementation prevents
algorithm drift; it does not automatically prove the wrapper has no overhead.
