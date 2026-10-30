using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.UI;
using SystemTask = System.Threading.Tasks.Task;
using NativeExpander = Microsoft.UI.Xaml.Controls.Expander;

namespace Syno.Settings;

public sealed partial class View : UserControl
{
    private Catalog? _catalog;
    private IReadOnlyDictionary<string, Func<FrameworkElement>> _tasks = new Dictionary<string, Func<FrameworkElement>>();
    private readonly List<RowView> _rows = [];
    private readonly Dictionary<string, FrameworkElement> _targets = [];
    private readonly Dictionary<string, NativeExpander> _expanders = [];
    private readonly Dictionary<string, bool> _expanded = [];
    private readonly Dictionary<string, double> _positions = [];
    private readonly List<Value> _sources = [];
    private readonly List<Action> _refresh = [];
    private readonly Dictionary<string, NavigationViewItem> _categories = [];
    private Header? _header;
    private Category? _category;
    private bool _advanced;
    private bool _attached;
    private bool _dialogOpen;

    public View()
    {
        InitializeComponent();
        Advanced.Icon = Icon(Lucide.Square, 24);
        Loaded += (_, _) => Attach();
        Unloaded += (_, _) => Detach();
    }

    public void Configure(Catalog catalog, IReadOnlyDictionary<string, Func<FrameworkElement>>? tasks = null)
    {
        tasks ??= new Dictionary<string, Func<FrameworkElement>>();
        catalog.Validate(tasks.Keys.ToHashSet(StringComparer.Ordinal));
        Detach();
        _catalog = catalog;
        _tasks = tasks;
        _sources.Clear();
        _sources.AddRange(catalog.Categories.SelectMany(category => category.Blocks)
            .SelectMany(block => block is ITransaction transaction ? [transaction.Source] : block.Fields.Select(row => row.Source).OfType<Value>())
            .Distinct());
        Navigation.MenuItems.Clear();
        _categories.Clear();
        foreach (var category in catalog.Categories)
        {
            var item = new NavigationViewItem { Content = category.Title, Icon = Icon(category.Icon, 24), Tag = category };
            AutomationProperties.SetAutomationId(item, "category." + category.Key);
            _categories.Add(category.Key, item);
            Navigation.MenuItems.Add(item);
        }
        if (IsLoaded)
            Attach();
        Navigation.SelectedItem = _categories[catalog.Categories[0].Key];
    }

    public SystemTask Drain() => SystemTask.WhenAll(_sources.Select(source => source.Drain));

    public async SystemTask PrepareClose()
    {
        foreach (var row in _rows)
            row.Commit();
        await Drain();
    }

    private void Attach()
    {
        if (_attached)
            return;
        foreach (var source in _sources)
        {
            source.Changed += OnValue;
            source.Failed += OnFailure;
        }
        _attached = true;
    }

    private void Detach()
    {
        foreach (var source in _sources)
        {
            source.Changed -= OnValue;
            source.Failed -= OnFailure;
        }
        _attached = false;
    }

    private void OnValue(object? sender, EventArgs args) => DispatcherQueue.TryEnqueue(() =>
    {
        foreach (var row in _rows)
            row.Refresh();
        _header?.Refresh();
        foreach (var update in _refresh)
            update();
    });

    private void OnFailure(object? sender, Exception error) => DispatcherQueue.TryEnqueue(() =>
    {
        Failure.Message = error.Message;
        Failure.IsOpen = true;
    });

