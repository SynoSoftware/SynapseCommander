using Microsoft.UI.Xaml.Controls;

namespace SettingsSample;

public sealed record Shortcut(string Command, string Keys);

public sealed partial class Shortcuts : UserControl
{
    public Shortcuts()
    {
        InitializeComponent();
        Rows.ItemsSource = new Shortcut[] {
            new("Rename", "F2"), new("View", "F3"), new("Copy to folder", "F5"),
            new("Move to folder", "F6"), new("New folder", "F7")
        };
    }
}
