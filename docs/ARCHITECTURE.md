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
Build the DLL from that source owner and include the matching output in the
application's release. Record the source revision used; publishing a separate
library package is not a prerequisite. SynapseCommander does not vendor crates,
copy algorithms or import goz history. Build configuration locates the source
checkout without committing a developer's absolute path. Engine fixes are made
once and each product deliberately adopts the selected revision.

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

gozler-ui and TinyTorrent2 restructured their engine boundaries several times.
The contract keeps what those changes established:

- **A range-read list reads an engine-held result set.** The engine keeps an
  immutable result for each search or listing, and the UI reads pages of it
  through a handle with a defined release. A row whose file is gone keeps its
  place and reads as gone; a result the engine dropped makes the UI ask again.
  Without a held result, rows of different pages came from different moments
  and the list stopped at 100,000 rows (goz `result-snapshot.md`, `b2bca38`,
  `4f670e2`).
- **Paths keep the engine's meaning.** C# does not normalize, resolve, or
  rewrite paths, and it does not touch the disk to make a search work. Where
  the UI needs a derived value, such as match highlights, it calls the same
  engine function. A client-side path rewrite was deleted for that reason
  (goz `paths-through-links.md`, `9c1d086`).
- **The engine returns whole answers and exposes its limits as data.** A page
  carries its rows, the total and its completeness. Limits such as counts and
  ranges come from the engine, not from constants in C# (goz `d52b4ff`).
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

Finalize the concrete exports, payload representation and delivery mechanism in
stage 1 using the first operation slice. Avoid a generic RPC framework or
serializing every internal Rust structure as the public contract.
Transport/layout compatibility tests supplement the shared engine tests.

