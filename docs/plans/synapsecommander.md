# SynapseCommander implementation plan

Status: stage 0 workspace cleanup completed; window skeleton planned
2026-11-01; remaining work pending.
The owner approved an all-C# product: WinUI 3 presentation over C# engine
libraries in this repository, with no Rust ([ADR 0001](../adr/0001-no-rust.md)).
There is no extra engine executable or service. Writing this plan does not
change either application's implementation.

Read [product scope](../../PRODUCT.md) and
[architecture](../ARCHITECTURE.md) for their authoritative decisions.
This document owns sequence and completion evidence. Only stage 0's workspace
cleanup is complete; application and engine integration work remain pending.

The owner subsequently requested the reusable
[Settings component trial](settings-component.md) as the current development
task. Reuse TinyTorrent2's existing TableView in it and in the product; do not
create another table implementation. The trial does not complete this plan's
main-app/engine stages.

## How decisions are made

**Owner ruling, 2026-11-01: the work never stops to ask the owner.** The plan
holds clear rules, so a question to the owner would only keep the owner busy.
The lead and the coder decide every design, ownership, technical and UX
question themselves, from this plan, the repository's AGENTS.md files and
contracts, and Fluent 2 and Windows conventions. Where a judgment needs a
point of view those do not give, the lead roleplays the expert or the user it
needs (a WinUI and Fluent reviewer, a busy person with 100,000 files in a
folder), decides, and writes the decision and its reason into this plan.
Starting a stage authorizes everything that stage lists. The only stop is one
the rules already name: the same build error or test failure after a fix,
which the lead reports in its final report and works around where it can.

Git actions, such as a commit or a push, still need the owner's request.

Decisions the lead made under this ruling. Each reason is in the linked
section.

