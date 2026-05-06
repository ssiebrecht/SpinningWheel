using SpinningWheel.Models;

namespace SpinningWheel.Services;

/// <inheritdoc />
public sealed class NameStore(ILocalStorageService storage) : INameStore
{
    private const string StorageKey = "spinning-wheel.entries.v1";

    private static readonly string[] SegmentTokens =
    [
        "--color-segment-1",
        "--color-segment-2",
        "--color-segment-3",
        "--color-segment-4",
        "--color-segment-5",
        "--color-segment-6",
        "--color-segment-7",
        "--color-segment-8",
    ];

    private readonly List<WheelEntry> _entries = [];

    public IReadOnlyList<WheelEntry> Entries => _entries;

    public event Action? OnChanged;

    public async Task LoadAsync()
    {
        var saved = await storage.GetAsync<List<WheelEntry>>(StorageKey);
        _entries.Clear();
        if (saved is not null)
        {
            _entries.AddRange(saved);
        }
        ReassignColors();
        OnChanged?.Invoke();
    }

    public async Task AddAsync(string name)
    {
        var trimmed = name?.Trim();
        if (string.IsNullOrEmpty(trimmed))
        {
            return;
        }

        var token = SegmentTokens[_entries.Count % SegmentTokens.Length];
        _entries.Add(new WheelEntry(Guid.NewGuid(), trimmed, token));
        await PersistAsync();
    }

    public async Task RemoveAsync(Guid id)
    {
        var idx = _entries.FindIndex(e => e.Id == id);
        if (idx < 0)
        {
            return;
        }
        _entries.RemoveAt(idx);
        ReassignColors();
        await PersistAsync();
    }

    public async Task ClearAsync()
    {
        if (_entries.Count == 0)
        {
            return;
        }
        _entries.Clear();
        await PersistAsync();
    }

    private void ReassignColors()
    {
        for (var i = 0; i < _entries.Count; i++)
        {
            var token = SegmentTokens[i % SegmentTokens.Length];
            if (_entries[i].ColorToken != token)
            {
                _entries[i] = _entries[i] with { ColorToken = token };
            }
        }
    }

    private async Task PersistAsync()
    {
        await storage.SetAsync(StorageKey, _entries);
        OnChanged?.Invoke();
    }
}
