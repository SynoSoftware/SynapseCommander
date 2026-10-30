using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace SettingsSample;

public partial class App : Application
{
    private Window? _window;

    public App() => InitializeComponent();

    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        try
        {
            var arguments = Environment.GetCommandLineArgs();
            var option = Array.IndexOf(arguments, "--data-dir");
            var directory = option >= 0 && option + 1 < arguments.Length ? arguments[option + 1]
                : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "SynapseCommander", "SettingsSample");
            _window = new MainWindow(await Store.Open(directory));
        }
        catch (Exception error)
        {
            _window = new Window { Title = "Settings sample",
                Content = new TextBlock { Text = error.Message, Margin = new Thickness(24), TextWrapping = TextWrapping.Wrap } };
        }
        _window.Activate();
    }
}
