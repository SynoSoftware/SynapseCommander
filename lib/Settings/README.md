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
Build outputs are directed into this workspace's ignored `artifacts/` directory.
The sample is a development host, not an additional shipped engine executable.

The sample's host stores its own demonstration preferences at
`%LOCALAPPDATA%\SynapseCommander\SettingsSample\settings.json`. Pass
`-DataDirectory <temporary folder>` for an isolated run. The component itself
does no file I/O. The proxy and file-operation preferences demonstrate editing;
they do not configure a real proxy or connect the Rust engine.

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
shared dialog button icons, theme previews, and TinyTorrent's shared
motion/decoration treatment. These are
not established by the first sample. Hardware measurement, speed testing and
the production file-manager engine are outside this component trial.

The [component plan](../../docs/plans/settings-component.md) tracks completion.
Build, model checks and rendered verification are reported there after they run.
The model checks compile the exact non-UI source files, with no duplicate model
implementation and no running application/service requirement.

## Reuse

Settings owns presentation and commit mechanics; the host supplies typed domain
values, actual validation, commands, localization and persistence. TableView
retains table authority. Package publication and TinyTorrent migration are
separate steps after the sample satisfies the contract; no TinyTorrent source
was changed for this trial.
