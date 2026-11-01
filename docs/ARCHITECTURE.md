# Architecture: C# engine, WinUI presentation

Status: agreed direction, 2026-10-30; changed to an all-C# product on
2026-11-01. No engine or application code exists yet.
[Product scope](../PRODUCT.md) and the
[implementation plan](plans/synapsecommander.md) are separate authorities.

## Process and source ownership

SynapseCommander is one C#/WinUI 3 application process. Its file engine,
folder listings, viewer reads, archives and search client are C# libraries in
this repository ([ADR 0001](adr/0001-no-rust.md)). This repository has no Rust.
C or C++ is used only where C# cannot do a job, such as an existing native
library. There is no additional engine executable, background host, Windows
service or new file-operation pipe.

gozler-ui keeps its own Rust engine in the goz repository. The two products
share no engine code, so a fix in one is not a fix in the other. Both run
operations with their own user token, have separate queues and do not
coordinate disk scheduling.

The existing indexing service, gozlerd, keeps indexing, search and its trust
contract. It gains no copy, move or delete commands. SynapseCommander talks to
it through its existing pipe protocol, with a C# client.

```text
SynapseCommander process, all C#
    WinUI application --> Presentation --> Syno.Files: operations, queue, recovery,
                                           listings, watching, viewer reads, archives
                                       --> Syno.Index: query parsing, pipe protocol
Syno.Files --> filesystem, with the person's token
Syno.Index --> the installed gozlerd service, unchanged
```

**The goz repository is the specification, not a dependency.** goz
`docs/DESIGN.md` ("The copy, move and delete engine", "File operations",
"Device admission", "Synchronize folders"), its research
(`docs/research/fast-copy-delete.md`, `ssd-copy-scheduling.md`) and its
measurements (`docs/BENCHMARKS.md`) give the behavior, the integrity rules and
the methods. The Rust code, currently at
`C:\SynoSoftware\TinyTorrent2\3rdParty\goz`, is a reference for edge cases.
SynapseCommander does not translate it line by line, copy its tests or import
its history. It writes its own tests from the specification. Where this
product's rules differ, such as the Delete lane or the Mirror question, this
document wins.

## Responsibility map

| Owner | Responsibilities |
| --- | --- |
| `Syno.Files`: file operations | Preparation, validation, conflicts, destination rules, copy/move/delete/rename/new folder, Sync and Mirror, scheduling in two lanes, progress, reports, recovery and speed values |
| `Syno.Files`: listings and viewers | Folder and archive enumeration, watching, file identity and folder sizes; viewer reads, decoding and byte navigation |
| `Syno.Files`: archives | The archive interface and its format modules |
| `Syno.Index` | gozler's query syntax, request construction, the pipe protocol and its trust checks, result snapshots, matching metadata, export semantics and service diagnostics |
| C# presentation | Windows, panels/tabs, navigation history, focus/selection, command routing, dialogs, localized text, view state and accessibility |
| C# Windows integration | Clipboard and drag/drop, shell launch, app activation, notification area and global shortcuts |
| C# persistence | SynapseCommander settings and window/tab state; recovery records belong to `Syno.Files` |

The engine libraries have no WinUI and no presentation code. All P/Invoke and
`unsafe` code is inside them, each declaration beside the owner that uses it.
Batch-rename masks and checksum algorithms also belong to `Syno.Files` as their
features land; a preview and its rename call the same function. External
tools retain their own implementation. The UI gathers intent and presents
outcomes.

**The query parser is a port with checks.** gozlerd's pipe carries a parsed
query (goz `crates/gozler-core/src/proto/types.rs`), so the client parses.
`Syno.Index` ports the parser and the binary protocol to C#. Its checks are
built from goz's query tests and the query help's examples, so drift between
the two parsers fails a check. Changing gozlerd to parse query text would keep
one parser, but it needs Rust work, which the owner ruled out (lead decision,
2026-11-01).

**Archives are format modules behind one archive interface** (owner ruling,
2026-11-01; [ADR 0004](adr/0004-archive-format-modules.md)). The interface is
a C# interface in `Syno.Files`. A module lists, reads and, where its format
allows, creates archives:

- ZIP, TAR and TGZ use .NET's `ZipArchive`, `TarReader`, `TarWriter` and
  `GZipStream`.
- 7z and RAR reading uses SharpCompress (MIT license). RAR is read only in
  every library.
- 7z creation uses SharpCompress if its current release writes 7z, which its
  repository notes suggest but which is not verified; otherwise 7-Zip's
  `7z.dll` (C++, LGPL, loaded as a separate DLL) as one native module. Stage
  6 checks and records the choice.

The engine lists an archive the same way it lists a folder, so a panel shows
an archive folder with the same kind of result set. An encrypted archive's
password is asked for when it opens, is held in memory only while it is open
and is never saved. Design the interface with the `codebase-design` skill
before the first module.

## Engine libraries

`Syno.Files` and `Syno.Index` are in-process libraries with ordinary C# APIs.
Their API follows these rules:

- **Delivery.** The engine runs its own threads and publishes snapshots of its
  state. It never calls into WinUI or into a presentation lock. After each
  change it signals once; `Shared.Engine` takes the latest snapshot and the
  replies off the UI thread and posts one apply. Terminal states and failures
  stay observable even when intermediate progress is coalesced.
- **The index is a dependency of the engine.** `Syno.Files` receives the index
  it uses for totals, the gozlerd client or none, when it is created. Tests
  create it with none, so they never depend on the user's service.
- **Requests have identities and can be cancelled.** A result has an explicit
  release. A cancelled observation does not imply that an accepted filesystem
  change was rolled back.
- **Close is ordered.** Engine close refuses new work, stops its operations,
  writes the recovery record and ends its threads, all off the UI thread; the
  caller waits for the returned task off the UI thread.
