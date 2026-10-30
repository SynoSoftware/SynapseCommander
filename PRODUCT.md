# SynapseCommander product plan

Status: planned. Scope and shared-library direction agreed with the owner on
2026-10-30; feature ordering below is the proposed delivery sequence.
This document owns product scope. [Architecture](docs/ARCHITECTURE.md) owns
implementation responsibilities; the
[implementation plan](docs/plans/synapsecommander.md) owns stage completion.

## Purpose and scope

A responsive Windows file manager for finding, organizing, transferring and
inspecting files, with two folder panels, indexed search and a command line
for work in the active folder.

The baseline is **all working gozler-ui capabilities**, not a smaller replacement
MVP. Add the improvements below. Stages make delivery manageable; completing
an early stage does not mean feature parity or release readiness.

Parity means the same useful tasks, data integrity and responsiveness. Rebuild
their presentation with WinUI 3; native Win32 layout, internal identifiers,
paint techniques and inherited implementation constraints are not requirements.
A disabled placeholder is not a working feature: F4 Edit is an addition.

## Vocabulary

- A **panel** shows a folder or scoped indexed search. A **tab** retains one
  panel view, including navigation and selection.
- **Search** asks the index for matching descendants or global results.
  **Quick search** focuses a matching visible name; **quick filter** narrows
  the current folder listing.
- An **operation** is accepted engine work. Its identity survives presentation
  changes; a **report** describes its actual outcome.
- **Recovery** continues interrupted work after verification. It is not Undo.
- The **command line** is the editable command bar below the panels, as in
  the owner's reference image. It is distinct from WinUI's toolbar control
  named CommandBar.

## Baseline inventory

Every row is required for parity. Preserve the behaviors described by the
source references, resolving actual conflicts against explicit new product
decisions rather than silently dropping a feature.

| ID | Capability | Required behavior |
| --- | --- | --- |
| B01 | Application lifetime | Search/Commander switching, activation, single-instance handoff and optional extra windows, notification-area behavior, global activation shortcuts, startup preference, own-product update checking and About |
| B02 | Two panels | Independent folders, active side, editable paths, parent/root navigation, drive and recent-folder menus, remembered folders and view state |
| B03 | Live folder views | Async enumeration, watching and refresh, rename/change handling, inaccessible or removed drives, safe removable-drive release and retry |
| B04 | Selection | Focus separate from marks, mouse and keyboard ranges, select/clear all, selection masks, counts and sizes, stable captured command targets |
| B05 | File presentation | Icons, sortable columns, order and visibility, fitting, full-value tooltips, regional sizes/dates, known folder sizes and truthful unknown values |
| B06 | Indexed search | Search as you type or explicit submission, case/path matching, word-start/name-start/anywhere/whole-word/regex modes, existing query functions and errors |
| B07 | Search filters | Kind, location/direct-child scope, modified date and custom ranges, size and custom ranges, attributes, clear filters and syntax help |
| B08 | Large search results | Virtualized/paged results, match highlighting, sorting and refresh, retained snapshot selection, bounded memory, truthful incomplete/indexing/unavailable state |
| B09 | Inspector | Single-entry metadata, multi-selection totals, copyable names/paths/folders and direct file actions, remembered visibility and pane size |
| B10 | Open and shell actions | Open, locate, show in Commander, Explorer and Properties; configured external open commands; captured targets and reported launch failures |
| B11 | Existing clipboard and drag/drop | Search Cut/Copy/Paste and text copies, drag-out, drop onto supported Search folder targets, source lifetime and copy/move intent |
| B12 | Copy, move and creation | Fast engine paths and speed settings, same-volume move, cross-volume move, rename via Move, new folder, explicit destinations and existing-file policies |
| B13 | Permanent delete | Fast delete, explicit confirmation and scope, cancellation, partial failure reports and clear irreversible behavior |
| B14 | Synchronization | One-way Compare/Sync/Mirror, preview invalidation, conflict refusal, direct Sync/Mirror entry points, verified Mirror deletions and fresh comparison on recovery |
| B15 | Operation control | Device scheduling, queue limits and ordering, displacement/run-now behavior, pause/resume/cancel/cancel-all, byte/item progress, current file, rates and taskbar progress |
| B16 | Reports and recovery | Durable accepted work, interrupted-operation Continue/Dismiss, recovery before new submissions, retained reports, skipped/failed/successful distinctions, panel refresh after completion |
| B17 | Text and Hex viewers | Separate windows, large-file random access, bounded reading/cache, encoding detection, wrap/navigation, select-all/copy limits, retry/removal handling and remembered placement |
| B18 | Export | All matching results to CSV/TSV/TXT, shared query/output semantics, background execution, cancellation and write failures |
| B19 | Personal settings | Searchable settings pages, individual ordinary commits, atomic transaction dialogs and reset; General, Search, Context menu, Keyboard, File operations and Performance; persistence and shortcut conflict handling |
| B20 | Index administration | Drives/service status, diagnostic report, supported repair actions, Include/Visibility settings with proper permissions, creation-date availability and missing/version-mismatched service handling |
| B21 | Language and help | English, Dutch, Spanish and Turkish coverage, language switching, Windows language fallback, regional formatting, search syntax and accessible messages |
| B22 | Responsiveness and access | No blocking UI work, bounded large collections, keyboard and pointer access, light/dark/high contrast, DPI scaling, focus restoration and clear errors |

