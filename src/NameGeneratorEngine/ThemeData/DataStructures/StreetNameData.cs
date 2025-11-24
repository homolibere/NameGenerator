using System.Text.Json.Serialization;

namespace NameGeneratorEngine.ThemeData.DataStructures;

/// <summary>
/// Represents street name data within a theme.
/// </summary>
internal class StreetNameData
{
    /// <summary>
    /// Gets the array of prefix syllables for street names.
    /// </summary>
    [JsonPropertyName("prefixes")]
    required public string[] Prefixes { get; init; }

    /// <summary>
    /// Gets the array of core syllables for street names.
    /// </summary>
    [JsonPropertyName("cores")]
    required public string[] Cores { get; init; }

    /// <summary>
    /// Gets the array of street suffix terms (e.g., "Street", "Avenue", "Boulevard").
    /// </summary>
    [JsonPropertyName("streetSuffixes")]
    required public string[] StreetSuffixes { get; init; }
}
