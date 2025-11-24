using NameGeneratorEngine.Enums;
using NameGeneratorEngine.Orchestration;

namespace NameGeneratorEngine;

/// <summary>
/// Main entry point for procedural name generation with deterministic, seeded randomization.
/// Provides methods to generate names for various entity types (NPCs, buildings, cities, districts, streets)
/// across multiple themes (Cyberpunk, Elves, Orcs) with duplicate prevention within a session.
/// </summary>
/// <remarks>
/// This class is not thread-safe. Each instance should be used by a single thread only.
/// </remarks>
public class NameGenerator
{
    private readonly SessionState _sessionState;
    private readonly GenerationCoordinator _coordinator;

    /// <summary>
    /// Gets the seed value used for this generator instance.
    /// </summary>
    public int Seed => _sessionState.Random.Seed;

    /// <summary>
    /// Initializes a new instance of the NameGenerator class.
    /// </summary>
    /// <param name="seed">Optional seed value for deterministic generation. If null, a random seed is generated.</param>
    public NameGenerator(int? seed = null)
    {
        int actualSeed = seed ?? Random.Shared.Next();
        _sessionState = new SessionState(actualSeed);
        _coordinator = new GenerationCoordinator();
    }

    /// <summary>
    /// Generates an NPC name for the specified theme and optional gender.
    /// </summary>
    /// <param name="theme">The theme to use for name generation.</param>
    /// <param name="gender">Optional gender for the NPC. If null, a random gender is selected deterministically.</param>
    /// <returns>A generated NPC name that is unique within the current session.</returns>
    /// <exception cref="ArgumentException">Thrown when an invalid theme or gender value is provided.</exception>
    /// <exception cref="NamePoolExhaustedException">Thrown when unable to generate a unique name after 1000 attempts.</exception>
    public string GenerateNpcName(Theme theme, Gender? gender = null)
    {
        // Validate theme parameter
        if (!Enum.IsDefined(typeof(Theme), theme))
        {
            throw new ArgumentException(
                $"Invalid theme value: {(int)theme}. Expected values are: {string.Join(", ", Enum.GetNames(typeof(Theme)))}.",
                nameof(theme));
        }

        // Validate gender parameter if provided
        if (gender.HasValue && !Enum.IsDefined(typeof(Gender), gender.Value))
        {
            throw new ArgumentException(
                $"Invalid gender value: {(int)gender.Value}. Expected values are: {string.Join(", ", Enum.GetNames(typeof(Gender)))}.",
                nameof(gender));
        }

        var parameters = gender.HasValue
            ? new Dictionary<string, object> { ["gender"] = gender.Value }
            : null;

        return _coordinator.GenerateName(EntityType.Npc, theme, _sessionState, parameters);
    }

    /// <summary>
    /// Generates a building name for the specified theme and optional building type.
    /// </summary>
    /// <param name="theme">The theme to use for name generation.</param>
    /// <param name="buildingType">Optional building type. If null, a generic building name is generated.</param>
    /// <returns>A generated building name that is unique within the current session.</returns>
    /// <exception cref="ArgumentException">Thrown when an invalid theme or building type value is provided.</exception>
    /// <exception cref="NamePoolExhaustedException">Thrown when unable to generate a unique name after 1000 attempts.</exception>
    public string GenerateBuildingName(Theme theme, BuildingType? buildingType = null)
    {
        // Validate theme parameter
        if (!Enum.IsDefined(typeof(Theme), theme))
        {
            throw new ArgumentException(
                $"Invalid theme value: {(int)theme}. Expected values are: {string.Join(", ", Enum.GetNames(typeof(Theme)))}.",
                nameof(theme));
        }

        // Validate buildingType parameter if provided
        if (buildingType.HasValue && !Enum.IsDefined(typeof(BuildingType), buildingType.Value))
        {
            throw new ArgumentException(
                $"Invalid buildingType value: {(int)buildingType.Value}. Expected values are: {string.Join(", ", Enum.GetNames(typeof(BuildingType)))}.",
                nameof(buildingType));
        }

        var parameters = buildingType.HasValue
            ? new Dictionary<string, object> { ["buildingType"] = buildingType.Value }
            : null;

        return _coordinator.GenerateName(EntityType.Building, theme, _sessionState, parameters);
    }