| Date | Decision | Where |
| --- | --- | --- |
| 2026-11-01 | Everything that came from Rust moves to C#: operations, queue, recovery, listings, watching, viewer reads, archives and the search client | [Process and source ownership](../ARCHITECTURE.md#process-and-source-ownership) |
| 2026-11-01 | goz `DESIGN.md`, `docs/research` and `BENCHMARKS.md` are the specification; the Rust code is only a reference, and tests are written fresh | [Process and source ownership](../ARCHITECTURE.md#process-and-source-ownership) |
| 2026-11-01 | Two engine libraries, `lib/Files` and `lib/Index`, each with its own checks; all P/Invoke inside them | [Projects](../ARCHITECTURE.md#projects) |
| 2026-11-01 | The query parser and pipe protocol are ported to C#, with checks built from goz's query tests | [Responsibility map](../ARCHITECTURE.md#responsibility-map) |
| 2026-11-01 | Copy and delete are built and measured first; the measurement chooses the methods | Stage 1 |
| 2026-11-01 | Recovery records use SynapseCommander's own format | [Lifetime, persistence and recovery](../ARCHITECTURE.md#lifetime-persistence-and-recovery) |
| 2026-11-01 | Archives: .NET for ZIP, TAR and TGZ; SharpCompress for 7z and RAR reading; 7z creation by SharpCompress or `7z.dll`, checked in stage 6 | [Responsibility map](../ARCHITECTURE.md#responsibility-map) |

## Lessons from earlier plans

SynapseCommander rebuilds gozler-ui in WinUI 3, in the style of TinyTorrent2.
Both projects already corrected structural mistakes through refactoring
plans. Learn from those plans before the skeleton is built, so the same
mistakes are not designed in again.

Record a lesson only when a plan corrected a structural cause that a WinUI C#
presentation could repeat. Write it once, at its owner in
[ARCHITECTURE.md](../ARCHITECTURE.md), with the source plan and commit. An
engine-internal lesson stays with the engine libraries. The
[window rules](../ARCHITECTURE.md#window-rules), and their first rule, that
the window never blocks, are not weakened by any lesson.

| Source | Status | Recorded at |
| --- | --- | --- |
| TinyTorrent2 `14-inspector-plan.md` (`c1d54c9`), fixed in `6c6f57b` | Done | [Publication and activity](../ARCHITECTURE.md#publication-and-activity) |
| goz `window-architecture.md` (`a2daf14` to `c698a56`) and `ui-architecture-fixes.md` | Done | [Window rules](../ARCHITECTURE.md#window-rules) |
| TinyTorrent2 `15-workspace-shell-study.md`, `14-removed-torrents-plan.md`, `tableview-performance-plan.md`, `communication-plan.md`, `library-cache-plan.md`, `payload-move-plan.md`; the restructuring commits `1faab3f` and `7eb19cc` | Done | [Window rules](../ARCHITECTURE.md#window-rules), [Operations in the window](../ARCHITECTURE.md#operations-in-the-window), [Tables](../ARCHITECTURE.md#tables), [Engine libraries](../ARCHITECTURE.md#engine-libraries) |
| goz `window-responsiveness.md`, `locations-that-do-not-answer.md`, `maintainability.md`, `result-snapshot.md`, `paths-through-links.md`, `copy-behaviour.md`, `service-repair.md`, `file-manager-spec.md`; the commits `3b307e5`, `0ed6c44`, `d52b4ff` | Done | The same sections, and `Shared.Storage` in [Presentation owners](../ARCHITECTURE.md#presentation-owners) |
| TinyTorrent2 `torrent-control-plan.md` and audit, `drive-work-plan.md`, `peer-map-plan.md`, `subtitles-records-plan.md`, the indexing and crawl research; goz `faster-start.md`, `visibility-plan.md`, `service-settings.md`, `copy-policies.md`, `sorted-index.md`, the early `ui-*.md` plans, `window-painting.md` | Done: no new presentation lesson. Their lessons are engine-internal, specific to the domain or to Win32 painting, or already recorded above. | |

**Completion:** every source is Done or recorded as having no presentation
lesson, and the skeleton in stage 0 starts after this review.

## Stage 0: freeze scope and prepare the new workspace

1. Reconcile PRODUCT.md's baseline rows and 43-command checkpoint against
   the current goz working tree: command table, menu/control gestures,
   settings, viewers, transfers, resources and relevant design sections.
   Record source revision and working-tree changes without modifying them.
2. Size the C# engine work of every stage from goz `docs/DESIGN.md`: for each
   stage, the engine behavior it needs, the goz sections that specify it and
   the checks that prove it. Record it as a table in this plan, in delivery
   order. The engine is likely a large share of the whole effort.
3. **Completed 2026-10-30:** removed the superseded Rust application extraction,
   build tooling, CI, installer and inherited documents. Retained the active
   plans, license and repository configuration, with ignore rules for the new
   C# application. The original checkout remains the reference; no archive
   branch or second engine was created.
4. Select supported platform/runtime versions and installer prerequisites for
   the agreed release model in ARCHITECTURE.md. Establish one WinUI build/run
   entry point. Account for unpackaged startup, runtime initialization and
   Windows integration.
5. Move the Settings projects and the solution from TinyTorrent2's removed
   `lib/Lucide` to `lib/Glyphs`. Today their build stops at the
   `RequireComponents` check against the current TinyTorrent2 checkout. Lower
   Settings from Windows App SDK 2.6.4 to TinyTorrent2's 2.5.1, following the
   [version rule](../ARCHITECTURE.md#projects); a failing build names the API
   that sets the minimum.
6. Create `lib/Controls` (`Syno.Controls`) with copies of TinyTorrent2's
   `Controls.Pane`, `Controls.Splitter` and `Controls.MenuItems` at
   `446b3f2` (owner decision, 2026-11-01). Do not change TinyTorrent2. The
   copy must:
   - replace their use of TinyTorrent2's `Strings` catalogue and its `Shell`
     and `Shared` namespaces with the library's own English text and types;
   - take `Pane`'s template from TinyTorrent2's `App.xaml` into the library's
     `Themes/Generic.xaml`, and rename `TinyTorrentSplitterOpacity` to a
     library resource key;
   - make the types that hosts use public, and add the project to the
     solution and `Build.ps1`.
7. Build the window skeleton that
   [WinUI application structure](../ARCHITECTURE.md#winui-application-structure)
   describes. It contains no listing, search or operation code:
   - The three projects: the WinUI application in `app/src`, Presentation in
     `app/presentation` without a WinUI reference, and `PresentationChecks`.
   - `App`, `MainWindow` and `MainViewModel`, with the title bar, its Back
     button and the menu bar.
   - The Commander page and an empty Search page until stage 4, with page
     navigation and its history.
   - Two panels, each with the drive button, path box, a `Table` with Name,
     Ext, Size and Date modified columns and no rows, and a summary line.
   - The active side: `Tab` and a click switch it, and the active panel shows
     it.
   - The splitter between the panels, the command line editor without launch,
     the key bar and the status bar.
   - `Shell.Commands` with the commands of this stage, such as Exit, Show
     Search and Show Commander. The menu bar, key bar and accelerators are
     built from it. A command whose feature has not arrived is not in the
     table.
   - Saved window placement, active side, panel folders and splitter
     positions. The window always opens on the Commander page.
   - Light, Dark and High Contrast resources and the TinyTorrent2 window
     material.
   - The enforcement of the
     [window rules](../ARCHITECTURE.md#window-rules): `Shared.Work`, the
     thread analyzers and banned-symbols list in both product projects, and
     the Debug-build stall detector. The skeleton's own file work, the saving of
     view state, goes through `Shared.Work`.

**Proof:** the baseline checklist accounts for the working source surfaces;
the new app builds and opens a responsive window; the active build does not
compile an extracted gozler-ui. Shared source locations are explicit build
inputs, with developer-specific absolute paths outside version control.
Document actual prerequisites without installing new tools implicitly.
For the skeleton: `PresentationChecks` passes for the active side, page
navigation and its history, and the command availability, without a window.
The analyzers report a deliberate `.Result` on the UI thread, and the stall
detector records a deliberate 300 ms `Thread.Sleep` in a Debug build. Both
deliberate cases are removed afterwards. The window reopens at its saved
placement on the Commander page. `Tab` moves the active side and the keyboard
focus together. `Alt+F7` opens the Search page, and Back, `Alt+Left` and the
mouse back button return to the Commander page with its focus kept. The Back
button stays in place, disabled, on the Commander page. The menu bar, the key
bar and a key show the same command with the same availability. Screenshots
show Light, Dark and High Contrast.

## Stage 1: the C# engine's first slice

This stage builds `Syno.Files` from goz `docs/DESIGN.md` ("The copy, move and
delete engine", "File operations", "Device admission") and goz
`docs/research/fast-copy-delete.md`, and proves it with a real consumer here.

1. **Measure first.** Build the delete and the parallel copy alone, with the
   candidate methods the research names (`CopyFile2` with and without
   unbuffered I/O, an overlapped ring, parallel POSIX delete). Run them on the
   test folders that goz `docs/BENCHMARKS.md` describes, on the owner's machine
   as recorded there, and record the results beside goz's numbers in
   `FilesChecks`. The fastest correct method for each pair of drive kinds
   becomes the default; the measurement does not reopen the choice of C#.
2. Build the queue with its two lanes, Transfers per device and Deletes per
   device, order changes including moving ahead of a running operation, pause,
   Stop and Cancel all for each lane, and the recovery record in this
   product's own format.
3. Build delivery: snapshots, one signal per change, and `Shared.Engine`'s
   drain and apply ([engine libraries](../ARCHITECTURE.md#engine-libraries)).
   `Syno.Files` takes its index as a dependency, so checks run with none.
4. Exercise temporary-file copy and delete through the presentation owners,
   including progress, a refusal/failure and Stop. Confirm recovery state is
   restored before accepting new work.
5. Read one folder listing as an engine-held result set: pages with the total
   and completeness, a row that reads as gone after its file is deleted, a
   change delivered by the folder watch, and the release of the result. Panels
   and Search both depend on this, and it was the costliest boundary
   correction in goz (`result-snapshot.md`), so it is proven here rather than
   in stage 2.

**Proof:** the measurements are recorded, and each chosen method is at least as
fast as goz's recorded number for the same case, or the gap and its cause are
written down. `FilesChecks` cover, on real temporary folders, every integrity
rule of goz `DESIGN.md` that this slice implements: a link is deleted and
never followed, two selected entries never overwrite each other, a stopped
copy removes only its own unfinished file, an interrupted copy resumes inside
the file, and a running Delete stops between items and keeps what it already
deleted. No engine host or service is installed or spawned.

## Stage 2: folder workflow and command line

Deliver B02-B05, the applicable shell/clipboard parts of B10-B11, and A01-A06,
A10-A11, A16, A19, A20 and A22. Use B22 throughout. The
[Total Commander command list](../total-commander-commands.md) names the
commands that each stage implements.

1. Complete folder enumeration, watching and identity work in `Syno.Files`.
   Keep current accepted rows usable while replacements arrive.
2. Build the two panels, active side, paths, drives/recent locations, marks,
   sortable/virtualized columns, live refresh and removed-drive behavior.
   Fill the skeleton's panels with these listings. Make the TableView changes
   for marks and drag/drop in TinyTorrent2 first. Measure a large folder in
   `Table` against the performance plan's remaining limits.
3. Add per-panel history/favorites, local filtering and panel utilities,
   hidden and system files (A19) and folder sizes on demand through the engine
   (A20). Capture commands once; hidden marks never turn into unseen
   destructive targets.
4. Complete panel clipboard actions, incoming/outgoing drag/drop and Rename,
   routing mutations through the stage 1 engine.
5. Build the editable command line using PRODUCT.md's rules. Add terminal
   launch, recall/draft, name/path insertion and cancellable path completion.
6. Fill the key bar and apply the
   [key map](../ARCHITECTURE.md#keyboard-input) on every page (A22).

**Proof:** slow or unreachable folders leave the other panel and window usable;
stale listings/completions do not replace newer state. Clipboard/drop actions
target the captured entries and pointer destination. A command launched in a
folder containing spaces receives that folder; standalone cd navigates; compound
commands retain shell behavior. Check shell metacharacters, focus routing and
failed launches. Use temporary folders for destructive checks.

## Stage 3: complete operations, control and recovery parity

Deliver B12-B16, A21, and the Transfers per device and Deletes per device
settings from B19. Stage 2 controls already use this engine; this stage
completes its presentation and exposes the remaining capabilities. The rules
are in [Operations in the window](../ARCHITECTURE.md#operations-in-the-window).

1. Present the Copy dialog: destination, existing-file policy, and the Sync
   and Mirror buttons, each explained in one line. Present New folder and the
   permanent Delete question with its count. Show no speed presets.
2. Present Sync and Mirror from that dialog, with the Mirror question after the
   comparison. Preserve the current Mirror integrity and recovery guarantees.
3. Add one status bar item per operation with its own Stop, the operations
   button and its menu, and the taskbar progress. Build the File operations
   page with the transfer and Delete lists, reorder including ahead of a
   running operation, pause/resume, Stop, Cancel all copies and Cancel all
   deletes.
4. Present structured partial outcomes and retained reports, including how far
   a stopped operation got. Coordinate panel refresh and taken entries from
   accepted engine results.
5. Mark the differences between the two panels through the engine's
   comparison (A21).
6. Complete restore-before-submit, Continue/Dismiss and explicit close/Exit
   handling, including close during initialization and required persistence.

The `Syno.Files` work of this stage, each with the `FilesChecks` case that
proves it:

| Engine behavior | Check that proves it |
| --- | --- |
| Existing-file policies and conflicts, from goz `DESIGN.md` "File operations" | Each policy on a prepared destination gives the documented result; two selected entries never overwrite each other |
| Sync and Mirror comparison, from goz "Synchronize folders" and decision D13 | Mirror deletes only unchanged destination-only entries, after every required copy succeeded and each entry is checked again |
| A direct Mirror with deletions waits after its comparison; the summary has file count, folder count and bytes; the commit can finish as a Sync | Nothing is copied before the answer; Keep them deletes nothing |
| The two-lane queue from stage 1 under real operations | A Delete waits for a Delete on its disk, never for a copy; moving an operation ahead of a running one pauses the displaced one, which resumes with its progress; Cancel all deletes leaves transfers running |
| Comparison marks for A21 | New and newer entries on each side are marked; equal entries are not |

**Proof:** UI actions follow authoritative engine states across preparation,
waiting, running, manual pause, Stop, failure and recovery. Controlled
interruption demonstrates truthful partial results and continuation; reopening
does not resume destructive work without consent. Mirror recovery compares again.
Closing a tab does not stop a transfer, and starting an operation never
changes the page. No UI-thread join waits for disk work.

## Stage 4: indexed search and export parity

Deliver B06-B09, B18 and the remaining search actions of B10-B11.

1. Build `Syno.Index`: port the query parser, the pipe protocol with its trust
   checks and the output rules from goz to C#. `IndexChecks` are built from
   goz's query tests and help examples and run against a fake pipe server, so
   drift between the two parsers fails a check.
2. Present global and in-panel search, every match mode/filter, scoped queries,
   highlighting, columns, selection snapshots and inspector metadata. Search
   reads names from the index only; it never looks inside files or archives.
   The inspector shows below the results and, as Quick View (`Ctrl+Q`), in
   place of the other panel.
3. Implement bounded pages/snapshots and disposal, consistent sorts and explicit
   incomplete/indexing/unavailable/error state. The results table needs
   TableView's support for millions of rows (performance plan F6).
4. Complete Search clipboard/drop/open routes, including configured external
   commands, and export all results to CSV/TSV/TXT.
5. Preserve useful local browsing and operations without a compatible service.

**Proof:** fixture-driven cases preserve query/output semantics and snapshots
without a live service. One bounded integration check uses an explicitly selected
compatible test service without changing the user's installed service. Large
results stay bounded; a sort, new query or closed view rejects stale replies.

## Stage 5: viewers and application/settings parity

Deliver B01, B17, B19-B21 and finish any uncovered baseline behavior.

1. Build viewer reads, decoding and navigation in `Syno.Files` from goz
   `docs/DESIGN.md`, and independent Text/Hex windows with bounded cache, copy
   limits, wrap, retry and removal behavior.
2. Integrate Syno.Settings for searchable settings, individual commits, atomic
   transaction dialogs and reset, persistence, configurable
   shortcuts/context actions and index diagnostics/administration.
3. Implement application activation, optional tray behavior, single-instance
   handoff and the elevation rule
   ([one process, one main window](../ARCHITECTURE.md#one-process-one-main-window)),
   startup/global shortcuts and own-product update workflow. With the tray
   option on, Close hides the window; Exit asks once while operations run.
   Service administration explicitly targets the configured compatible product.
4. Complete translations, regional formats, syntax help and About.

**Proof:** a large file opens by reading bounded regions; closing viewers releases
native work. Settings survive restart and retain unrelated changes; cancelling a
draft does not apply it. Activation/tray/Exit behave consistently during operations.
Translations cover displayed surfaces and long text. Every B ID has explicit
completion evidence before this stage is called baseline-complete.

## Stage 6: tabs, document actions and archives

Deliver A07-A09. Reuse the owners already established.

1. Add native folder tabs and per-tab state without changing operation lifetime.
2. Complete Open with, Explorer handoff, external F4 editor and New text file.
3. Design the archive interface in `Syno.Files` with the `codebase-design`
   skill. Check whether the current SharpCompress release writes 7z; record
   the result here and use `7z.dll` only if it does not. Then add the ZIP, TAR,
   TGZ, 7z and RAR modules
   ([ADR 0004](../adr/0004-archive-format-modules.md)): archive folders in the
   panels, viewing and copying out, and creating ZIP, 7z, TAR and TGZ. RAR is
   read only. The release carries each library's license notice.

**Proof:** a closed tab cannot receive late updates; file creation cannot replace
an existing name. Archive tests cover destination escape, absolute/traversal
names, link handling and collisions, plus Stop and partial extraction. An
encrypted archive asks for its password once and never saves it. Source
enumeration and archive I/O remain outside the UI thread.

## Stage 7: advanced everyday tools

Deliver A12-A15, A17-A18.

- Multi-Rename page (`Ctrl+M`, Total Commander's Multi-Rename Tool as
  reference): masks parsed and applied by one `Syno.Files` function for both
  the preview and the rename, collision/cycle execution through the engine,
  and Undo of the last batch only.
- External comparison: captured paths and reliable argument handling.
- Checksums: shared cancellable reads and explicit verification outcomes.
- Indexed branch view: existing snapshot/completeness rules.
- Viewer find: shared text/hex search over bounded reads.
- Image/PDF workflow: assess stock/system capabilities first; provide external
  viewing where it completes the task without a new rendering framework.

**Proof:** each feature has a bounded caller-visible scenario and integrity check
at its owner. A preview is not permission to overwrite; metadata is not content
verification; an incomplete index never looks like a complete branch listing.

## Completion and verification discipline

All baseline and addition IDs remain pending. On completion, append the ID,
engine/app revision and concise evidence to this plan; stages above provide the
initial pending checklist. Do not mark grouped IDs complete when only part of
their behavior landed.

Review coherent stages and run the established build entry point after review.
Engine invariants are checked in `FilesChecks` and `IndexChecks` on real
temporary folders and a fake gozlerd pipe; UI state/routing tests watch actual
presentation failures. Use a temporary data
directory and fixtures. Avoid fixed-time waits, foreground-dependent suites and
repeated whole-application automation after every internal edit.

Save screenshots for changed states. Maintain an After install list for hardware,
network-share, shell integration and owner-specific checks; these do not stop
independently verifiable implementation. Hardware claims require measurements.

Before release, verify all scope IDs, installer contents/version compatibility,
prerequisite acquisition, fresh install/upgrade/uninstall behavior, state
preservation, light/dark/high contrast, keyboard/accessibility,
DPI, realistic collection sizes and operation interruption/recovery. No extra
engine executable/service may appear in the shipped package or runtime topology.
User-launched terminals/editors and the pre-existing index service are distinct.

## Calls that can wait

The [window rules](../ARCHITECTURE.md#window-rules) require this list. Each
entry is a Windows call that the C# side makes and that can wait on a disk,
the network, another process or a lock. Add an entry before the feature that
makes the call lands. Folder reads, file operations, search and viewer reads
belong to the engine libraries, which run them on their own threads, and are
not listed here.

| Call | Can wait because | Owner | Stage |
| --- | --- | --- | --- |
| Save of preferences and view state | Disk write | `Shared.Storage` through `Shared.Work` | 0 |
| Drive list, labels and free space | A sleeping or disconnected drive or share | `Panels` through `Shared.Work` | 2 |
| File and folder icons from the shell | The shell reads the file or its handler on a slow path | `Panels` through `Shared.Work` | 2 |
| Path exists or is a folder, for the path box and `cd` | A share that does not answer | `Panels` through `Shared.Work` | 2 |
| Shell launch: Open, Open with, Explorer, Properties, the terminal, the external editor | The shell resolves the path and starts a process | `Shared.Desktop` through `Shared.Work` | 2 |
| Clipboard read and write | Another program holds the clipboard | `Shared.Desktop` through `Shared.Work` | 2 |
| Drop data read | The source program answers slowly | `Shared.Desktop` through `Shared.Work` | 2 |

## Open decisions with a deadline

Resolve these in their named stage, with a recorded choice rather than an
assumed inherited policy:

| Decision | Resolve by |
| --- | --- |
| Supported OS/runtime versions and installer prerequisites | Stage 0 |
| Analyzer packages and rules that enforce the window rules. Recommended: Microsoft.VisualStudio.Threading.Analyzers for synchronous waits and `async void`, and Microsoft.CodeAnalysis.BannedApiAnalyzers for the banned symbols. | Stage 0 |
| The engine API's concrete types, delivery and shutdown | Stage 1 |
| The copy and delete method for each pair of drive kinds, from the measurements | Stage 1 |
| Command-line grammar and quoting | Stage 2 |
| TableView's marks keyboard mode and row drag/drop, made in TinyTorrent2 ([Tables](../ARCHITECTURE.md#tables)) | Stage 2 |
| TableView's support for millions of rows (performance plan F6, now "Not planned"), including the change to the public `Selection` type, made in TinyTorrent2 | Stage 4, before B08 |
| Search-result open default and index-administration permissions/target | Stage 4 |
| Tray default, activation and update behavior | Stage 5 |
| The archive interface and each format module's library and license | Stage 6 |
| Checksum formats and preview implementation scope | Stage 7 |

## First implementation task

After the current Settings component trial, resume stage 0, including the
window skeleton. Then build the stage 1 slice: measure the C# copy and delete
against goz's benchmarks, then the queue, delivery, recovery and one folder
listing as an engine-held result set, with one real consumer. Prove the
speed, the integrity rules and the engine's lifetime before expanding the UI.
This is the first development slice, not a redefinition of the agreed
complete product.
