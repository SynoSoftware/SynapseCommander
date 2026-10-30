using SystemTask = System.Threading.Tasks.Task;

namespace Syno.Settings;

public abstract class Value(string key)
{
    internal object? Draft { get; init; }
    public string Key { get; } = key;
    public Func<string?>? Disabled { get; init; }
    public string? Reason => Disabled?.Invoke();
    public string? Error { get; protected set; }
    public event EventHandler? Changed;
    public event EventHandler<Exception>? Failed;
    protected void Change() => Changed?.Invoke(this, EventArgs.Empty);
    protected void Fail(Exception error) => Failed?.Invoke(this, error);
    public abstract SystemTask Drain { get; }
}

public sealed class Value<T>(string key, T initial, Func<T, System.Threading.Tasks.Task<T>> commit,
    Func<T, string?>? validate = null) : Value(key)
{
    private T _confirmed = initial;
    private T _requested = initial;
    private bool _hasNext;
    private bool _running;
    private long _revision;
    private SystemTask _drain = SystemTask.CompletedTask;

    public T Confirmed => _confirmed;
    public T Current => _running ? _requested : _confirmed;
    public override SystemTask Drain => _drain;

    public SystemTask Submit(T value)
    {
        if (Reason is { } reason)
        {
            Error = reason;
            Change();
            return SystemTask.CompletedTask;
        }
        Error = validate?.Invoke(value);
        if (Error is not null)
        {
            Change();
            return SystemTask.CompletedTask;
        }
        if (!_running && EqualityComparer<T>.Default.Equals(value, _confirmed))
        {
            Change();
            return SystemTask.CompletedTask;
        }
        _requested = value;
        _hasNext = true;
        _revision++;
        if (!_running)
        {
            _running = true;
            _drain = Save();
        }
        Change();
        return _drain;
    }

    private async SystemTask Save()
    {
        try
        {
            while (_hasNext)
            {
                var sent = _requested;
                var revision = _revision;
                _hasNext = false;
                try
                {
                    _confirmed = await commit(sent);
                    if (revision == _revision)
                        Error = null;
                }
                catch (Exception error)
                {
                    if (revision == _revision)
                        Error = error.Message;
                    Fail(error);
                }
                Change();
            }
        }
        finally
        {
            _running = false;
            Change();
        }
    }
}
