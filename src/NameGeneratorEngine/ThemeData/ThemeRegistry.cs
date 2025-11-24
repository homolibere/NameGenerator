using NameGeneratorEngine.Enums;
using NameGeneratorEngine.ThemeData.DataStructures;

namespace NameGeneratorEngine.ThemeData;

/// <summary>
/// Manages both built-in and custom themes, providing unified theme lookup with extension support.
/// </summary>
internal sealed class ThemeRegistry
{
    private readonly Dictionary<string, ThemeData> _customThemes = new Dictionary<string, ThemeData>(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, List<ThemeData>> _extensions = new Dictionary<string, List<ThemeData>>(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, ThemeData> _mergedThemeCache = new Dictionary<string, ThemeData>(StringComparer.OrdinalIgnoreCase);
    private readonly ThemeProvider _builtInProvider = new ThemeProvider();

    /// <summary>
    /// Registers a custom theme with the specified identifier.
    /// </summary>
    /// <param name="identifier">The unique identifier for the custom theme (case-insensitive).</param>
    /// <param name="themeData">The theme data to register.</param>
    /// <exception cref="ArgumentNullException">Thrown when identifier or themeData is null.</exception>
    /// <exception cref="ArgumentException">Thrown when identifier is whitespace-only.</exception>
    /// <exception cref="InvalidOperationException">Thrown when a theme with the same identifier already exists.</exception>
    public void RegisterCustomTheme(string identifier, ThemeData themeData)
    {
        ArgumentNullException.ThrowIfNull(identifier);

        if (string.IsNullOrWhiteSpace(identifier))
            throw new ArgumentException("Theme identifier cannot be whitespace-only.", nameof(identifier));

        ArgumentNullException.ThrowIfNull(themeData);

        // Check for conflicts with existing custom themes
        if (_customThemes.ContainsKey(identifier))
        {
            throw new InvalidOperationException(
                $"A custom theme with identifier '{identifier}' has already been registered. " +
                $"Please use a different identifier.");
        }

        // Check for conflicts with built-in themes (case-insensitive)
        var reservedIdentifiers = new[] { "cyberpunk", "elves", "orcs" };
        if (reservedIdentifiers.Contains(identifier.ToLowerInvariant()))
        {
            throw new InvalidOperationException(
                $"The identifier '{identifier}' is reserved for built-in themes. " +
                $"Please use a different identifier.");
        }

        _customThemes[identifier] = themeData;
        
        // Clear merged cache for this identifier in case it had extensions
        _mergedThemeCache.Remove(identifier);
    }

    /// <summary>
    /// Registers an extension to be merged with a base theme.
    /// </summary>
    /// <param name="baseThemeIdentifier">The identifier of the base theme to extend.</param>
    /// <param name="extensionData">The extension data to merge with the base theme.</param>
    /// <exception cref="ArgumentNullException">Thrown when baseThemeIdentifier or extensionData is null.</exception>
    /// <exception cref="ArgumentException">Thrown when baseThemeIdentifier is whitespace-only.</exception>
    public void RegisterExtension(string baseThemeIdentifier, ThemeData extensionData)
    {
        ArgumentNullException.ThrowIfNull(baseThemeIdentifier);

        if (string.IsNullOrWhiteSpace(baseThemeIdentifier))
            throw new ArgumentException("Base theme identifier cannot be whitespace-only.", nameof(baseThemeIdentifier));

        ArgumentNullException.ThrowIfNull(extensionData);

        if (!_extensions.ContainsKey(baseThemeIdentifier))
        {
            _extensions[baseThemeIdentifier] =
            [
            ];
        }

        _extensions[baseThemeIdentifier].Add(extensionData);
        
        // Clear merged cache for this identifier since we added an extension
        _mergedThemeCache.Remove(baseThemeIdentifier);
    }

    /// <summary>
    /// Gets the theme data for a built-in theme.
    /// </summary>
    /// <param name="builtInTheme">The built-in theme to retrieve.</param>
    /// <returns>The theme data, merged with any registered extensions.</returns>
    public ThemeData GetTheme(Theme builtInTheme)
    {
        var identifier = builtInTheme.ToString().ToLowerInvariant();
        return GetTheme(identifier);
    }

    /// <summary>
    /// Gets the theme data for a custom theme or built-in theme by identifier.
    /// </summary>
    /// <param name="identifier">The theme identifier (case-insensitive).</param>
    /// <returns>The theme data, merged with any registered extensions.</returns>
    /// <exception cref="ArgumentNullException">Thrown when identifier is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the theme identifier is not registered.</exception>
    public ThemeData GetTheme(string identifier)
    {
        ArgumentNullException.ThrowIfNull(identifier);

        // Check cache first
        if (_mergedThemeCache.TryGetValue(identifier, out var cachedTheme))
        {
            return cachedTheme;
        }

        // Try to get base theme data
        ThemeData baseTheme;
        
        // Check if it's a custom theme
        if (_customThemes.TryGetValue(identifier, out var customTheme))
        {
            baseTheme = customTheme;
        }
        // Check if it's a built-in theme
        else if (Enum.TryParse<Theme>(identifier, ignoreCase: true, out var builtInTheme))
        {
            baseTheme = _builtInProvider.GetThemeData(builtInTheme);
        }
        else
        {
            // Theme not found
            var availableThemes = GetRegisteredThemeNames();
            throw new ArgumentException(
                $"Theme '{identifier}' is not registered. " +
                $"Available themes: {string.Join(", ", availableThemes)}", 
                nameof(identifier));
        }

        // Merge with extensions if any exist
        var mergedTheme = MergeThemeWithExtensions(baseTheme, identifier);
        
        // Cache the merged result
        _mergedThemeCache[identifier] = mergedTheme;
        
        return mergedTheme;
    }

    /// <summary>
    /// Checks if a custom theme with the specified identifier is registered.
    /// </summary>
    /// <param name="identifier">The theme identifier to check (case-insensitive).</param>
    /// <returns>True if a custom theme with the identifier exists; otherwise, false.</returns>
    public bool HasCustomTheme(string? identifier)
    {
        if (identifier == null)
            return false;
        
        return _customThemes.ContainsKey(identifier);
    }

    /// <summary>
    /// Gets the names of all registered themes (built-in and custom).
    /// </summary>
    /// <returns>A read-only collection of theme identifiers.</returns>
    public IReadOnlyCollection<string> GetRegisteredThemeNames()
    {
        var builtInThemes = Enum.GetNames<Theme>();
        var customThemes = _customThemes.Keys;
        
        return builtInThemes.Concat(customThemes).ToList();
    }

    /// <summary>
    /// Merges a base theme with all registered extensions for that theme.
    /// </summary>
    /// <param name="baseTheme">The base theme data.</param>
    /// <param name="identifier">The theme identifier.</param>
    /// <returns>The merged theme data.</returns>
    private ThemeData MergeThemeWithExtensions(ThemeData baseTheme, string identifier)
    {
        // If no extensions exist, return the base theme as-is
        if (!_extensions.TryGetValue(identifier, out var extensions) || extensions.Count == 0)
        {
            return baseTheme;
        }

        // Start with base theme data
        var mergedNpcNames = MergeNpcNameData(baseTheme.NpcNames, extensions);
        var mergedBuildingNames = MergeBuildingNameData(baseTheme.BuildingNames, extensions);
        var mergedCityNames = MergeCityNameData(baseTheme.CityNames, extensions);
        var mergedDistrictNames = MergeDistrictNameData(baseTheme.DistrictNames, extensions);
        var mergedStreetNames = MergeStreetNameData(baseTheme.StreetNames, extensions);
        var mergedFactionNames = MergeFactionNameData(baseTheme.FactionNames, extensions);

        // Create merged theme data
        return new ThemeData
        {
            Theme = baseTheme.Theme,
            NpcNames = mergedNpcNames,
            BuildingNames = mergedBuildingNames,
            CityNames = mergedCityNames,
            DistrictNames = mergedDistrictNames,
            StreetNames = mergedStreetNames,
            FactionNames = mergedFactionNames
        };
    }

    private NpcNameData MergeNpcNameData(NpcNameData baseData, List<ThemeData> extensions)
    {
        var malePrefixes = baseData.Male.Prefixes.ToList();
        var maleCores = baseData.Male.Cores.ToList();
        var maleSuffixes = baseData.Male.Suffixes.ToList();
        
        var femalePrefixes = baseData.Female.Prefixes.ToList();
        var femaleCores = baseData.Female.Cores.ToList();
        var femaleSuffixes = baseData.Female.Suffixes.ToList();
        
        var neutralPrefixes = baseData.Neutral.Prefixes.ToList();
        var neutralCores = baseData.Neutral.Cores.ToList();
        var neutralSuffixes = baseData.Neutral.Suffixes.ToList();

        foreach (var extension in extensions)
        {
            malePrefixes.AddRange(extension.NpcNames.Male.Prefixes);
            maleCores.AddRange(extension.NpcNames.Male.Cores);
            maleSuffixes.AddRange(extension.NpcNames.Male.Suffixes);

            femalePrefixes.AddRange(extension.NpcNames.Female.Prefixes);
            femaleCores.AddRange(extension.NpcNames.Female.Cores);
            femaleSuffixes.AddRange(extension.NpcNames.Female.Suffixes);

            neutralPrefixes.AddRange(extension.NpcNames.Neutral.Prefixes);
            neutralCores.AddRange(extension.NpcNames.Neutral.Cores);
            neutralSuffixes.AddRange(extension.NpcNames.Neutral.Suffixes);
        }

        return new NpcNameData
        {
            Male = new GenderNameData
            {
                Prefixes = malePrefixes.ToArray(),
                Cores = maleCores.ToArray(),
                Suffixes = maleSuffixes.ToArray()
            },
            Female = new GenderNameData
            {
                Prefixes = femalePrefixes.ToArray(),
                Cores = femaleCores.ToArray(),
                Suffixes = femaleSuffixes.ToArray()
            },
            Neutral = new GenderNameData
            {
                Prefixes = neutralPrefixes.ToArray(),
                Cores = neutralCores.ToArray(),
                Suffixes = neutralSuffixes.ToArray()
            }
        };
    }

    private BuildingNameData MergeBuildingNameData(BuildingNameData baseData, List<ThemeData> extensions)
    {
        var genericPrefixes = baseData.GenericPrefixes.ToList();
        var genericSuffixes = baseData.GenericSuffixes.ToList();
        var typeData = new Dictionary<BuildingType, BuildingTypeData>();

        // Copy base type data
        foreach (var kvp in baseData.TypeData)
        {
            typeData[kvp.Key] = new BuildingTypeData
            {
                Prefixes = kvp.Value.Prefixes.ToArray(),
                Descriptors = kvp.Value.Descriptors.ToArray(),
                Suffixes = kvp.Value.Suffixes.ToArray()
            };
        }

        foreach (var extension in extensions)
        {
            genericPrefixes.AddRange(extension.BuildingNames.GenericPrefixes);
            genericSuffixes.AddRange(extension.BuildingNames.GenericSuffixes);

            foreach (var kvp in extension.BuildingNames.TypeData)
            {
                if (!typeData.ContainsKey(kvp.Key))
                {
                    typeData[kvp.Key] = new BuildingTypeData
                    {
                        Prefixes = kvp.Value.Prefixes.ToArray(),
                        Descriptors = kvp.Value.Descriptors.ToArray(),
                        Suffixes = kvp.Value.Suffixes.ToArray()
                    };
                }
                else
                {
                    var existing = typeData[kvp.Key];
                    var prefixes = existing.Prefixes.ToList();
                    var descriptors = existing.Descriptors.ToList();
                    var suffixes = existing.Suffixes.ToList();

                    prefixes.AddRange(kvp.Value.Prefixes);
                    descriptors.AddRange(kvp.Value.Descriptors);
                    suffixes.AddRange(kvp.Value.Suffixes);

                    typeData[kvp.Key] = new BuildingTypeData
                    {
                        Prefixes = prefixes.ToArray(),
                        Descriptors = descriptors.ToArray(),
                        Suffixes = suffixes.ToArray()
                    };
                }
            }
        }

        return new BuildingNameData
        {
            GenericPrefixes = genericPrefixes.ToArray(),
            GenericSuffixes = genericSuffixes.ToArray(),
            TypeData = typeData
        };
    }

    private CityNameData MergeCityNameData(CityNameData baseData, List<ThemeData> extensions)
    {
        var prefixes = baseData.Prefixes.ToList();
        var cores = baseData.Cores.ToList();
        var suffixes = baseData.Suffixes.ToList();

        foreach (var extension in extensions)
        {
            prefixes.AddRange(extension.CityNames.Prefixes);
            cores.AddRange(extension.CityNames.Cores);
            suffixes.AddRange(extension.CityNames.Suffixes);
        }

        return new CityNameData
        {
            Prefixes = prefixes.ToArray(),
            Cores = cores.ToArray(),
            Suffixes = suffixes.ToArray()
        };
    }

    private DistrictNameData MergeDistrictNameData(DistrictNameData baseData, List<ThemeData> extensions)
    {
        var descriptors = baseData.Descriptors.ToList();
        var locationTypes = baseData.LocationTypes.ToList();

        foreach (var extension in extensions)
        {
            descriptors.AddRange(extension.DistrictNames.Descriptors);
            locationTypes.AddRange(extension.DistrictNames.LocationTypes);
        }

        return new DistrictNameData
        {
            Descriptors = descriptors.ToArray(),
            LocationTypes = locationTypes.ToArray()
        };
    }

    private StreetNameData MergeStreetNameData(StreetNameData baseData, List<ThemeData> extensions)
    {
        var prefixes = baseData.Prefixes.ToList();
        var cores = baseData.Cores.ToList();
        var streetSuffixes = baseData.StreetSuffixes.ToList();

        foreach (var extension in extensions)
        {
            prefixes.AddRange(extension.StreetNames.Prefixes);
            cores.AddRange(extension.StreetNames.Cores);
            streetSuffixes.AddRange(extension.StreetNames.StreetSuffixes);
        }

        return new StreetNameData
        {
            Prefixes = prefixes.ToArray(),
            Cores = cores.ToArray(),
            StreetSuffixes = streetSuffixes.ToArray()
        };
    }

    private FactionNameData MergeFactionNameData(FactionNameData baseData, List<ThemeData> extensions)
    {
        var prefixes = baseData.Prefixes.ToList();
        var cores = baseData.Cores.ToList();
        var suffixes = baseData.Suffixes.ToList();

        foreach (var extension in extensions)
        {
            prefixes.AddRange(extension.FactionNames.Prefixes);
            cores.AddRange(extension.FactionNames.Cores);
            suffixes.AddRange(extension.FactionNames.Suffixes);
        }

        return new FactionNameData
        {
            Prefixes = prefixes.ToArray(),
            Cores = cores.ToArray(),
            Suffixes = suffixes.ToArray()
        };
    }
}
