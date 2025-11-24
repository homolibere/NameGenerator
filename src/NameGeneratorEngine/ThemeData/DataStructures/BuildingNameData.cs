using System.Collections.Generic;
using System.Text.Json.Serialization;
using NameGeneratorEngine.Enums;

namespace NameGeneratorEngine.ThemeData.DataStructures;

/// <summary>
/// Represents building name data for all building types within a theme.
/// </summary>
internal class BuildingNameData
{
    /// <summary>
    /// Gets the dictionary mapping building types to their specific name data.
    /// </summary>
    [JsonPropertyName("typeData")]
    public required Dictionary<BuildingType, BuildingTypeData> TypeData { get; init; }

    /// <summary>
    /// Gets the array of generic prefixes used when no specific building type is provided.
    /// </summary>
    [JsonPropertyName("genericPrefixes")]
    public required string[] GenericPrefixes { get; init; }

    /// <summary>
    /// Gets the array of generic suffixes used when no specific building type is provided.
    /// </summary>
    [JsonPropertyName("genericSuffixes")]
    public required string[] GenericSuffixes { get; init; }
}
