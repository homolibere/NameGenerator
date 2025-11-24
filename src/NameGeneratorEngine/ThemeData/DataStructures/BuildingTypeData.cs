using System.Text.Json.Serialization;

namespace NameGeneratorEngine.ThemeData.DataStructures;

/// <summary>
/// Represents name data for a specific building type.
/// </summary>
internal class BuildingTypeData
{
    /// <summary>
    /// Gets the array of prefix syllables for building names.
    /// </summary>
    [JsonPropertyName("prefixes")]
    required public string[] Prefixes { get; init; }

    /// <summary>
    /// Gets the array of descriptive terms for building names.
    /// </summary>
    [JsonPropertyName("descriptors")]
    required public string[] Descriptors { get; init; }

    /// <summary>
    /// Gets the array of suffix syllables for building names.
    /// </summary>
    [JsonPropertyName("suffixes")]
    required public string[] Suffixes { get; init; }
}
