using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Foundation;

namespace Syno.Settings;

internal sealed partial class Header : UserControl
{
    private readonly Category _category;
    private readonly List<TextBlock> _values = [];

    public Header(Category category)
    {
        InitializeComponent();
        _category = category;
        Title.Text = category.Title;
        ToolTipService.SetToolTip(Title, category.Title);
        Artwork.Source = new BitmapImage(new Uri(category.Artwork));
        SubjectTitle.Text = category.Subject ?? "";
        Context.Visibility = string.IsNullOrEmpty(category.Subject) ? Visibility.Collapsed : Visibility.Visible;
        foreach (var fact in category.Facts)
        {
            var lines = new StackPanel { Spacing = 12, VerticalAlignment = VerticalAlignment.Center };
            var label = new TextBlock { Text = fact.Label, FontSize = 14, FontWeight = Microsoft.UI.Text.FontWeights.SemiBold };
            var value = new TextBlock { FontSize = 14, TextTrimming = TextTrimming.CharacterEllipsis };
            ToolTipService.SetToolTip(label, fact.Label);
            lines.Children.Add(label);
            lines.Children.Add(value);
            var layout = new Grid { ColumnSpacing = 12, MinWidth = 150 };
            layout.ColumnDefinitions.Add(new() { Width = GridLength.Auto });
            layout.ColumnDefinitions.Add(new() { Width = new GridLength(1, GridUnitType.Star) });
            var icon = new FontIcon { FontFamily = Lucide.Font, Glyph = fact.Icon, FontSize = 24, VerticalAlignment = VerticalAlignment.Center };
            Microsoft.UI.Xaml.Automation.AutomationProperties.SetAccessibilityView(icon,
                Microsoft.UI.Xaml.Automation.Peers.AccessibilityView.Raw);
            layout.Children.Add(icon);
            Grid.SetColumn(lines, 1);
            layout.Children.Add(lines);
            Grid.SetColumn(layout, _values.Count);
            Facts.Children.Add(layout);
            _values.Add(value);
        }
        Refresh();
    }

    internal void Refresh()
    {
        SubjectValue.Text = _category.Context?.Invoke() ?? "";
        ToolTipService.SetToolTip(SubjectValue, SubjectValue.Text);
        for (var index = 0; index < _values.Count; index++)
        {
            _values[index].Text = _category.Facts[index].Read();
            ToolTipService.SetToolTip(_values[index], _values[index].Text);
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var unlimited = new Size(double.PositiveInfinity, double.PositiveInfinity);
        Subject.Measure(unlimited);
        var widths = Facts.Children.Cast<FrameworkElement>().Select(child =>
        {
            child.Measure(unlimited);
            return child.DesiredSize.Width;
        }).ToArray();
        var total = widths.Sum() + Math.Max(0, widths.Length - 1) * Facts.ColumnSpacing;
        var stacked = total > availableSize.Width;
        var below = stacked || total + Subject.DesiredSize.Width + Layout.ColumnSpacing > availableSize.Width;
        Grid.SetColumn(Facts, below ? 0 : 1);
        Grid.SetRow(Facts, below ? 1 : 0);
        Grid.SetColumnSpan(Facts, below ? 2 : 1);
        Grid.SetColumnSpan(Subject, below ? 2 : 1);
        for (var index = 0; index < Facts.Children.Count; index++)
        {
            Grid.SetColumn((FrameworkElement)Facts.Children[index], stacked ? 0 : index);
            Grid.SetRow((FrameworkElement)Facts.Children[index], stacked ? index : 0);
        }
        return base.MeasureOverride(availableSize);
    }
}
