# SynapseCommander

A two-panel Windows file manager in C#, built on gozler-ui's design and
gozler's file index. These are the words its documents, code and interface
use.

## Window and pages

**Main window**:
The one window that holds every page. Only viewers and editors open other
windows.
_Avoid_: Commander window, second window

**Page**:
One full view of the main window. Exactly one page shows at a time.
_Avoid_: Mode, screen, workspace

**Commander page**:
The home page, with the two panels, the command line and the key bar.
_Avoid_: Commander mode

**Secondary page**:
A page opened from another page, with a Back arrow that returns to it: the
Search page, the Multi-Rename page, the File operations page and Settings.
_Avoid_: Peer page, dialog

**Search page**:
The secondary page that searches the index with all match and filter
controls.
_Avoid_: Search mode, Find Files

**File operations page**:
The secondary page that lists every operation for inspection, reordering and
scheduling.
_Avoid_: Transfer manager, operations window, File transfers pane

**Viewer**:
A separate window that shows one file as text or hex.
_Avoid_: Lister

## Panels

**Panel**:
One side of the Commander page, showing a folder, an archive or scoped search
results.
_Avoid_: Pane, side view

**Active panel**:
The panel that commands act on.
_Avoid_: Source panel, current panel

**Other panel**:
The panel that is not active; its folder is the default destination.
_Avoid_: Target panel, inactive panel

**Tab**:
One retained panel view, with its own navigation, filter, focus, sort and
scroll.

**Focus**:
The one entry under the cursor.
_Avoid_: Current item, selection

**Marks**:
The entries a command acts on. When nothing is marked, a command acts on the
focused entry.
_Avoid_: Selection, checked items

**Quick search**:
Typing in a panel to move the focus to a matching visible name.

**Quick filter**:
A temporary filter that narrows the panel's listing.

**Quick View**:
The inspector and preview shown in place of the other panel.
_Avoid_: Lower pane, preview pane

**Inspector**:
The details of the focused entry or totals of the marks.

**Path box**:
The editable path of a panel's folder.
_Avoid_: Address bar

**Command line**:
The editable box below the panels that runs commands in the active panel's
folder.
_Avoid_: Command bar, console

**Key bar**:
The row of function-key buttons below the command line.
_Avoid_: Button bar, F-key strip

**Archive folder**:
An archive opened in a panel and browsed like a read-only folder.
_Avoid_: Packed folder, virtual folder

**Format module**:
The engine part that reads, and where supported creates, one archive format.
_Avoid_: Plugin, packer

## Operations

**Operation**:
One accepted Copy, Move, Sync, Mirror or Delete request, with its own progress
and its own Stop.
_Avoid_: Process, job, task, transfer

**Transfer**:
An operation that copies data: Copy, Move, Sync or Mirror.

**Lane**:
One of the two queues that operations wait in: transfers, or Deletes. An
operation waits only for operations in its own lane.
_Avoid_: Queue type, priority class

**Transfers per device**:
The person's limit on how many transfers may use one disk at the same time.

**Deletes per device**:
The person's limit on how many Deletes may run on one disk at the same time.

**Stop**:
Ending an operation early. The work it finished stays, and its report says how
far it got.
_Avoid_: Cancel, abort

**Sync**:
Copying new and changed files from a source folder to a destination folder,
keeping files that only the destination has.
_Avoid_: Update, two-way sync

**Mirror**:
A Sync that also permanently deletes the files that only the destination has.
_Avoid_: Clone, exact copy

**Comparison**:
The engine's reading of both folders that decides what a Sync or Mirror will
do.
_Avoid_: Preview, scan

**Taken entry**:
An entry that an accepted Delete or Move will remove, shown struck through
until the operation ends.

**Report**:
What an operation actually did: done, skipped and failed items, and why.
_Avoid_: Log, result

**Recovery**:
Continuing an interrupted operation after verifying what it already did.
_Avoid_: Undo, resume

**Status bar item**:
One operation's label, progress and Stop in the status bar.
_Avoid_: Progress button, transfer summary

## Search

**Index**:
The gozler service's record of every file name, which search reads.

**Search**:
Finding entries by name in the index. It never looks inside files or
archives.
_Avoid_: Find in files, content search
