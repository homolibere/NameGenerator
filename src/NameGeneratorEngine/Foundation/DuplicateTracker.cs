using NameGeneratorEngine.Enums;

namespace NameGeneratorEngine.Foundation;

/// <summary>
/// Tracks generated names to prevent duplicates within a session.
/// </summary>
internal class DuplicateTracker
{
    private readonly Dictionary<EntityType, HashSet<string>> _trackedNames = new();

    /// <summary>
    /// Checks if a name is unique for the specified entity type.
    /// </summary>
    /// <param name="entityType">The entity type to check.</param>
    /// <param name="name">The name to check for uniqueness.</param>
    /// <returns>True if the name has not been generated before for this entity type; otherwise, false.</returns>
    public bool IsUnique(EntityType entityType, string name)
    {
        if (!_trackedNames.ContainsKey(entityType))
        {
            return true;
        }

        return !_trackedNames[entityType].Contains(name);
    }

    /// <summary>
    /// Tracks a generated name for the specified entity type.
    /// </summary>
    /// <param name="entityType">The entity type.</param>
    /// <param name="name">The name to track.</param>
    public void Track(EntityType entityType, string name)
    {
        if (!_trackedNames.ContainsKey(entityType))
        {
            _trackedNames[entityType] = new HashSet<string>();
        }

        _trackedNames[entityType].Add(name);
    }

    /// <summary>
    /// Clears all tracked names, allowing previously generated names to be generated again.
    /// </summary>
    public void Clear()
    {
        _trackedNames.Clear();
    }

    /// <summary>
    /// Gets the number of names tracked for a specific entity type.
    /// </summary>
    /// <param name="entityType">The entity type.</param>
    /// <returns>The count of tracked names for the entity type.</returns>
    public int GetAttemptCount(EntityType entityType)
    {
        if (!_trackedNames.ContainsKey(entityType))
        {
            return 0;
        }

        return _trackedNames[entityType].Count;
    }
}
