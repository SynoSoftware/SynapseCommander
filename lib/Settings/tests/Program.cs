using Syno.Settings;
using SystemTask = System.Threading.Tasks.Task;

static void Check(bool condition, string failure)
{
    if (!condition)
        throw new InvalidOperationException(failure);
}

var first = new TaskCompletionSource<bool>();
var calls = new List<bool>();
var value = new Value<bool>("switch", false, async next =>
{
    calls.Add(next);
    if (calls.Count == 1)
        await first.Task;
    return next;
});
var saving = value.Submit(true);
_ = value.Submit(false);
Check(!value.Current, "An older pending save replaced a newer switch choice.");
first.SetResult(true);
await saving;
Check(calls.SequenceEqual([true, false]) && !value.Confirmed,
    "A rapid return to the original value was lost while saving.");
Console.WriteLine("PASS: rapid choices retain the newest value and commit in order.");

var failure = new TaskCompletionSource<bool>();
var attempts = 0;
var recover = new Value<bool>("recover", false, async next =>
{
    if (++attempts == 1)
        await failure.Task;
    return next;
});
var retrying = recover.Submit(true);
_ = recover.Submit(false);
failure.SetException(new IOException("The first write failed."));
await retrying;
Check(attempts == 2 && !recover.Confirmed && recover.Error is null,
    "An older failure rejected a newer successful choice.");
Console.WriteLine("PASS: an older failure cannot reject a newer successful choice.");

var writes = 0;
var proxy = new Value<(string Address, int Port)>("proxy", ("old", 80), next =>
{
    writes++;
    return System.Threading.Tasks.Task.FromResult(next);
});
var form = new Form<(string Address, int Port)>("Edit proxy", draft => [
    new Syno.Settings.Text(draft.Field("address", current => current.Address,
        (current, address) => (address, current.Port)), "Address"),
    new Number(draft.Field("port", current => current.Port,
        (current, port) => (current.Address, port)), "Port")
]);
var editor = new Editor<(string Address, int Port)>(proxy, form);
await ((Syno.Settings.Text)editor.Rows[0]).Value.Submit("new");
await ((Number)editor.Rows[1]).Value.Submit(1080);
Check(writes == 0 && proxy.Confirmed == ("old", 80), "Editing a dialog wrote part of a live proxy.");
Check(await editor.Save(), "A valid proxy draft was refused.");
Check(writes == 1 && proxy.Confirmed == ("new", 1080), "A proxy was not saved as one coherent value.");
Console.WriteLine("PASS: a dialog edits only its draft and saves the whole value once.");

var live = new Value<string>("live", "old", next => System.Threading.Tasks.Task.FromResult(next));
try
{
    _ = new Editor<string>(live, new("Bad editor", _ => [new Syno.Settings.Text(live, "Address")]));
    throw new InvalidOperationException("A dialog accepted a live field binding.");
}
catch (ArgumentException) { }
Console.WriteLine("PASS: live settings cannot be used as transaction draft fields.");

var catalog = new Catalog([new Category("general", "General", "icon", "asset", [],
    [new Card("card", "", new Toggle(value, "Enabled"))])]);
try
{
    catalog.Validate(new HashSet<string>());
    throw new InvalidOperationException("A card without an icon was accepted.");
}
catch (ArgumentException) { }
Console.WriteLine("PASS: incomplete presentation metadata is rejected before rendering.");
Console.WriteLine("5 settings contract checks passed.");
