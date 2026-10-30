using System.Globalization;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.System;
using SystemTask = System.Threading.Tasks.Task;

namespace Syno.Settings;

internal sealed partial class RowView : UserControl
{
    private readonly Row _row;
    private Control _editor = null!;
    private Action? _read;
    private Func<bool>? _commit;
    private bool _updating;
    private bool _composing;
    private bool _dirty;
    private bool _executing;
    private string? _invalid;
    internal string Key => _row.Key;

    public RowView(Row row, string? icon = null)
    {
        InitializeComponent();
        _row = row;
        Label.Text = row.Label;
        ToolTipService.SetToolTip(Label, row.Label);
        Description.Text = row.Description ?? "";
        Description.Visibility = string.IsNullOrEmpty(row.Description) ? Visibility.Collapsed : Visibility.Visible;
        Layout.MinHeight = Description.Visibility == Visibility.Visible ? 64 : 52;
        if (icon is not null)
        {
            Identity.FontFamily = Lucide.Font;
            Identity.Glyph = icon;
            Identity.Visibility = Visibility.Visible;
        }
        CreateEditor();
        AutomationProperties.SetAutomationId(_editor, row.Key);
        AutomationProperties.SetName(_editor, row.Label);
        AutomationProperties.SetHelpText(_editor, row.Description ?? row.Tooltip ?? "");
        Editors.Children.Insert(0, _editor);
        Refresh();
    }

    private void CreateEditor()
    {
        switch (_row)
        {
            case Toggle toggle:
                var control = new ToggleSwitch();
                _editor = control;
                _read = () => control.IsOn = toggle.Value.Current;
                control.Toggled += async (_, _) =>
                {
                    if (!_updating)
                        await toggle.Value.Submit(control.IsOn);
                };
                break;
            case IChoice choice:
                var choices = new ComboBox { ItemsSource = choice.Labels, MinWidth = 180, MaxWidth = 280 };
                _editor = choices;
                _read = () => choices.SelectedIndex = choice.Selected;
                choices.SelectionChanged += async (_, _) =>
                {
                    if (!_updating && choices.SelectedIndex >= 0)
                        await choice.Select(choices.SelectedIndex);
                };
                break;
            case Number number:
                Input(() => number.Value.Current.ToString(CultureInfo.CurrentCulture), text =>
                {
                    if (!int.TryParse(text, NumberStyles.Integer, CultureInfo.CurrentCulture, out var value))
                        return "Enter a whole number.";
                    _ = number.Value.Submit(value);
                    return number.Value.Error;
                }, 120);
                if (number.Unit.Length > 0)
                    Editors.Children.Add(new TextBlock { Text = number.Unit, VerticalAlignment = VerticalAlignment.Center });
                break;
            case Text text:
                Input(() => text.Value.Current, input => { _ = text.Value.Submit(input); return text.Value.Error; }, 240);
                break;
            case Folder folder:
                Input(() => folder.Value.Current, input => { _ = folder.Value.Submit(input); return folder.Value.Error; }, 240);
                var browse = Button("Browse", Lucide.FolderOpen, "Choose a folder.");
                browse.Click += async (_, _) =>
                {
                    try
                    {
                        if (await folder.Browse() is { } path)
                            await folder.Value.Submit(path);
                    }
                    catch (Exception error) { ShowError(error.Message); }
                };
                Editors.Children.Add(browse);
                break;
            case Command command:
                var button = Button(command.Caption, command.Icon, command.Tooltip!);
                _editor = button;
                button.Click += async (_, _) =>
                {
                    _executing = true;
                    Refresh();
                    try { await command.Run(); }
                    catch (Exception error) { ShowError(error.Message); }
                    finally { _executing = false; Refresh(); }
                };
                break;
            default:
                throw new ArgumentException($"Unsupported settings row '{_row.GetType().Name}'.");
        }
    }

    private void Input(Func<string> read, Func<string, string?> write, double width)
    {
        var box = new TextBox { Width = width, Text = read() };
        _editor = box;
        _read = () =>
        {
            if (!_dirty && box.FocusState == FocusState.Unfocused && _invalid is null)
                box.Text = read();
        };
        _commit = () =>
        {
            if (_composing)
                return false;
            if (!_dirty)
                return _row.Source?.Error is null;
            _invalid = write(box.Text);
            _dirty = _invalid is not null;
            Refresh();
            return _invalid is null;
        };
        box.TextCompositionStarted += (_, _) => _composing = true;
        box.TextCompositionEnded += (_, _) => _composing = false;
        box.TextChanged += (_, _) =>
        {
            if (!_updating)
            {
                _dirty = true;
                _invalid = null;
                ShowError(null);
            }
        };
        box.KeyDown += (_, args) =>
        {
            if (_composing)
                return;
            if (args.Key == VirtualKey.Enter)
            {
                Commit();
                args.Handled = true;
            }
            else if (args.Key == VirtualKey.Escape)
            {
                _invalid = null;
                box.Text = read();
                _dirty = false;
                Refresh();
                args.Handled = true;
            }
        };
        box.LostFocus += (_, _) => Commit();
    }

    internal bool Commit() => !_editor.IsEnabled || (_commit?.Invoke() ?? true);
    internal void FocusEditor() => _editor.Focus(FocusState.Programmatic);

    internal void Refresh()
    {
        _updating = true;
        try
        {
            _read?.Invoke();
            var reason = _row.Source?.Reason;
            foreach (var control in Editors.Children.OfType<Control>())
                control.IsEnabled = reason is null && !_executing;
            ToolTipService.SetToolTip(Editors, reason ?? _row.Tooltip);
            ShowError(_invalid ?? _row.Source?.Error);
        }
        finally { _updating = false; }
    }

    private void ShowError(string? message)
    {
        Issue.Text = message ?? "";
        Issue.Visibility = string.IsNullOrEmpty(message) ? Visibility.Collapsed : Visibility.Visible;
        ToolTipService.SetToolTip(Issue, message);
    }

    internal static Button Button(string caption, string glyph, string tooltip)
    {
        var content = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
        content.Children.Add(new FontIcon { FontFamily = Lucide.Font, Glyph = glyph, FontSize = 16 });
        content.Children.Add(new TextBlock { Text = caption });
        var button = new Button { Content = content };
        ToolTipService.SetToolTip(button, tooltip);
        AutomationProperties.SetName(button, tooltip);
        return button;
    }
}
