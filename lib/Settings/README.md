# Syno.Settings

A reusable WinUI 3 settings presentation library, developed in SynapseCommander
before possible adoption by TinyTorrent2. The
[contract](docs/contract.md) owns the common language and presentation rules;
[source research](../../docs/research/tinytorrent-settings.md) records their origin.

## Run the sample

From the repository root:

```powershell
pwsh ./Build.ps1 -ComponentsRoot C:\SynoSoftware\TinyTorrent2 -Launch
```

`ComponentsRoot` explicitly names the existing TableView/Lucide source checkout.
The projects contain no machine-specific default or copied implementation.
Command-line build outputs go into this workspace's ignored `artifacts/` directory.
The sample is a development host, not an additional shipped engine executable.

The sample's host stores its own demonstration preferences at
`%LOCALAPPDATA%\SynapseCommander\SettingsSample\settings.json`. Pass
`-DataDirectory <temporary folder>` for an isolated run. The component itself
does no file I/O. The proxy and file-operation preferences demonstrate editing;
they do not configure a real proxy or connect the Rust engine.

## Visual Studio

Open `SynapseCommander.sln` at the repository root. Select **Debug | x64**,
set **SettingsSample** as the startup project, and use its unpackaged launch
profile. Debug/Release configurations also include ARM64; x64 is the verified
development target.

The solution expects TinyTorrent2 beside SynapseCommander and includes the
original TableView and Lucide projects under Shared components. Their source is
not copied. Visual Studio builds those projects in TinyTorrent2's own ignored
`artifacts/` directory; this solution's projects build into its own `artifacts/`.

The matching dependency path is already configured on this machine in ignored
`Directory.Build.local.props`. On another checkout, copy
`Directory.Build.local.props.example` to that filename and set `ComponentsRoot`
to that sibling TinyTorrent2 checkout. This supplies the same property as the build
script's argument without committing a developer's absolute path.

`Build.ps1` builds the solution and runs the model checks. Its explicit
`ComponentsRoot` argument must identify the same checkout as the solution's
shared projects. It directs all command-line outputs into this workspace.
Visual Studio builds all five projects itself; no preliminary script run is
needed after the local dependency path is configured.

## Declare content

See the real [sample catalog](sample/Catalog.cs). A category is data; it contains
Card, Group, Expander, Transaction and registered Workflow declarations. No entry
sets margins, template resources or control geometry.

```csharp
new Expander("copy.options", "Copying", Lucide.Copy,
    new Detail.Summary(() => confirmedCopySummary),
    [new Choice<Policy>(policy, "Existing files", choices),
     new Number(transfers, "Transfers per device")]);
```

A proxy uses `Transaction<Proxy>` and `Form<Proxy>`. Each form field comes from
`Draft<Proxy>.Field`; construction rejects a live preference accidentally placed
inside that dialog. The shared dialog applies the whole proxy only on Save.
The custom keyboard task embeds the actual `Syno.TableView.Table` control.

## Implementation status

The first implementation includes typed declarations, catalog validation,
cards/groups/native expanders, category artwork/facts, ordinary field editors,
individual serialized saves, isolated transaction forms, search/reveal routing
and registered rich task content. The sample includes the real TableView.

The complete contract also calls for shared adaptive row layout, localization
integration, full reveal outlining, role-token consolidation, host notification
handoff, global-search integration, richer workflow navigation/action slots,
shared dialog button icons, theme previews, and the remaining decorative
treatment. These are
not established by the first sample. Hardware measurement, speed testing and
the production file-manager engine are outside this component trial.

The [component plan](../../docs/plans/settings-component.md) tracks completion.
Build, model checks and rendered verification are reported there after they run.
The model checks compile the exact non-UI source files, with no duplicate model
implementation and no running application/service requirement.

TinyTorrent's expander and category motion are copied with their matching
template/clipping. Advanced mode updates visibility in place and preserves the
existing controls. With a freshly launched sample (Advanced mode off), run
`pwsh ./lib/Settings/tests/Advanced.ps1 -AppPid <sample PID>` from the repository
root. This bounded WinApp UI check activates the footer through Enter, verifies
the advanced row appears/disappears, and checks that editor/expander identities
and expansion survive both toggles. It takes no screenshots or recordings and
does not change stored preferences. SelectionItemPattern alone selects a
navigation item; it does not activate this nonselecting footer command.

## Reuse

Settings owns presentation and commit mechanics; the host supplies typed domain
values, actual validation, commands, localization and persistence. TableView
retains table authority. Consumers use source project references and ship the
compiled libraries/resources with their application installer; separate package
publication is not required. TinyTorrent migration remains a separate step after
the sample satisfies the contract; no TinyTorrent source was changed for this trial.
