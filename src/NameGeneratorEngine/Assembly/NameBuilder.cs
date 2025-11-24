using NameGeneratorEngine.Enums;
using NameGeneratorEngine.Foundation;
using NameGeneratorEngine.ThemeData.DataStructures;

namespace NameGeneratorEngine.Assembly;

/// <summary>
/// Assembles names from syllable components according to patterns.
/// </summary>
internal class NameBuilder
{
    private readonly SyllableSelector _selector;

    /// <summary>
    /// Initializes a new instance of the NameBuilder class.
    /// </summary>
    public NameBuilder()
    {
        _selector = new SyllableSelector();
    }

    /// <summary>
    /// Builds an NPC name from gender-specific data.
    /// </summary>
    /// <param name="data">The gender-specific name data.</param>
    /// <param name="random">The seeded random generator.</param>
    /// <returns>A generated NPC name.</returns>
    public string BuildNpcName(GenderNameData data, SeededRandom random)
    {
        string prefix = _selector.SelectFrom(data.Prefixes, random);
        string core = _selector.SelectFrom(data.Cores, random);
        string suffix = _selector.SelectFrom(data.Suffixes, random);

        return prefix + core + suffix;
    }

    /// <summary>
    /// Builds a building name from building-specific data.
    /// </summary>
    /// <param name="data">The building name data.</param>
    /// <param name="type">The optional building type.</param>
    /// <param name="random">The seeded random generator.</param>
    /// <returns>A generated building name.</returns>
    public string BuildBuildingName(BuildingNameData data, BuildingType? type, SeededRandom random)
    {
        if (type.HasValue && data.TypeData.TryGetValue(type.Value, out var typeData))
        {
            // Use type-specific data
            string prefix = _selector.SelectFrom(typeData.Prefixes, random);
            string descriptor = _selector.SelectFrom(typeData.Descriptors, random);
            string suffix = _selector.SelectFrom(typeData.Suffixes, random);

            return prefix + " " + descriptor + suffix;
        }
        else
        {
            // Use generic data
            string prefix = _selector.SelectFrom(data.GenericPrefixes, random);
            string suffix = _selector.SelectFrom(data.GenericSuffixes, random);

            return prefix + suffix;
        }
    }

    /// <summary>
    /// Builds a city name from city-specific data.
    /// </summary>
    /// <param name="data">The city name data.</param>
    /// <param name="random">The seeded random generator.</param>
    /// <returns>A generated city name.</returns>
    public string BuildCityName(CityNameData data, SeededRandom random)
    {
        string prefix = _selector.SelectFrom(data.Prefixes, random);
        string core = _selector.SelectFrom(data.Cores, random);
        string suffix = _selector.SelectFrom(data.Suffixes, random);

        return prefix + core + suffix;
    }

    /// <summary>
    /// Builds a district name from district-specific data.
    /// </summary>
    /// <param name="data">The district name data.</param>
    /// <param name="random">The seeded random generator.</param>
    /// <returns>A generated district name.</returns>
    public string BuildDistrictName(DistrictNameData data, SeededRandom random)
    {
        string descriptor = _selector.SelectFrom(data.Descriptors, random);
        string locationType = _selector.SelectFrom(data.LocationTypes, random);

        return descriptor + " " + locationType;
    }

    /// <summary>
    /// Builds a street name from street-specific data.
    /// </summary>
    /// <param name="data">The street name data.</param>
    /// <param name="random">The seeded random generator.</param>
    /// <returns>A generated street name.</returns>
    public string BuildStreetName(StreetNameData data, SeededRandom random)
    {
        string prefix = _selector.SelectFrom(data.Prefixes, random);
        string core = _selector.SelectFrom(data.Cores, random);
        string streetSuffix = _selector.SelectFrom(data.StreetSuffixes, random);

        return prefix + core + " " + streetSuffix;
    }

    /// <summary>
    /// Builds a faction name from faction-specific data.
    /// </summary>
    /// <param name="data">The faction name data.</param>
    /// <param name="random">The seeded random generator.</param>
    /// <returns>A generated faction name.</returns>
    public string BuildFactionName(FactionNameData data, SeededRandom random)
    {
        string prefix = _selector.SelectFrom(data.Prefixes, random);
        string core = _selector.SelectFrom(data.Cores, random);
        string suffix = _selector.SelectFrom(data.Suffixes, random);

        return prefix + " " + core + suffix;
    }
}
