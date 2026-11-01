# SynapseCommander implementation plan

Status: stage 0 workspace cleanup completed; window skeleton planned
2026-11-01; remaining work pending.
The owner approved C#/WinUI 3 presentation over a shared in-process Rust library.
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

## Lessons from earlier plans

SynapseCommander rebuilds gozler-ui in WinUI 3, in the style of TinyTorrent2.
Both projects already corrected structural mistakes through refactoring
plans. Learn from those plans before the skeleton is built, so the same
mistakes are not designed in again.

Record a lesson only when a plan corrected a structural cause that a WinUI C#
presentation could repeat. Write it once, at its owner in
[ARCHITECTURE.md](../ARCHITECTURE.md), with the source plan and commit. An
engine-internal lesson stays with the Rust engine. The
[window rules](../ARCHITECTURE.md#window-rules), and their first rule, that
the window never blocks, are not weakened by any lesson.

| Source | Status | Recorded at |
| --- | --- | --- |
| TinyTorrent2 `14-inspector-plan.md` (`c1d54c9`), fixed in `6c6f57b` | Done | [Publication and activity](../ARCHITECTURE.md#publication-and-activity) |
| goz `window-architecture.md` (`a2daf14` to `c698a56`) and `ui-architecture-fixes.md` | Done | [Window rules](../ARCHITECTURE.md#window-rules) |
| TinyTorrent2 `15-workspace-shell-study.md`, `14-removed-torrents-plan.md`, `tableview-performance-plan.md`, `communication-plan.md`, `library-cache-plan.md`, `payload-move-plan.md`; the restructuring commits `1faab3f` and `7eb19cc` | Done | [Window rules](../ARCHITECTURE.md#window-rules), [Operations in the window](../ARCHITECTURE.md#operations-in-the-window), [Tables](../ARCHITECTURE.md#tables), [Native interface](../ARCHITECTURE.md#native-interface) |
| goz `window-responsiveness.md`, `locations-that-do-not-answer.md`, `maintainability.md`, `result-snapshot.md`, `paths-through-links.md`, `copy-behaviour.md`, `service-repair.md`, `file-manager-spec.md`; the commits `3b307e5`, `0ed6c44`, `d52b4ff` | Done | The same sections, and `Shared.Storage` in [Presentation owners](../ARCHITECTURE.md#presentation-owners) |
| TinyTorrent2 `torrent-control-plan.md` and audit, `drive-work-plan.md`, `peer-map-plan.md`, `subtitles-records-plan.md`, the indexing and crawl research; goz `faster-start.md`, `visibility-plan.md`, `service-settings.md`, `copy-policies.md`, `sorted-index.md`, the early `ui-*.md` plans, `window-painting.md` | Done: no new presentation lesson. Their lessons are engine-internal, specific to the domain or to Win32 painting, or already recorded above. | |

**Completion:** every source is Done or recorded as having no presentation
lesson, and the skeleton in stage 0 starts after this review.

## Stage 0: freeze scope and prepare the new workspace

1. Reconcile PRODUCT.md's baseline rows and 43-command checkpoint against
   the current goz working tree: command table, menu/control gestures,
   settings, viewers, transfers, resources and relevant design sections.
   Record source revision and working-tree changes without modifying them.
2. Confirm the source owner and reproducible library build route in goz. Size
   the goz work of every stage, not only the first slice: for each stage, the
   code that moves out of `gozler-ui::core` into a shared crate, the new
   exports, and the gozler-ui callers that change with it. Record it as a table
   in this plan, in delivery order. This work is likely a large share of the
   whole effort, and it happens in a repository where other sessions have
   uncommitted work, so agree with the owner how goz changes are sequenced
   before stage 1 starts.
3. **Completed 2026-10-30:** removed the superseded Rust application extraction,
   build tooling, CI, installer and inherited documents. Retained the active
   plans, license and repository configuration, with ignore rules for the new
   C# application. The original checkout remains the reference; no archive
   branch or second engine was created.
4. Select supported platform/runtime versions and installer prerequisites for
   the agreed release model in ARCHITECTURE.md. Establish one WinUI build/run
   entry point and a reproducible build of the shared native library. Account
   for unpackaged startup, runtime initialization and Windows integration.
5. Move the Settings projects and the solution from TinyTorrent2's removed
   `lib/Lucide` to `lib/Glyphs`. Today their build stops at the
   `RequireComponents` check against the current TinyTorrent2 checkout.
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
   - `App`, `MainWindow` and `MainViewModel`, with the title bar and menu bar.
   - Both mode views. Search mode is an empty workspace until stage 4.
   - Two panels, each with the drive button, path box, a `Table` with Name,
     Ext, Size and Date modified columns and no rows, and a summary line.
   - The active side: `Tab` and a click switch it, and the active panel shows
     it.
   - The splitter between the panels, the lower pane with its splitter (empty
     and closed), the command line editor without launch, the key bar and the
     status bar.
   - `Shell.Commands` with the commands of this stage, such as Exit, Show
     Search and Show Commander. The menu bar, key bar and accelerators are
     built from it. A command whose feature has not arrived is not in the
     table.
   - Saved window placement, mode, active side, panel folders and splitter
     positions.
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
For the skeleton: `PresentationChecks` passes for the active side, the mode
switch and the command availability, without a window. The analyzers report a
deliberate `.Result` on the UI thread, and the stall detector records a deliberate 300 ms `Thread.Sleep` in
a Debug build. Both deliberate cases are removed afterwards. The window
reopens at its saved placement and mode. `Tab`
moves the active side and the keyboard focus together. `Alt+F7` and
`Alt+F12` switch modes and keep each mode's focus. The menu bar, the key bar
and a key show the same command with the same availability. Screenshots show
Light, Dark and High Contrast.

## Stage 1: share the existing engine through a native library

This stage includes changes in goz and a real managed consumer here. Follow each
repository's instructions and preserve existing work; cross-repository changes
must be reviewable independently.

1. Define the first versioned ABI for engine initialization, request submission,
   state/outcome retrieval, cancellation and orderly shutdown. Resolve buffer
   ownership, string representation, handles, errors and delivery against
   ARCHITECTURE.md.
2. Add a Rust cdylib adapter in the engine's source repository around the existing
   queue. Keep algorithms and policies in their current owner. Preserve direct
   Rust use by gozler-ui.
3. Add the C# client using the matching contract and explicit native lifetimes.
   Keep it separate from WinUI rendering and expose only current consumer needs.
4. Exercise temporary-file copy and delete through that client, including
   progress, a refusal/failure and cancellation. Confirm engine recovery state
   is restored before accepting new work.
5. Read one folder listing through an engine-held result handle: pages with
   the total and completeness, a row that reads as gone after its file is
   deleted, a change delivered by the folder watch, and the release of the
   handle. Panels and Search both depend on this part of the ABI, and it was
   the costliest boundary correction in goz (`result-snapshot.md`), so it is
   proven here rather than in stage 2.
6. Build a reproducible DLL from the selected shared source revision and stage
   it with the matching C# application output. Establish ABI compatibility
   checks and an intentional source-update procedure; no separate package feed
   or library release is required.

**Proof:** the WinUI process loads the DLL and executes the real shared engine;
no engine host/service is installed or spawned. Native boundary tests detect
invalid inputs, incompatible versions, premature releases and lost terminal
outcomes. A smoke check confirms direct Rust use remains intact. Compare a
representative direct/managed operation for wrapper overhead; reuse the
engine's existing correctness evidence rather than copying its test suite.

## Stage 2: folder workflow and command line

Deliver B02-B05, the applicable shell/clipboard parts of B10-B11, and A01-A06,
A10-A11, A16. Use B22 throughout.

1. Expose existing folder enumeration/watch and identity work through its shared
   Rust owner. Keep current accepted rows usable while replacements arrive.
2. Build the two panels, active side, paths, drives/recent locations, marks,
   sortable/virtualized columns, live refresh and removed-drive behavior.
   Fill the skeleton's panels with these listings. Make the TableView changes
   for marks and drag/drop in TinyTorrent2 first. Measure a large folder in
   `Table` against the performance plan's remaining limits.
3. Add per-panel history/favorites, local filtering and panel utilities. Capture
   commands once; hidden marks never turn into unseen destructive targets.
4. Complete panel clipboard actions, incoming/outgoing drag/drop and Rename,
   routing mutations through the stage 1 client.
5. Build the editable command bar using PRODUCT.md's rules. Add terminal launch,
   recall/draft, name/path insertion and cancellable path completion.

**Proof:** slow or unreachable folders leave the other panel and window usable;
stale listings/completions do not replace newer state. Clipboard/drop actions
target the captured entries and pointer destination. A command launched in a
folder containing spaces receives that folder; standalone cd navigates; compound
commands retain shell behavior. Check shell metacharacters, focus routing and
failed launches. Use temporary folders for destructive checks.

## Stage 3: complete operations, control and recovery parity

Deliver B12-B16 and the operation/performance settings from B19.
Stage 2 controls already use this engine; this stage completes its presentation
and exposes the remaining capabilities.

1. Present complete Copy/Move destination and conflict policies, speed settings,
   new folder and explicitly permanent fast delete.
2. Present Compare/Sync/Mirror with preview invalidation and engine-owned commit.
   Preserve the current Mirror integrity and recovery guarantees.
3. Add the shared transfer pane, queue limits/order, run-now/displacement,
   pause/resume/cancel/cancel-all, aggregate/current-file progress and taskbar state.
   Preserve Delete's distinct progress/report behavior where it matters.
4. Present structured partial outcomes and retained reports. Coordinate panel
   refresh and taken entries from accepted engine results.
5. Complete restore-before-submit, Continue/Dismiss and explicit close/Exit
   handling, including close during initialization and required persistence.

**Proof:** UI actions follow authoritative engine states across preparation,
waiting, running, manual pause, cancellation, failure and recovery. Controlled
interruption demonstrates truthful partial results and continuation; reopening
does not resume destructive work without consent. Mirror recovery compares again.
Closing a tab does not cancel a transfer. No UI-thread join waits for disk work.

## Stage 4: indexed search and export parity

Deliver B06-B09, B18 and the remaining search actions of B10-B11.

1. Expose the existing query parser, protocol client and output rules through
   shared Rust code; extract embedded reusable code and update gozler-ui callers.
2. Present global and in-panel search, every match mode/filter, scoped queries,
   highlighting, columns, selection snapshots and inspector metadata.
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

1. Expose shared viewer reads/decoding/navigation and build independent Text/Hex
   windows with bounded cache, copy limits, wrap, retry and removal behavior.
2. Integrate Syno.Settings for searchable settings, individual commits, atomic
   transaction dialogs and reset, persistence, configurable
   shortcuts/context actions and index diagnostics/administration.
3. Implement application activation, optional tray behavior, optional extra
   instances, startup/global shortcuts and own-product update workflow.
   Service administration explicitly targets the configured compatible product.
4. Complete translations, regional formats, syntax help and About.

**Proof:** a large file opens by reading bounded regions; closing viewers releases
native work. Settings survive restart and retain unrelated changes; cancelling a
draft does not apply it. Activation/tray/Exit behave consistently during operations.
Translations cover displayed surfaces and long text. Every B ID has explicit
completion evidence before this stage is called baseline-complete.

## Stage 6: tabs, document actions and ZIP

Deliver A07-A09. Reuse the owners already established.

1. Add native folder tabs and per-tab state without changing operation lifetime.
2. Complete Open with, Explorer handoff, external F4 editor and New text file.
3. Add ZIP creation/extraction through a shared Rust owner or a deliberately
   selected integration that both clients can reuse. Define dependency and
   distribution before implementation; do not add a second C# archive engine.

**Proof:** a closed tab cannot receive late updates; file creation cannot replace
an existing name. Archive tests cover destination escape, absolute/traversal
names, link handling and collisions, plus cancellation and partial extraction.
Source enumeration and archive I/O remain outside the UI thread.

## Stage 7: advanced everyday tools

Deliver A12-A15, A17-A18.

- Batch rename: preview and collision/cycle execution through the shared engine.
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
ABI tests use the real DLL; engine invariants stay in the engine suite; UI
state/routing tests watch actual presentation failures. Use a temporary data
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
belong to the Rust engine and are not listed here.

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
| Concrete ABI payload/delivery, session ownership and shutdown protocol | Stage 1 |
| Command-line grammar, quoting and shortcut conflicts | Stage 2 |
| TableView's marks keyboard mode and row drag/drop, made in TinyTorrent2 ([Tables](../ARCHITECTURE.md#tables)) | Stage 2 |
| TableView's support for millions of rows (performance plan F6, now "Not planned"), including the change to the public `Selection` type, made in TinyTorrent2 | Stage 4, before B08 |
| Whether Commander mode shows the inspector for the active panel. gozler-ui shows it in Search mode only; `Shell.Inspector` supports both. | Stage 4 |
| Recycle Bin mode/default as an addition to permanent fast delete | Stage 3, before the affected behavior |
| Search-result open default and index-administration permissions/target | Stage 4 |
| Tray default, viewer lifetime, activation and update behavior | Stage 5 |
| ZIP implementation/dependency and common distribution | Stage 6 |
| Checksum formats and preview implementation scope | Stage 7 |

## First implementation task

After the current Settings component trial, resume stage 0, including the
window skeleton. Then build the stage 1 vertical slice: one shared Rust DLL
built from a selected source revision, one real C# consumer,
copy/delete/progress/cancel/recovery and one folder listing through a result
handle. Prove shared ownership and native
lifetime before expanding the UI. This is the first development slice, not a
redefinition of the agreed complete product.
