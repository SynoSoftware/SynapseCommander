# SynapseCommander implementation plan

Status: stage 0 workspace cleanup completed; remaining work pending, 2026-10-30.
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

## Stage 0: freeze scope and prepare the new workspace

1. Reconcile PRODUCT.md's baseline rows and 43-command checkpoint against
   the current goz working tree: command table, menu/control gestures,
   settings, viewers, transfers, resources and relevant design sections.
   Record source revision and working-tree changes without modifying them.
2. Confirm the source owner and artifact release route in goz. Identify the
   reusable code still embedded in gozler-ui and the minimum extraction
   needed for the first file-operation slice.
3. **Completed 2026-10-30:** removed the superseded Rust application extraction,
   build tooling, CI, installer and inherited documents. Retained the active
   plans, license and repository configuration, with ignore rules for the new
   C# application. The original checkout remains the reference; no archive
   branch or second engine was created.
4. Select supported platform/runtime versions and initial packaging from the
   installed toolchain and official requirements. Establish one WinUI build/run
   entry point and dependency restoration for the versioned native artifact.
5. Bootstrap the minimal WinUI application with light/dark resources and keyboard
   focus. Keep configuration and entry points only as large as this requires.

**Proof:** the baseline checklist accounts for the working source surfaces;
the new app builds and opens a responsive window; the active build does not
compile an extracted gozler-ui or require a hard-coded sibling checkout.
Document actual prerequisites without installing new tools implicitly.

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
5. Package a reproducible, versioned native artifact and pin it here. Establish
   ABI compatibility checks and an intentional dependency-update procedure.

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
   incomplete/indexing/unavailable/error state.
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

Before release, verify all scope IDs, package contents/version compatibility,
fresh install/upgrade behavior, light/dark/high contrast, keyboard/accessibility,
DPI, realistic collection sizes and operation interruption/recovery. No extra
engine executable/service may appear in the shipped package or runtime topology.
User-launched terminals/editors and the pre-existing index service are distinct.

## Open decisions with a deadline

Resolve these in their named stage, with a recorded choice rather than an
assumed inherited policy:

| Decision | Resolve by |
| --- | --- |
| Supported OS/runtime versions, packaging and canonical artifact publication | Stage 0 |
| Concrete ABI payload/delivery, session ownership and shutdown protocol | Stage 1 |
| Command-line grammar, quoting and shortcut conflicts; width persistence/auto-fit | Stage 2 |
| Recycle Bin mode/default as an addition to permanent fast delete; extra-process recovery ownership | Stage 3, before the affected behavior |
| Search-result open default and index-administration permissions/target | Stage 4 |
| Tray default, viewer lifetime, activation and update distribution | Stage 5 |
| ZIP implementation/dependency and common distribution | Stage 6 |
| Checksum formats and preview implementation scope | Stage 7 |

## First implementation task

After the current Settings component trial, resume stage 0 and the stage 1
vertical slice: one pinned Rust DLL, one real C#
consumer, copy/delete/progress/cancel/recovery. Prove shared ownership and native
lifetime before expanding the UI. This is the first development slice, not a
redefinition of the agreed complete product.
