using NameGeneratorEngine.Enums;
using NameGeneratorEngine.ThemeData.DataStructures;

namespace NameGeneratorEngine;

/// <summary>
/// Provides a fluent interface for building name data.
/// </summary>
/// <remarks>
/// This builder supports two modes:
/// - For new themes: Use WithGenericNames() and WithTypeNames() to provide complete building data
/// - For extensions: Use Add* methods to add to existing data
/// </remarks>
public class BuildingNameDataBuilder
{
    private readonly bool _isExtension;
    
    private string[]? _genericPrefixes;
    private string[]? _genericSuffixes;
    private readonly Dictionary<BuildingType, BuildingTypeData> _typeData = new Dictionary<BuildingType, BuildingTypeData>();

    /// <summary>
    /// Initializes a new instance of the <see cref="BuildingNameDataBuilder"/> class.
    /// </summary>
    /// <param name="isExtension">Whether this builder is for a theme extension.</param>
    internal BuildingNameDataBuilder(bool isExtension)
    {
        _isExtension = isExtension;
    }

    #region Complete Building Data Methods (for new themes)

    /// <summary>
    /// Configures generic building name data used when no specific building type is provided.
    /// </summary>
    /// <param name="prefixes">The array of generic prefix syllables.</param>
    /// <param name="suffixes">The array of generic suffix syllables.</param>
    /// <returns>This builder instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    /// <exception cref="ArgumentException">Thrown when any array is empty or contains invalid strings.</exception>
    public BuildingNameDataBuilder WithGenericNames(string[] prefixes, string[] suffixes)
    {
        ValidateStringArray(prefixes, nameof(prefixes), "Generic building prefixes");
        ValidateStringArray(suffixes, nameof(suffixes), "Generic building suffixes");

        _genericPrefixes = prefixes.ToArray(); // Copy array to ensure immutability
        _genericSuffixes = suffixes.ToArray();
        return this;
    }

    /// <summary>
    /// Configures building name data for a specific building type.
    /// </summary>
    /// <param name="type">The building type to configure.</param>
    /// <param name="prefixes">The array of prefix syllables for this building type.</param>
    /// <param name="descriptors">The array of descriptive terms for this building type.</param>
    /// <param name="suffixes">The array of suffix syllables for this building type.</param>
    /// <returns>This builder instance for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="type"/> is not a valid enum value.</exception>
    /// <exception cref="ArgumentNullException">Thrown when any array parameter is null.</exception>
    /// <exception cref="ArgumentException">Thrown when any array is empty or contains invalid strings.</exception>
    public BuildingNameDataBuilder WithTypeNames(BuildingType type, string[] prefixes, string[] descriptors, string[] suffixes)
    {
        if (!Enum.IsDefined(typeof(BuildingType), type))
            throw new ArgumentException($"Invalid building type: {type}. Expected values: {string.Join(", ", Enum.GetNames(typeof(BuildingType)))}", nameof(type));

        ValidateStringArray(prefixes, nameof(prefixes), $"{type} building prefixes");
        ValidateStringArray(descriptors, nameof(descriptors), $"{type} building descriptors");
        ValidateStringArray(suffixes, nameof(suffixes), $"{type} building suffixes");

        _typeData[type] = new BuildingTypeData
        {
            Prefixes = prefixes.ToArray(), // Copy array to ensure immutability
            Descriptors = descriptors.ToArray(),
            Suffixes = suffixes.ToArray()
        };
        return this;
    }

    #endregion

    #region Extension Methods (for theme extensions)

    /// <summary>
    /// Adds generic building prefixes to the extension.
    /// </summary>
    /// <param name="prefixes">The prefix syllables to add.</param>
    /// <returns>This builder instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="prefixes"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the array is empty or contains invalid strings.</exception>
    public BuildingNameDataBuilder AddGenericPrefixes(params string[] prefixes)
    {
        ValidateStringArray(prefixes, nameof(prefixes), "Generic building prefixes");
        _genericPrefixes = CombineArrays(_genericPrefixes, prefixes.ToArray()); // Copy array to ensure immutability
        return this;
    }

