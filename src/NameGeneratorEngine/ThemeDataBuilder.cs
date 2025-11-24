using NameGeneratorEngine.Enums;
using NameGeneratorEngine.ThemeData.DataStructures;

namespace NameGeneratorEngine;

/// <summary>
/// Provides a fluent interface for building custom theme data.
/// </summary>
/// <remarks>
/// Use this builder to create complete custom themes or extensions to existing themes.
/// For new themes, all entity types must be provided. For extensions, only the entity types
/// being extended need to be specified.
/// </remarks>
/// <example>
/// <code>
/// // Create a new custom theme
/// var customTheme = new ThemeDataBuilder()
///     .WithNpcNames(npc => npc
///         .WithMaleNames(new[] { "Prefix" }, new[] { "Core" }, new[] { "Suffix" })
///         .WithFemaleNames(new[] { "Prefix" }, new[] { "Core" }, new[] { "Suffix" })
///         .WithNeutralNames(new[] { "Prefix" }, new[] { "Core" }, new[] { "Suffix" }))
///     .WithBuildingNames(building => building
///         .WithGenericNames(new[] { "Prefix" }, new[] { "Suffix" }))
///     .WithCityNames(new[] { "Prefix" }, new[] { "Core" }, new[] { "Suffix" })
///     .WithDistrictNames(new[] { "Descriptor" }, new[] { "LocationType" })
///     .WithStreetNames(new[] { "Prefix" }, new[] { "Core" }, new[] { "StreetSuffix" })
///     .WithFactionNames(new[] { "Prefix" }, new[] { "Core" }, new[] { "Suffix" })
///     .Build();
/// </code>
/// </example>
public class ThemeDataBuilder
{
    private readonly bool _isExtension;
    private readonly string? _baseThemeIdentifier;
    
    private NpcNameData? _npcNames;
    private BuildingNameData? _buildingNames;
    private CityNameData? _cityNames;
    private DistrictNameData? _districtNames;
    private StreetNameData? _streetNames;
    private FactionNameData? _factionNames;

    /// <summary>
    /// Initializes a new instance of the <see cref="ThemeDataBuilder"/> class for creating a new theme.
    /// </summary>
    public ThemeDataBuilder()
    {
        _isExtension = false;
        _baseThemeIdentifier = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ThemeDataBuilder"/> class for extending an existing theme.
    /// </summary>
    /// <param name="baseThemeIdentifier">The identifier of the base theme being extended.</param>
    private ThemeDataBuilder(string baseThemeIdentifier)
    {
        _isExtension = true;
        _baseThemeIdentifier = baseThemeIdentifier;
    }

    /// <summary>
    /// Creates a builder for extending a built-in theme.
    /// </summary>
    /// <param name="baseTheme">The built-in theme to extend.</param>
    /// <returns>A <see cref="ThemeDataBuilder"/> configured for creating a theme extension.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="baseTheme"/> is not a valid enum value.</exception>
    /// <example>
    /// <code>
    /// var extension = ThemeDataBuilder.Extend(Theme.Cyberpunk)
    ///     .WithNpcNames(npc => npc
    ///         .AddMalePrefixes("Neo", "Cyber"))
    ///     .BuildExtension();
    /// </code>
    /// </example>
    public static ThemeDataBuilder Extend(Theme baseTheme)
    {
        return !Enum.IsDefined(typeof(Theme), baseTheme)
            ? throw new ArgumentException($"Invalid theme value: {baseTheme}. Expected values: {string.Join(", ", Enum.GetNames(typeof(Theme)))}", nameof(baseTheme))
            : new ThemeDataBuilder(baseTheme.ToString().ToLowerInvariant());
    }

    /// <summary>
    /// Creates a builder for extending a custom theme.
    /// </summary>
    /// <param name="baseThemeIdentifier">The identifier of the custom theme to extend.</param>
    /// <returns>A <see cref="ThemeDataBuilder"/> configured for creating a theme extension.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="baseThemeIdentifier"/> is null or whitespace.</exception>
    /// <example>
    /// <code>
    /// var extension = ThemeDataBuilder.Extend("steampunk")
    ///     .WithCityNames(new[] { "Brass" }, new[] { "forge" }, new[] { "ton" })
    ///     .BuildExtension();
    /// </code>
    /// </example>
    public static ThemeDataBuilder Extend(string baseThemeIdentifier)
    {
        return string.IsNullOrWhiteSpace(baseThemeIdentifier)
            ? throw new ArgumentException("Base theme identifier cannot be null or whitespace.", nameof(baseThemeIdentifier))
            : new ThemeDataBuilder(baseThemeIdentifier.ToLowerInvariant());
    }

    /// <summary>
    /// Configures NPC name data for this theme.
    /// </summary>
    /// <param name="configure">An action that configures the NPC name data using <see cref="NpcNameDataBuilder"/>.</param>
    /// <returns>This builder instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="configure"/> is null.</exception>
    public ThemeDataBuilder WithNpcNames(Action<NpcNameDataBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        var builder = new NpcNameDataBuilder(_isExtension);
        configure(builder);
        _npcNames = builder.Build();
        return this;
    }

    /// <summary>
    /// Configures building name data for this theme.
    /// </summary>
    /// <param name="configure">An action that configures the building name data using <see cref="BuildingNameDataBuilder"/>.</param>
    /// <returns>This builder instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="configure"/> is null.</exception>
    public ThemeDataBuilder WithBuildingNames(Action<BuildingNameDataBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);

        var builder = new BuildingNameDataBuilder(_isExtension);
        configure(builder);
        _buildingNames = builder.Build();
        return this;
    }