Use .NET [native interop guidance](https://learn.microsoft.com/en-us/dotnet/standard/native-interop/best-practices),
including LibraryImport where suitable and SafeHandle for native resources.
[P/Invoke](https://learn.microsoft.com/en-us/dotnet/standard/native-interop/pinvoke)
is the supported managed/native call mechanism. The responsibilities above are
project design decisions, not claims that .NET provides an engine abstraction.

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
| Presentation | `app/presentation/Presentation.csproj` | The presentation owners: view models, commands, panels, search results, operations state, the inspector's state, `Shared.Engine`, `Shared.Work` and `Shared.Storage` | .NET only, no WinUI; loads the engine DLL |
| PresentationChecks | `app/tests/PresentationChecks.csproj` | Console checks of the presentation owners, without a window | Presentation |
| Settings | `lib/Settings/src` | `Syno.Settings`, the settings presentation library | WinUI, Glyphs (Lucide until stage 0 moves it) |
| SettingsSample, SettingsChecks | `lib/Settings/sample`, `lib/Settings/tests` | Settings development host and model checks | Settings |
| Controls | `lib/Controls/src` | `Syno.Controls`: generic window controls with no product rules. It starts with `Pane`, `Splitter` and `MenuItems`, copied from TinyTorrent2's application | WinUI, Glyphs |
| TableView | TinyTorrent2 `lib/TableView/src` | The only table control | WinUI, Glyphs |
| Glyphs | TinyTorrent2 `lib/Glyphs/src` | Names for the glyphs of Windows' icon font | WinUI |
| Engine DLL | goz repository | The shared Rust engine behind the [native interface](#native-interface) | Rust crates |

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
| `Shell/` | `MainWindow`, `MainViewModel`, the command table, the shared inspector, the lower pane, message bar and notifications, About and update checks. It composes the features and coordinates their lifetime. |
| `Panels/` | Commander mode: both panels, tabs, folder listings as presented, focus and marks, history and favorites, quick search and quick filter, drives, the command line, the key bar and panel drag/drop. |
| `Search/` | `Search.Results`, the one owner of a search's query, match modes, filters, result pages, highlights and selection snapshot, and the Search mode view that hosts it (stage 4). |
| `Operations/` | The presented queue, the File transfers view, reports, recovery prompts and the drafts of Copy, Move, Delete, New folder and Synchronize. |
| `Viewers/` | Text and Hex viewer windows (stage 5). |
| `Settings/` | The typed preferences and configured shortcuts, in Presentation, and the `Syno.Settings` catalogue declarations that bind them, in the WinUI application. |
| `Shared/` | Common dialogs, text formatting and `Work`, the owner of UI background work. `Engine/` holds the native declarations, safe handles, session and delivery. `Desktop/` holds clipboard, drag/drop, shell launch, the notification area, activation, global shortcuts and taskbar progress. `Storage/` holds settings and view-state files. |

A type that serves several areas lives in `Shared/`. Feature areas have one
exception, and it points one way: a panel tab whose location is a search holds
a `Search.Results`, the same owner that Search mode uses. `Search/` never uses
`Panels/`. gozler-ui also has one search list type, `SearchList`, for Search
mode and for a panel search. Two result owners would be two implementations of
paging, highlights and the selection snapshot. Type names below are the
planned owners. Implementation may refine them under the
[naming rules](../AGENTS.md#naming-and-structure).

### Main window

One window shows one of two modes, as in gozler-ui. Commander mode shows the
two panels. Search mode shows indexed search across the index. Each mode keeps
its own state while the other mode shows. View > Commander (`Alt+F12`) and
View > Search (`Alt+F7`) switch modes. The window opens in the mode that it
closed in.

```text
+----------------------------------------------------------------------+
| Title bar: icon, title, menu bar (File Edit View Help ... Language)  |
+----------------------------------+-----------------------------------+
| Left panel                       | Right panel                       |
|  [tabs, stage 6]                 |                                   |
|  drive button | path box         |                                   |
|  TableView: Name Ext Size Date   |          (splitter, A16)          |
|  summary: marks, sizes, free     |                                   |
+----------------------------------+-----------------------------------+
| Lower pane (splitter): File transfers or inspector                   |
+----------------------------------------------------------------------+
| Command line: C:\Work>  [                                          ] |
| Key bar: F3 View  F4 Edit  F5 Copy  F6 Move  F7 New folder  F8 Delete|
| Status bar: active panel | transfer summary | messages               |
+----------------------------------------------------------------------+
```

Search mode replaces the panels with the search strip, the filter pane and
the results table. It has no command line and no key bar. The title bar, the
lower pane and the status bar are the same in both modes.

| Region | WinUI composition |
| --- | --- |
| Title bar | `ExtendsContentIntoTitleBar` with a `MenuBar`. Windows draws the caption buttons. |
| Workspace | One `Grid` row. Each mode is one `UserControl`, created when it first shows and then kept loaded, so a mode switch keeps focus, scroll and selection. The hidden mode's owners are inactive ([publication and activity](#publication-and-activity)). |
| Panel path row | A `DropDownButton` with a `MenuFlyout` for drives and recent folders, and an `AutoSuggestBox` for the editable path and its completion. |
| Panel listing | `Syno.TableView` `Table`, bound to the active tab's rows. |
| Panel summary | Text: marked count and size, totals, free space. |
| Between panels | A vertical splitter. Its position is saved. |
| Lower pane | One row behind a horizontal splitter with one remembered height. It shows File transfers, or the inspector where the mode shows it. It never opens by itself. |
| Command line | An editor with the working folder as its visible label (PRODUCT.md, Command bar behavior). |
| Key bar | Buttons that invoke the command table's commands. They show its icon, key and availability. |
| Status bar | Fixed-width groups that do not move when their text changes. |
| Dialogs | `ContentDialog`, one at a time, through one `MainWindow` entry point, as TinyTorrent2's `Shared.Dialog` and `Interact` do. |
| Secondary windows | One `Window` per viewer. Each viewer has its own placement. |

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
already do what the lower pane, both splitters and the menus need.
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
[native interface](#native-interface) requires.

### Presentation owners

WinUI uses MVVM with `INotifyPropertyChanged` and `ICommand` directly, without
an MVVM framework, as in TinyTorrent2. View models own presentation state and
requests. Views keep native controls, focus, scrolling and window lifetime.

| Owner | Owns |
| --- | --- |
| `App` | Startup, single-instance handoff, creation and closing of the engine session, and the main window. |
| `Shell.MainWindow` | Chrome, the two mode views, dialogs, focus movement between regions, splitter mechanics and the order of the Close and Exit steps. |
| `Shell.MainViewModel` | The shown mode and the active list: the active panel's tab in Commander mode, or the results in Search mode. Also the context for command availability, the message bar and the model steps of Close and Exit. |
| `Shell.Commands` | The one command table: identity, caption key, icon, default keys, availability and the owner that runs each command. The menu bar, context menus, key bar and keyboard accelerators are built from it. Configured shortcuts replace the default keys. |
| `Shell.Inspector` | The inspector's activity, observation of the active list's accepted selection, subject, displayed content and the lifetime of its reads, under the rules of [publication and activity](#publication-and-activity). |
| `Panels.Commander` | Both `Panel`s, the active side and Swap (A10). The other panel's folder is the default destination for Copy and Move. |
| `Panels.Panel` | One side: its ordered tabs and the active tab. It has one tab until stage 6. |
| `Panels.Tab` | One retained view. Its location is a folder, with its accepted rows, or a scoped search, with its `Search.Results`. It also holds history, sort, quick filter, focus, marks, scroll place and a generation number that rejects late results. |
| `Panels.CommandLine` | The draft, history, completion requests, standalone `cd`, and launch through `Shared.Desktop`. |
| `Operations.State` | The process-wide queue snapshot, reports and the entries that waiting and running operations have taken. Every window reads it. |
| `Operations.Transfers` | One window's File transfers view: selection in the queue, Flip and Maximize. Pause, Cancel and reorder are requests to the engine. |
| `Operations` drafts | The unfinished choices of one Copy, Move, Delete, New folder or Synchronize dialog, until the engine accepts or refuses the request. |
| `Viewers.Viewer` | One viewer window's native viewer handle, bounded reads and placement. |
| `Settings` | Catalogue declarations for `Syno.Settings` and the typed preferences they bind. |
| `Shared.Engine` | Native declarations, safe handles, the engine session and delivery to the dispatcher. |
| `Shared.Work` | The policy for UI background work: off-thread execution, replacement by purpose, separation by location, slowness, delivery only while the task's window is open, and exit without waiting ([window rules](#window-rules)). |
| `Shared.Storage` | Preferences and view state in SynapseCommander's data directory. It writes only the values that changed, so one window never undoes another window's save (goz `0ed6c44`). The engine's recovery records stay in the Rust owner's store. |

The model holds facts and identities, not display strings. Views format sizes,
dates and captions from the current language and region.

### Windows and owner scope

All windows run on the one UI thread, so an owner that several windows read
needs no lock and no thread hop. gozler-ui made the same decision ("No thread
per top-level window") in its window-architecture plan.

Extra main windows (B01) are windows of the same process. A second launch
hands its request to the running process, which activates a window or opens
another one. Every window can start operations, because all windows share one
engine session and one queue, so two processes never compete for one recovery
record. gozler-ui's extra window was a second process and therefore had no
file operations; this product changes that on purpose.

| Scope | Owners |
| --- | --- |
| Process: one of each, created by `App` | `App`, the `Shared.Engine` session and delivery, `Operations.State`, `Shared.Storage`, the `Settings` preferences and configured shortcuts, the command definitions of `Shell.Commands`, the `Shared.Work` policy and the stall detector |
| Main window: one set per window | `MainWindow`, `MainViewModel`, `Panels.Commander` with its panels and tabs, Search mode's `Search.Results`, `Shell.Inspector`, the lower pane's state, the window's command availability and accelerators, `Operations.Transfers`, the message bar and notifications |
| Viewer window | `Viewers.Viewer` |

- A window's owner reads process-wide owners. It never holds a reference to
  another window's owners.
- Closing a window ends its deliveries, observations and `Shared.Work` tasks.
  Process-wide work and accepted operations continue.
- A process-wide owner publishes to every window; it never names a window, as
  [publication and activity](#publication-and-activity) requires.

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
6. The window's accelerators from `Shell.Commands`: function keys, Alt and Ctrl
   chords, and F6 and Shift+F6 between regions.

`Shell.Commands` resolves keys. Each command declares its key and whether it
also runs while a text editor has focus. Function keys and the mode switches
(F3 to F8, Alt+F7, Alt+F12) run there; keys that an editor uses never do. A key
has one owner at each level. Settings refuses a configured shortcut that a
lower level already owns, and says which owner has it (B19).

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
  mode shown, the pane open, its tab current, the window minimized. While
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
| `Shell.MainViewModel`: shown mode and active list | `Shell.Inspector`, `Shell.Commands`, each tab's activity |
| `Shared.Engine`: accepted and finished operations with their affected folders | `Operations.State`, which publishes the queue and the taken entries |
| `Operations.State`: queue, reports and taken entries | Each window's `Operations.Transfers`, and each `Panels.Tab` for its rows and its own folder |

A tab is active while its mode shows and it is its panel's current tab. An
inactive tab stops watching its folder and makes no reads, as gozler-ui's
panels do while Search shows. A tab that becomes active reads its folder
again.

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
| **An area, a Settings page or a secondary window is created when it first shows.** | Opening Settings took 325 ms and the main window 627 ms before first show, until `32cfeb4` created pages and the File operations window on first use. |
| **Pane state is model state.** The lower pane's open, compact and maximized state live in `Shell.MainViewModel`, not in the pane control, and every mode uses the same state. | TinyTorrent2's Search page kept its own splitter, height and maximized state, and the maximized state lived in the `Pane` control (`15-workspace-shell-study.md`, `43ae7bc`). |

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
- **A batch action is one engine request.** Cancel all is one request over the
  captured operations, not a loop of single cancels.
- **A command reports what the engine did.** A cut and Paste shows "moved"
  only after the engine reports the move, not at submission.
- **Progress never opens a pane or takes focus.**
- **An action that outlives its dialog belongs to an owner that outlives the
  dialog,** and it ends in a typed outcome: done, cancelled, refused, or failed
  with a reason. gozler-ui's service fixes ran and were forgotten, so a
  refused prompt showed nothing (`service-repair.md`).

### Flow of a listing and a command

A folder listing:

1. `Panels.Tab` asks `Shared.Engine` to read its folder and gives the request
   a new generation number. The UI thread does not wait.
2. The Rust owner reads and sorts the folder and delivers a snapshot, or pages
   for a large result.
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
4. `Operations.Transfers` presents the engine's state. A finished operation
   names its affected folders. Each active tab that shows one of them reads
   it again; an inactive tab reads when it becomes active.

A later change of the active panel cannot redirect a captured command.

## Lifetime, persistence and recovery

One engine session owns the application's operation queue. Panel/tab/viewer
lifetime is independent of accepted operations. Closing a tab or search view
releases observations and cancels its reads, not unrelated transfers.

Restore engine recovery state before enabling new mutations. Rust writes
recovery records; C# writes presentation settings. SynapseCommander uses its
own data directory and never opens gozler-ui's live queue as its own. Extra
windows run in the one process ([windows and owner scope](#windows-and-owner-scope)),
and a second launch hands off to it instead of opening its own engine
session, so two processes never resume one record.

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

Follow TinyTorrent2's installer approach, documented in its `installer/README.md`:
an unpackaged desktop application delivered as a per-user Inno Setup `.exe`
installer through GitHub Releases. The installer checks the required .NET and
Windows App Runtime versions and obtains missing prerequisites. Installer
generation and publication are separate actions.

Build shared C# libraries through project references to their original source
owners. Build the Rust DLL through its original source owner. The application
release includes their assemblies/DLL, XAML resources, fonts, assets and required
license notices. Separate NuGet/native package publication and a package feed
are not prerequisites for reuse or release. Existing third-party NuGet
dependencies retain their normal restore workflow.

One product release version identifies the application and installer. Record
the shared source revisions, ABI version, architecture and checksums in release
metadata. Load the matching DLL from the controlled installation directory;
do not search PATH or the current folder for a substitute.

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
