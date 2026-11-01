# SynapseCommander

Build an all-C# WinUI 3 file manager: the presentation and its engine
libraries live in this repository, with no Rust. Use C or C++ only where C#
cannot do a job, such as an existing native library.
The simplest correct code should make intent and ownership obvious on first
reading. One responsibility has one authority and one implementation.

## Read before changing behavior

- Read [PRODUCT.md](PRODUCT.md) for scope and [CONTEXT.md](CONTEXT.md) for
  vocabulary. Decisions that are hard to reverse are in [docs/adr](docs/adr).
- Read [architecture](docs/ARCHITECTURE.md) before changing ownership, native
  interop, background work, persistence, search, operation lifetime, shared
  dependency sourcing or release distribution.
- Read the relevant stage of the
  [implementation plan](docs/plans/synapsecommander.md) before implementation.
- Before changing settings, dialogs or their declarations, read the
  [Settings component contract](lib/Settings/docs/contract.md). Its
  [plan](docs/plans/settings-component.md) owns the current component trial.

These are the active project documents. The separate goz checkout, currently
at `C:\SynoSoftware\TinyTorrent2\3rdParty\goz`, holds gozler-ui. Its
`docs/DESIGN.md`, `docs/research` and `docs/BENCHMARKS.md` are the engine's
specification, and its Rust code is a reference for edge cases. Its Win32
control rules, Rust build workflow, daemon policies and product defaults do
not apply to this application. Never stop, replace or reconfigure the
installed goz service as part of development.

## Code and ownership

- Keep file-operation algorithms, policies, scheduling and recovery in
  `Syno.Files`, and the search client in `Syno.Index`. The presentation
  requests work and presents outcomes. The architecture assigns the other
  engine responsibilities.
- Owners publish accepted state; consumers observe it and own their activity.
  Read [publication and activity](docs/ARCHITECTURE.md#publication-and-activity)
  before one owner's change causes another feature's work.
- Menus, keys, buttons and context menus invoke the same command implementation.
  Store facts once and derive presentation. Keep related decisions together;
  the main window composes their owners.
- Use concrete types and small interfaces. Add an interface where behavior
  actually varies or a real external dependency needs isolation. Every new
  layer, dependency and abstraction must answer a current requirement.
- Use types to express meaningful states and identities. Keep effects explicit
  in method names and signatures. Return structured failures across the native
  interface; localize their presentation in C#.
- Use WinUI controls, system resources and established .NET patterns. Prefer
  strongly typed bindings where suitable. Keep view code concerned with
  rendering, input, focus and window lifetime; avoid both business rules in
  code-behind and forwarding-only view models.
- Keep disk, network, service, shell and expensive collection work off the UI
  thread. Each owner defines cancellation, stale-result rejection and shutdown.
  `Syno.Files` owns the one operation scheduler; nothing else creates a
  competing one.
  Read the [window rules](docs/ARCHITECTURE.md#window-rules) before writing
  window, view-model or background-work code.
- Review resource lifetime explicitly, especially native handles, event
  subscriptions, buffers and worker completion. Closing a view invalidates its
  pending deliveries without losing accepted operations.
- Validate untrusted disk, archive and native inputs with checked lengths and
  bounded allocations. Keep P/Invoke and `unsafe` code small and inside the
  engine library that owns it, or in `Shared.Desktop` for shell and OLE calls
  that WinUI does not wrap. Preserve the integrity rules of goz
  `docs/DESIGN.md` that the engine implements.
- Make the smallest coherent change that solves the whole task. Remove the
  superseded path and orphans created by the change. Preserve unrelated work.
  Edit existing files through the file-edit tool and preserve their encoding
  and byte-order mark. Where `.gitattributes` owns line endings, leave them to Git.

## Naming and structure

Use C#/.NET conventions. Follow
canonical domain and platform vocabulary, with clarity at the use site first.

- Do not repeat context the owner already supplies: `Panel.Path`,
  `Command.Label`, `operation.Cancel()`; not `Panel.PanelPath`,
  `Command.CommandLabel` or `operation.CancelOperation()`.
- Let namespaces, types and relationships carry context. Use a typed identity
  when a value travels apart from its owner. Add distinguishing words when
  meaning would otherwise be ambiguous.
- Types name concepts; methods name actions; collections are plural; boolean
  conditions read affirmatively, such as `IsActive` and `CanCancel`.
- Challenge filler such as Data, Info, Helper, Manager, Service and Model.
  Retain a word when it represents a real distinction. Do not abbreviate for
  brevity or impose a word-count quota.
- Keep P/Invoke declarations faithful to the Windows or native library
  signature. Use clear C# names in the managed API. Resource keys need enough
  context to remain unique in their shared dictionary.
- Organize code around responsibility, lifetime and ownership. Keep related
  declarations together; avoid generic dumping grounds and layers that merely
  rename another owner's methods.
- Review callers and literal consumers before renaming persisted keys, resource
  names or public contracts. Naming rules do not authorize unrelated renaming.

These principles adapt TinyTorrent2's coding and naming guidance. Its project
specific file-size, enum-placement and naming approval rules are not adopted.

## Verification and reporting

Name the user-visible failure or lasting invariant before adding a test.
Test it at its owner; do not duplicate engine tests in the presentation.
Engine checks run on real temporary folders and a fake gozlerd pipe; ordinary
automated tests must not depend on a running indexing service or the user's
files.

Review each coherent stage, then run its applicable checks. Use state-based
completion rather than fixed sleeps. Run focused checks earlier only when the
answer changes the next edit. Save screenshots of changed UI states for review;
owner-only environment checks do not block a stage.

Markdown-only changes need no build. For the current settings component, run
`pwsh ./Build.ps1 -ComponentsRoot <TinyTorrent2 checkout>`; add `-Launch` for UI
verification. This builds the sample and runs its model checks. It is not yet
a build of the complete file manager. Launch after UI changes and report actual
results and remaining checks accurately.

Writing or approving a plan does not itself start implementation. Change
documents when an approved decision changes; keep each rule authoritative in
one place and link to it from the other documents.
