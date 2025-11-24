using System.Text.Json.Serialization;

namespace NameGeneratorEngine.ThemeData.DataStructures;

/// <summary>
/// Represents city name data within a theme.
/// </summary>
internal class CityNameData
{
    /// <summary>
    /// Gets the array of prefix syllables for city names.
    /// </summary>
    [JsonPropertyName("prefixes")]
    public required string[] Prefixes { get; init; }

    /// <summary>
    /// Gets the array of core syllables for city names.
    /// </summary>
    [JsonPropertyName("cores")]
    public required string[] Cores { get; init; }

    /// <summary>
    /// Gets the array of suffix syllables for city names.
    /// </summary>
    [JsonPropertyName("suffixes")]
    public required string[] Suffixes { get; init; }
}
