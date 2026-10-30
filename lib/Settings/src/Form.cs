using SystemTask = System.Threading.Tasks.Task;

namespace Syno.Settings;

public sealed record Form<T>(string Title, Func<Draft<T>, IReadOnlyList<Row>> Fields);

public sealed class Draft<T>(T initial)
{
    internal object Identity { get; } = new();
    public T Current { get; private set; } = initial;

    public Value<TField> Field<TField>(string key, Func<T, TField> read,
        Func<T, TField, T> replace, Func<TField, string?>? validate = null,
        Func<string?>? disabled = null) =>
        new(key, read(Current), value =>
        {
            Current = replace(Current, value);
            return System.Threading.Tasks.Task.FromResult(value);
        }, validate) { Draft = Identity, Disabled = disabled };
}

internal interface ITransaction
{
    string Title { get; }
    string Icon { get; }
    string Summary { get; }
    Value Source { get; }
    IEditor CreateEditor();
}

internal interface IEditor
{
    string Title { get; }
    IReadOnlyList<Row> Rows { get; }
    string? Error { get; }
    System.Threading.Tasks.Task<bool> Save();
}

internal sealed class Editor<T> : IEditor
{
    private readonly Value<T> _value;
    private readonly Draft<T> _draft;
    public string Title { get; }
    public IReadOnlyList<Row> Rows { get; }
    public string? Error => _value.Error;

    public Editor(Value<T> value, Form<T> form)
    {
        _value = value;
        _draft = new(value.Confirmed);
        Title = form.Title;
        Rows = form.Fields(_draft);
        if (string.IsNullOrWhiteSpace(Title) || Rows.Count == 0)
            throw new ArgumentException("An editor needs a title and fields.");
        var keys = new HashSet<string>(StringComparer.Ordinal);
        foreach (var row in Rows)
        {
            if (row.Source?.Draft != _draft.Identity)
                throw new ArgumentException("Dialog fields must bind to their own draft, never live settings.");
            if (!keys.Add(row.Key))
                throw new ArgumentException($"Duplicate dialog field '{row.Key}'.");
        }
    }

    public async System.Threading.Tasks.Task<bool> Save()
    {
        await SystemTask.WhenAll(Rows.Select(row => row.Source!.Drain));
        if (Rows.Any(row => row.Source!.Error is not null))
            return false;
        await _value.Submit(_draft.Current);
        return _value.Error is null;
    }
}