### Command coverage checkpoint

The source command table contains 43 identifiers at the research snapshot.
The following mapping accounts for each, including commands reached outside the
menu. It is a coverage checkpoint, not a promise to preserve Rust identifiers
or shortcut conflicts in the C# product.

| Source identifiers | Scope |
| --- | --- |
| Open, OpenFileLocation, ShowInCommander, Explore, Properties | B10 |
| ExportResults | B18 |
| Cancel, FileOperations | B15 |
| Settings | B19 |
| Exit, ShowSearch, ShowCommander, Commander, Update, About | B01 |
| Copy, CopyAsPath, CopyName, CopyFolder, Cut, Paste | B11 |
| SelectAll, ClearSelection, SelectFiles, UnselectFiles | B04 |
| SearchInThisFolder, Find, Match | B06 |
| Filters, ClearFilters | B07 |
| Inspector | B09 |
| Columns, SortBy | B05 |
| Language, SearchSyntax | B21 |
| Refresh | B03, B08 |
| CopyToFolder, MoveToFolder, NewFolder | B12 |
| Delete | B13 |
| Synchronize | B14 |
| View | B17 |
| Edit | A08; disabled in the baseline source |

Settings, viewers, queue actions and list gestures expose behavior beyond this
table. Stage 0 checks their coverage too; counting commands alone cannot prove
parity.

## Additions

All listed additions belong to the target scope. Their priority is the order
below, after or alongside the baseline area they extend. Their completion
criteria refine implementation scope without reducing the baseline.

| ID | Task | First complete delivery |
| --- | --- | --- |
| A01 | Run commands here | Editable command bar, terminal launch, bounded history/draft recall, path completion and focused name/path insertion |
| A02 | Use familiar file clipboard actions | Cut/Copy/Paste and name/path copying in folder panels and empty-area menus, sharing the existing operation route |
| A03 | Rename directly | Discoverable Rename and F2; select the stem by default, preserve the parent, refuse unintended replacement |
| A04 | Return to places | Per-panel/tab Back/Forward and history, saved favorites, visible commands and keyboard access |
| A05 | Narrow this folder | Local quick filter with explicit scope/counts, Esc restoration and safe visible-selection targeting |
| A06 | Drop files into panels | Incoming Explorer and inter-panel drops onto the intended folder or panel background, with correct copy/move feedback |
| A07 | Keep places open | Folder tabs on both sides, each retaining navigation, filter, focus, sort and scroll |
| A08 | Open and create documents | Open with, Show in File Explorer, functioning F4 external editor and New text file without overwrite |
| A09 | Exchange ZIP files | Create ZIP and extract into a chosen folder, with safe paths, collisions, progress, cancellation and partial failure |
| A10 | Arrange the panels | Swap complete panel views and show the active folder on the other side |
| A11 | Select related files | Invert visible selection and select/unselect by focused extension |
| A12 | Rename a batch | Preview names/collisions, then execute through the shared engine with cycle handling, partial results and recovery |
| A13 | Compare two files | Launch a configured external comparison tool with captured, correctly quoted paths |
| A14 | Check file contents | Create/verify checksum files with cancellable reads, mismatch reporting and readable partial failures |
| A15 | List descendants together | Indexed branch view with coverage/completeness visible |
| A16 | Keep a useful layout | Adjustable panel splitter and remembered user column widths, with an explicit auto-fit option |
| A17 | Find within a viewed file | Cancellable text/hex find, next/previous and position reporting without loading the whole file |
| A18 | Preview common documents | Image preview and a PDF opening/preview workflow; external viewing first where it serves the task |