- **Native calls** follow .NET's
  [native interop guidance](https://learn.microsoft.com/en-us/dotnet/standard/native-interop/best-practices):
  `LibraryImport` where suitable, `SafeHandle` for every native handle, and
  checked lengths. A Windows path stays a .NET string, which can hold the
  unpaired surrogates that NTFS allows; path, display text and shell command
  text stay distinct types.

gozler-ui and TinyTorrent2 restructured their engine boundaries several times.
These rules keep what those changes established:

- **A range-read list reads an engine-held result set.** The engine keeps an
  immutable result for each search or listing, and the UI reads pages of it
  through a handle with a defined release. A row whose file is gone keeps its
  place and reads as gone; a result the engine dropped makes the UI ask again.
  Without a held result, rows of different pages came from different moments
  and the list stopped at 100,000 rows (goz `result-snapshot.md`, `b2bca38`,
  `4f670e2`).
- **Paths keep the engine's meaning.** The presentation does not normalize,
  resolve, or rewrite paths, and it does not touch the disk to make a search
  work. Where
  the UI needs a derived value, such as match highlights, it calls the same
  engine function. A client-side path rewrite was deleted for that reason
  (goz `paths-through-links.md`, `9c1d086`).
- **The engine returns whole answers and exposes its limits as data.** A page
  carries its rows, the total and its completeness. Limits such as counts and
  ranges come from the engine, not from constants in the presentation (goz
  `d52b4ff`).
  Sequences that take locks stay inside one engine call (`4f670e2`).
- **Last-known state is shown at once and marked as not yet confirmed.** An
  unavailable location is "unavailable", not an empty listing. Work that
  changes files waits for confirmed state (TinyTorrent2
  `library-cache-plan.md`).
- **Operations report bytes and per-item completion,** so progress never shows
  a dash while work runs (TinyTorrent2 `payload-move-plan.md`, `0d6e588`).
- **Requests have identities and can be cancelled from the first version.** A
  streaming result keeps only its latest value for each key. TinyTorrent2's
  pipe first waited for each reply before the next request
  (`communication-plan.md`, `536c662`).

Stage 1 settles the concrete engine API with the first operation slice.

## WinUI application structure

Status: planned skeleton, 2026-11-01. No product project exists yet. This
section owns the C# project layout, the main window's composition and the
presentation owners. [Product scope](../PRODUCT.md) owns what each region must
do.

The feature areas and MVVM style copy TinyTorrent2's WinUI application,
because both products share TableView, the settings language and an
inspector. Unlike TinyTorrent2, the presentation owners have their own project
without WinUI ([application areas](#application-areas)). Its
`docs/contracts/architecture.md` (Command and presentation flow, Projects and
source layout) and `docs/agents/naming.md` (App folders) are the reference.
The structure of gozler-ui's window comes from goz `docs/DESIGN.md`: Main
window, Commander mode's window, The key bar and File transfers. The checked
revisions are TinyTorrent2 `446b3f2` and goz `e1b5b8b`.

### Projects

| Project | Location | Role | Depends on |
| --- | --- | --- | --- |
| SynapseCommander | `app/src/SynapseCommander.csproj` | The WinUI application: `App`, windows, XAML views, the Settings catalogue and Windows integration (`Shared.Desktop`) | Presentation, Settings, Controls, TableView, Glyphs, WinUI, .NET |
| Presentation | `app/presentation/Presentation.csproj` | The presentation owners: view models, commands, panels, search results, operations state, the inspector's state, `Shared.Engine`, `Shared.Work` and `Shared.Storage` | Files, Index, .NET; no WinUI |
| PresentationChecks | `app/tests/PresentationChecks.csproj` | Console checks of the presentation owners, without a window | Presentation |
| Settings | `lib/Settings/src` | `Syno.Settings`, the settings presentation library | WinUI, Glyphs (Lucide until stage 0 moves it) |
| SettingsSample, SettingsChecks | `lib/Settings/sample`, `lib/Settings/tests` | Settings development host and model checks | Settings |
| Controls | `lib/Controls/src` | `Syno.Controls`: generic window controls with no product rules. It starts with `Pane`, `Splitter` and `MenuItems`, copied from TinyTorrent2's application | WinUI, Glyphs |
| TableView | TinyTorrent2 `lib/TableView/src` | The only table control | WinUI, Glyphs |
| Glyphs | TinyTorrent2 `lib/Glyphs/src` | Names for the glyphs of Windows' icon font | WinUI |
| Files | `lib/Files/src` | `Syno.Files`: file operations, the queue, recovery, listings, watching, viewer reads and archives ([engine libraries](#engine-libraries)) | .NET, SharpCompress; Windows through P/Invoke |
| FilesChecks | `lib/Files/tests` | Checks of `Syno.Files` on real temporary folders, written from goz `docs/DESIGN.md`, and the speed measurements against goz `docs/BENCHMARKS.md` | Files |
| Index | `lib/Index/src` | `Syno.Index`: the gozlerd client, its query parser and pipe protocol | .NET |
| IndexChecks | `lib/Index/tests` | Parser and protocol checks built from goz's query tests, with a fake pipe server; no running service | Index |

Settings presentation belongs to `Syno.Settings`, whose
[contract](../lib/Settings/docs/contract.md) owns declarations, layout and commit
mechanics. Hosts supply content, typed bindings and persistence. Ordinary
preferences apply individually; coherent values use transaction forms; complex
work such as speed testing uses a registered workflow in the shared shell. Its
development [plan](plans/settings-component.md) precedes main application
integration.

TableView and Glyphs are project references through `ComponentsRoot`, as the
Settings sample already does. TinyTorrent2 replaced its Lucide library with
Glyphs in commit `201577b`. The Settings projects and the solution still name
`lib/Lucide`, so they do not build against the current TinyTorrent2 checkout.
Glyphs uses Segoe Fluent Icons, which is also gozler-ui's icon source, so one
icon per command stays the same in both products.

`SynapseCommander.sln` gains the product project. `Build.ps1` stays the one
build entry point and gains the product build and launch.

The application pins one Windows App SDK release for itself and its
installer. Each library references the lowest release it needs, and the
application's release is at least as high as every library's. Prefer
TinyTorrent2's release, 2.5.1, so both products share one installed Windows
App Runtime. Settings references 2.6.4 since `4fd3bfc`, with no recorded
reason; stage 0 lowers it and a build against 2.5.1 shows whether any newer
API is needed.

### Application areas

The product has two projects that share the area folders and namespaces,
`Syno.SynapseCommander.<Area>`. An area's owners are in Presentation and its
views are in the WinUI application, so `Panels.Tab` and the panel's XAML are
in the same namespace. Each area starts flat and owns its presentation,
behavior and state, so a feature change stays local. `App`, the WinUI project
and the manifest stay at `app/src`. `Assets/`, `Resources/` (language
catalogues, `en.json` first) and `Properties/` keep their usual roles.

**Presentation owners hold no WinUI types.** Presentation does not reference
WinUI, so the compiler refuses a control, a XAML type or a `DispatcherQueue`
in an owner. Only the WinUI application's windows, views and `Shared.Desktop`
hold WinUI and Windows integration parts. `Shared.Engine` and
`Shared.Work` are the only owners that move work to the UI thread. They do it
through one small interface that the WinUI application implements with the
window's `DispatcherQueue`, because the UI thread is a real external
dependency. Owners therefore receive deliveries already on the UI thread.
`PresentationChecks` drives the owners with a synchronous implementation of
that interface and asserts what a person would see: rows, focus, marks,
availability and the order of effects. This follows `SettingsChecks`, which
already checks Settings' model without WinUI, and gozler-ui's window rule 21:
pure rules need no window. A check that needs real WinUI behavior gets one
bounded check in the running application, not a permanent suite.

| Area | Owns |
| --- | --- |
| `Shell/` | `MainWindow`, `MainViewModel`, page navigation and its history, the command table, the shared inspector, the status bar, message bar and notifications, About and update checks. It composes the features and coordinates their lifetime. |
| `Panels/` | The Commander page: both panels, tabs, folder and archive listings as presented, focus and marks, history and favorites, quick search and quick filter, Quick View's place, drives, the command line, the key bar and panel drag/drop. |
| `Search/` | `Search.Results`, the one owner of a search's query, match modes, filters, result pages, highlights and selection snapshot, and the Search page that hosts it (stage 4). |
| `Operations/` | The presented queue, the File operations page, the status bar items of operations, reports, recovery prompts, the Mirror question, the Multi-Rename page and the drafts of Copy, Move, Delete, New folder, Sync, Mirror and Multi-Rename. |
| `Viewers/` | Text and Hex viewer windows (stage 5). |
| `Settings/` | The typed preferences and configured shortcuts, in Presentation, and the `Syno.Settings` catalogue declarations that bind them, in the WinUI application. |
| `Shared/` | Common dialogs, text formatting and `Work`, the owner of UI background work. `Engine/` holds the engine instance and its delivery. `Desktop/` holds clipboard, drag/drop, shell launch, the notification area, activation, global shortcuts and taskbar progress. `Storage/` holds settings and view-state files. |

A type that serves several areas lives in `Shared/`. Feature areas have one
exception, and it points one way: a panel tab whose location is a search holds
a `Search.Results`, the same owner that the Search page uses. `Search/` never
uses `Panels/`. gozler-ui also has one search list type, `SearchList`, for
Search mode and for a panel search. Two result owners would be two implementations of
paging, highlights and the selection snapshot. Type names below are the
planned owners. Implementation may refine them under the
[naming rules](../AGENTS.md#naming-and-structure).

### Main window

**Owner rulings, 2026-11-01:** one main window shows one page at a time
([ADR 0002](adr/0002-one-window-of-pages.md)). The Commander page is home.
The Search page (`Alt+F7`), the Multi-Rename page (`Ctrl+M`), the File
operations page and Settings are secondary pages. Only viewers (F3) are other
windows of ours; F4 starts the external editor. The window always opens on
the Commander page.

```text
+----------------------------------------------------------------------+
| Title bar: [Back] icon, title, menu bar (File Edit View ... Help)    |
+----------------------------------+-----------------------------------+
| Left panel                       | Right panel, or Quick View        |
|  [tabs, stage 6]                 |                                   |
|  drive button | path box         |                                   |
|  TableView: Name Ext Size Date   |          (splitter, A16)          |
|  summary: marks, sizes, free     |                                   |
+----------------------------------+-----------------------------------+
| Command line: C:\Work>  [                                          ] |
| Key bar: F3 View  F4 Edit  F5 Copy  F6 Move  F7 New folder  F8 Delete|
| Status bar: active panel | Copying 42 % [x] | Deleting [x] | 3 ops v  |
+----------------------------------------------------------------------+
```

The Search page replaces the panels with the search strip, the filter pane,
the results table and the inspector below the results. It has the key bar
and no command line. The title bar and the status bar are the same on every
page.

| Region | WinUI composition |
| --- | --- |
| Title bar | `ExtendsContentIntoTitleBar` with a Back button at the left, then the icon, title and a `MenuBar`. Windows draws the caption buttons. |
| Pages | One `Grid` row. Each page is one `UserControl`, created when it first shows and then kept loaded, so returning keeps focus, scroll and selection. A hidden page's owners are inactive ([publication and activity](#publication-and-activity)). |
| Panel path row | A `DropDownButton` with a `MenuFlyout` for drives and recent folders, and an `AutoSuggestBox` for the editable path and its completion. |
| Panel listing | `Syno.TableView` `Table`, bound to the active tab's rows. An archive folder uses the same table. |
| Panel summary | Text: marked count and size, totals, free space. |
| Between panels | A vertical splitter. Its position is saved. |
| Quick View | `Ctrl+Q` replaces the other panel's listing with the inspector and the A18 preview. Closing it shows the other panel again. |
| Search inspector | A pane below the results behind a horizontal splitter, with remembered visibility and height. |
| Command line | An editor with the working folder as its visible label (PRODUCT.md, Command line behavior). |
| Key bar | Buttons that invoke the command table's commands. They show its icon, key and availability. |
| Status bar | TinyTorrent2's design (`docs/contracts/ui/feedback.md`, Status bar): one item per operation and per `Shared.Work` task, in a stable order, each with a label, a stock `ProgressBar` and its own Stop button. The items are display only; a click on the progress does nothing. One `DropDownButton`, such as "3 operations", shows while any operation exists. Its menu lists every operation and has "Open File operations"; it also holds the items that do not fit. |
| Dialogs | `ContentDialog`, one at a time, through one `MainWindow` entry point, as TinyTorrent2's `Shared.Dialog` and `Interact` do. |
| Viewer windows | One `Window` per viewer. Each viewer has its own placement (B17). |

Page navigation follows TinyTorrent2's `Shell.Navigation`:

- Opening a page that is already in the history returns to that entry and
  drops the entries after it, so no page appears twice. Opening the Commander
  page clears the history. Commander, then Search, then Settings goes back to
  Search, then to Commander.
- Back is the title bar's Back button, `Alt+Left` on a secondary page and the
  mouse back button. The Back button stays in place on every page and is
  disabled on the Commander page, so the title bar never shifts. On the
  Commander page `Alt+Left` is folder history (A04).
- Every secondary page has a menu item that shows its key, because the menu
  is the one visible list of pages.
- Esc never navigates ([keyboard input](#keyboard-input)).

Regions use native WinUI controls and theme resources. A custom control needs
a demonstrated gap; gozler-ui's Win32 painting approvals do not carry over.
The window uses the same base material, theme dictionaries and title-bar
treatment as TinyTorrent2, so the two products look related. Light, Dark and
High Contrast each have explicit resources. WinUI does not size a window to
its content, so the first window gets an explicit size of at least the
measured minimum. gozler-ui calculates its Commander minimum at about 833 DIP
wide in English; that figure is not measured. After the first start, the
window uses its saved placement.

TinyTorrent2's `Controls.Pane`, `Controls.Splitter` and `Controls.MenuItems`
already do what the Search page's inspector pane, both splitters and the
menus need.
**Owner decision, 2026-11-01:** copy them into a new library here,
`lib/Controls`, and leave TinyTorrent2 unchanged. TableView stays a thin
table component, and Settings stays a settings library. Like those libraries,
Controls owns its templates and English text and has no application
dependency. The copy is a second implementation until TinyTorrent2 adopts
Controls; that adoption is a later, separate decision.

### Tables

Every list uses `Syno.TableView.Table`. At the checked revision it gives this
product:

- Columns declared in XAML, with typed row keys and sort keys through
  `Schema<TRow>`. A folder row's key is its name, so marks and focus survive a
  re-read.
- `Selection(items, current)`, in which `Current` is separate from the
  selected items. The table owns interactive selection; the tab accepts or
  refuses it, as TinyTorrent2's `MainViewModel.Select` does.
- `ItemInvoked` for Enter and double-click, and `ItemContextRequested` for the
  host's context menu.
- `Layout` for column order, visibility and sort, and `Placeholder` for the
  Empty, Loading and NoResults states.
- No handling of typed characters. Quick search therefore belongs to the panel,
  which moves `Current`.

A host updates a table by its changes. It does not publish a whole new list
or call `RefreshView` for each engine update, because each publish reads every
row. A row raises `PropertyChanged` for the property that changed, never
`PropertyChanged("")`, which makes every realized row evaluate every binding
again (performance plan L7).

Column widths follow TableView's owner ruling (contract §10): each table fits
and fills its width, a manual resize lasts for that table's lifetime, and
widths are never saved. gozler-ui behaves the same way.

TableView's scale work belongs to the owner's
`tableview-performance-plan.md` in TinyTorrent2 (open, `8c1aa93`). It targets
10,000 to 100,000 rows that change often, which covers folder listings: a
publish where nothing moved, selection, sorting and membership changes cost
the change, not the row count. The plan keeps one key read per row per publish
and one notification per arriving or leaving row (its "Limits that remain").
Stage 2 measures a large folder against those limits. The plan lists millions
of rows (its F6) under "Not planned" because no host had such a source; this
product's search is that host.

The product needs three things that `Table` does not have and that the
performance plan does not cover. Each is changed in TableView's own
repository, so there is still one table implementation.

| Gap | Product requirement | Needed by | Recommended change |
| --- | --- | --- | --- |
| Millions of rows (performance plan F6). Rows on screen are virtualised (contract §20), but every publish reads every row. | B08: millions of search results with bounded memory. gozler-ui keeps at most 32,768 rows in memory for any match count. | Stage 4 | The performance plan's F6 shape: a source whose indexer creates rows on demand from a bounded cache, `SortMode.External`, and a selection stored as index ranges. The last part changes the public `Selection` type, so it is a TableView contract decision. |
| In `Extended` mode the arrow keys select rows, and Insert does nothing. | B04: the cursor keys move focus only; Insert, Space and the selection keys change the marks. | Stage 2 | A marks mode in TableView. `Table/Input.cs` already moves focus without selecting when Ctrl is held, so plain arrows can take that path. Insert marks the focused row and moves down, and a plain click moves focus only. A host key filter would duplicate the table's navigation, such as page size and skipped rows. |
| No drag source and no drop target. | B11 drag-out and A06 drops onto a folder or a panel. | Stage 2 | Row drag-out and drop-target hit testing in TableView. The host keeps the data package and the copy or move intent. The contract (§14) already decides a gesture by what the press lands on: with drag-out on, a press on a row drags, and a marquee starts from empty space, as in File Explorer. |

### Delivery to the window

TinyTorrent2 talks to its engine through a named pipe, so only its delivery
pattern applies here, not its transport. `Shared.Engine` keeps the latest
delivery of each kind, queues at most one apply on the `DispatcherQueue`, and
discards a delivery whose sequence is not newer than the last one applied.
Terminal operation states are never coalesced away, as the
[engine libraries](#engine-libraries) require.

### Presentation owners

WinUI uses MVVM with `INotifyPropertyChanged` and `ICommand` directly, without
an MVVM framework, as in TinyTorrent2. View models own presentation state and
requests. Views keep native controls, focus, scrolling and window lifetime.

| Owner | Owns |
| --- | --- |
| `App` | Startup, single-instance handoff, creation and closing of the engine session, and the main window. |
| `Shell.MainWindow` | Chrome, the page views, dialogs, focus movement between regions, splitter mechanics and the order of the Close and Exit steps. |
| `Shell.MainViewModel` | The shown page, its navigation history and the active list: the active panel's tab on the Commander page, or the results on the Search page. Also the context for command availability, the message bar and the model steps of Close and Exit. |
| `Shell.WorkStatus` | The status bar. It reads the ordered items of `Shared.Work` and `Operations.State` and owns only how they look. |
| `Shell.Commands` | The one command table: identity, caption key, icon, default keys, availability and the owner that runs each command. The menu bar, context menus, key bar and keyboard accelerators are built from it. Configured shortcuts replace the default keys. |
| `Shell.Inspector` | The inspector's activity, observation of the active list's accepted selection, subject, displayed content and the lifetime of its reads, under the rules of [publication and activity](#publication-and-activity). |
| `Panels.Commander` | Both `Panel`s, the active side and Swap (A10). The other panel's folder is the default destination for Copy and Move. |
| `Panels.Panel` | One side: its ordered tabs and the active tab. It has one tab until stage 6. |
| `Panels.Tab` | One retained view. Its location is a folder, with its accepted rows, or a scoped search, with its `Search.Results`. It also holds history, sort, quick filter, focus, marks, scroll place and a generation number that rejects late results. |
| `Panels.CommandLine` | The draft, history, completion requests, standalone `cd`, and launch through `Shared.Desktop`. |
| `Operations.State` | The queue snapshot, reports and the entries that waiting and running operations have taken. It builds one status item per operation from the engine's snapshot and asks the Mirror question. |
| `Operations.Page` | The File operations page: the transfer and Delete lists and selection in them. Reorder, Pause, Stop, Cancel all copies and Cancel all deletes are requests to the engine. |
| `Operations` drafts | The unfinished choices of one Copy, Move, Delete, New folder, Sync, Mirror or Multi-Rename request, until the engine accepts or refuses it. |
| `Viewers.Viewer` | One viewer window's `Syno.Files` viewer reader, bounded reads and placement. |
| `Settings` | Catalogue declarations for `Syno.Settings` and the typed preferences they bind. |
| `Shared.Engine` | The `Syno.Files` engine instance and delivery of its snapshots to the dispatcher. |
| `Shared.Work` | The policy for UI background work: off-thread execution, replacement by purpose, separation by location, slowness, delivery only while the task's window is open, and exit without waiting ([window rules](#window-rules)). It also owns the status item type that it and `Operations.State` supply: identity, label, progress, whether it can stop, outcome, owning page and start order. |
| `Shared.Storage` | Preferences and view state in SynapseCommander's data directory. It writes only the values that changed, so one save never undoes another owner's change (goz `0ed6c44`). The engine's recovery records stay in `Syno.Files`' own store. |

The model holds facts and identities, not display strings. Views format sizes,
dates and captions from the current language and region.

### One process, one main window

The main window and the viewer windows run on the one UI thread, so an owner
that a viewer also reads needs no lock and no thread hop. gozler-ui made the
same decision ("No thread per top-level window") in its window-architecture
plan.

There is one process per data folder and one main window in it, with one
engine session and one queue, so two sessions never resume one recovery
record.

- **A second launch hands its request to the running process,** such as
  `--search`, `--location`, `--reveal` or `--open`, and ends. The request
  goes through a per-user, per-data-folder named pipe whose access list
  admits the same user at medium integrity, so a normal start reaches an
  elevated process. Windows App SDK `AppInstance` redirection across
  integrity levels is not verified; use it only if a check proves it.
- **An elevated start that finds a normal process** sends it the request and
  Exit. The running process runs its normal Exit, including the operations
  question. If the person keeps the operations, or a dialog is open, the
  running window comes forward and the elevated start ends. Otherwise the
  elevated start waits off the UI thread for the old process to end, then
  takes the data folder.
- **A viewer window** holds only `Viewers.Viewer`. Closing it ends its
  deliveries and reads; it never keeps the process alive.

### Keyboard input

A key goes to the first of these that owns it. The order follows TinyTorrent2's
input precedence (`docs/contracts/ui.md`, Keyboard and accessibility):

1. An open popup, flyout or menu, or a text composition in progress.
2. A focused text editor: the path box, the command line, the search box or a
   dialog field. It keeps characters, Enter, Esc, Tab, Backspace, Delete, the
   arrows, Home, End and Ctrl+A, C, V, X and Z.
3. The open dialog.
4. The focused table: the navigation and marks keys (contract §13 and the
   marks mode in [Tables](#tables)), Enter through `ItemInvoked`, and Menu or
   Shift+F10 for the context menu.
5. The focused panel: typed characters start quick search, Backspace opens
   the parent folder and Tab moves to the other panel.
6. The window's accelerators from `Shell.Commands`: function keys and Alt and
   Ctrl chords. F6 is Move, as in Total Commander, not Windows' move between
   regions.

`Shell.Commands` resolves keys. Each command declares its key and whether it
also runs while a text editor has focus. Function keys and the page keys (F3
to F8, Alt+F7, Ctrl+M) run there; keys that an editor uses never do. A key
has one owner at each level. Settings refuses a configured shortcut that a
lower level already owns, and says which owner has it (B19).

**Esc closes temporary UI only** (owner decision, 2026-11-01, from
Microsoft's [keyboard interactions](https://learn.microsoft.com/en-us/windows/apps/develop/input/keyboard-interactions)
guidance). It closes the open dialog, menu or popup, and a dialog waiting
behind it then shows. A text editor keeps its own Esc; on the Search page Esc
clears the search box. Esc never navigates back, never stops an operation and
never asks whether to stop one.

One key map serves every page (owner decision, 2026-11-01). Total Commander's
key is kept unless the table says otherwise.

| Key | Commander page | Search page | Other pages |
| --- | --- | --- | --- |
| F2 | Rename (A03); Total Commander uses it for refresh | Rename | — |
| F3 / F4 | View / Edit with the external editor | View / Edit | — |
| F5 / F6 | Copy / Move or rename, through the Copy dialog | Copy / Move | — |
| Shift+F6 | Rename in place | — | — |
| F7 | New folder | — | — |
| F8, Delete | Delete, permanently | Delete | — |
| Shift+F2 | Mark differences between the panels (A21) | — | — |
| Alt+F1 / Alt+F2 | Left / right drive menu | — | — |
| Alt+F7 | Search page | — | Search page |
| Alt+Shift+F7 | Search page scoped to the active panel's folder | — | — |
| Ctrl+M | Multi-Rename page with the captured marks | Same | — |
| Ctrl+F | Search in the active panel | Focus the search box | — |
| Ctrl+Q | Quick View | — | — |
| Ctrl+R | Refresh | Run the search again | Refresh where the page has one |
| Ctrl+S | Quick filter | — | — |
| Ctrl+L | Path box | Search box | — |
| Alt+Left / Alt+Right | Folder history back / forward (A04) | Back | Back |
| Alt+Down | Folder history list | — | — |
| Alt+Shift+Enter | Size all folders in the panel (A20) | — | — |
| Ctrl+D | Favorites | — | — |
| Ctrl+T / Ctrl+W / Ctrl+Tab | New / close / next tab (A07) | — | — |
| Ctrl+U | Swap panels (A10) | — | — |
| Ctrl+C / X / V | File clipboard (A02); text in a text editor | Same | — |
| Ctrl+Enter / Ctrl+Shift+Enter / Ctrl+P | Insert name / full path / current path into the command line (A01) | — | — |
| Insert, Space, Num+, Num−, Num* | Mark, mark, select mask, unselect mask, invert (A11) | Mark | — |
| Alt+Enter | Properties | Properties | — |
| Tab | Other panel | Next control | Next control |
| Esc | Closes quick search or the quick filter | Clears the search box | Closes temporary UI only |

The File operations page has no default key, as in Total Commander; the
status bar's operations button and the menu open it. FTP is not planned, so
Total Commander's FTP keys, such as Ctrl+F, are free.

### Publication and activity

**Owner ruling, 2026-11-01:** an owner publishes its accepted state, and each
consumer decides its own work. TinyTorrent2 commit `6c6f57b` corrected the
opposite design. There, `MainViewModel.Select` called the inspector's `Show`,
`RefreshScope` and `Refresh`, so a selection loaded and reset the inspector, a
closed inspector still did work, and one pane had two lifetimes for Torrents
and Library.

- A publisher, such as a selection, listing or operation owner, commits its
  accepted state and raises its change notification. It names no consumer and
  starts no consumer's reads.
- Each consumer owns its activity. It derives activity from its own facts: the
  page shown, the pane open, its tab current, the window minimized. While
  active, it observes. When it becomes inactive, it detaches and cancels its
  work once. When it becomes active again, it reads the current state and does
  not replay the changes it missed.
- A notification handler cancels obsolete work and schedules the latest
  request after the publishing change has rendered. Loading, parsing and
  collection work stay out of the publisher's call.
- A consumer has one activity rule, whatever content it shows.

| Publisher and state | Consumers that observe it |
| --- | --- |
| `Panels.Tab` and Search results: accepted selection and focus | `Shell.Inspector`, `Shell.Commands` availability, the status bar |
| `Shell.MainViewModel`: shown page and active list | `Shell.Inspector`, `Shell.Commands`, each tab's activity |
| `Shared.Engine`: accepted and finished operations with their affected folders | `Operations.State`, which publishes the queue, the status items and the taken entries |
| `Operations.State`: queue, status items, reports and taken entries | `Shell.WorkStatus` on every page, `Operations.Page` while it shows, and each `Panels.Tab` for its rows and its own folder |

A tab is active while the Commander page shows and it is its panel's current
tab. An inactive tab stops watching its folder and makes no reads, as
gozler-ui's panels do while Search shows. A tab that becomes active shows its
last rows as not yet confirmed and reads its folder again.

### Window rules

**Owner ruling, 2026-11-01: the window never blocks.** This is the first
window rule. The UI thread never waits for a disk, the network, the index
service, the engine, another process or a lock. No later plan, feature,
measurement or convenience weakens this rule. A design that needs a wait on
the UI thread is redesigned; it does not get an exception. gozler-ui exists
because other file tools freeze while they work, so a frozen window defeats
the product.

gozler-ui's window needed a planned refactoring: goz
`docs/plans/window-architecture.md`, from `a2daf14` to `c698a56`, 21–25
October 2026. Its causes were structural, not specific to Rust or Win32. These
rules carry the lessons into C# and WinUI, so this product does not need the
same refactoring. The plan's painting and message-loop rules do not apply,
because XAML owns rendering.

| Rule | What went wrong in gozler-ui |
| --- | --- |
| **Nothing on the UI thread waits.** No file, registry, shell, clipboard, pipe or service call, no `Thread.Sleep`, and no `.Wait()`, `.Result` or `GetAwaiter().GetResult()` on the UI thread. | Five filed waits froze the window: drive labels every 2 seconds, a folder check after each key, the shell calls of Open, Explore and Properties, the save of `ui.json`, and clipboard retries (#140–#144). |
| **One owner for UI background work,** `Shared.Work`, following TinyTorrent2's `Shared.BackgroundWork` and `CancellableWork`. It runs a task off the UI thread, marks a task slow after one shared threshold, and delivers only while the task's window is open. A newer request cancels interest in the older one of the same purpose and never waits for it. Feature owners keep their request identity and cancellation; the engine keeps its scheduler. | About 19 places in 12 files started threads, each with its own rules for cancelling, waking the window and exit. |
| **A call that has not returned holds nothing that other work needs.** Work that can hang on a location, such as a share, a drive or a prompt, is separated by location, not only by purpose. The person sees the named cause, such as "Waiting for \\\\nas", and unrelated work continues. | Work slots were "per purpose, not per location", and a cancelled call kept its slot until Windows returned. One hung share blocked the next Open, the panel's next read and the free-space read (goz `locations-that-do-not-answer.md`, `7e3ccba`). |
| **A derived fact about a large collection is computed once by its owner, off the UI thread, and carried in the snapshot.** A view model does not recompute it in a getter, a timer or a refresh. | The window recomputed an operation's common source folder from every source on each update until `dd13c78` moved it into the operation snapshot. |
| **A suspected cost is measured before it is fixed.** | Most suspected waits in goz `window-responsiveness.md` were cheap when timed; "Each fix below was built only because its cost was measured" (`f6a8e7e`). |
| **A pass over many items has a bound.** A pass over a selection, a listing, a search page or an operation's sources runs inline up to one shared size, and otherwise as a `Shared.Work` task over a snapshot. | A 60 ms pass over 100,000 rows on the UI thread, and about 300,000 messages for one whole-selection deselect. |
| **Features own their state; the shell composes.** A new feature adds its own owner and view. `MainWindow` and `MainViewModel` gain only the line that composes it. Another owner reads a fact from its owner, never from a copy. | `main_window.rs` and `main_window/panels.rs` grew to 7,900 lines, `model.rs` to 3,700 and `panel.rs` to 4,050, because every feature added to them. TinyTorrent2 needed `1faab3f` (90 files, +6,741/−5,745) to give Settings, Close and Exit, toasts and engine start-up one owner each. |
| **A lifecycle belongs to its owner, not to each caller.** A task or subscription receives its cancellation and release from its owner when it is created. Correct behavior never depends on a call that each caller must remember to make. | Every group of goz background tasks forwarded `tick`, `take_retired` and `close_delivery` by hand; a forgotten call compiled, and the task never timed out (goz `maintainability.md`). |
| **A state is a type, not a flag or a stand-in value.** `null` means "not received yet"; an enum holds states that exclude each other. | TinyTorrent2 `7eb19cc` (71 files) replaced flags and `0` or `""` stand-ins with enums and `null`. gozler-ui `0e82a1f` replaced an `Option` and an empty string in the inspector's captions with an enum. |
| **A notification names the fact that changed.** Captions, availability and visibility are derived from facts by their owner, not set by another handler. `PropertyChanged` is raised only when a value changed. | One `Change` result had 23 overlapping arms. `Status` refreshed the status bar, the locations, the message and both panels, and also acted. |
| **The control owns selection, focus, scroll, caret, typed text and column widths and order.** A view model changes them only as an explicit effect, such as focusing an entry after navigation or emptying the text for Clear, never during a refresh. | A refresh that set these values undid a column drag, or showed old rows for a new search with the same total. |
| **After every `await`, and in every handler of an event that our own set raised, check that the result still belongs to the current request** (its generation or owner identity) before applying it. | A control set dispatched a newer update before the outer one resumed, so old effects undid newer state. |
| **Before the focused element is hidden or disabled, its owner moves focus on purpose.** | Windows drops the focus of a control that it disables, so the plan had to order structure changes, refresh and focus. |
| **A dialog started by an engine event or a reply waits behind the open dialog,** through the one dialog entry point. | Dialogs that replies opened ran before older replies were applied (window rule 13). WinUI also throws when a second `ContentDialog` opens. |
| **Exit hides the window, stops deliveries, requests cancellation and lets essential writes finish off the UI thread.** | Exit joined threads that waited for a dead share or a permission prompt. |
| **A page, a Settings section or a viewer window is created when it first shows.** | Opening Settings took 325 ms and the main window 627 ms before first show, until `32cfeb4` created pages and the File operations window on first use. |
| **Page and pane state is model state.** The shown page, the navigation history, Quick View and the Search inspector pane's open state and height live in `Shell.MainViewModel`, not in a control. | TinyTorrent2's Search page kept its own splitter, height and maximized state, and the maximized state lived in the `Pane` control (`15-workspace-shell-study.md`, `43ae7bc`). |

The plan's main finding was that nothing enforced these rules. Here they are
enforced from the first commit of the product:

- **At build time:** analyzers report synchronous waits and `async void` outside
  event handlers, and a banned-symbols list covers `Thread.Sleep`,
  `Task.Wait` and `Task.Result` in the app project. Stage 0 chooses the
  analyzer packages and rules.
- **At run time:** a Debug-build stall detector, like gozler-ui's watchdog. A
  background thread records each time the UI dispatcher does not answer
  within 200 ms, with the work that was running.
- **In the plan:** a list of every Windows call that the C# side makes and that
  can wait, each with its owner. The list is kept current before each feature
  lands.

### Operations in the window

Both earlier products restructured how an operation's lifecycle meets the
presentation: gozler-ui in `3b307e5` (+2,892/−1,178) and `8ec9f8c`, and
TinyTorrent2 in `11be2e8`. These rules follow from those changes.

- **The engine's operation state is the authority.** Presentation keeps a
  snapshot for rendering, never a second operation state machine. The UI
  requests pause, cancel and reorder and shows the state that the engine
  reports.
- **A row is never flagged for an operation.** A panel's rows are its listing
  minus the entries that waiting and running operations have taken. An entry
  is released when the first complete listing after the operation's end is
  applied. TinyTorrent2's `deleted` flag had 44 reads in 12 files, and one
  reader forgot it (`14-removed-torrents-plan.md`).
- **One owner decides the operation's presentation lifecycle,** in
  `Operations`. A person's acknowledgement of a report never delays the
  release of taken entries or the panel's re-read. gozler-ui had released
  them only after the person reported the failures (`copy-behaviour.md`).
- **A batch action is one engine request.** Cancel all copies and Cancel all
  deletes are each one request over the captured operations of one lane, not
  a loop of single stops.
- **A command reports what the engine did.** A cut and Paste shows "moved"
  only after the engine reports the move, not at submission.
- **Progress never opens a page or takes focus.** Starting an operation keeps
  the current page.

The scheduling rules are the engine's; these owner rulings of 2026-11-01 say
what it must do ([ADR 0003](adr/0003-copy-and-delete-lanes.md)):

- **Two lanes, one scheduler.** Transfers (Copy, Move, Sync, Mirror) and
  Deletes wait in separate lanes. A Delete never waits for a transfer and a
  transfer never waits for a Delete. Each lane counts its own users of each
  disk, against Transfers per device or Deletes per device: the person's
  settings, from 1 to 3, default 1. In goz a Delete takes no disk and never
  waits (`prepare.rs`); here the Delete lane is part of the engine from the
  start. New folder never waits.
- **The person orders the queue; the engine decides the threads.** Moving a
  waiting operation ahead of a running one in the same lane on the same disk
  starts it and pauses the running one, which resumes, with its progress, when
  its turn comes. A running operation can also be moved down. Work on other
  disks is not affected, and an operation the person paused stays paused. goz
  `Queue::move_waiting` is the reference for waiting transfers; here Deletes
  and a running operation move the same way. SynapseCommander always uses the
  engine's automatic speed values and shows no speed presets.
- **Stop keeps finished work.** Each operation's status bar item has its own
  Stop. The report says how far the operation got, such as "Stopped: 37 of
  120 copied". Stop asks nothing.
- **Mirror asks once before it deletes.** When a Mirror's comparison ends with
  entries to delete, the engine waits before copying anything, holding no
  disks. `Operations.State` queues one question through the dialog entry
  point, and the status bar item says that Mirror waits for an answer:
  - Title: "Mirror C:\Work to D:\Backup\Work".
  - Text: "Mirror will permanently delete 1,240 files and 85 folders (3.2 GB)
    from D:\Backup\Work because they aren't in C:\Work."
  - Buttons: Delete them, Cancel Mirror, and Keep them as the default Close
    button, so Enter and Esc delete nothing. Keep them finishes the same
    comparison as a Sync.

  A Mirror with nothing to delete never asks. The engine's per-file checks
  before each deletion still run after the answer. The comparison's summary
  carries the file count, the folder count and the bytes to delete; goz's
  `ComparisonSummary` counts files and folders together and has no bytes.
- **An action that outlives its dialog belongs to an owner that outlives the
  dialog,** and it ends in a typed outcome: done, cancelled, refused, or failed
  with a reason. gozler-ui's service fixes ran and were forgotten, so a
  refused prompt showed nothing (`service-repair.md`).

### Flow of a listing and a command

A folder listing:

1. `Panels.Tab` asks `Shared.Engine` to read its folder and gives the request
   a new generation number. The UI thread does not wait.
2. `Syno.Files` reads and sorts the folder off the UI thread and delivers a
   snapshot, or pages for a large result.
3. `Shared.Engine` posts the delivery to the window's `DispatcherQueue`.
4. The tab discards a delivery that has an old generation or arrives after the
   tab closed. Otherwise it replaces its rows in one update and keeps focus,
   marks and scroll place by entry name.
5. `Table` shows the rows. The old rows stay visible until the new rows arrive.

A command:

1. A menu item, context menu, key bar button or key resolves to one entry in
   `Shell.Commands`.
2. The command captures its target once: the origin tab, its folder, the marked
   entries or else the focused entry, and the default destination.
3. The owning feature opens its draft or dialog, or sends the request to the
   engine.
4. `Operations.State` presents the engine's state in the status bar and on the
   File operations page. A finished operation names its affected folders. Each active tab that shows one of them reads
   it again; an inactive tab reads when it becomes active.

A later change of the active panel cannot redirect a captured command.

## Lifetime, persistence and recovery

One engine session owns the application's operation queue. Panel/tab/viewer
lifetime is independent of accepted operations. Closing a tab or search view
releases observations and cancels its reads, not unrelated transfers.

Restore engine recovery state before enabling new mutations. `Syno.Files`
writes recovery records in its own format; `Shared.Storage` writes
presentation settings. SynapseCommander uses its
own data directory and never opens gozler-ui's live queue as its own. A
second launch hands off to the running process instead of opening its own
engine session ([one process, one main window](#one-process-one-main-window)),
so two processes never resume one record.

Closing behavior is explicit:

- Hiding to the notification area, when enabled, keeps the same application
  process and engine running.
- Actual Exit during work offers to remain open or stop safely and exit.
  Engine work cannot continue after process termination.
- Shutdown rejects new requests, invalidates UI deliveries, requests engine
  cancellation and completes required persistence off the UI thread. Preserve
  the engine's guarantees about unfinished outputs and interrupted work.
- Dispose delivery subscriptions before releasing their targets. Release
  every native handle through its `SafeHandle` after the engine thread that
  uses it has ended. Finalizers are a fallback, not a blocking shutdown path.
- Session end and process crashes may interrupt writes; atomic recovery state
  and truthful next-start reporting matter more than promising completion.

The stop protocol follows goz `docs/DESIGN.md`'s copy cancellation and
unfinished-output rules: a stopped copy removes only its own unfinished file,
and an interrupted copy resumes inside the file.

## Dependency and release contract

Follow TinyTorrent2's installer approach, documented in its `installer/README.md`:
an unpackaged desktop application delivered as a per-user Inno Setup `.exe`
installer through GitHub Releases. The installer checks the required .NET and
Windows App Runtime versions and obtains missing prerequisites. Installer
generation and publication are separate actions.

Build shared C# libraries through project references to their original source
owners. The application release includes their assemblies, any native library
such as `7z.dll`, XAML resources, fonts, assets and the required license
notices. Separate NuGet/native package publication and a package feed are not
prerequisites for reuse or release. Third-party NuGet dependencies, such as
SharpCompress, keep their normal restore workflow.

One product release version identifies the application and installer. Record
the shared source revisions, architecture and checksums in release metadata.
Load a native library only from the controlled installation directory; do not
search PATH or the current folder for a substitute.

Plan a root `Release.cmd` entry point and `installer/Build.ps1`, following
TinyTorrent's build, publish, validation, optional signing and installer staging
flow.
Use SynapseCommander's own identity and operation-safe upgrade/shutdown behavior.
The installer carries the application and its in-process libraries, with no
additional engine executable or service. These release tools are not implemented
yet. The current Settings sample is a development host and does not
establish the production distribution model.

The initial vertical slice targets x64. Choose supported Windows/.NET/Windows
App SDK versions and runtime requirements from verified toolchain/platform
requirements in the foundation stage. Each additional architecture needs its
matching binaries.

A missing or incompatible native library, such as `7z.dll`, disables only the
feature that needs it, with the reason; it never stops startup.
Missing/incompatible indexing service affects indexed features;
ordinary folder browsing and file operations remain useful. Shared helpers
must tolerate index absence rather than introducing an implicit service
requirement for copy/delete.

Testing covers the engine libraries with their own checks on real temporary
folders and a fake gozlerd pipe, and C# presentation behavior separately.
Measure representative operations against goz `docs/BENCHMARKS.md` on the same
machine before claiming parity of speed or memory use with gozler-ui.