    /// <summary>
    /// Generates a city name for the specified theme.
    /// </summary>
    /// <param name="theme">The theme to use for name generation.</param>
    /// <returns>A generated city name that is unique within the current session.</returns>
    /// <exception cref="ArgumentException">Thrown when an invalid theme value is provided.</exception>
    /// <exception cref="NamePoolExhaustedException">Thrown when unable to generate a unique name after 1000 attempts.</exception>
    public string GenerateCityName(Theme theme)
    {
        // Validate theme parameter
        if (!Enum.IsDefined(typeof(Theme), theme))
        {
            throw new ArgumentException(
                $"Invalid theme value: {(int)theme}. Expected values are: {string.Join(", ", Enum.GetNames(typeof(Theme)))}.",
                nameof(theme));
        }

        return _coordinator.GenerateName(EntityType.City, theme, _sessionState);
    }

    /// <summary>
    /// Generates a district name for the specified theme.
    /// </summary>
    /// <param name="theme">The theme to use for name generation.</param>
    /// <returns>A generated district name that is unique within the current session.</returns>
    /// <exception cref="ArgumentException">Thrown when an invalid theme value is provided.</exception>
    /// <exception cref="NamePoolExhaustedException">Thrown when unable to generate a unique name after 1000 attempts.</exception>
    public string GenerateDistrictName(Theme theme)
    {
        // Validate theme parameter
        if (!Enum.IsDefined(typeof(Theme), theme))
        {
            throw new ArgumentException(
                $"Invalid theme value: {(int)theme}. Expected values are: {string.Join(", ", Enum.GetNames(typeof(Theme)))}.",
                nameof(theme));
        }

        return _coordinator.GenerateName(EntityType.District, theme, _sessionState);
    }

    /// <summary>
    /// Generates a street name for the specified theme.
    /// </summary>
    /// <param name="theme">The theme to use for name generation.</param>
    /// <returns>A generated street name that is unique within the current session.</returns>
    /// <exception cref="ArgumentException">Thrown when an invalid theme value is provided.</exception>
    /// <exception cref="NamePoolExhaustedException">Thrown when unable to generate a unique name after 1000 attempts.</exception>
    public string GenerateStreetName(Theme theme)
    {
        // Validate theme parameter
        if (!Enum.IsDefined(typeof(Theme), theme))
        {
            throw new ArgumentException(
                $"Invalid theme value: {(int)theme}. Expected values are: {string.Join(", ", Enum.GetNames(typeof(Theme)))}.",
                nameof(theme));
        }

        return _coordinator.GenerateName(EntityType.Street, theme, _sessionState);
    }

    /// <summary>
    /// Generates a faction name for the specified theme.
    /// </summary>
    /// <param name="theme">The theme to use for name generation.</param>
    /// <returns>A generated faction name that is unique within the current session.</returns>
    /// <exception cref="ArgumentException">Thrown when an invalid theme value is provided.</exception>
    /// <exception cref="NamePoolExhaustedException">Thrown when unable to generate a unique name after 1000 attempts.</exception>
    public string GenerateFactionName(Theme theme)
    {
        // Validate theme parameter
        if (!Enum.IsDefined(typeof(Theme), theme))
        {
            throw new ArgumentException(
                $"Invalid theme value: {(int)theme}. Expected values are: {string.Join(", ", Enum.GetNames(typeof(Theme)))}.",
                nameof(theme));
        }

        return _coordinator.GenerateName(EntityType.Faction, theme, _sessionState);
    }

    /// <summary>
    /// Resets the generation session, clearing all tracked names and allowing previously generated names to be reused.
    /// The random number generator is reinitialized with the same seed to maintain deterministic behavior.
    /// </summary>
    public void ResetSession()
    {
        _sessionState.Reset();
    }
}
