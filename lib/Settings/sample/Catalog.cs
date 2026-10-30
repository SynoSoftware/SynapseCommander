using Syno;
using Syno.Settings;

namespace SettingsSample;

internal static class Examples
{
    internal static Catalog Create(Store store)
    {
        var keepRunning = store.Bind("app.keepRunning", state => state.KeepRunning, (state, value) => state with { KeepRunning = value });
        var showIcons = store.Bind("app.showIcons", state => state.ShowIcons, (state, value) => state with { ShowIcons = value });
        var search = store.Bind("search.asYouType", state => state.SearchAsYouType, (state, value) => state with { SearchAsYouType = value });
        var policy = store.Bind("copy.existingFiles", state => state.ExistingFiles, (state, value) => state with { ExistingFiles = value });
        var transfers = store.Bind("copy.transfers", state => state.Transfers, (state, value) => state with { Transfers = value }, value => value is >= 1 and <= 3 ? null : "Enter a number from 1 to 3.");
        var theme = store.Bind("app.theme", state => state.Theme, (state, value) => state with { Theme = value });
        var diagnostics = store.Bind("app.diagnostics", state => state.Diagnostics, (state, value) => state with { Diagnostics = value });
        var proxy = store.Bind("network.proxy", state => state.Connection ?? new Proxy(),
            (state, value) => state with { Connection = value }, value =>
                value.Type != "None" && string.IsNullOrWhiteSpace(value.Address) ? "Enter the proxy address."
                : value.Port is < 1 or > 65535 ? "Enter a port from 1 to 65535." : null);

        var proxyForm = new Form<Proxy>("Edit proxy", draft =>
        {
            string? Disabled() => draft.Current.Type == "None" ? "Choose a proxy type first." : null;
            return [
                new Choice<string>(draft.Field("proxy.type", value => value.Type, (value, type) => value with { Type = type }),
                    "Type", [new("None", "None"), new("HTTP", "HTTP"), new("SOCKS5", "SOCKS5")]),
                new Syno.Settings.Text(draft.Field("proxy.address", value => value.Address,
                    (value, address) => value with { Address = address }, disabled: Disabled), "Address"),
                new Number(draft.Field("proxy.port", value => value.Port, (value, port) => value with { Port = port },
                    port => port is >= 1 and <= 65535 ? null : "Enter a port from 1 to 65535.", Disabled), "Port")
            ];
        });

        return new([
            new Category("general", "General", Lucide.Settings,
                "ms-appx:///Assets/Settings/general.png",
                [new("Closing", Lucide.PanelTop, () => keepRunning.Confirmed ? "Keep running" : "Exit"),
                 new("Search", Lucide.Search, () => search.Confirmed ? "As you type" : "Press Enter")],
                [
                    new Group("general.startup", "Startup and closing", [
                        new Toggle(keepRunning, "Keep running in the notification area")
                            { Description = "Closing the window keeps accepted operations running." },
                        new Toggle(showIcons, "Show file icons")]),
                    new Card("general.search", Lucide.Search, new Toggle(search, "Search as you type")),
                    new Transaction<Proxy>("general.proxy", "Proxy server", Lucide.Globe, proxy,
                        value => value.Type == "None" ? "Off" : $"{value.Type} · {value.Address}:{value.Port}", proxyForm),
                    new Workflow("general.keyboard", "Keyboard shortcuts", Lucide.Keyboard, "shortcuts")
                ]),
            new Category("transfers", "File operations", Lucide.Files,
                "ms-appx:///Assets/Settings/transfers.png",
                [new("Existing files", Lucide.Copy, () => policy.Confirmed == "changed" ? "Update changed files" : "Keep both"),
                 new("Per device", Lucide.Gauge, () => transfers.Confirmed.ToString())],
                [
                    new Syno.Settings.Expander("copy.options", "Copying", Lucide.Copy,
                        new Detail.Summary(() => $"{(policy.Confirmed == "changed" ? "Update changed files" : "Keep both")} · {transfers.Confirmed} per device"),
                        [new Choice<string>(policy, "Existing files", [new("Update changed files", "changed"), new("Keep both", "both")]),
                         new Number(transfers, "Transfers per device")]),
                    new Card("copy.diagnostics", Lucide.Activity,
                        new Toggle(diagnostics, "Record diagnostic details")) { IsAdvanced = true }
                ]),
            new Category("appearance", "Appearance", Lucide.Palette,
                "ms-appx:///Assets/Settings/appearance.png",
                [new("Theme", Lucide.Palette, () => theme.Confirmed switch { "light" => "Light", "dark" => "Dark", _ => "Windows default" }),
                 new("Language", Lucide.Languages, () => "English")],
                [new Card("appearance.theme", Lucide.Palette,
                    new Choice<string>(theme, "Theme", [new("Windows default", "system"), new("Light", "light"), new("Dark", "dark")]))])
        ]);
    }
}
