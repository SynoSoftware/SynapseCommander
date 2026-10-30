# Community Toolkit settings options

Reviewed 2026-10-30. Decision: keep the TinyTorrent-based direction. This is an
options report, not approval to replace the implementation or add dependencies.

## What already exists

The Windows Community Toolkit provides `SettingsCard` and `SettingsExpander`
in the focused `CommunityToolkit.WinUI.Controls.SettingsControls` package.
`SettingsCard` supplies header, description, icon, trailing content, clickable
cards and responsive wrapping. `SettingsExpander` groups cards and supports
items, templates and header/footer content. These are useful reusable controls;
the inspected APIs do not supply our complete typed catalog, save lifecycle,
transaction drafts, category overview or rich-workflow contract.

See the [card documentation](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/windows/settingscontrols/settingscard),
[expander documentation](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/windows/settingscontrols/settingsexpander)
and [component source](https://github.com/CommunityToolkit/Windows/tree/main/components/SettingsControls).
The missing integration above is an assessment of these controls, not a claim
that no other library could provide it.

## Options

| Option | Useful gain | Cost and fit | Decision |
| --- | --- | --- | --- |
| Keep TinyTorrent presentation; generalize its app boundary | Preserves the owner's debugged geometry and motion; no new package | We own the extracted styles and controls, and must verify the adaptation | Continue |
| Use Toolkit cards and expanders behind our declarations | Replaces some presentation maintenance with existing responsive controls and automation support | Adds a dependency and migration checks; default geometry and animation differ from TinyTorrent | Reconsider only for a demonstrated maintenance saving |
| Use Toolkit cards while retaining TinyTorrent's expander | Potentially gains card wrapping while preserving drawer motion | Two card compositions must still follow one visual contract; an adapter may erase the saving | Low expected gain now |
| Copy Toolkit control sources | Avoids a runtime package reference | We own its implementation, resources, dependencies and future fixes; copying does not remove complexity | Avoid |

These are engineering judgments. No package-size, performance or accessibility
comparison has been measured, and no Toolkit package has been added.

## Animation is a material difference

The inspected [Toolkit expander template](https://github.com/CommunityToolkit/Windows/blob/main/components/SettingsControls/src/SettingsExpander/SettingsExpander.xaml)
animates translated content using visual-state storyboards: approximately
333 ms to open and 167 ms for the closing translation. TinyTorrent's
`app/src/Controls/SettingsExpander.cs` animates the clipped drawer's height for
260 ms opening and 220 ms closing, handles retargeting and honors the system
animation preference. They are different implementations. Installing Toolkit
does not establish the exact TinyTorrent motion requested by the owner.

Replacing Toolkit's template and motion with ours would retain maintenance
of the part we most need to preserve. That weakens the immediate adoption case.

Category-to-category motion is a separate responsibility. TinyTorrent's
`Motion.Enter` and `SettingsPage.ShowCategory` provide a directional 250-ms
slide. Neither adopting a card nor an expander wires that into our category
navigation. Preserve that implementation too; assessing only the drawer
animation misses part of the working settings experience.

## Dependency cost

Evaluate the SettingsControls package alone, not the whole Toolkit. Its
[package listing](https://www.nuget.org/packages/CommunityToolkit.WinUI.Controls.SettingsControls/)
identifies the two controls. A dependency is not automatically bloat, but a
new abstraction that leaves all our existing rendering in place earns little.
Before adoption, compare resolved dependencies and published files against our
current output, and count which existing implementation can actually be deleted.
Do not invent a size estimate from the package download size.

## How to see it

Install the [Windows Community Toolkit Gallery](https://apps.microsoft.com/detail/9nblggh4tlcq)
and search for SettingsCard and SettingsExpander. The
[repository](https://github.com/CommunityToolkit/Windows) links to that gallery.
The [WinUI Gallery Settings page](https://github.com/microsoft/WinUI-Gallery/blob/main/WinUIGallery/Pages/SettingsPage.xaml)
also uses these controls.

## What would justify revisiting

Use one representative category to prove that Toolkit allows us to remove more
code than its adapter adds, while preserving our icon/text alignment, summaries,
input semantics and required motion. Check narrow layouts, text scaling,
keyboard use and themes in the real host. Keep the existing implementation
until that comparison demonstrates a worthwhile gain; never ship two selectable
renderers for this experiment.

For now, copy the working TinyTorrent presentation and matching animation first,
then generalize only its content and host bindings. The
[component contract](../../lib/Settings/docs/contract.md) remains authoritative.
