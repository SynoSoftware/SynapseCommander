using System.Runtime.InteropServices;
using Microsoft.UI.Xaml;
using Windows.Graphics;

namespace SettingsSample;

public sealed partial class MainWindow : Window
{
    private readonly MainPage _page;
    private readonly Store _store;
    private bool _closing;

    [DllImport("user32.dll")]
    private static extern uint GetDpiForWindow(IntPtr window);

    public MainWindow(Store store)
    {
        InitializeComponent();
        _store = store;
        Title = "Syno.Settings — component sample";
        AppTitleBar.Title = "Settings component";
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);
        AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets", "AppIcon.ico"));
        var scale = GetDpiForWindow(WinRT.Interop.WindowNative.GetWindowHandle(this)) / 96.0;
        AppWindow.Resize(new SizeInt32((int)(1240 * scale), (int)(860 * scale)));
        _page = new MainPage(store);
        RootFrame.Content = _page;
        _store.Changed += OnPreferences;
        Closed += (_, _) => _store.Changed -= OnPreferences;
        OnPreferences(this, EventArgs.Empty);
        AppWindow.Closing += async (_, args) =>
        {
            if (_closing)
                return;
            args.Cancel = true;
            await _page.PrepareClose();
            _closing = true;
            Close();
        };
    }

    private void OnPreferences(object? sender, EventArgs args) =>
        Root.RequestedTheme = _store.Current.Theme switch {
            "light" => ElementTheme.Light, "dark" => ElementTheme.Dark, _ => ElementTheme.Default
        };
}
