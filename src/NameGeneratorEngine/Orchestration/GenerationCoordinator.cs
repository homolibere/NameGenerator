using NameGeneratorEngine.Assembly;
using NameGeneratorEngine.Enums;
using NameGeneratorEngine.Exceptions;
using NameGeneratorEngine.ThemeData;

namespace NameGeneratorEngine.Orchestration;

/// <summary>
/// Coordinates the name generation process, managing the flow from request to final name.
/// </summary>
internal class GenerationCoordinator
{
    private const int MaxRetryAttempts = 1000;
    private readonly ThemeProvider _themeProvider;
    private readonly NameBuilder _nameBuilder;

    /// <summary>
    /// Initializes a new instance of the GenerationCoordinator class.
    /// </summary>
    public GenerationCoordinator()
    {
        _themeProvider = new ThemeProvider();
        _nameBuilder = new NameBuilder();
    }

    /// <summary>
    /// Generates a name for the specified entity type and theme with duplicate prevention.
    /// </summary>
    /// <param name="entityType">The type of entity to generate a name for.</param>
    /// <param name="theme">The theme to use for generation.</param>
    /// <param name="sessionState">The current session state.</param>
    /// <param name="parameters">Optional parameters specific to the entity type.</param>
    /// <returns>A unique generated name.</returns>
    /// <exception cref="NamePoolExhaustedException">Thrown when unable to generate a unique name after maximum attempts.</exception>
    public string GenerateName(
        EntityType entityType,
        Theme theme,
        SessionState sessionState,
        Dictionary<string, object>? parameters = null)
    {
        var themeData = _themeProvider.GetThemeData(theme);
        int attempts = 0;

        while (attempts < MaxRetryAttempts)
        {
            attempts++;

            // Generate a candidate name based on entity type
            string candidateName = entityType switch
            {
                EntityType.Npc => GenerateNpcName(themeData, sessionState, parameters),
                EntityType.Building => GenerateBuildingName(themeData, sessionState, parameters),
                EntityType.City => GenerateCityName(themeData, sessionState),
                EntityType.District => GenerateDistrictName(themeData, sessionState),
                EntityType.Street => GenerateStreetName(themeData, sessionState),
                _ => throw new ArgumentException($"Unknown entity type: {entityType}", nameof(entityType))
            };

            // Check if the name is unique for this entity type
            if (sessionState.Tracker.IsUnique(entityType, candidateName))
            {
                sessionState.Tracker.Track(entityType, candidateName);
                return candidateName;
            }
        }

        // If we reach here, we've exhausted all attempts
        throw new NamePoolExhaustedException(entityType, theme, attempts);
    }

    private string GenerateNpcName(
        ThemeData.ThemeData themeData,
        SessionState sessionState,
        Dictionary<string, object>? parameters)
    {
        // Extract gender parameter or randomly select one
        Gender gender;
        if (parameters != null && parameters.TryGetValue("gender", out var genderObj) && genderObj is Gender g)
        {
            gender = g;
        }
        else
        {
            // Randomly select a gender using the seeded random
            var genderValues = Enum.GetValues<Gender>();
            gender = genderValues[sessionState.Random.Next(genderValues.Length)];
        }

        // Get the appropriate gender data
        var genderData = gender switch
        {
            Gender.Male => themeData.NpcNames.Male,
            Gender.Female => themeData.NpcNames.Female,
            Gender.Neutral => themeData.NpcNames.Neutral,
            _ => throw new ArgumentException($"Unknown gender: {gender}")
        };

        return _nameBuilder.BuildNpcName(genderData, sessionState.Random);
    }

    private string GenerateBuildingName(
        ThemeData.ThemeData themeData,
        SessionState sessionState,
        Dictionary<string, object>? parameters)
    {
        // Extract building type parameter (optional)
        BuildingType? buildingType = null;
        if (parameters != null && parameters.TryGetValue("buildingType", out var typeObj) && typeObj is BuildingType bt)
        {
            buildingType = bt;
        }

        return _nameBuilder.BuildBuildingName(themeData.BuildingNames, buildingType, sessionState.Random);
    }

    private string GenerateCityName(ThemeData.ThemeData themeData, SessionState sessionState)
    {
        return _nameBuilder.BuildCityName(themeData.CityNames, sessionState.Random);
    }

    private string GenerateDistrictName(ThemeData.ThemeData themeData, SessionState sessionState)
    {
        return _nameBuilder.BuildDistrictName(themeData.DistrictNames, sessionState.Random);
    }

    private string GenerateStreetName(ThemeData.ThemeData themeData, SessionState sessionState)
    {
        return _nameBuilder.BuildStreetName(themeData.StreetNames, sessionState.Random);
    }
}
