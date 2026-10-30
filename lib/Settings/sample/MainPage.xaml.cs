using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace SettingsSample;

public sealed partial class MainPage : Page
{
    public MainPage(Store store)
    {
        InitializeComponent();
        Settings.Configure(Examples.Create(store),
            new Dictionary<string, Func<FrameworkElement>> { ["shortcuts"] = () => new Shortcuts() });
    }

    internal Task PrepareClose() => Settings.PrepareClose();
}
