# Settings component contract

Status: initial contract, 2026-10-30. The sample exercises a first subset;
[implementation status](../README.md#implementation-status) distinguishes working
behavior from the remaining contract. This document owns presentation and input
rules. Hosts own meaning, validation, storage and domain operations.

## Purpose

`Syno.Settings` is a reusable WinUI 3 library, developed here before possible
adoption by TinyTorrent2. Like `Syno.TableView`, it owns a consistent interaction
and visual language without owning application data. It is a presentation
library in the application process, not the shared Rust file-operation engine.

Authors declare content and intent in typed C#. The renderer owns XAML, control
selection, hierarchy, icons, spacing, keyboard behavior, accessibility and
feedback placement. Ordinary settings pages do not contain per-entry XAML.

Start with C# definitions because bindings, choices, validation and commands
are typed and refactorable. JSON is not an additional authoring authority.
If external catalogs become necessary, compile a constrained JSON format into
the same definitions; do not add a second renderer or an expression language.

## Three kinds of interaction

| Kind | Declaration | Apply behavior | Presentation |
| --- | --- | --- | --- |
| Preference | A typed field | A switch/choice applies individually; text applies when valid and committed | A row in the appropriate card/group |
| Transaction | One value plus a typed `Form<T>` | Edit a fresh draft; Save submits the complete value once; Cancel submits nothing | Summary card and Edit, opening the shared dialog |
| Workflow | A registered task | Its operation owner defines start, cancel, proposal and commit | Shared task shell containing a purpose-built WinUI control |

These are semantic distinctions. Do not make a preference transactional to get
a Save button or a workflow to escape ordinary row layout.

A proxy is a transaction: type, address, port and credentials form one usable
configuration. A speed test is a workflow: it may measure, report phases, draw
a chart and produce a proposal. Measuring never applies the proposal. Its Apply
submits the coherent proposal through the owner; Cancel discards it. A settings
file or generic form is not the owner of measurement/scheduling logic.

The owner explicitly requires individual application of ordinary settings.
There is no page-wide Apply/Save/Cancel or dirty-page confirmation. This adopts
TinyTorrent's intentional interaction design; it is not a claim that WinUI's
type system enforces one commit policy for every application.

## Ownership and authoring locations

| Place | Authority |
| --- | --- |
| `lib/Settings/src` | Definition types, renderer, input lifecycle, dialog mechanics, shared styles and catalog validation |
| Host's settings catalog | Category order, task grouping, localized labels, meaningful icons/artwork, descriptions, fields and references to tasks |
| Host's typed bindings | Confirmed values, valid ranges/choices, availability reasons and calls to the actual setting owner |
| Host's persistence owner | Where and how accepted preferences are stored; migrations and failure handling |
| Registered workflow | Rich task content and its operation-specific state, within the shared shell |

The library chooses no filename and performs no settings-file I/O. Production
SynapseCommander uses its own local application-data directory for personal
preferences; Rust owns operation recovery, and the index service owns its own
administrative configuration. Windows registrations remain observed OS state.
Do not copy service settings or registrations into a second authoritative store.
The sample has a separate store documented in its README. Credentials require
a suitable protected store, not an ordinary JSON preference field.

There is one canonical declaration for each preference. Home shortcuts or
related links refer to it by identity, rather than restating its label, range,
control and help. A default and a validity rule live with their existing owner;
the renderer never infers them from a label or invents a fallback value.

## The presentation grammar

| Construct | Use it for | Renderer owns |
| --- | --- | --- |
| Category | One user task area | Native navigation item, required icon/artwork, page title and measured overview header |
| Card | One independent preference | One card, one leading subject icon, one field |
| Group | Two or more related, directly accessible preferences | External heading, one shared card, text-only rows and separators |
| Expander | Related secondary configuration that benefits from disclosure | One native expander, required subject icon, header detail and grouped body |
| Transaction | A coherent value edited separately | Subject icon, title, confirmed summary and Edit |
| Workflow | A richer manual task | Subject heading/icon, content insets, lifecycle and the registered task content |
| Fact | A current category fact | Required icon, label/value pair, tooltip, typography and responsive placement |

One field does not get a Group heading or an expander. An expander is not a
container for arbitrary unrelated controls. Rows inside a group do not repeat
its icon or its title. Distinct manual tasks inside a larger workflow have their
own task heading and icon; they are not ordinary preferences with more padding.

The initial grammar is deliberately bounded: no arbitrary nested cards, no
per-node templates, margins, widths, font sizes, colors or XAML/HTML strings.
New reusable compositions extend this contract and the renderer together.

### Category and header

Use native NavigationView semantics. Category icons occupy the native icon
slot at 24 DIPs and retain accessible names in the compact rail. Category
selection keeps a stable content viewport; it does not reset unrelated work.

Use TinyTorrent's overview: artwork beside the category title/context, with up
to two current facts aligned alongside when space permits. A distinct page
heading above that overview is optional, not a repeated category name. Artwork is
required even when context text is absent. Optional subject/context must say
something useful about the category; omit a generic product/version sentence.
Every fact has an icon. Facts read confirmed state from existing owners and
perform no I/O or new query while rendering.

Share measured wide, facts-below and stacked-facts layouts. Decide from actual
available space and text, not a page-specific breakpoint. Hide an absent slot
without leaving a blank line or empty column. Do not manufacture facts to fill
two slots. Allowing zero to two facts is an intentional generalization of
TinyTorrent's two-fact categories.

The host selects one settings-search entry point. A global search can consume
the component's index/reveal contract; otherwise the component presents its own
search. Do not show both for the same task. TinyTorrent's title-bar search and
workspace-return navigation are host integrations, not hard-coded torrent rules.

### Text roles and wording

| Slot | Content rule | Example |
| --- | --- | --- |
| Category/group title | Familiar subject, sentence case, no generic filler | File operations; Startup and closing |
| Field label | The exact choice or behavior, without repeating the enclosing subject | Existing files; Transfers per device |
| Value | Current confirmed fact, with unit/unknown state where relevant | 1 per device; Not available |
| Summary | Concise configured state, derived from the owner | Update changed files · 1 per device |
| Consequence | One short sentence explaining an otherwise surprising effect | Closing the window keeps accepted operations running. |
| Error | What failed and what the person can correct | Enter a number from 1 to 3. |
| Action | Familiar verb; scope in nearby content and tooltip | Edit; Browse; Retry |

Every visible word has one role. Do not repeat a label in its description or
put instructions such as “Use this setting to…” under every row. Help that adds
no new consequence belongs outside the scanning surface. Full-value tooltips
are an accessibility route to trimmed text, not a second explanatory paragraph.

`Detail.Summary` and `Detail.Consequence` are exclusive. Summary is visible only
while collapsed; expanding removes its line and keeps its state at the fields.
A consequence remains visible in both states because its warning still matters.
Static groups have external headings; they do not inherit expander header layout.

Use Body text for labels, values and feedback, Body Strong for group/fact
labels, and the shared page-title role for category titles. Secondary facts
retain readable body size and use the secondary theme brush. Sentence case and
plain language follow [Fluent content guidance](https://fluent2.microsoft.design/content-design).
Semantics and wording still require a content decision; a validator cannot
prove that a paraphrase is useful or that the chosen grouping matches a task.

### Icons, geometry and states

Adopt TinyTorrent's neutral Lucide subject/action language. Icons are supplied
by semantic glyph names, not local FontIcon objects. The renderer owns font,
size, alignment, color and automation treatment. Category color artwork/icon
assets need an appropriate high-contrast fallback in the complete component.

| Role/token | Baseline |
| --- | --- |
| Maximum content width | 1200 DIPs |
| Major task gap | 24 DIPs |
| Sibling card gap / heading-to-card gap | 8 DIPs |
| Horizontal card inset | 16 DIPs |
| Row minimum / with a description | 52 / 64 DIPs |
| Header / navigation / statistic icon | 20 / 24 / 24 DIPs |
| Artwork | 96 DIPs |
| Ordinary title/detail gap / category overview gap | 4 / 12 DIPs |

These are shared baselines, not per-entry knobs or fixed heights. Grow for
Windows text scaling. Trailing editors align to the real right inset; reserve
an action column only for related actions. Icons center on the whole text block
and do not enlarge the container. Cards retain native themed fill, stroke and
rounding; nested rows use separators rather than nested card borders.

Preserve TinyTorrent's expander code and matching template together: animate
the clipped drawer's height for 260 ms opening (cubic ease out) and 220 ms closing
(cubic ease in/out), including interruption, content remeasurement, unload and
the system animation preference. The header stays centered while a collapsed
summary disappears. A stock or Toolkit expander is not proof of equivalent motion.

Category changes use TinyTorrent's shared `Motion.Enter` implementation: a
250 ms cubic ease-out horizontal slide, forward or backward according to the
category order. Clip at the page host so moving content stays outside the
navigation pane. Preserve interrupted-transition cleanup and respect the
system animation preference. Stop motion when the view detaches.

Labels, values and short feedback stay single-line and expose their full text
through keyboard-accessible help, selection or an accessible description.
At narrow widths, stack trailing editors through one shared rule before they
can overlap. Never shrink text to fit. Dialog prose may wrap. Long diagnostics
use a selectable details surface, not a taller ordinary preference row.

Keep dependent fields visible and disabled with an accessible reason. Advanced
mode is a separate discoverability policy: hide deliberate tuning/diagnostics,
retain their values and include them in search. A custom workflow may have
state-specific content; that does not authorize each field to hide its siblings.

Advanced mode changes visibility in place, as TinyTorrent does. Keep the
category overview, existing editors, workflow controls and expanders mounted;
do not clear/rebuild the page or restart page-entry motion. Preserve input,
expansion and view state. Hide a group's heading with its body, and show row
separators only between visible rows. Footer invocation and search reveal use
the same disclosure owner; changing the footer state updates its existing icon.

### Editors

| Meaning | Native presentation |
| --- | --- |
| Immediately effective boolean | ToggleSwitch with one On/Off state presentation |
| Finite mutually exclusive choice | ComboBox; specialized semantic compositions need a shared contract |
| Text | Labeled TextBox |
| Integer/limit | TextBox with native clear button; parsing/range/unit come from its owner |
| Folder | Labeled path field and Browse using the host's native picker |
| Read-only fact | Copyable value, with explicit unavailable state |
| Command | Familiar icon-led button invoking the host's command owner |

Optional/unlimited values require an explicit domain representation; zero is
not silently interpreted as unlimited. Theme previews and multi-selection
lists are reusable semantic compositions, not a local replacement for a choice.
The prototype currently uses an ordinary choice for its theme example.

## Commit and lifetime rules

Switches and choices submit when changed. Text/numbers validate and submit on
Enter or appropriate focus departure. Escape restores unfinished input. IME
composition, typing and clearing are not requests to save partial values.
Navigation captures valid input and proceeds; invalid departing input returns
to confirmed state. Ordinary saves keep the control enabled and focused and
do not flash a spinner, success toast or disabled state for each quick write.

Each binding serializes accepted changes with a bounded newest-pending value.
An old success/failure cannot overwrite newer input. Failure leaves an honest
confirmed state and reports the actionable reason; the host's notification
route still observes it after leaving Settings. Engine unavailability differs
from invalid input and never traps normal navigation waiting for reconnection.

Views observe values; the host retains accepted work. Unsubscribe observations
on disposal. Closing a category must not stop accepted saves or a running test.
The application coordinates shutdown/draining without blocking its UI thread.
Keep ordinary drafts, confirmed values and accepted operations distinct.

## Common dialog language

A transaction supplies a subject icon, title, `Value<T>` and `Form<T>`. The
form declares typed fields against a fresh `Draft<T>`; fields cannot bind to
live settings. The host's immutable value is replaced only after Save. Draft
dependencies disable irrelevant inputs without hiding them or shifting layout.

The shared dialog owns heading/icon, form rows, units, error placement,
Save/Cancel captions and icons, default action, focus, Enter/Escape, accessible
names, width and scroll ownership. A definition supplies no ContentDialog
template or button layout. Save waits for validation and one whole-value
submission; failure keeps the dialog and attempted input. Cancel and ordinary
close discard only the draft. A submitted operation is not retroactively
cancelled by disposing the dialog.

Long-running tasks add named Check/Test/Cancel/Apply actions through the shared
workflow shell. Their labels, icons, status and progress have reserved slots;
they do not recreate the dialog footer or place a Save button on each field.
Tests that operate on a draft do not persist it. A confirmation or destructive
operation is a distinct dialog purpose, not a transaction with a red Save button.

## Rich workflows and TableView

Register complex task controls separately and reference them by stable identity.
Examples include a speed test, shortcut editor, inclusion tree and synchronization
preview. The task may own a graph, TableView, native selectors and a proposal.
It retains operation-specific scheduling, validation and cancellation. The
settings renderer owns the surrounding visual/navigation contract.

Registration is a deliberate extension seam, not arbitrary Content on every
setting. A workflow cannot replace the category header or turn normal fields
into locally styled rows. It must publish search/reveal destinations, focus and
departure behavior, availability, progress and failure to the host shell.

Speed testing in particular requires a real measurement owner, confirmed current
values and a separate proposal. Starting/stopping a test, applying its result
and leaving the page are distinct actions. Charts and sample data are not
declarative preferences. The prototype demonstrates the rich-content seam with
the actual TinyTorrent TableView; it implements no speed measurement engine.

The complete workflow shell provides these roles consistently for an inline
task or a dedicated child page:

| Role | Shared presentation | Host responsibility |
| --- | --- | --- |
| Identity | Subject icon, title, concise purpose and Back on a child page | Localized content and reveal identity |
| Inputs | The same field labels, units, availability reasons and validation as a form | A task-local draft and domain validation |
| Execution | Start/Test action, current phase, progress and Cancel | Real work, cancellation and truthful progress |
| Outcome | Results area, selectable details and actionable failure | Measurements, diagnostics and result validity |
| Proposal | Current/proposed comparison and explicit Apply | One coherent proposal and its atomic submission |

Use one primary action appropriate to the current state. A test offers Test
before it runs, Cancel while it runs, and Apply only when a valid proposal is
available. A failed or cancelled test cannot leave an old proposal looking
current. Applying reports its own outcome; finishing measurement is not a
successful settings save. Do not invent percentage progress for an operation
that only reports phases.

The host retains the task state when its control unloads. Back/navigation
leaves the workflow; Cancel requests cancellation of the work. They are separate
actions. Reserve child pages for tasks whose charts, results or editing need
that space; they retain the same header, action and feedback language. A rich
control supplies its specialized body, not a replacement settings shell.

TableView remains authoritative for table behavior. During development the sample
references TinyTorrent2's existing project through `ComponentsRoot`, without
copying its source. Consumers build shared source projects and include their
assemblies, resources, assets and licenses in their application release, under
the [release contract](../../../docs/ARCHITECTURE.md#dependency-and-release-contract).
Separate component package publication is not required. Migrating TinyTorrent
remains separate work, not implied by a local source reference.

## Enforcement and acceptance

Before rendering, reject missing/duplicate identities, missing required labels
or icons, missing category artwork, unknown tasks, unsupported compositions,
empty choices and inappropriate single-field groups. Transaction construction
rejects live-field bindings and duplicate draft identities. No reflection over
property paths or page-specific switches decide presentation.

Complete validation also checks all supported localized resources/assets,
choice values and unique primary locations; resource updates preserve stable
identities. Mechanical checks catch exact repeated text; content review decides
semantic redundancy. Stable generated automation identities identify the
setting rather than its label or row position.

One reveal path selects category/task, exposes advanced content, expands the
right container, scrolls and focuses the field. Outline its entire card,
including padding/icon/content, for three seconds or until user interaction;
external headings are excluded. Focus restoration must survive async layout
and a newer navigation invalidates an older reveal.

Verify representative cards, groups, expanders, transactions and workflows
with keyboard, long translations, narrow widths, text scaling, light/dark and
High Contrast. Prove values and domain integrity at their owners, dialog atomicity
and stale-save behavior in model checks, and actual layout/automation in a real
host. A successful build is not evidence of correct pixels or Narrator behavior.