    /// <summary>
    /// Configures city name data for this theme.
    /// </summary>
    /// <param name="prefixes">The array of prefix syllables for city names.</param>
    /// <param name="cores">The array of core syllables for city names.</param>
    /// <param name="suffixes">The array of suffix syllables for city names.</param>
    /// <returns>This builder instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    /// <exception cref="ArgumentException">Thrown when any array is empty or contains invalid strings.</exception>
    public ThemeDataBuilder WithCityNames(string[] prefixes, string[] cores, string[] suffixes)
    {
        ValidateStringArray(prefixes, nameof(prefixes), "City name prefixes");
        ValidateStringArray(cores, nameof(cores), "City name cores");
        ValidateStringArray(suffixes, nameof(suffixes), "City name suffixes");

        _cityNames = new CityNameData
        {
            Prefixes = prefixes.ToArray(), // Copy array to ensure immutability
            Cores = cores.ToArray(),
            Suffixes = suffixes.ToArray()
        };
        return this;
    }

    /// <summary>
    /// Configures district name data for this theme.
    /// </summary>
    /// <param name="descriptors">The array of descriptive terms for district names.</param>
    /// <param name="locationTypes">The array of location type terms for district names.</param>
    /// <returns>This builder instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    /// <exception cref="ArgumentException">Thrown when any array is empty or contains invalid strings.</exception>
    public ThemeDataBuilder WithDistrictNames(string[] descriptors, string[] locationTypes)
    {
        ValidateStringArray(descriptors, nameof(descriptors), "District descriptors");
        ValidateStringArray(locationTypes, nameof(locationTypes), "District location types");

        _districtNames = new DistrictNameData
        {
            Descriptors = descriptors.ToArray(), // Copy array to ensure immutability
            LocationTypes = locationTypes.ToArray()
        };
        return this;
    }

    /// <summary>
    /// Configures street name data for this theme.
    /// </summary>
    /// <param name="prefixes">The array of prefix syllables for street names.</param>
    /// <param name="cores">The array of core syllables for street names.</param>
    /// <param name="streetSuffixes">The array of street suffix terms (e.g., "Street", "Avenue").</param>
    /// <returns>This builder instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    /// <exception cref="ArgumentException">Thrown when any array is empty or contains invalid strings.</exception>
    public ThemeDataBuilder WithStreetNames(string[] prefixes, string[] cores, string[] streetSuffixes)
    {
        ValidateStringArray(prefixes, nameof(prefixes), "Street name prefixes");
        ValidateStringArray(cores, nameof(cores), "Street name cores");
        ValidateStringArray(streetSuffixes, nameof(streetSuffixes), "Street suffixes");

        _streetNames = new StreetNameData
        {
            Prefixes = prefixes.ToArray(), // Copy array to ensure immutability
            Cores = cores.ToArray(),
            StreetSuffixes = streetSuffixes.ToArray()
        };
        return this;
    }

    /// <summary>
    /// Configures faction name data for this theme.
    /// </summary>
    /// <param name="prefixes">The array of prefix syllables for faction names.</param>
    /// <param name="cores">The array of core syllables for faction names.</param>
    /// <param name="suffixes">The array of suffix syllables for faction names.</param>
    /// <returns>This builder instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    /// <exception cref="ArgumentException">Thrown when any array is empty or contains invalid strings.</exception>
    public ThemeDataBuilder WithFactionNames(string[] prefixes, string[] cores, string[] suffixes)
    {
        ValidateStringArray(prefixes, nameof(prefixes), "Faction name prefixes");
        ValidateStringArray(cores, nameof(cores), "Faction name cores");
        ValidateStringArray(suffixes, nameof(suffixes), "Faction name suffixes");

        _factionNames = new FactionNameData
        {
            Prefixes = prefixes.ToArray(), // Copy array to ensure immutability
            Cores = cores.ToArray(),
            Suffixes = suffixes.ToArray()
        };
        return this;
    }

