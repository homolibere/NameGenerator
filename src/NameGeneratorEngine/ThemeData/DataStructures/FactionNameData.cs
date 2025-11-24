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
    public required string[] Prefixes { get; init; }

    /// <summary>
    /// Gets the core syllables for faction names.
    /// </summary>
    [JsonPropertyName("cores")]
    public required string[] Cores { get; init; }

    /// <summary>
    /// Gets the suffix syllables for faction names.
    /// </summary>
    [JsonPropertyName("suffixes")]
    public required string[] Suffixes { get; init; }
}
