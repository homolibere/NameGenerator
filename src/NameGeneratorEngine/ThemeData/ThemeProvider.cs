using System.Text.Json;
using NameGeneratorEngine.Enums;
using NameGeneratorEngine.ThemeData.DataStructures;

namespace NameGeneratorEngine.ThemeData;

/// <summary>
/// Provides access to theme-specific data for name generation.
/// Loads and validates theme data from embedded JSON resources.
/// </summary>
internal class ThemeProvider
{
    private readonly Dictionary<Theme, ThemeData> _themeCache = new Dictionary<Theme, ThemeData>();
    private readonly System.Reflection.Assembly _assembly;

    /// <summary>
    /// Initializes a new instance of the ThemeProvider class.
    /// </summary>
    public ThemeProvider()
    {
        _assembly = System.Reflection.Assembly.GetExecutingAssembly();
    }

    /// <summary>
    /// Gets the theme data for the specified theme.
    /// </summary>
    /// <param name="theme">The theme to retrieve data for.</param>
    /// <returns>The theme data.</returns>
    /// <exception cref="InvalidOperationException">Thrown when theme data cannot be loaded or is invalid.</exception>
    public ThemeData GetThemeData(Theme theme)
    {
        // Return cached data if available
        if (_themeCache.TryGetValue(theme, out var cachedData))
        {
            return cachedData;
        }

        // Load and cache the theme data
        var themeData = LoadThemeData(theme);
        ValidateThemeData(themeData);
        _themeCache[theme] = themeData;
        return themeData;
    }

    /// <summary>
    /// Validates that the theme data is complete and contains all required elements.
    /// </summary>
    /// <param name="themeData">The theme data to validate.</param>
    /// <exception cref="InvalidOperationException">Thrown when theme data is incomplete or invalid.</exception>
    public void ValidateThemeData(ThemeData themeData)
    {
        var errors = new List<string>();

        // Validate NPC name data
        ValidateGenderNameData(themeData.NpcNames.Male, "NPC Male", errors);
        ValidateGenderNameData(themeData.NpcNames.Female, "NPC Female", errors);
        ValidateGenderNameData(themeData.NpcNames.Neutral, "NPC Neutral", errors);

        // Validate building name data
        ValidateStringArray(themeData.BuildingNames.GenericPrefixes, "Building Generic Prefixes", errors);
        ValidateStringArray(themeData.BuildingNames.GenericSuffixes, "Building Generic Suffixes", errors);

        // Validate each building type has data
        foreach (var buildingType in Enum.GetValues<BuildingType>())
        {
            if (!themeData.BuildingNames.TypeData.ContainsKey(buildingType))
            {
                errors.Add($"Building type '{buildingType}' is missing from theme data");
            }
            else
            {
                var typeData = themeData.BuildingNames.TypeData[buildingType];
                ValidateStringArray(typeData.Prefixes, $"Building {buildingType} Prefixes", errors);
                ValidateStringArray(typeData.Descriptors, $"Building {buildingType} Descriptors", errors);
                ValidateStringArray(typeData.Suffixes, $"Building {buildingType} Suffixes", errors);
            }
        }

        // Validate city name data
        ValidateStringArray(themeData.CityNames.Prefixes, "City Prefixes", errors);
        ValidateStringArray(themeData.CityNames.Cores, "City Cores", errors);
        ValidateStringArray(themeData.CityNames.Suffixes, "City Suffixes", errors);

        // Validate district name data
        ValidateStringArray(themeData.DistrictNames.Descriptors, "District Descriptors", errors);
        ValidateStringArray(themeData.DistrictNames.LocationTypes, "District Location Types", errors);

        // Validate street name data
        ValidateStringArray(themeData.StreetNames.Prefixes, "Street Prefixes", errors);
        ValidateStringArray(themeData.StreetNames.Cores, "Street Cores", errors);
        ValidateStringArray(themeData.StreetNames.StreetSuffixes, "Street Suffixes", errors);

        if (errors.Count > 0)
        {
            throw new InvalidOperationException(
                $"Theme data validation failed for '{themeData.Theme}' theme. " +
                $"The following issues were found:\n- {string.Join("\n- ", errors)}");
        }
    }

    private ThemeData LoadThemeData(Theme theme)
    {
        var resourceName = GetResourceName(theme);
        
        using var stream = _assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            throw new InvalidOperationException(
                $"Unable to load theme data for '{theme}' theme. " +
                $"The embedded resource '{resourceName}' was not found. " +
                $"Ensure the theme JSON file exists and is configured as an embedded resource.");
        }

        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
            };

            var themeData = JsonSerializer.Deserialize<ThemeData>(stream, options);
            
            if (themeData == null)
            {
                throw new InvalidOperationException(
                    $"Failed to deserialize theme data for '{theme}' theme. " +
                    $"The JSON file may be empty or malformed.");
            }

            return themeData;
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException(
                $"Failed to parse theme data for '{theme}' theme. " +
                $"The JSON file contains invalid syntax or structure: {ex.Message}", ex);
        }
    }

    private string GetResourceName(Theme theme)
    {
        var themeName = theme.ToString().ToLowerInvariant();
        return $"NameGeneratorEngine.ThemeData.{themeName}.json";
    }

    private void ValidateGenderNameData(GenderNameData data, string context, List<string> errors)
    {
        ValidateStringArray(data.Prefixes, $"{context} Prefixes", errors);
        ValidateStringArray(data.Cores, $"{context} Cores", errors);
        ValidateStringArray(data.Suffixes, $"{context} Suffixes", errors);
    }

    private void ValidateStringArray(string[] array, string fieldName, List<string> errors)
    {
        if (array == null || array.Length == 0)
        {
            errors.Add($"{fieldName} is null or empty");
        }
        else
        {
            var invalidIndices = new List<string>();
            for (var i = 0; i < array.Length; i++)
            {
                if (array[i] == null)
                {
                    invalidIndices.Add($"index {i} (null)");
                }
                else if (array[i].Length > 0 && string.IsNullOrWhiteSpace(array[i]))
                {
                    invalidIndices.Add($"index {i} (whitespace-only: '{array[i]}')");
                }
            }
            
            if (invalidIndices.Count > 0)
            {
                errors.Add($"{fieldName} contains invalid entries at {string.Join(", ", invalidIndices)}");
            }
        }
    }
}