    private void OnCategory(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItem is not NavigationViewItem { Tag: Category category })
            return;
        foreach (var row in _rows)
            row.Commit();
        if (_category is not null)
            _positions[_category.Key] = Scroll.VerticalOffset;
        Render(category);
        Scroll.ChangeView(null, _positions.GetValueOrDefault(category.Key), null, true);
    }

    private void Render(Category category)
    {
        _category = category;
        _rows.Clear();
        _targets.Clear();
        _expanders.Clear();
        _refresh.Clear();
        Body.Children.Clear();
        _header = new Header(category);
        Body.Children.Add(_header);
        foreach (var block in category.Blocks.Where(block => _advanced || !block.IsAdvanced))
        {
            var rows = block.Fields.Where(row => _advanced || !row.IsAdvanced).ToArray();
            if (block is not Workflow and not ITransaction && rows.Length == 0)
                continue;
            FrameworkElement surface;
            switch (block)
            {
                case Card card:
                    surface = Card(Rows(rows, card.Icon));
                    break;
                case Group group:
                    var groupLayout = new StackPanel { Spacing = 8 };
                    groupLayout.Children.Add(Heading(group.Title));
                    surface = Card(Rows(rows));
                    groupLayout.Children.Add(surface);
                    Body.Children.Add(groupLayout);
                    Track(block, rows, surface);
                    continue;
                case Expander expander:
                    var heading = new StackPanel { Spacing = 4 };
                    heading.Children.Add(Heading(expander.Title));
                    var detail = new TextBlock { FontSize = 14, TextTrimming = TextTrimming.CharacterEllipsis };
                    heading.Children.Add(detail);
                    var native = new NativeExpander
                    {
                        Header = Subject(expander.Icon, heading), Content = Rows(rows),
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        HorizontalContentAlignment = HorizontalAlignment.Stretch,
                        IsExpanded = _expanded.GetValueOrDefault(block.Key)
                    };
                    void Update()
                    {
                        detail.Text = expander.Detail switch { Detail.Summary summary => summary.Read(), Detail.Consequence consequence => consequence.Text, _ => "" };
                        detail.Visibility = native.IsExpanded && expander.Detail is Detail.Summary ? Visibility.Collapsed : Visibility.Visible;
                        ToolTipService.SetToolTip(detail, detail.Text);
                    }
                    native.Expanding += (_, _) => { _expanded[block.Key] = true; Update(); };
                    native.Collapsed += (_, _) => { _expanded[block.Key] = false; Update(); };
                    _refresh.Add(Update);
                    Update();
                    _expanders[block.Key] = native;
                    surface = native;
                    break;
                case Workflow task:
                    var taskLayout = new StackPanel { Spacing = 16, Margin = new Thickness(16) };
                    taskLayout.Children.Add(Subject(task.Icon, Heading(task.Title)));
                    taskLayout.Children.Add(_tasks[task.Content]());
                    surface = Card(taskLayout);
                    break;
                case ITransaction transaction:
                    var summary = new TextBlock { FontSize = 14, TextTrimming = TextTrimming.CharacterEllipsis };
                    var transactionHeading = new StackPanel { Spacing = 4 };
                    transactionHeading.Children.Add(Heading(transaction.Title));
                    transactionHeading.Children.Add(summary);
                    var transactionLayout = new Grid { Padding = new Thickness(16), ColumnSpacing = 16 };
                    transactionLayout.ColumnDefinitions.Add(new() { Width = new GridLength(1, GridUnitType.Star) });
                    transactionLayout.ColumnDefinitions.Add(new() { Width = GridLength.Auto });
                    transactionLayout.Children.Add(Subject(transaction.Icon, transactionHeading));
                    var edit = RowView.Button("Edit", Lucide.Pencil, "Edit " + transaction.Title.ToLowerInvariant() + ".");
                    AutomationProperties.SetAutomationId(edit, block.Key + ".edit");
                    edit.Click += async (_, _) => await Edit(transaction);
                    Grid.SetColumn(edit, 1);
                    transactionLayout.Children.Add(edit);
                    void Summary() { summary.Text = transaction.Summary; ToolTipService.SetToolTip(summary, summary.Text); }
                    _refresh.Add(Summary);
                    Summary();
                    surface = Card(transactionLayout);
                    break;
                default:
                    throw new ArgumentException("Unsupported settings block.");
            }
            Body.Children.Add(surface);
            Track(block, rows, surface);
        }
    }

    private void Track(Block block, Row[] rows, FrameworkElement surface)
    {
        AutomationProperties.SetAutomationId(surface, block.Key);
        _targets[block.Key] = surface;
        foreach (var row in rows)
            _targets[row.Key] = surface;
    }

    private StackPanel Rows(IEnumerable<Row> rows, string? icon = null)
    {
        var panel = new StackPanel();
        foreach (var row in rows)
        {
            if (panel.Children.Count > 0)
                panel.Children.Add(new Border { Height = 1, Background = (Brush)Application.Current.Resources["DividerStrokeColorDefaultBrush"] });
            var view = new RowView(row, icon);
            _rows.Add(view);
            panel.Children.Add(view);
        }
        return panel;
    }

    private Border Card(UIElement content) => new() { Style = (Style)Resources["Card"], Child = content };

    private static TextBlock Heading(string text)
    {
        var heading = new TextBlock { Text = text, FontSize = 14, FontWeight = Microsoft.UI.Text.FontWeights.SemiBold, TextTrimming = TextTrimming.CharacterEllipsis, VerticalAlignment = VerticalAlignment.Center };
        AutomationProperties.SetHeadingLevel(heading, Microsoft.UI.Xaml.Automation.Peers.AutomationHeadingLevel.Level2);
        ToolTipService.SetToolTip(heading, text);
        return heading;
    }

    private static FontIcon Icon(string glyph, double size)
    {
        var icon = new FontIcon { FontFamily = Lucide.Font, Glyph = glyph, FontSize = size, VerticalAlignment = VerticalAlignment.Center };
        AutomationProperties.SetAccessibilityView(icon, Microsoft.UI.Xaml.Automation.Peers.AccessibilityView.Raw);
        return icon;
    }

    private static Grid Subject(string glyph, UIElement content)
    {
        var grid = new Grid { ColumnSpacing = 24, MinHeight = 52 };
        grid.ColumnDefinitions.Add(new() { Width = GridLength.Auto });
        grid.ColumnDefinitions.Add(new() { Width = new GridLength(1, GridUnitType.Star) });
        grid.Children.Add(Icon(glyph, 20));
        Grid.SetColumn((FrameworkElement)content, 1);
        grid.Children.Add(content);
        return grid;
    }

    private void OnFooter(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        if (args.InvokedItemContainer != Advanced)
            return;
        foreach (var row in _rows)
            row.Commit();
        _advanced = !_advanced;
        Advanced.Icon = Icon(_advanced ? Lucide.SquareCheck : Lucide.Square, 24);
        if (_category is not null)
            Render(_category);
    }

    private sealed record Destination(Category Category, Block Block, Row? Row)
    {
        public override string ToString() => $"{Category.Title} · {Row?.Label ?? Block.Key}";
    }

    private IEnumerable<Destination> Destinations() => _catalog?.Categories.SelectMany(category =>
        category.Blocks.SelectMany(block => block.Fields.Any()
            ? block.Fields.Select(row => new Destination(category, block, row))
            : [new Destination(category, block, null)])) ?? [];

    private void OnSearch(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason != AutoSuggestionBoxTextChangeReason.UserInput)
            return;
        var words = sender.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        sender.ItemsSource = words.Length == 0 ? [] : Destinations().Where(destination =>
            words.All(word => destination.ToString().Contains(word, StringComparison.CurrentCultureIgnoreCase))).Take(12).ToArray();
    }

    private void OnSuggestion(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        if (args.SelectedItem is Destination destination)
            Reveal(destination.Row?.Key ?? destination.Block.Key);
    }

    public void Reveal(string key)
    {
        var destination = Destinations().FirstOrDefault(item => item.Row?.Key == key || item.Block.Key == key)
            ?? throw new ArgumentException($"Unknown setting '{key}'.");
        if (destination.Block.IsAdvanced || destination.Row?.IsAdvanced == true)
        {
            _advanced = true;
            Advanced.Icon = Icon(Lucide.SquareCheck, 24);
        }
        Navigation.SelectedItem = _categories[destination.Category.Key];
        Render(destination.Category);
        if (_expanders.TryGetValue(destination.Block.Key, out var expander))
            expander.IsExpanded = true;
        DispatcherQueue.TryEnqueue(() =>
        {
            if (_targets.TryGetValue(key, out var target))
                target.StartBringIntoView();
            _rows.FirstOrDefault(row => row.Key == key)?.FocusEditor();
        });
    }
}
