# TinyTorrent settings: extracted rules

Reviewed 2026-10-30 from `C:\SynoSoftware\TinyTorrent2`, HEAD
`09ee6f111890096b23cb3c31f65058962d8cd690`. This was a source/contract review,
not a live audit of every TinyTorrent page. TinyTorrent was not edited.

The normative adaptation is the
[Settings component contract](../../lib/Settings/docs/contract.md). This file
records the evidence and rationale; it is not another presentation authority.

## Evidence map

| TinyTorrent source | What it establishes |
| --- | --- |
| `docs/contracts/ui/settings.md`: Settings navigation, Category headers | Native category rail; 24-DIP icons; artwork/title/context and two facts; measured header layouts |
| Same contract: Cards and categories | Static headings outside cards; subject icons on single cards/expanders; no repeated row icons; 24/8/16-DIP rhythm; 52/64-DIP row baselines |
| Same contract: Revealing settings | One reveal-expand-scroll-focus route; whole-card outline; shared navigation behavior |
| Same contract: Proxy server, Committing settings | Individual ordinary saves; coherent proxy transaction; disabled dependent fields retain geometry; accepted saves outlive navigation |
| Same contract: Connection setup | Measurements produce a proposal; explicit Apply/Cancel; feedback/progress reserve space; real custom workflow |
| `docs/contracts/ui.md`: Text, icons, and typography | Short labels; useful consequence descriptions; no redundant help; body-sized secondary facts; role-sized icons; shared alignment |
| Same policy: Committing edits | TextBox numeric input; immediate switches/choices; Enter/focus-departure commit; Escape/IME safety; coherent explicit editors |
| `app/src/Settings/Header.xaml` and `.xaml.cs` | One header visual tree; wide/facts-below/stacked layouts measured from available width |
| `app/src/Settings/Fact.xaml`, `Overview.cs` | Fact icons and values from existing owners; no invented overview data or extra query |
| `app/src/Controls/SettingsRow.cs`, `App.xaml` | Shared editor/name/error/unit handling and row/card templates |
| `app/src/Controls/SettingsSection.cs`, `SettingsExpander.cs` | Header summary/description distinction and native expander mechanics with one animation owner |
| `app/src/Controls/Motion.cs`, `SettingsPage.ShowCategory` | Directional 250-ms category entry, interrupted-motion cleanup, host clipping and system animation preference |
| `app/src/Settings/Setting.cs` | Confirmed versus draft/pending state; newer-input protection; individual setting persistence callbacks |
| `app/src/Settings/SettingsPage.xaml` and `.xaml.cs` | Current real compositions, special tasks, page-owned text commit handlers and inline overrides |
| `lib/TableView/AGENTS.md`, `docs/tableview-contract.md` | Independently documented reusable library with sample/tests; host supplies data/commands/persistence |

The [WinUI Expander guidance](https://learn.microsoft.com/en-us/windows/apps/develop/ui/controls/expander)
supports disclosing related secondary content. Native toggles are appropriate
for [immediately effective binary choices](https://learn.microsoft.com/en-us/windows/apps/develop/ui/controls/toggles).
Those controls do not define the full application settings language.

The prototype now copies `Motion.cs` and `SettingsExpander.cs` from this
revision. Source comparison verifies that only namespaces and the expander type
name/qualification differ. The expander template is copied with its native
states and drawer parts, and the category renderer calls the copied `Enter`
path using navigation order. The overview visual states, styles and measurement
come from TinyTorrent's Header; its content bindings use our category/fact
declarations. TinyTorrent's sources remain unchanged. This preserves the working
presentation before generalizing its host boundary. See also the
[Community Toolkit options](community-toolkit-settings.md).

## What the new abstraction must enforce

1. TinyTorrent shares controls, but callers still assemble rows and choose
   templates. Model Card, Group and Expander as different declarations; remove
   local layout properties from the authoring surface.
2. SettingsSection accepts both Description and Summary and gives description
   precedence. Declare either a persistent consequence or a collapsed-only
   current-state summary, so authors cannot accidentally combine their roles.
3. SettingsRow can override the setting label and supply arbitrary content/icons.
   New declarations have one field label and role-specific mandatory metadata;
   their editor is chosen by the typed field kind.
4. Page code attaches numeric/text commit handlers. Move ordinary input lifecycle
   into the reusable renderer, rather than copying that page code into each app.
5. The actual page has local indents and manual icon composition. Derive any
   dependency indentation and all icon geometry from structure in one renderer.
6. Group fields deliberately omit repeated subject icons. Requiring an icon on
   every row would be a regression; require it only at the hierarchy roles that
   identify a new subject, including the dialog header.
7. All short On/Off, unit, disabled-reason and feedback placements belong to the
   renderer. Authors should not repeat a StateText binding for every switch.
8. A proxy cannot be saved one property at a time. Keep its form draft isolated
   and enforce one complete-value submission. Speed testing remains custom
   workflow content within a common shell, not a large JSON control tree.

## Adaptations rather than accidental inheritance

- The component contains no torrent, libtorrent, service registration or provider
  types. It neither owns an engine process nor persists application preferences.
- Category/context/facts remain shared. The generic contract permits zero to two
  meaningful facts; TinyTorrent currently supplies two per category.
- TinyTorrent's global title-bar search, return-to-Torrents/Library actions,
  category list and startup default belong to its host. SynapseCommander supplies
  its own integration while retaining one search/reveal mechanism.
- Category artwork and optional decorative treatment are shared-shell concerns.
  Torrent branding is not copied into the component. The sample reuses three
  reference artwork assets with their Microsoft MIT license.
- Existing custom drawer/category motion is a reusable visual requirement to
  implement once, respecting Windows animation settings. It is not justification
  for hand-built disclosure on every page.
- The owner confirmed immediate ordinary saves and the proxy exception during
  this work. These replace the inherited goz-style whole-page Apply/Cancel plan.

## Component/dependency direction

Develop `Syno.Settings` and a small non-torrent sample here. Refer to the existing
TableView/Lucide source projects through an explicit development root. Fix table
behavior in its original owner, not a copied implementation here. A second
product can use source project references and ship the compiled libraries,
resources and licenses with its own installer. Verify the consumer project has
no dependency on TinyTorrent application code. The owner subsequently chose
TinyTorrent's installer release model; separate library package publication is
not a prerequisite.

This uses TableView's organizational pattern, not its arbitrary-cell-content
contract: settings need stronger composition constraints to prevent the very
inconsistency the owner wants removed.