    /// <summary>
    /// Adds generic building suffixes to the extension.
    /// </summary>
    /// <param name="suffixes">The suffix syllables to add.</param>
    /// <returns>This builder instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="suffixes"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the array is empty or contains invalid strings.</exception>
    public BuildingNameDataBuilder AddGenericSuffixes(params string[] suffixes)
    {
        ValidateStringArray(suffixes, nameof(suffixes), "Generic building suffixes");
        _genericSuffixes = CombineArrays(_genericSuffixes, suffixes.ToArray()); // Copy array to ensure immutability
        return this;
    }

    /// <summary>
    /// Adds building type-specific prefixes to the extension.
    /// </summary>
    /// <param name="type">The building type to add prefixes for.</param>
    /// <param name="prefixes">The prefix syllables to add.</param>
    /// <returns>This builder instance for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="type"/> is not a valid enum value.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="prefixes"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the array is empty or contains invalid strings.</exception>
    public BuildingNameDataBuilder AddTypePrefixes(BuildingType type, params string[] prefixes)
    {
        if (!Enum.IsDefined(typeof(BuildingType), type))
            throw new ArgumentException($"Invalid building type: {type}. Expected values: {string.Join(", ", Enum.GetNames(typeof(BuildingType)))}", nameof(type));

        ValidateStringArray(prefixes, nameof(prefixes), $"{type} building prefixes");

        var prefixesCopy = prefixes.ToArray(); // Copy array to ensure immutability
        
        if (!_typeData.ContainsKey(type))
        {
            _typeData[type] = new BuildingTypeData
            {
                Prefixes = prefixesCopy,
                Descriptors = Array.Empty<string>(),
                Suffixes = Array.Empty<string>()
            };
        }
        else
        {
            var existing = _typeData[type];
            _typeData[type] = new BuildingTypeData
            {
                Prefixes = CombineArrays(existing.Prefixes, prefixesCopy),
                Descriptors = existing.Descriptors,
                Suffixes = existing.Suffixes
            };
        }
        return this;
    }

    /// <summary>
    /// Adds building type-specific descriptors to the extension.
    /// </summary>
    /// <param name="type">The building type to add descriptors for.</param>
    /// <param name="descriptors">The descriptive terms to add.</param>
    /// <returns>This builder instance for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="type"/> is not a valid enum value.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="descriptors"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the array is empty or contains invalid strings.</exception>
    public BuildingNameDataBuilder AddTypeDescriptors(BuildingType type, params string[] descriptors)
    {
        if (!Enum.IsDefined(typeof(BuildingType), type))
            throw new ArgumentException($"Invalid building type: {type}. Expected values: {string.Join(", ", Enum.GetNames(typeof(BuildingType)))}", nameof(type));

        ValidateStringArray(descriptors, nameof(descriptors), $"{type} building descriptors");

        var descriptorsCopy = descriptors.ToArray(); // Copy array to ensure immutability
        
        if (!_typeData.ContainsKey(type))
        {
            _typeData[type] = new BuildingTypeData
            {
                Prefixes = Array.Empty<string>(),
                Descriptors = descriptorsCopy,
                Suffixes = Array.Empty<string>()
            };
        }
        else
        {
            var existing = _typeData[type];
            _typeData[type] = new BuildingTypeData
            {
                Prefixes = existing.Prefixes,
                Descriptors = CombineArrays(existing.Descriptors, descriptorsCopy),
                Suffixes = existing.Suffixes
            };
        }
        return this;
    }

