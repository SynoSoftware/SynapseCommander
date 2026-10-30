using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Foundation;

namespace Syno.Settings;

public sealed partial class Header : UserControl
{
    private Category? _category;
    private readonly List<TextBlock> _values = [];

    internal Header(Category category) : this()
    {
        _category = category;
        Title = category.Subject ?? category.Title;
        HeadingLevel = AutomationHeadingLevel.Level1;
        Artwork = new BitmapImage(new Uri(category.Artwork));
        foreach (var fact in category.Facts)
        {
            var lines = new StackPanel { Spacing = 12, VerticalAlignment = VerticalAlignment.Center };
            var label = new TextBlock
            {
                Text = fact.Label, Style = (Style)Application.Current.Resources["BodyStrongTextBlockStyle"],
                TextLineBounds = TextLineBounds.Tight, TextTrimming = TextTrimming.CharacterEllipsis,
                VerticalAlignment = VerticalAlignment.Center
            };
            var value = new TextBlock { Style = (Style)Resources["DetailStyle"], TextTrimming = TextTrimming.CharacterEllipsis };
            ToolTipService.SetToolTip(label, fact.Label);
            lines.Children.Add(label);
            lines.Children.Add(value);
            var layout = new Grid { ColumnSpacing = 12 };
            layout.ColumnDefinitions.Add(new() { Width = GridLength.Auto });
            layout.ColumnDefinitions.Add(new() { Width = new GridLength(1, GridUnitType.Star) });
            var icon = new TextBlock
            {
                FontFamily = Lucide.Font, Text = fact.Icon, FontSize = 24,
                TextLineBounds = TextLineBounds.TrimToBaseline, IsTextScaleFactorEnabled = false,
                HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center,
                Foreground = (Brush)Application.Current.Resources["TextFillColorSecondaryBrush"]
            };
            Microsoft.UI.Xaml.Automation.AutomationProperties.SetAccessibilityView(icon, AccessibilityView.Raw);
            layout.Children.Add(new Grid { Width = 24, Height = 24, VerticalAlignment = VerticalAlignment.Center, Children = { icon } });
            Grid.SetColumn(lines, 1);
            layout.Children.Add(lines);
            if (_values.Count == 0)
                First = layout;
            else
                Second = layout;
            _values.Add(value);
        }
        Refresh();
    }

    internal void Refresh()
    {
        if (_category is null)
            return;
        Subtitle = _category.Context?.Invoke() ?? "";
        for (var index = 0; index < _values.Count; index++)
        {
            _values[index].Text = _category.Facts[index].Read();
            ToolTipService.SetToolTip(_values[index], _values[index].Text);
        }
    }

    public static Visibility HasText(string text) => string.IsNullOrEmpty(text) ? Visibility.Collapsed : Visibility.Visible;
    public static readonly DependencyProperty PageTitleProperty = DependencyProperty.Register(
        nameof(PageTitle), typeof(string), typeof(Header), new PropertyMetadata(string.Empty)
    );
    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        nameof(Title), typeof(string), typeof(Header), new PropertyMetadata(string.Empty)
    );
    public static readonly DependencyProperty SubtitleProperty = DependencyProperty.Register(
        nameof(Subtitle), typeof(string), typeof(Header), new PropertyMetadata(string.Empty)
    );
    public static readonly DependencyProperty FirstProperty = DependencyProperty.Register(
        nameof(First), typeof(FrameworkElement), typeof(Header), new PropertyMetadata(null)
    );
    public static readonly DependencyProperty SecondProperty = DependencyProperty.Register(
        nameof(Second), typeof(FrameworkElement), typeof(Header), new PropertyMetadata(null)
    );
    public static readonly DependencyProperty ArtworkProperty = DependencyProperty.Register(
        nameof(Artwork), typeof(ImageSource), typeof(Header),
        new PropertyMetadata(null)
    );
    public static readonly DependencyProperty HeadingLevelProperty = DependencyProperty.Register(
        nameof(HeadingLevel), typeof(AutomationHeadingLevel), typeof(Header),
        new PropertyMetadata(AutomationHeadingLevel.Level2)
    );

    public ImageSource? Artwork
    {
        get => (ImageSource?)GetValue(ArtworkProperty);
        set => SetValue(ArtworkProperty, value);
    }
    public AutomationHeadingLevel HeadingLevel
    {
        get => (AutomationHeadingLevel)GetValue(HeadingLevelProperty);
        set => SetValue(HeadingLevelProperty, value);
    }
    public string PageTitle
    {
        get => (string)GetValue(PageTitleProperty);
        set => SetValue(PageTitleProperty, value);
    }
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
    public string Subtitle
    {
        get => (string)GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }
    public FrameworkElement? First
    {
        get => (FrameworkElement?)GetValue(FirstProperty);
        set => SetValue(FirstProperty, value);
    }
    public FrameworkElement? Second
    {
        get => (FrameworkElement?)GetValue(SecondProperty);
        set => SetValue(SecondProperty, value);
    }

    public Header()
    {
        InitializeComponent();
    }

    public static bool IsPageHeading(AutomationHeadingLevel level) => level == AutomationHeadingLevel.Level1;

    public bool FocusHeading(FocusState state) =>
        (PageTitleText.Visibility == Visibility.Visible ? PageTitleText : TitleText).Focus(state);

    protected override Size MeasureOverride(Size availableSize)
    {
        var unlimited = new Size(double.PositiveInfinity, double.PositiveInfinity);
        Subject.Measure(unlimited);
        FirstPresenter.Measure(unlimited);
        SecondPresenter.Measure(unlimited);
        var availableWidth = Math.Max(0, availableSize.Width - Padding.Left - Padding.Right);
        var factsWidth = FirstPresenter.DesiredSize.Width + Facts.ColumnSpacing + SecondPresenter.DesiredSize.Width;
        var state = factsWidth > availableWidth ? "StackedFacts"
            : Subject.DesiredSize.Width + Layout.ColumnSpacing + factsWidth > availableWidth ? "FactsBelow"
            : "InlineFacts";
        VisualStateManager.GoToState(this, state, false);
        return base.MeasureOverride(availableSize);
    }
}
