using System.Text.Json;
using Syno.Settings;

namespace SettingsSample;

public sealed record Proxy(string Type = "None", string Address = "", int Port = 8080);

public sealed record Preferences(bool KeepRunning = false, bool ShowIcons = true,
    bool SearchAsYouType = true, string ExistingFiles = "changed", int Transfers = 1,
    string Theme = "system", bool Diagnostics = false, Proxy? Connection = null);

public sealed class Store
{
    private readonly string _path;
    private readonly SemaphoreSlim _writing = new(1, 1);
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web) { WriteIndented = true };
    public Preferences Current { get; private set; }
    public event EventHandler? Changed;

    private Store(string path, Preferences current) { _path = path; Current = current; }

    public static async Task<Store> Open(string directory)
    {
        var path = Path.Combine(directory, "settings.json");
        var current = File.Exists(path)
            ? JsonSerializer.Deserialize<Preferences>(await File.ReadAllTextAsync(path), Json)
                ?? throw new InvalidDataException("The sample settings file is empty.")
            : new Preferences();
        return new(path, current);
    }

    public Value<T> Bind<T>(string key, Func<Preferences, T> read,
        Func<Preferences, T, Preferences> replace, Func<T, string?>? validate = null) =>
        new(key, read(Current), async value =>
        {
            await _writing.WaitAsync();
            try
            {
                var next = replace(Current, value);
                Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
                var temporary = _path + ".tmp";
                await File.WriteAllTextAsync(temporary, JsonSerializer.Serialize(next, Json));
                File.Move(temporary, _path, true);
                Current = next;
                Changed?.Invoke(this, EventArgs.Empty);
                return read(next);
            }
            finally { _writing.Release(); }
        }, validate);
}