    /// <summary>
    /// Adds building type-specific suffixes to the extension.
    /// </summary>
    /// <param name="type">The building type to add suffixes for.</param>
    /// <param name="suffixes">The suffix syllables to add.</param>
    /// <returns>This builder instance for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="type"/> is not a valid enum value.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="suffixes"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the array is empty or contains invalid strings.</exception>
    public BuildingNameDataBuilder AddTypeSuffixes(BuildingType type, params string[] suffixes)
    {
        if (!Enum.IsDefined(typeof(BuildingType), type))
            throw new ArgumentException($"Invalid building type: {type}. Expected values: {string.Join(", ", Enum.GetNames(typeof(BuildingType)))}", nameof(type));

        ValidateStringArray(suffixes, nameof(suffixes), $"{type} building suffixes");

        var suffixesCopy = suffixes.ToArray(); // Copy array to ensure immutability
        
        if (!_typeData.ContainsKey(type))
        {
            _typeData[type] = new BuildingTypeData
            {
                Prefixes = Array.Empty<string>(),
                Descriptors = Array.Empty<string>(),
                Suffixes = suffixesCopy
            };
        }
        else
        {
            var existing = _typeData[type];
            _typeData[type] = new BuildingTypeData
            {
                Prefixes = existing.Prefixes,
                Descriptors = existing.Descriptors,
                Suffixes = CombineArrays(existing.Suffixes, suffixesCopy)
            };
        }
        return this;
    }

    #endregion

    /// <summary>
    /// Builds the building name data.
    /// </summary>
    /// <returns>A <see cref="BuildingNameData"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when required data is missing for a new theme.</exception>
    internal BuildingNameData Build()
    {
        if (_isExtension)
        {
            return new BuildingNameData
            {
                GenericPrefixes = _genericPrefixes ?? Array.Empty<string>(),
                GenericSuffixes = _genericSuffixes ?? Array.Empty<string>(),
                TypeData = new Dictionary<BuildingType, BuildingTypeData>(_typeData)
            };
        }

        // For new themes, generic data and all building types must be provided
        var missingFields = new List<string>();

        if (_genericPrefixes == null || _genericSuffixes == null)
            missingFields.Add("Generic building names (prefixes and suffixes required)");

        // Check that all building types have data
        var allBuildingTypes = Enum.GetValues(typeof(BuildingType)).Cast<BuildingType>();
        var missingTypes = allBuildingTypes.Where(t => !_typeData.ContainsKey(t)).ToList();

        if (missingTypes.Any())
        {
            missingFields.Add($"Building type data for: {string.Join(", ", missingTypes)}");
        }

        if (missingFields.Count > 0)
        {
            throw new ArgumentException(
                $"Missing required building name data: {string.Join(", ", missingFields)}. " +
                "All building types must have complete data when creating a new theme.");
        }

        return new BuildingNameData
        {
            GenericPrefixes = _genericPrefixes ?? Array.Empty<string>(),
            GenericSuffixes = _genericSuffixes ?? Array.Empty<string>(),
            TypeData = new Dictionary<BuildingType, BuildingTypeData>(_typeData)
        };
    }

    /// <summary>
    /// Validates that a string array is not null, not empty, and contains only valid strings.
    /// </summary>
    private static void ValidateStringArray(string[] array, string paramName, string fieldName)
    {
        if (array == null)
            throw new ArgumentNullException(paramName, $"{fieldName} cannot be null.");

        if (array.Length == 0)
            throw new ArgumentException($"{fieldName} cannot be empty. At least one element is required.", paramName);

        for (var i = 0; i < array.Length; i++)
        {
            if (array[i] == null)
                throw new ArgumentException($"{fieldName} contains null value at index {i}.", paramName);

            if (string.IsNullOrWhiteSpace(array[i]))
                throw new ArgumentException($"{fieldName} contains whitespace-only value at index {i}.", paramName);
        }
    }

    /// <summary>
    /// Combines two arrays or returns the new array if the existing is null.
    /// </summary>
    private static string[] CombineArrays(string[]? existing, string[] newItems)
    {
        return existing == null ? newItems : existing.Concat(newItems).ToArray();
    }
}
