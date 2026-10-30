# Reusable settings component

Status: phase 1 complete, 2026-10-30. Phases 2–4 remain pending. This component
trial precedes the main file-manager engine integration at the owner's request.
It does not mark the main application foundation complete.

The owner is continuing design. Keep the current sample as a prototype.
The requested presentation corrections copy TinyTorrent's working layout and
motion; broader implementation remains deferred until requested.

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

Preserve TinyTorrent's working presentation by copying its controls/templates
and motion before adapting app-specific bindings. Do not reconstruct the same
behavior from screenshots. The
[Community Toolkit options report](../research/community-toolkit-settings.md)
records alternatives; no additional dependency or renderer is authorized by it.

The current correction copies TinyTorrent's category overview and shared
page/expander motion, restores row insets and centered header/navigation text,
and keeps app content behind the existing declarations. This is a subset of
phase 2, not completion of its full acceptance matrix. Code comparison checks
the motion class, expander class and matching template against TinyTorrent's
source; further video/screenshot capture was stopped at the owner's request.

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

## 4. Reuse and ship with the application

Keep Settings, TableView and Lucide as source project dependencies, each with one
implementation owner. Include their compiled assemblies, resources, assets and
licenses in each consuming application's normal release output, following the
[application release contract](../ARCHITECTURE.md#dependency-and-release-contract).
Record the source revisions used; separate library package publication is not a
gate for reuse. Verify the consumer has no dependency on TinyTorrent application
code. Evaluate TinyTorrent migration against this contract and its existing
proxy, schedule and speed-test behavior; migrate one coherent category at a time,
removing superseded presentation in that change. Do not start TinyTorrent
migration or modify its sources in this trial.

**Completion:** a reproducible consumer build resolves the declared shared source
dependencies, and the installed application carries all required resources and
licenses. Equivalent settings have one presentation implementation in both
products; the installed application needs no source checkout or component feed.

## Evidence

Verified on 2026-10-30:

- Pre-commit review: an independent reviewer completed the five code passes and
  WinUI review. Its introduced reconfiguration finding was fixed by clearing the
  current category before replacing the catalog; the delta review found no
  outstanding material defects. One final default `Build.ps1` run passed with
  zero warnings/errors and all five model checks. Default unpackaged launch and
  `lib/Settings/tests/Advanced.ps1` passed against isolated settings. The earlier
  packaged/unpackaged startup failure did not recur after rebuilding the current
  configuration. Reconfiguration was traced in code; the model checks do not
  exercise WinUI view lifetime. Remaining visual acceptance is still listed below.

- Advanced-mode correction: the footer now updates disclosure without rebuilding
  the category or replacing its icon. Search uses the same owner. Like
  TinyTorrent's `ShowCategory`, selecting the already-current category does not
  rebuild it. Three full `Build.ps1` runs passed with zero warnings/errors and
  all five model checks; the third used an environment-only MSIX override for
  verification because unrelated unpackaged launch edits were in progress.
  `lib/Settings/tests/Advanced.ps1` passed using actual keyboard activation:
  the diagnostic row appears/disappears, native editor/expander identities stay
  unchanged and expansion survives both toggles. The earlier SelectionItemPattern
  probe selected the footer without activating it; it is not evidence of the
  actual Advanced toggle. No new model test attempts to simulate visual lifetime.

- Presentation correction: two full `Build.ps1` runs passed the build and five
  existing model checks. The first exposed a helper-name hiding warning;
  using the copied shared `Motion.Clip` removed it. The final run had zero
  warnings/errors and the final sample launched. Source comparisons passed for
  the expander class, its template and the page motion class. Existing checks
  cover settings semantics, not animation pixels. No new model test was added
  for timing constants; the owner requested code comparison rather than more
  captures. Narrow/text-scale/High Contrast acceptance remains pending below.

- Reviewed the declarations, renderer, transaction flow, value lifetime, sample
  persistence and actual callers. TinyTorrent's referenced TableView/Lucide and
  settings sources remain unchanged; their build outputs are in this workspace.
- `pwsh ./Build.ps1 -ComponentsRoot C:\SynoSoftware\TinyTorrent2` is green,
  with zero warnings/errors on the final run. Four full runs: the first found a
  renderer compile error; subsequent runs passed, including after the two
  runtime fixes below. A standalone model-check run also passed.
- Five model checks watch lost rapid changes, stale failure delivery, partial
  proxy writes, accidental live bindings in a draft, and missing icon metadata.
- The Visual Studio solution includes the sample, library, model checks and
  original TableView/Lucide projects. Its Debug/x64 IDE build succeeded for all
  five projects. The solution-based `Build.ps1` also passed its build and five
  checks. The local dependency path stays in ignored configuration; shared
  component source remains in TinyTorrent2.
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
icons, workflow action/lifetime integration, localization and release integration.
ARM64 and installer distribution were not checked. No CI pipeline exists
for this new component yet; no hardware speed measurement was performed.
