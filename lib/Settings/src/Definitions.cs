namespace Syno.Settings;

public sealed record Catalog(IReadOnlyList<Category> Categories)
{
    public void Validate(IReadOnlySet<string> tasks)
    {
        var keys = new HashSet<string>(StringComparer.Ordinal);
        void Key(string key)
        {
            if (string.IsNullOrWhiteSpace(key) || !keys.Add(key))
                throw new ArgumentException($"A settings key must be nonempty and unique: '{key}'.");
        }
        static void Required(string value, string role)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"Settings {role} is required.");
        }
        if (Categories.Count == 0)
            throw new ArgumentException("A settings catalog needs a category.");
        foreach (var category in Categories)
        {
            Key(category.Key);
            Required(category.Title, "category title");
            Required(category.Icon, "navigation icon");
            Required(category.Artwork, "category artwork");
            if (category.Facts.Count > 2)
                throw new ArgumentException("A category header has at most two facts.");
            foreach (var fact in category.Facts)
            {
                Required(fact.Label, "fact label");
                Required(fact.Icon, "fact icon");
            }
            if (category.Blocks.Count == 0)
                throw new ArgumentException($"Category '{category.Key}' has no content.");
            foreach (var block in category.Blocks)
            {
                Key(block.Key);
                switch (block)
                {
                    case Card card:
                        Required(card.Icon, "card icon");
                        break;
                    case Group group:
                        Required(group.Title, "group title");
                        if (group.Rows.Count < 2)
                            throw new ArgumentException("One setting belongs in a Card, not a Group.");
                        break;
                    case Expander expander:
                        Required(expander.Title, "expander title");
                        Required(expander.Icon, "expander icon");
                        if (expander.Rows.Count < 2)
                            throw new ArgumentException("An expander needs related settings, not one hidden field.");
                        break;
                    case Workflow task:
                        Required(task.Title, "task title");
                        Required(task.Icon, "task icon");
                        if (!tasks.Contains(task.Content))
                            throw new ArgumentException($"Unknown settings task '{task.Content}'.");
                        break;
                    case ITransaction transaction:
                        Required(transaction.Title, "transaction title");
                        Required(transaction.Icon, "transaction icon");
                        break;
                    default:
                        throw new ArgumentException($"Unsupported settings block '{block.GetType().Name}'.");
                }
                foreach (var row in block.Fields)
                {
                    Key(row.Key);
                    Required(row.Label, "field label");
                    if (row.Description?.Trim().TrimEnd('.') == row.Label.Trim().TrimEnd('.'))
                        throw new ArgumentException($"The description of '{row.Key}' repeats its label.");
                    if (row is IChoice choice && choice.Labels.Count == 0)
                        throw new ArgumentException($"Choice '{row.Key}' has no options.");
                    if (row is Command command)
                    {
                        Required(command.Caption, "action caption");
                        Required(command.Icon, "action icon");
                        Required(command.Tooltip ?? "", "action tooltip");
                    }
                }
            }
        }
    }
}

public sealed record Category(string Key, string Title, string Icon, string Artwork,
    IReadOnlyList<Fact> Facts, IReadOnlyList<Block> Blocks)
{
    public string? Subject { get; init; }
    public Func<string>? Context { get; init; }
}

public sealed record Fact(string Label, string Icon, Func<string> Read);

public abstract record Block(string Key)
{
    public bool IsAdvanced { get; init; }
    public abstract IEnumerable<Row> Fields { get; }
}

public sealed record Card(string Key, string Icon, Row Field) : Block(Key)
{
    public override IEnumerable<Row> Fields => [Field];
}

public sealed record Group(string Key, string Title, IReadOnlyList<Row> Rows) : Block(Key)
{
    public override IEnumerable<Row> Fields => Rows;
}

public sealed record Expander(string Key, string Title, string Icon, Detail Detail,
    IReadOnlyList<Row> Rows) : Block(Key)
{
    public override IEnumerable<Row> Fields => Rows;
}

public abstract record Detail
{
    public sealed record Summary(Func<string> Read) : Detail;
    public sealed record Consequence(string Text) : Detail;
}

public sealed record Workflow(string Key, string Title, string Icon, string Content) : Block(Key)
{
    public override IEnumerable<Row> Fields => [];
}

public sealed record Transaction<T>(string Key, string Title, string Icon, Value<T> Value,
    Func<T, string> Describe, Form<T> Form) : Block(Key), ITransaction
{
    public override IEnumerable<Row> Fields => [];
    Value ITransaction.Source => Value;
    string ITransaction.Summary => Describe(Value.Confirmed);
    IEditor ITransaction.CreateEditor() => new Editor<T>(Value, Form);
}

public abstract record Row(string Key, string Label)
{
    public string? Description { get; init; }
    public string? Tooltip { get; init; }
    public bool IsAdvanced { get; init; }
    public abstract Value? Source { get; }
}

public sealed record Toggle(Value<bool> Value, string Label) : Row(Value.Key, Label)
{
    public override Value Source => Value;
}

public sealed record Number(Value<int> Value, string Label, string Unit = "") : Row(Value.Key, Label)
{
    public override Value Source => Value;
}

public sealed record Text(Value<string> Value, string Label) : Row(Value.Key, Label)
{
    public override Value Source => Value;
}

public sealed record Folder(Value<string> Value, string Label,
    Func<System.Threading.Tasks.Task<string?>> Browse) : Row(Value.Key, Label)
{
    public override Value Source => Value;
}

public sealed record Command(string Key, string Label, string Caption, string Icon,
    Func<System.Threading.Tasks.Task> Run) : Row(Key, Label)
{
    public override Value? Source => null;
}

public sealed record Option<T>(string Label, T Value);

internal interface IChoice
{
    IReadOnlyList<string> Labels { get; }
    int Selected { get; }
    System.Threading.Tasks.Task Select(int index);
}

public sealed record Choice<T>(Value<T> Value, string Label,
    IReadOnlyList<Option<T>> Options) : Row(Value.Key, Label), IChoice
{
    public override Value Source => Value;
    IReadOnlyList<string> IChoice.Labels => Options.Select(option => option.Label).ToArray();
    int IChoice.Selected => Options.ToList().FindIndex(option => EqualityComparer<T>.Default.Equals(option.Value, Value.Current));
    System.Threading.Tasks.Task IChoice.Select(int index) => Value.Submit(Options[index].Value);
}
