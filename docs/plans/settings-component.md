# Reusable settings component

Status: phase 1 complete, 2026-10-30. Phases 2–4 remain pending. This component
trial precedes the main file-manager engine integration at the owner's request.
It does not mark the main application foundation complete.

The owner is continuing design. Keep the current sample as a prototype;
further implementation is deferred until the design is agreed and requested.

Read the [component contract](../../lib/Settings/docs/contract.md) before changing
definitions, controls or dialogs. The [source audit](../research/tinytorrent-settings.md)
records TinyTorrent evidence. This plan owns execution/status, not visual rules.

## 1. Establish the language and a real host

- Extract TinyTorrent's consistent hierarchy, headers, icons, text and commit
  rules. Separate reusable rules from torrent-specific decisions.
- Implement a reusable WinUI library here with typed C# declarations. The sample
  supplies content/bindings/storage and demonstrates independence from both apps.
- Use the existing TableView and Lucide projects through an explicit development
  path; keep one source implementation. Place generated output here.
- Prove ordinary individual saves, cards/groups/expanders, confirmed summaries,
  current facts and validation of incomplete declarations.
- Prove transaction drafts, common dialog rendering, whole-value Save and Cancel
  without partial writes. Use proxy configuration as the concrete scenario.
- Demonstrate a registered rich task using the actual TableView. Record speed
  testing as a supported workflow shape without implementing fake measurements.

**Completion:** library/sample compile, model checks pass, the real window is
responsive, TableView is visible, and representative input/transaction behavior
is checked against its isolated settings file. Save screenshots of new states.

## 2. Finish shared presentation enforcement

Consolidate the contract's metrics/styles/icons into their shared owner. Complete
measured narrow-row layout, header alignment, single-line feedback/details,
localization/resource validation, theme/High Contrast behavior and text scaling.
Complete shared dialog button icons and keyboard/focus/scroll treatment.
Add the full whole-card reveal cue, stable focus/scroll restoration and stale
navigation rejection. Reproduce the agreed TinyTorrent motion and decorative
treatment once, rather than as per-page overrides.

**Completion:** the representative control matrix works at narrow/wide widths,
with long text, scaled text, light/dark/High Contrast and keyboard navigation.
Catalog authors cannot reintroduce missing icons or local layout overrides.

## 3. Complete host and workflow integration

Provide the single search/index/reveal integration, app-wide save-failure
handoff, view-state persistence and lifecycle hooks. Extend the shared task
shell with actions, progress, feedback, proposal/Apply behavior and child-page
navigation for rich scenarios such as speed testing. Keep the task's real engine
and proposal semantics with its host owner. Dialogs and pages share the same
text, icon, button and feedback language without sharing arbitrary visual trees.

**Completion:** a coherent transaction and a long-running custom workflow can
leave/reenter Settings without losing accepted work or applying a draft. A test
does not save settings; applying a proposal is one explicit owner request.

## 4. Package and adopt deliberately

Publish versioned Settings, TableView and Lucide artifacts with assets, licenses
and compatible WinUI dependencies. Replace the local development source path
with pinned packages for distribution. Evaluate TinyTorrent migration against
this contract and its existing proxy, schedule and speed-test behavior; migrate
one coherent category at a time, removing superseded presentation in that change.
Do not start TinyTorrent migration or modify its sources in this trial.

**Completion:** a clean consumer can restore the shared libraries without a
TinyTorrent application checkout; equivalent settings have one presentation
implementation in both products.

## Evidence

Verified on 2026-10-30:

- Reviewed the declarations, renderer, transaction flow, value lifetime, sample
  persistence and actual callers. TinyTorrent's referenced TableView/Lucide and
  settings sources remain unchanged; their build outputs are in this workspace.
- `pwsh ./Build.ps1 -ComponentsRoot C:\SynoSoftware\TinyTorrent2` is green,
  with zero warnings/errors on the final run. Four full runs: the first found a
  renderer compile error; subsequent runs passed, including after the two
  runtime fixes below. A standalone model-check run also passed.
- Five model checks watch lost rapid changes, stale failure delivery, partial
  proxy writes, accidental live bindings in a draft, and missing icon metadata.
- A real packaged x64 WinUI sample launched. A bounded UI probe checked an
  individual toggle against the isolated file, proxy dependencies, draft
  isolation, Cancel with no write, atomic Save of address and port, numeric
  focus-departure commit, category navigation, theme selection and preservation
  of unrelated preferences. It found and drove a fix for sibling refresh
  replacing uncommitted text.
- Visual review of saved screenshots covered cards, grouped rows, the native
  expander, proxy dialog, facts/artwork and the original TableView. Light-theme
  review found a host backdrop mismatch; theme ownership moved to the window
  root. A focused recheck of that final build passed, including persisted values
  after relaunch. Representative dark and corrected light states were viewed.

Local evidence is in ignored `artifacts/`: `settings-general.png`,
`settings-proxy-draft.png`, `settings-transfers.png`, `settings-light.png`,
`settings-general-light.png` and `settings-table.png`. The bounded probe is
`artifacts/settings-ui-check.ps1`; its isolated data is in
`V:\temp\SynoSettings-20261030-fixed`.

Full contract compliance remains pending: narrow/long-text/scaled-text layouts,
High Contrast and Narrator, complete keyboard/focus coverage, dialog action
icons, workflow action/lifetime integration, localization and distributable
packages. ARM64 and release packaging were not checked. No CI pipeline exists
for this new component yet; no hardware speed measurement was performed.
