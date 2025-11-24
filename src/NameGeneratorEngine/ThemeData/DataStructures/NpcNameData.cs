using System.Text.Json.Serialization;

namespace NameGeneratorEngine.ThemeData.DataStructures;

/// <summary>
/// Represents NPC name data for all genders within a theme.
/// </summary>
internal class NpcNameData
{
    /// <summary>
    /// Gets the name data for male NPCs.
    /// </summary>
    [JsonPropertyName("male")]
    public required GenderNameData Male { get; init; }

    /// <summary>
    /// Gets the name data for female NPCs.
    /// </summary>
    [JsonPropertyName("female")]
    public required GenderNameData Female { get; init; }

    /// <summary>
    /// Gets the name data for gender-neutral NPCs.
    /// </summary>
    [JsonPropertyName("neutral")]
    public required GenderNameData Neutral { get; init; }
}
