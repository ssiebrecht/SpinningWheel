using SpinningWheel.Models;

namespace SpinningWheel.Services;

/// <summary>
/// Reactive in-memory store for wheel entries with local-storage persistence.
/// </summary>
public interface INameStore
{
    IReadOnlyList<WheelEntry> Entries { get; }

    /// <summary>Fired whenever <see cref="Entries"/> changes.</summary>
    event Action? OnChanged;

    Task LoadAsync();
    Task AddAsync(string name);
    Task RemoveAsync(Guid id);
    Task ClearAsync();
}
