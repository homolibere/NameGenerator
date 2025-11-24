using System.Text.Json.Serialization;

namespace NameGeneratorEngine.ThemeData.DataStructures;

/// <summary>
/// Represents faction name data for a theme.
/// </summary>
internal class FactionNameData
{
    /// <summary>
    /// Gets the prefix syllables for faction names.
    /// </summary>
    [JsonPropertyName("prefixes")]
    required public string[] Prefixes { get; init; }

    /// <summary>
    /// Gets the core syllables for faction names.
    /// </summary>
    [JsonPropertyName("cores")]
    required public string[] Cores { get; init; }

    /// <summary>
    /// Gets the suffix syllables for faction names.
    /// </summary>
    [JsonPropertyName("suffixes")]
    required public string[] Suffixes { get; init; }
}
