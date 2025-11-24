namespace NameGeneratorEngine.Enums;

/// <summary>
/// Represents the category of named entity.
/// </summary>
public enum EntityType
{
    /// <summary>
    /// Non-Player Character entity.
    /// </summary>
    Npc,

    /// <summary>
    /// Building entity.
    /// </summary>
    Building,

    /// <summary>
    /// City entity.
    /// </summary>
    City,

    /// <summary>
    /// District entity (subdivision of a city).
    /// </summary>
    District,

    /// <summary>
    /// Street entity (thoroughfare).
    /// </summary>
    Street,

    /// <summary>
    /// Faction entity (organization, group, or alliance).
    /// </summary>
    Faction
}