A09 is create/extract, not archives as navigable folders. A13 is an external
tool handoff, not an embedded diff editor. A18 does not require hosting
arbitrary shell preview extensions. FTP, cloud-provider abstractions, a plugin
platform, full Total Commander command compatibility and general Undo need
separate product decisions.

### Command bar behavior

Place the editable command bar below the two panels and above any function-key
strip. Show the active working folder. Use native WinUI editing, history and
focus behavior rather than an embedded terminal.

- Enter captures the active panel's underlying folder and the command. Launch
  through the command prompt in a visible console whose output remains
  available. Open terminal uses the same launch owner.
- Standalone cd/chdir, including quoted paths and cd /d, navigates the active
  panel. Compound commands go unchanged to the shell; do not implement a shell
  parser. Define the supported standalone grammar before implementation.
- Recall retains the current draft. Blank input does nothing; launch failure
  retains text. Esc returns to the panel without discarding the draft.
- Insert the focused name or full path at the caret. Shell quoting must cover
  metacharacters as well as spaces; insertion must not accidentally execute text.
- Complete paths asynchronously. A changed draft, folder or tab invalidates old
  completion results. Typing in a panel retains quick search.
- Assign shortcuts after auditing focus and existing reservations. Text editing
  keeps its own Enter, Esc, Tab and clipboard semantics.

A launched terminal or user-selected editor is an explicit user action, not an
additional engine process or installed service.

## Integrity and product decisions

Permanent fast delete remains required by B13. Recycle Bin deletion is a
recommended additional mode whose default and implementation are not yet
settled. Never silently turn a recycle request into permanent deletion.
Recovery is not Undo, and equal size/date is not content verification.

Preserve engine conflict and Mirror integrity guarantees. Product defaults for
delete, tray behavior, opening search results and column sizing are chosen for
SynapseCommander; source defaults are reference, not inherited owner rulings.
Record a changed behavior explicitly before implementing it.

A local folder remains browsable and file operations remain available when the
indexing service is absent. Search-dependent features explain their missing
prerequisite without disabling unrelated tasks.

## Evidence and parity tracking

Research snapshot: 2026-10-30, working folder
`C:\SynoSoftware\TinyTorrent2\3rdParty\goz`, origin
`https://github.com/SynoSoftware/gozler.git`, HEAD
`451fccfae60516ddd272321653d7707adc29eea1`.
The checkout has uncommitted UI and documentation changes; HEAD alone does not
describe this snapshot. This is a source/document review, not a completed
runtime or accessibility audit.

Key source anchors:

- `docs/DESIGN.md`: The window, Commander mode, File operations, and The copy,
  move and delete engine.
- `crates/gozler-ui/src/core/commands.rs`: command identifiers and availability.
- `crates/gozler-ui/resources/en.json`: dialogs and settings beyond menus.
- `crates/gozler-fileops/src/lib.rs` and `queue.rs`: actual engine ownership.
- Existing `docs/plans/commander-features.md` in that checkout: earlier task
  research transferred to goz.

SHA-256 at review: commands.rs
`ACBAE1FA5BE65DB44E9D48DFDE83AEEB54BC0D6FADFFE080A59D9E73514FEA3C`;
DESIGN.md
`5B560A4432B3145065B007273F2D6DF65C32ABD81A4DDF0182866005C964C2F0`.

Before each baseline stage, check the corresponding current source surfaces.
Record any source changes separately from this baseline; do not silently expand
a moving target. Track implemented IDs and evidence in the implementation plan.
An ID is complete only when its full behavior works, not when its button exists.

The [Total Commander command reference](https://www.ghisler.ch/wiki/index.php?title=List_of_internal_commands)
informed additions such as history, filtering, tabs, batch rename and checksums.
It is an idea source, not a requirement for identical command IDs or behavior.
