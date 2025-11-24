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
    required public GenderNameData Male { get; init; }

    /// <summary>
    /// Gets the name data for female NPCs.
    /// </summary>
    [JsonPropertyName("female")]
    required public GenderNameData Female { get; init; }

    /// <summary>
    /// Gets the name data for gender-neutral NPCs.
    /// </summary>
    [JsonPropertyName("neutral")]
    required public GenderNameData Neutral { get; init; }
}