    /// <summary>
    /// Builds a complete custom theme from the configured data.
    /// </summary>
    /// <returns>A <see cref="CustomThemeData"/> instance containing the theme data.</returns>
    /// <exception cref="InvalidOperationException">Thrown when this builder was created for extensions.</exception>
    /// <exception cref="ArgumentException">Thrown when required fields are missing for a new theme.</exception>
    public CustomThemeData Build()
    {
        if (_isExtension)
        {
            throw new InvalidOperationException("Cannot call Build() on an extension builder. Use BuildExtension() instead.");
        }

        // Validate all required fields are present for a new theme
        var missingFields = new List<string>();
        
        if (_npcNames == null) missingFields.Add("NPC names");
        if (_buildingNames == null) missingFields.Add("Building names");
        if (_cityNames == null) missingFields.Add("City names");
        if (_districtNames == null) missingFields.Add("District names");
        if (_streetNames == null) missingFields.Add("Street names");
        if (_factionNames == null) missingFields.Add("Faction names");

        if (missingFields.Count > 0)
        {
            throw new ArgumentException(
                $"Missing required fields for custom theme: {string.Join(", ", missingFields)}. " +
                "All entity types must be provided when creating a new theme.");
        }

        var themeData = new ThemeData.ThemeData
        {
            Theme = Theme.Cyberpunk, // Placeholder - custom themes don't use this field
            NpcNames = _npcNames!,
            BuildingNames = _buildingNames!,
            CityNames = _cityNames!,
            DistrictNames = _districtNames!,
            StreetNames = _streetNames!,
            FactionNames = _factionNames!
        };

        return new CustomThemeData(themeData);
    }

    /// <summary>
    /// Builds a theme extension from the configured data.
    /// </summary>
    /// <returns>A <see cref="ThemeExtension"/> instance containing the extension data.</returns>
    /// <exception cref="InvalidOperationException">Thrown when this builder was not created for extensions.</exception>
    /// <exception cref="ArgumentException">Thrown when no entity data was provided for the extension.</exception>
    public ThemeExtension BuildExtension()
    {
        if (!_isExtension)
            throw new InvalidOperationException("Cannot call BuildExtension() on a new theme builder. Use Build() instead.");

        // Validate at least one entity type is provided for an extension
        if (_npcNames == null && _buildingNames == null && _cityNames == null && 
            _districtNames == null && _streetNames == null && _factionNames == null)
        {
            throw new ArgumentException(
                "At least one entity type must be provided when creating a theme extension.");
        }

        // Create partial theme data with only the provided fields
        // Use empty arrays for missing fields (they won't be merged)
        var themeData = new ThemeData.ThemeData
        {
            Theme = Theme.Cyberpunk, // Placeholder - extensions don't use this field
            NpcNames = _npcNames ?? CreateEmptyNpcNameData(),
            BuildingNames = _buildingNames ?? CreateEmptyBuildingNameData(),
            CityNames = _cityNames ?? CreateEmptyCityNameData(),
            DistrictNames = _districtNames ?? CreateEmptyDistrictNameData(),
            StreetNames = _streetNames ?? CreateEmptyStreetNameData(),
            FactionNames = _factionNames ?? CreateEmptyFactionNameData()
        };

        return new ThemeExtension(_baseThemeIdentifier!, themeData);
    }

    /// <summary>
    /// Validates that a string array is not null, not empty, and contains only valid strings.
    /// </summary>
    private static void ValidateStringArray(string[] array, string paramName, string fieldName)
    {
        if (array == null)
        {
            throw new ArgumentNullException(paramName, $"{fieldName} cannot be null.");
        }

        if (array.Length == 0)
        {
            throw new ArgumentException($"{fieldName} cannot be empty. At least one element is required.", paramName);
        }

        for (var i = 0; i < array.Length; i++)
        {
            if (array[i] == null)
            {
                throw new ArgumentException($"{fieldName} contains null value at index {i}.", paramName);
            }

            if (string.IsNullOrWhiteSpace(array[i]))
            {
                throw new ArgumentException($"{fieldName} contains whitespace-only value at index {i}.", paramName);
            }
        }
    }

    // Helper methods to create empty data structures for extensions
    private static NpcNameData CreateEmptyNpcNameData() =>
        new NpcNameData
    {
        Male = new GenderNameData { Prefixes = [], Cores = [], Suffixes = [] },
        Female = new GenderNameData { Prefixes = [], Cores = [], Suffixes = [] },
        Neutral = new GenderNameData { Prefixes = [], Cores = [], Suffixes = [] }
    };

    private static BuildingNameData CreateEmptyBuildingNameData() =>
        new BuildingNameData
    {
        GenericPrefixes = [],
        GenericSuffixes = [],
        TypeData = new Dictionary<BuildingType, BuildingTypeData>()
    };

    private static CityNameData CreateEmptyCityNameData() =>
        new CityNameData
    {
        Prefixes = [],
        Cores = [],
        Suffixes = []
    };

    private static DistrictNameData CreateEmptyDistrictNameData() =>
        new DistrictNameData
    {
        Descriptors = [],
        LocationTypes = []
    };

    private static StreetNameData CreateEmptyStreetNameData() =>
        new StreetNameData
    {
        Prefixes = [],
        Cores = [],
        StreetSuffixes = []
    };

    private static FactionNameData CreateEmptyFactionNameData() =>
        new FactionNameData
    {
        Prefixes = [],
        Cores = [],
        Suffixes = []
    };
}
