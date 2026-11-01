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
Starting a stage authorizes everything that stage lists, except what the two
exceptions below hold for the owner. The only stop is one the rules already
name: the same build error or test failure after a fix, which the lead
reports in its final report and works around where it can.

Git actions, such as a commit or a push, still need the owner's request.

**Owner ruling, 2026-11-01: a departure from Total Commander's behavior
needs the owner's approval.** This is the first exception to deciding without
asking. Total Commander's keys, gestures, commands and what each command does
are the interaction model; a departure is written into this plan only after
the owner approves it, with Total Commander's behavior and the reason. Until
the owner approves a proposed departure, the work follows Total Commander's
behavior, so no stage stops for the answer. The owner rulings recorded in
these documents, such as F2 for Rename, Ctrl+L for the path box, the pages
and the permanent Delete, are approved departures. Controls and visuals
follow WinUI 3 and Fluent 2 and are not departures from Total Commander.
Total Commander's window waits on a share or a slow folder; SynapseCommander's
never does, because the window never blocks (owner ruling, 2026-11-01). That
is an approved departure wherever Total Commander waits. Total Commander's
copy also stops at each question; in SynapseCommander nothing waits for the
person (owner ruling, 2026-11-01): questions go into a list, and every task
that can be done is done meanwhile, as
[Operations in the window](../ARCHITECTURE.md#operations-in-the-window) says.

**Owner ruling, 2026-11-01: every piece of UI is prototyped first and
approved by the owner.** Each window, page, dialog, pane, flyout, menu, bar
and status item is first built as a throwaway prototype with the
`prototype` skill and shown to the owner. Its real implementation starts
only after the owner approves that prototype, and it follows the approved
prototype; a change to an approved UI gets a new prototype and approval.
This is the second exception to deciding without asking. While a prototype
waits for approval, the stage goes on with its other work, such as engine
behavior and presentation owners, and that UI's implementation waits. Each
stage's completion evidence names the approved prototypes it implemented.
Controls and visuals are part of what a prototype shows and the owner
approves.

Decisions the lead made under the first ruling, that the work never stops to
ask. Each reason is in the linked section.

| Date | Decision | Where |
| --- | --- | --- |
| 2026-11-01 | Everything that came from Rust moves to C#: operations, queue, recovery, listings, watching, viewer reads, archives and the search client | [Process and source ownership](../ARCHITECTURE.md#process-and-source-ownership) |
| 2026-11-01 | goz `DESIGN.md`, `docs/research` and `BENCHMARKS.md` are the specification; the Rust code is only a reference, and tests are written fresh | [Process and source ownership](../ARCHITECTURE.md#process-and-source-ownership) |
| 2026-11-01 | Two engine libraries, `lib/Files` and `lib/Index`, each with its own checks; all P/Invoke inside them, except the shell and OLE calls in `Shared.Desktop` | [Projects](../ARCHITECTURE.md#projects) |
| 2026-11-01 | The query parser and pipe protocol are ported to C#, with checks built from goz's query tests | [Responsibility map](../ARCHITECTURE.md#responsibility-map) |
| 2026-11-01 | Copy and delete are built and measured first; the measurement chooses the methods | Stage 1 |
| 2026-11-01 | Recovery records use SynapseCommander's own format | [Lifetime, persistence and recovery](../ARCHITECTURE.md#lifetime-persistence-and-recovery) |
| 2026-11-01 | Archives: .NET for ZIP, TAR and TGZ; SharpCompress for 7z and RAR reading; 7z creation by SharpCompress or `7z.dll`, checked in stage 6 | [Responsibility map](../ARCHITECTURE.md#responsibility-map) |
| 2026-11-01 | `IndexChecks` compares the port with goz's protocol version and query files, because gozlerd serves one version | [Responsibility map](../ARCHITECTURE.md#responsibility-map) |
| 2026-11-01 | A search list that lost its gozlerd snapshot searches again and keeps the focus and marks by path | [Engine libraries](../ARCHITECTURE.md#engine-libraries) |
| 2026-11-01 | Rename, New folder and a same-volume Move take no disk and never wait; a Rename or same-volume Move of an entry that a transfer still uses runs at once, as in Total Commander, and the transfer's report lists the entry as not found | [Operations in the window](../ARCHITECTURE.md#operations-in-the-window) |
| 2026-11-01 | An operation is saved at Exit when it has a recovery record; PRODUCT B16 reads "Durable started work" | [Operations in the window](../ARCHITECTURE.md#operations-in-the-window) |
| 2026-11-01 | The goz checkout's newest release tag, not its working tree, is the version `Syno.Index` follows | [Responsibility map](../ARCHITECTURE.md#responsibility-map) |
| 2026-11-01 | `cm_LeftOpenDrives` and `cm_RightOpenDrives` are Covered by Alt+F1 and Alt+F2, to agree with the owner's key map | [Command list](../total-commander-commands.md) |
| 2026-11-01 | goz's admission rules apply to both lanes; one state type and a sequence check hold the queue's rules | [Operations in the window](../ARCHITECTURE.md#operations-in-the-window) |
| 2026-11-01 | Exit ends within 10 seconds; a second launch waits up to 10 seconds for a process without a window | [Engine libraries](../ARCHITECTURE.md#engine-libraries), [One process, one main window](../ARCHITECTURE.md#one-process-one-main-window) |
| 2026-11-01 | Copy methods are measured for SSD to SSD on this machine; other drive pairs use goz's Automatic values until measured | Stage 1 |
| 2026-11-01 | A restart in either direction starts the new process before the running one stops anything; the new process continues the refused operation, with its refused entries, during recovery | [One process, one main window](../ARCHITECTURE.md#one-process-one-main-window) |
| 2026-11-01 | A Move across volumes flushes each copied file before it deletes the source | [Lifetime, persistence and recovery](../ARCHITECTURE.md#lifetime-persistence-and-recovery) |
| 2026-11-01 | A recovery record names each end's volume; Continue never writes to another volume that took the letter | [Engine libraries](../ARCHITECTURE.md#engine-libraries) |

## Lessons from earlier plans

SynapseCommander rebuilds gozler-ui in WinUI 3, in the style of TinyTorrent2.
Both projects already corrected structural mistakes through refactoring
plans. Learn from those plans before the skeleton is built, so the same
mistakes are not designed in again.

Record a lesson only when a plan corrected a structural cause that this
product's presentation or its C# engine libraries could repeat. Write it once,
at its owner in [ARCHITECTURE.md](../ARCHITECTURE.md), with the source plan
and commit; an engine lesson goes to
[Engine libraries](../ARCHITECTURE.md#engine-libraries). Where goz built a
lesson into its engine, goz `docs/DESIGN.md` holds it, and the stage that
builds that behavior reads it there. The
[window rules](../ARCHITECTURE.md#window-rules), and their first rule, that
the window never blocks, are not weakened by any lesson.

| Source | Status | Recorded at |
| --- | --- | --- |
| TinyTorrent2 `14-inspector-plan.md` (`c1d54c9`), fixed in `6c6f57b` | Done | [Publication and activity](../ARCHITECTURE.md#publication-and-activity) |
| goz `window-architecture.md` (`a2daf14` to `c698a56`) and `ui-architecture-fixes.md` | Done | [Window rules](../ARCHITECTURE.md#window-rules) |
| goz `window-responsiveness.md` rank 7 (issue #87, `89891df`) and DESIGN "Dragging" (`b974bcb`, `c90aa4d`) | Done | [Drag and drop](../ARCHITECTURE.md#drag-and-drop) |
| TinyTorrent2 `15-workspace-shell-study.md`, `14-removed-torrents-plan.md`, `tableview-performance-plan.md`, `communication-plan.md`, `library-cache-plan.md`, `payload-move-plan.md`; the restructuring commits `1faab3f` and `7eb19cc` | Done | [Window rules](../ARCHITECTURE.md#window-rules), [Operations in the window](../ARCHITECTURE.md#operations-in-the-window), [Tables](../ARCHITECTURE.md#tables), [Engine libraries](../ARCHITECTURE.md#engine-libraries) |
| goz `window-responsiveness.md`, `locations-that-do-not-answer.md`, `maintainability.md`, `result-snapshot.md`, `paths-through-links.md`, `copy-behaviour.md`, `service-repair.md`, `file-manager-spec.md`; the commits `3b307e5`, `0ed6c44`, `d52b4ff` | Done | The same sections, and `Shared.Storage` in [Presentation owners](../ARCHITECTURE.md#presentation-owners) |
| goz `locations-that-do-not-answer.md` (a plan, not yet in DESIGN.md); TinyTorrent2 `torrent-control-plan.md` and `drive-work-plan.md`, through their results in `docs/contracts/engine/lifetime.md` ("Exit ends within 10 seconds") and `files.md` ("Where a folder really is"); goz DESIGN "Result snapshots" and "Running windows" | Done | [Engine libraries](../ARCHITECTURE.md#engine-libraries), [One process, one main window](../ARCHITECTURE.md#one-process-one-main-window) |
| TinyTorrent2's work on lost and hung shares: `docs/contracts/engine/files.md` ("Unfinished tasks", "Drive turns", "Where a folder really is", "Unfinished files"), `engine.md` (storage recovery), `engine/notifications.md`, `engine/lifetime.md`, `library/files.md`, `13-disk-discovery-plan.md`, `app/src/Shared/Desktop/DiskSpace.cs`, the `Checks.ps1` checks `AbsentLink`, `LinkedNotice`, `StoppedLocation` and `MoveRecovery`; the retired plans `drive-work-plan.md`, `payload-move-plan.md`, `torrent-control-plan.md`, `torrent-control-audit.md` and `14-removed-torrents-plan.md`; the commits `a484e66`, `b6c300c`, `78eb5b3`, `311da9a`, `235594a`, `9027e74`, `05b67fe`, `12b99cf`, `d8c33be`, `42b90c7`, `afd814e`, `04d1559`, `0d6e588`, `b033d3d` | Done. Their copy and delete methods are already in goz DESIGN. The new lessons: a missing entry is missing only while its location answers; a link that cannot be followed does not answer; one open per location at a time; work goes on around a location that does not answer; a resume never loops; a recovery record names volumes; one failed step never drops other work; a Move flushes before it deletes a source | [Engine libraries](../ARCHITECTURE.md#engine-libraries), [Operations in the window](../ARCHITECTURE.md#operations-in-the-window), [Lifetime, persistence and recovery](../ARCHITECTURE.md#lifetime-persistence-and-recovery) |
| goz `copy-behaviour.md` and its earlier form `copy-policies.md` (retired in `8ec9f8c`) | Done: DESIGN "Device admission", "Interrupted operations" and "Synchronize folders" hold their rules, which [Operations in the window](../ARCHITECTURE.md#operations-in-the-window) adopts | |
| TinyTorrent2's torrent-control audit, `peer-map-plan.md`, `subtitles-records-plan.md`, the indexing and crawl research; goz `faster-start.md` (retired in `cce0398`), `sorted-index.md` (`7b4cdc0`), `visibility-plan.md`, `service-settings.md`, the early `ui-*.md` plans, `window-painting.md` | Done: no new lesson. Their lessons are gozlerd's own, specific to the torrent domain or to Win32 painting, or already recorded above. | |

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
   `RequireComponents` check against the current TinyTorrent2 checkout. Keep
   Settings on the latest released Windows App SDK, following the
   [version rule](../ARCHITECTURE.md#projects).
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
   describes. It contains no listing, search or operation code. The window's
   XAML waits for the owner to approve its prototype
   ([How decisions are made](#how-decisions-are-made)); the projects,
   `MainViewModel` and its page history, `Shell.Commands`, `Shared.Work` and
   the window-rule enforcement do not wait:
   - The three projects: the WinUI application in `app/src`, Presentation in
     `app/presentation` without a WinUI reference, and `PresentationChecks`.
   - `App`, `MainWindow` and `MainViewModel`, with the title bar copied from
     TinyTorrent2's caption (its Back button, the menu bar and the theme
     button), as the Title bar row of
     [Main window](../ARCHITECTURE.md#main-window) describes.
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
button is hidden on the Commander page. The menu bar, the key bar and a key
show the same command with the same availability. The theme button switches
between Light and Dark, and the caption buttons follow it, as in
TinyTorrent2. Screenshots show Light, Dark and High Contrast beside
TinyTorrent2's title bar in the same theme.

## Stage 1: the C# engine's first slice

This stage builds `Syno.Files` from goz `docs/DESIGN.md` ("The copy, move and
delete engine", "File operations", "Device admission") and goz
`docs/research/fast-copy-delete.md`, and proves it with a real consumer here.

1. **Measure first.** Build the delete and the parallel copy alone, with the
   candidate methods the research names (`CopyFile2` with and without
   unbuffered I/O, an overlapped ring, parallel POSIX delete). Run them on the
   test folders that goz `docs/BENCHMARKS.md` describes, on the owner's machine
   as recorded there, and record the results beside goz's numbers in
   `FilesChecks`. This machine has one NVMe SSD, so it measures SSD to SSD
   only (goz `docs/BENCHMARKS.md`). The fastest correct method there becomes
   that pair's default. Hard disks, USB drives, shares and two separate disks
   use goz's Automatic values until they are measured, and the After install
   list names those measurements. The measurement does not reopen the choice
   of C#.
2. Build the queue with its two lanes, Transfers per device and Deletes per
   device, order changes including moving ahead of a running operation, pause,
   Stop and Cancel all for each lane, and the recovery record in this
   product's own format. Follow the queue rules, the operation state type and
   its sequence check in
   [Operations in the window](../ARCHITECTURE.md#operations-in-the-window),
   and the bounded close and the locations that do not answer in
   [Engine libraries](../ARCHITECTURE.md#engine-libraries).
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
deleted. The queue's sequence check passes. With a thread held in a call that
does not return, engine close lets the process end at 10 seconds. No engine
host or service is installed or spawned. The flush that a Move makes before it
deletes a source is measured on goz's 30,000 small files and recorded.

`FilesChecks` also make a location go away without administrator rights, as
TinyTorrent2's `AbsentLink` and `MoveRecovery` checks do: a `subst` letter
removed during the work, a junction to that letter, and
`\\127.0.0.1\<a share that does not exist>`. They show that a Delete, Move
or Copy of entries there treats nothing as missing and loses nothing,
that the operation's entries on other locations finish, that other
operations run, and that the entries resume once the letter returns. A
location that hangs cannot be made on demand, so that case is in the After
install list: a share made to hang during a copy from C: to D: that also
reads from it, a Delete and a Mirror, while another operation runs on other
disks.

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
   routing mutations through the stage 1 engine. One bounded check shows
   whether WinRT `Clipboard` works from `Shared.Desktop`'s apartment thread;
   if it does not, that thread uses the OLE clipboard calls. Record the
   result here. Build drag and drop as
   [drag and drop](../ARCHITECTURE.md#drag-and-drop) says. Before that,
   record here, from Total Commander's help, its drag behavior: the drop
   targets and effects, their modifier keys and defaults, the menu of a
   right-button drag, whether a drop between panels opens the Copy or Move
   dialog, a drop onto a file row, and what a drag that starts on an
   unmarked entry carries while other entries are marked. A difference
   between that record and the rules in ARCHITECTURE needs the owner's
   approval. Bounded checks settle what reading cannot, and each records
   its result here:
   - Whether a WinUI window can start `SHDoDragDrop` from TableView's drag
     gesture, with either mouse button, and whether a larger drag's data
     object, built off the UI thread, can be used by that drag.
   - Whether WinUI's drop events can report an optimized move, hold a drop
     until its operation ends, and reach the source's asynchronous
     capability. Explorer must keep its files when a queued move is dropped,
     and 7-Zip's dropped files must be copied. Where WinUI cannot, the
     shell's OLE drop target in `Shared.Desktop` takes the drop, and the
     check also settles how that target holds a drop while the window keeps
     running, because WinUI has no supported nested message loop. A held
     drop ends at Exit.
   - Whether reading an incoming drop's data from a slow source blocks the
     UI thread. If it does, that read moves to `Shared.Desktop`'s apartment
     thread through the OLE data object.
5. Build the editable command line using PRODUCT.md's rules. Add terminal
   launch, recall/draft, name/path insertion and cancellable path completion.
6. Fill the key bar and apply the
   [key map](../ARCHITECTURE.md#keyboard-input) on every page (A22).

**Proof:** slow or unreachable folders leave the other panel and window usable;
stale listings/completions do not replace newer state. Clipboard/drop actions
target the captured entries and pointer destination. Dragging 10,000 marked
entries to File Explorer, and dropping them back on the other panel, leave
the Debug stall detector with no record, and the drop effect follows Total
Commander while the pointer moves. A command launched in a
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
   deletes. Add the question list and the quiet notification that opens it.
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
| A direct Mirror with deletions asks through the question list and copies meanwhile; the summary has file count, folder count and bytes; Keep them ends it as a Sync | Nothing is deleted before Delete them; the copies run while the question waits; Keep them deletes nothing; Stop withdraws the question, and a late answer does nothing |
| A Sync's or Mirror's source or destination goes away during the comparison or the run, made as in stage 1 with a removed `subst` letter | Nothing is read as missing: Mirror deletes nothing that the source still holds, and Sync copies nothing twice; the entries resume once the letter returns |
| Questions never stop work: an access denied or another entry's question goes into the question list | The operation's other entries and the other operations finish while the question waits; answering runs only the answered entries; an entry that fails again without progress goes to the list instead of resuming again |
| A Delete of entries that a transfer still uses: the one Delete question adds the note and a Queue button, as the default, to the left of Delete and Cancel, which are aligned right and keep their places | With Queue, the shared part of the tree stays until the transfer ends and everything else is deleted at once; after the transfer, the entries it writes are deleted, and of the entries it reads only those it finished; a failed, stopped or dismissed transfer leaves its unread entries on disk; with Delete, the transfer's report lists the deleted entries as not found |
| The two-lane queue from stage 1 under real operations | A Delete waits for a Delete on its disk, never for a copy's disk; moving an operation ahead of a running one pauses the displaced one, which resumes with its progress; Cancel all deletes leaves transfers running |
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
   goz's query tests and help examples and run against a fake pipe server.
   They also compare the port with the protocol version and query files of
   goz's newest release tag
   ([query parser](../ARCHITECTURE.md#responsibility-map)).
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
without a live service. A second gozlerd cannot run beside the installed one,
because the pipe name is fixed and the daemon takes it as the first instance.
So one bounded, read-only check uses the installed gozlerd when it is
present: the trust check, one query, one page and its release. It is an
owner-only environment check and never stops, replaces or reconfigures the
service. Large results stay bounded; a sort, new query or closed view rejects
stale replies. A list whose snapshot gozlerd dropped searches again and keeps
its focus and marks by path.

## Stage 5: viewers and application/settings parity

Deliver B01, B17, B19-B21 and finish any uncovered baseline behavior.

1. Build viewer reads, decoding and navigation in `Syno.Files` from goz
   `docs/DESIGN.md`, and independent Text/Hex windows with bounded cache, copy
   limits, wrap, retry and removal behavior.
2. Integrate Syno.Settings for searchable settings, individual commits, atomic
   transaction dialogs and reset, persistence, configurable
   shortcuts/context actions and index diagnostics/administration.
3. Implement application activation, optional tray behavior, single-instance
   handoff, whole-window elevation from an access-denied failure and the
   Administrator button that returns to a normal process
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
the app revision, the TinyTorrent2 commit it built against
([Projects](../ARCHITECTURE.md#projects)) and concise evidence to this plan; stages above provide the
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
| Read of preferences and view state at start, such as the saved placement | Disk read | `App` reads them through `Shared.Storage` off the UI thread before it creates the main window, so no shown window waits | 0 |
| Save of preferences and view state | Disk write | `Shared.Storage` through `Shared.Work` | 0 |
| Drive list, labels and free space | A sleeping or disconnected drive or share | `Panels` through `Shared.Work` | 2 |
| File and folder icons from the shell | The shell reads the file or its handler on a slow path | `Panels` through `Shared.Work` | 2 |
| Path exists or is a folder, for the path box and `cd` | A share that does not answer | `Panels` through `Shared.Work` | 2 |
| Shell launch: Open, Open with, Explorer, Properties, the terminal, the external editor | The shell resolves the path and starts a process | `Shared.Desktop` through `Shared.Work` | 2 |
| Clipboard read and write | Another program holds the clipboard | `Shared.Desktop` on its OLE apartment thread, tracked by `Shared.Work` | 2 |
| Drop data read | The source program answers slowly | `Shared.Desktop` through an agile reference, tracked by `Shared.Work` | 2 |
| Naming dragged entries with `SHParseDisplayName` | The shell names each entry; with the row's find data it reads no disk, about 10 µs per entry | `Shared.Desktop`, inline up to the shared size, else through `Shared.Work` ([drag and drop](../ARCHITECTURE.md#drag-and-drop)) | 2 |

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
