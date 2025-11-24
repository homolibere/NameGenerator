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
    required public Theme Theme { get; init; }

    /// <summary>
    /// Gets the NPC name data for this theme.
    /// </summary>
    [JsonPropertyName("npcNames")]
    required public NpcNameData NpcNames { get; init; }

    /// <summary>
    /// Gets the building name data for this theme.
    /// </summary>
    [JsonPropertyName("buildingNames")]
    required public BuildingNameData BuildingNames { get; init; }

    /// <summary>
    /// Gets the city name data for this theme.
    /// </summary>
    [JsonPropertyName("cityNames")]
    required public CityNameData CityNames { get; init; }

    /// <summary>
    /// Gets the district name data for this theme.
    /// </summary>
    [JsonPropertyName("districtNames")]
    required public DistrictNameData DistrictNames { get; init; }

    /// <summary>
    /// Gets the street name data for this theme.
    /// </summary>
    [JsonPropertyName("streetNames")]
    required public StreetNameData StreetNames { get; init; }

    /// <summary>
    /// Gets the faction name data for this theme.
    /// </summary>
    [JsonPropertyName("factionNames")]
    required public FactionNameData FactionNames { get; init; }
}
