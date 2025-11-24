using System.Text.Json.Serialization;

namespace NameGeneratorEngine.ThemeData.DataStructures;

/// <summary>
/// Represents district name data within a theme.
/// </summary>
internal class DistrictNameData
{
    /// <summary>
    /// Gets the array of descriptive terms for district names.
    /// </summary>
    [JsonPropertyName("descriptors")]
    public required string[] Descriptors { get; init; }

    /// <summary>
    /// Gets the array of location type terms for district names (e.g., "District", "Quarter").
    /// </summary>
    [JsonPropertyName("locationTypes")]
    public required string[] LocationTypes { get; init; }
}
