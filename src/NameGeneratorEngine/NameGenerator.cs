using NameGeneratorEngine.Enums;
using NameGeneratorEngine.Orchestration;
using NameGeneratorEngine.ThemeData;

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
    private readonly ThemeRegistry _themeRegistry;

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
        var actualSeed = seed ?? Random.Shared.Next();
        _sessionState = new SessionState(actualSeed);
        _coordinator = new GenerationCoordinator();
        _themeRegistry = new ThemeRegistry();
    }

    /// <summary>
    /// Initializes a new instance of the NameGenerator class with custom theme configuration.
    /// </summary>
    /// <param name="config">Optional configuration containing custom themes and extensions. If null, only built-in themes are available.</param>
    /// <param name="seed">Optional seed value for deterministic generation. If null, a random seed is generated.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when the configuration contains invalid theme data or when theme validation fails.
    /// If multiple themes have validation errors, all errors are aggregated in the exception message.
    /// </exception>
    /// <remarks>
    /// All custom themes and extensions in the configuration are registered during construction.
    /// Theme validation occurs during registration, and any validation errors will prevent construction.
    /// </remarks>
    public NameGenerator(ThemeConfig? config, int? seed = null)
    {
        var actualSeed = seed ?? Random.Shared.Next();
        _sessionState = new SessionState(actualSeed);
        _coordinator = new GenerationCoordinator();
        _themeRegistry = new ThemeRegistry();

        if (config == null)
            return;

        // Collect all validation errors for aggregation
        var validationErrors = new List<string>();

        // Register all custom themes
        foreach (var kvp in config.CustomThemes)
        {
            try
            {
                _themeRegistry.RegisterCustomTheme(kvp.Key, kvp.Value.InternalData);
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                validationErrors.Add($"Theme '{kvp.Key}': {ex.Message}");
            }
        }

        // Apply all theme extensions
        foreach (var extension in config.ThemeExtensions)
        {
            try
            {
                _themeRegistry.RegisterExtension(extension.BaseThemeIdentifier, extension.ExtensionData);
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                validationErrors.Add($"Extension for '{extension.BaseThemeIdentifier}': {ex.Message}");
            }
        }

        // If there were any validation errors, throw an aggregated exception
        if (validationErrors.Count > 0)
        {
            throw new ArgumentException(
                $"Configuration contains invalid theme data:{Environment.NewLine}" +
                string.Join(Environment.NewLine, validationErrors.Select(e => $"  - {e}")));
        }
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

    /// <summary>
    /// Registers a custom theme with the specified identifier.
    /// </summary>
    /// <param name="identifier">The unique identifier for the custom theme (case-insensitive).</param>
    /// <param name="themeData">The custom theme data to register.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="identifier"/> or <paramref name="themeData"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="identifier"/> is whitespace-only or is a reserved identifier (cyberpunk, elves, orcs).
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when a theme with the same identifier has already been registered.
    /// </exception>
    /// <remarks>
    /// Custom themes can be used with the string-based generation methods after registration.
    /// Reserved identifiers (cyberpunk, elves, orcs) cannot be used for custom themes.
    /// </remarks>
    public void RegisterCustomTheme(string identifier, CustomThemeData themeData)
    {
        ArgumentNullException.ThrowIfNull(identifier);

        if (string.IsNullOrWhiteSpace(identifier))
            throw new ArgumentException("Theme identifier cannot be whitespace-only.", nameof(identifier));

        ArgumentNullException.ThrowIfNull(themeData);

        _themeRegistry.RegisterCustomTheme(identifier, themeData.InternalData);
    }

    /// <summary>
    /// Extends a built-in theme with additional data.
    /// </summary>
    /// <param name="baseTheme">The built-in theme to extend.</param>
    /// <param name="extension">The extension data to merge with the base theme.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="extension"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="baseTheme"/> is not a valid Theme enum value.</exception>
    /// <remarks>
    /// Multiple extensions can be applied to the same base theme.
    /// Extensions are merged in the order they are registered.
    /// The extension data is merged with the base theme data when names are generated.
    /// </remarks>
    public void ExtendTheme(Theme baseTheme, ThemeExtension extension)
    {
        if (!Enum.IsDefined(typeof(Theme), baseTheme))
        {
            throw new ArgumentException(
                $"Invalid theme value: {(int)baseTheme}. Expected values are: {string.Join(", ", Enum.GetNames(typeof(Theme)))}.",
                nameof(baseTheme));
        }

        ArgumentNullException.ThrowIfNull(extension);

        var identifier = baseTheme.ToString().ToLowerInvariant();
        _themeRegistry.RegisterExtension(identifier, extension.ExtensionData);
    }

    /// <summary>
    /// Extends a custom theme with additional data.
    /// </summary>
    /// <param name="baseThemeIdentifier">The identifier of the theme to extend (case-insensitive).</param>
    /// <param name="extension">The extension data to merge with the base theme.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="baseThemeIdentifier"/> or <paramref name="extension"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="baseThemeIdentifier"/> is whitespace-only.
    /// </exception>
    /// <remarks>
    /// Multiple extensions can be applied to the same base theme.
    /// Extensions are merged in the order they are registered.
    /// The base theme (built-in or custom) must exist before the extension can be applied.
    /// The extension data is merged with the base theme data when names are generated.
    /// </remarks>
    public void ExtendTheme(string baseThemeIdentifier, ThemeExtension extension)
    {
        ArgumentNullException.ThrowIfNull(baseThemeIdentifier);

        if (string.IsNullOrWhiteSpace(baseThemeIdentifier))
            throw new ArgumentException("Base theme identifier cannot be whitespace-only.", nameof(baseThemeIdentifier));

        ArgumentNullException.ThrowIfNull(extension);

        _themeRegistry.RegisterExtension(baseThemeIdentifier, extension.ExtensionData);
    }

    /// <summary>
    /// Generates an NPC name using a string theme identifier.
    /// </summary>
    /// <param name="themeIdentifier">The theme identifier (case-insensitive). Can be a built-in theme (Cyberpunk, Elves, Orcs) or a registered custom theme.</param>
    /// <param name="gender">Optional gender for the NPC. If null, a random gender is selected deterministically.</param>
    /// <returns>A generated NPC name that is unique within the current session.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="themeIdentifier"/> is null.</exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="themeIdentifier"/> is not registered.
    /// The exception message lists all available themes.
    /// </exception>
    /// <exception cref="NamePoolExhaustedException">Thrown when unable to generate a unique name after 1000 attempts.</exception>
    public string GenerateNpcName(string themeIdentifier, Gender? gender = null)
    {
        ArgumentNullException.ThrowIfNull(themeIdentifier);

        var themeData = _themeRegistry.GetTheme(themeIdentifier);

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

        return _coordinator.GenerateName(EntityType.Npc, themeData, themeIdentifier, _sessionState, parameters);
    }

    /// <summary>
    /// Generates a building name using a string theme identifier.
    /// </summary>
    /// <param name="themeIdentifier">The theme identifier (case-insensitive). Can be a built-in theme (Cyberpunk, Elves, Orcs) or a registered custom theme.</param>
    /// <param name="buildingType">Optional building type. If null, a generic building name is generated.</param>
    /// <returns>A generated building name that is unique within the current session.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="themeIdentifier"/> is null.</exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="themeIdentifier"/> is not registered.
    /// The exception message lists all available themes.
    /// </exception>
    /// <exception cref="NamePoolExhaustedException">Thrown when unable to generate a unique name after 1000 attempts.</exception>
    public string GenerateBuildingName(string themeIdentifier, BuildingType? buildingType = null)
    {
        ArgumentNullException.ThrowIfNull(themeIdentifier);

        var themeData = _themeRegistry.GetTheme(themeIdentifier);

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

        return _coordinator.GenerateName(EntityType.Building, themeData, themeIdentifier, _sessionState, parameters);
    }

    /// <summary>
    /// Generates a city name using a string theme identifier.
    /// </summary>
    /// <param name="themeIdentifier">The theme identifier (case-insensitive). Can be a built-in theme (Cyberpunk, Elves, Orcs) or a registered custom theme.</param>
    /// <returns>A generated city name that is unique within the current session.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="themeIdentifier"/> is null.</exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="themeIdentifier"/> is not registered.
    /// The exception message lists all available themes.
    /// </exception>
    /// <exception cref="NamePoolExhaustedException">Thrown when unable to generate a unique name after 1000 attempts.</exception>
    public string GenerateCityName(string themeIdentifier)
    {
        ArgumentNullException.ThrowIfNull(themeIdentifier);

        var themeData = _themeRegistry.GetTheme(themeIdentifier);
        return _coordinator.GenerateName(EntityType.City, themeData, themeIdentifier, _sessionState);
    }

    /// <summary>
    /// Generates a district name using a string theme identifier.
    /// </summary>
    /// <param name="themeIdentifier">The theme identifier (case-insensitive). Can be a built-in theme (Cyberpunk, Elves, Orcs) or a registered custom theme.</param>
    /// <returns>A generated district name that is unique within the current session.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="themeIdentifier"/> is null.</exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="themeIdentifier"/> is not registered.
    /// The exception message lists all available themes.
    /// </exception>
    /// <exception cref="NamePoolExhaustedException">Thrown when unable to generate a unique name after 1000 attempts.</exception>
    public string GenerateDistrictName(string themeIdentifier)
    {
        ArgumentNullException.ThrowIfNull(themeIdentifier);

        var themeData = _themeRegistry.GetTheme(themeIdentifier);
        return _coordinator.GenerateName(EntityType.District, themeData, themeIdentifier, _sessionState);
    }

    /// <summary>
    /// Generates a street name using a string theme identifier.
    /// </summary>
    /// <param name="themeIdentifier">The theme identifier (case-insensitive). Can be a built-in theme (Cyberpunk, Elves, Orcs) or a registered custom theme.</param>
    /// <returns>A generated street name that is unique within the current session.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="themeIdentifier"/> is null.</exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="themeIdentifier"/> is not registered.
    /// The exception message lists all available themes.
    /// </exception>
    /// <exception cref="NamePoolExhaustedException">Thrown when unable to generate a unique name after 1000 attempts.</exception>
    public string GenerateStreetName(string themeIdentifier)
    {
        ArgumentNullException.ThrowIfNull(themeIdentifier);

        var themeData = _themeRegistry.GetTheme(themeIdentifier);
        return _coordinator.GenerateName(EntityType.Street, themeData, themeIdentifier, _sessionState);
    }

    /// <summary>
    /// Generates a faction name using a string theme identifier.
    /// </summary>
    /// <param name="themeIdentifier">The theme identifier (case-insensitive). Can be a built-in theme (Cyberpunk, Elves, Orcs) or a registered custom theme.</param>
    /// <returns>A generated faction name that is unique within the current session.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="themeIdentifier"/> is null.</exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="themeIdentifier"/> is not registered.
    /// The exception message lists all available themes.
    /// </exception>
    /// <exception cref="NamePoolExhaustedException">Thrown when unable to generate a unique name after 1000 attempts.</exception>
    public string GenerateFactionName(string themeIdentifier)
    {
        ArgumentNullException.ThrowIfNull(themeIdentifier);

        var themeData = _themeRegistry.GetTheme(themeIdentifier);
        return _coordinator.GenerateName(EntityType.Faction, themeData, themeIdentifier, _sessionState);
    }

    /// <summary>
    /// Gets all available theme identifiers, including both built-in and custom themes.
    /// </summary>
    /// <returns>A read-only collection of theme identifiers (case-insensitive).</returns>
    /// <remarks>
    /// Built-in themes are returned as their enum names (Cyberpunk, Elves, Orcs).
    /// Custom themes are returned with their registered identifiers.
    /// </remarks>
    public IReadOnlyCollection<string> GetAvailableThemes()
    {
        return _themeRegistry.GetRegisteredThemeNames();
    }
}
