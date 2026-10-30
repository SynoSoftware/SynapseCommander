using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
using SystemTask = System.Threading.Tasks.Task;

namespace Syno.Settings;

public sealed partial class View
{
    private async SystemTask Edit(ITransaction transaction)
    {
        if (_dialogOpen)
            return;
        _dialogOpen = true;
        var sources = new List<Value>();
        var rows = new List<RowView>();
        void Refresh(object? sender, EventArgs args)
        {
            foreach (var row in rows)
                row.Refresh();
        }
        try
        {
            var editor = transaction.CreateEditor();
            var panel = new StackPanel();
            var issue = new InfoBar { IsOpen = false, Severity = InfoBarSeverity.Error };
            foreach (var row in editor.Rows)
            {
                var view = new RowView(row);
                rows.Add(view);
                sources.Add(row.Source!);
                panel.Children.Add(view);
                row.Source!.Changed += Refresh;
            }
            panel.Children.Add(issue);
            var dialog = new ContentDialog
            {
                XamlRoot = XamlRoot, Title = Subject(transaction.Icon, new TextBlock {
                    Text = editor.Title, FontSize = 20, FontWeight = Microsoft.UI.Text.FontWeights.SemiBold
                }), Content = panel,
                PrimaryButtonText = "Save", CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary
            };
            dialog.Resources["ContentDialogMaxWidth"] = 760d;
            AutomationProperties.SetAutomationId(dialog, "SettingsDialog");
            var saving = false;
            dialog.Closing += (_, args) => args.Cancel = saving;
            dialog.PrimaryButtonClick += async (_, args) =>
            {
                var deferral = args.GetDeferral();
                saving = true;
                dialog.IsPrimaryButtonEnabled = false;
                try
                {
                    var valid = true;
                    foreach (var row in rows)
                        valid &= row.Commit();
                    args.Cancel = !valid || !await editor.Save();
                    issue.Message = editor.Error ?? "Check the highlighted values.";
                    issue.IsOpen = args.Cancel;
                }
                catch (Exception error)
                {
                    args.Cancel = true;
                    issue.Message = error.Message;
                    issue.IsOpen = true;
                }
                finally
                {
                    saving = false;
                    dialog.IsPrimaryButtonEnabled = true;
                    deferral.Complete();
                }
            };
            await dialog.ShowAsync();
        }
        catch (Exception error)
        {
            Failure.Message = error.Message;
            Failure.IsOpen = true;
        }
        finally
        {
            foreach (var source in sources)
                source.Changed -= Refresh;
            _dialogOpen = false;
        }
    }
}
