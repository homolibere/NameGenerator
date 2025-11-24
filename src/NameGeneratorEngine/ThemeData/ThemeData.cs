using System.Text.Json.Serialization;
using NameGeneratorEngine.Enums;
using NameGeneratorEngine.ThemeData.DataStructures;

namespace NameGeneratorEngine.ThemeData;

/// <summary>
/// Represents all name generation data for a specific theme.
/// </summary>
internal class ThemeData
{
    /// <summary>
    /// Gets the theme this data represents.
    /// </summary>
    [JsonPropertyName("theme")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required Theme Theme { get; init; }

    /// <summary>
    /// Gets the NPC name data for this theme.
    /// </summary>
    [JsonPropertyName("npcNames")]
    public required NpcNameData NpcNames { get; init; }

    /// <summary>
    /// Gets the building name data for this theme.
    /// </summary>
    [JsonPropertyName("buildingNames")]
    public required BuildingNameData BuildingNames { get; init; }

    /// <summary>
    /// Gets the city name data for this theme.
    /// </summary>
    [JsonPropertyName("cityNames")]
    public required CityNameData CityNames { get; init; }

    /// <summary>
    /// Gets the district name data for this theme.
    /// </summary>
    [JsonPropertyName("districtNames")]
    public required DistrictNameData DistrictNames { get; init; }

    /// <summary>
    /// Gets the street name data for this theme.
    /// </summary>
    [JsonPropertyName("streetNames")]
    public required StreetNameData StreetNames { get; init; }
}
