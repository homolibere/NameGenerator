using NameGeneratorEngine.ThemeData.DataStructures;

namespace NameGeneratorEngine;

/// <summary>
/// Provides a fluent interface for building NPC name data.
/// </summary>
/// <remarks>
/// This builder supports two modes:
/// - For new themes: Use With*Names() methods to provide complete gender data
/// - For extensions: Use Add*Prefixes/Cores/Suffixes() methods to add to existing data
/// </remarks>
public class NpcNameDataBuilder
{
    private readonly bool _isExtension;
    
    private string[]? _malePrefixes;
    private string[]? _maleCores;
    private string[]? _maleSuffixes;
    
    private string[]? _femalePrefixes;
    private string[]? _femaleCores;
    private string[]? _femaleSuffixes;
    
    private string[]? _neutralPrefixes;
    private string[]? _neutralCores;
    private string[]? _neutralSuffixes;

    /// <summary>
    /// Initializes a new instance of the <see cref="NpcNameDataBuilder"/> class.
    /// </summary>
    /// <param name="isExtension">Whether this builder is for a theme extension.</param>
    internal NpcNameDataBuilder(bool isExtension)
    {
        _isExtension = isExtension;
    }

    #region Complete Gender Data Methods (for new themes)

    /// <summary>
    /// Configures complete male name data.
    /// </summary>
    /// <param name="prefixes">The array of prefix syllables for male names.</param>
    /// <param name="cores">The array of core syllables for male names.</param>
    /// <param name="suffixes">The array of suffix syllables for male names.</param>
    /// <returns>This builder instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    /// <exception cref="ArgumentException">Thrown when any array is empty or contains invalid strings.</exception>
    public NpcNameDataBuilder WithMaleNames(string[] prefixes, string[] cores, string[] suffixes)
    {
        ValidateStringArray(prefixes, nameof(prefixes), "Male name prefixes");
        ValidateStringArray(cores, nameof(cores), "Male name cores");
        ValidateStringArray(suffixes, nameof(suffixes), "Male name suffixes");

        _malePrefixes = prefixes.ToArray(); // Copy array to ensure immutability
        _maleCores = cores.ToArray();
        _maleSuffixes = suffixes.ToArray();
        return this;
    }

    /// <summary>
    /// Configures complete female name data.
    /// </summary>
    /// <param name="prefixes">The array of prefix syllables for female names.</param>
    /// <param name="cores">The array of core syllables for female names.</param>
    /// <param name="suffixes">The array of suffix syllables for female names.</param>
    /// <returns>This builder instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    /// <exception cref="ArgumentException">Thrown when any array is empty or contains invalid strings.</exception>
    public NpcNameDataBuilder WithFemaleNames(string[] prefixes, string[] cores, string[] suffixes)
    {
        ValidateStringArray(prefixes, nameof(prefixes), "Female name prefixes");
        ValidateStringArray(cores, nameof(cores), "Female name cores");
        ValidateStringArray(suffixes, nameof(suffixes), "Female name suffixes");

        _femalePrefixes = prefixes.ToArray(); // Copy array to ensure immutability
        _femaleCores = cores.ToArray();
        _femaleSuffixes = suffixes.ToArray();
        return this;
    }

    /// <summary>
    /// Configures complete neutral name data.
    /// </summary>
    /// <param name="prefixes">The array of prefix syllables for neutral names.</param>
    /// <param name="cores">The array of core syllables for neutral names.</param>
    /// <param name="suffixes">The array of suffix syllables for neutral names.</param>
    /// <returns>This builder instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    /// <exception cref="ArgumentException">Thrown when any array is empty or contains invalid strings.</exception>
    public NpcNameDataBuilder WithNeutralNames(string[] prefixes, string[] cores, string[] suffixes)
    {
        ValidateStringArray(prefixes, nameof(prefixes), "Neutral name prefixes");
        ValidateStringArray(cores, nameof(cores), "Neutral name cores");
        ValidateStringArray(suffixes, nameof(suffixes), "Neutral name suffixes");

        _neutralPrefixes = prefixes.ToArray(); // Copy array to ensure immutability
        _neutralCores = cores.ToArray();
        _neutralSuffixes = suffixes.ToArray();
        return this;
    }

    #endregion

    #region Extension Methods (for theme extensions)

    /// <summary>
    /// Adds male name prefixes to the extension.
    /// </summary>
    /// <param name="prefixes">The prefix syllables to add.</param>
    /// <returns>This builder instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="prefixes"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the array is empty or contains invalid strings.</exception>
    public NpcNameDataBuilder AddMalePrefixes(params string[] prefixes)
    {
        ValidateStringArray(prefixes, nameof(prefixes), "Male name prefixes");
        _malePrefixes = CombineArrays(_malePrefixes, prefixes.ToArray()); // Copy array to ensure immutability
        return this;
    }

    /// <summary>
    /// Adds male name cores to the extension.
    /// </summary>
    /// <param name="cores">The core syllables to add.</param>
    /// <returns>This builder instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="cores"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the array is empty or contains invalid strings.</exception>
    public NpcNameDataBuilder AddMaleCores(params string[] cores)
    {
        ValidateStringArray(cores, nameof(cores), "Male name cores");
        _maleCores = CombineArrays(_maleCores, cores.ToArray()); // Copy array to ensure immutability
        return this;
    }

    /// <summary>
    /// Adds male name suffixes to the extension.
    /// </summary>
    /// <param name="suffixes">The suffix syllables to add.</param>
    /// <returns>This builder instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="suffixes"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the array is empty or contains invalid strings.</exception>
    public NpcNameDataBuilder AddMaleSuffixes(params string[] suffixes)
    {
        ValidateStringArray(suffixes, nameof(suffixes), "Male name suffixes");
        _maleSuffixes = CombineArrays(_maleSuffixes, suffixes.ToArray()); // Copy array to ensure immutability
        return this;
    }

    /// <summary>
    /// Adds female name prefixes to the extension.
    /// </summary>
    /// <param name="prefixes">The prefix syllables to add.</param>
    /// <returns>This builder instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="prefixes"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the array is empty or contains invalid strings.</exception>
    public NpcNameDataBuilder AddFemalePrefixes(params string[] prefixes)
    {
        ValidateStringArray(prefixes, nameof(prefixes), "Female name prefixes");
        _femalePrefixes = CombineArrays(_femalePrefixes, prefixes.ToArray()); // Copy array to ensure immutability
        return this;
    }

    /// <summary>
    /// Adds female name cores to the extension.
    /// </summary>
    /// <param name="cores">The core syllables to add.</param>
    /// <returns>This builder instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="cores"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the array is empty or contains invalid strings.</exception>
    public NpcNameDataBuilder AddFemaleCores(params string[] cores)
    {
        ValidateStringArray(cores, nameof(cores), "Female name cores");
        _femaleCores = CombineArrays(_femaleCores, cores.ToArray()); // Copy array to ensure immutability
        return this;
    }

    /// <summary>
    /// Adds female name suffixes to the extension.
    /// </summary>
    /// <param name="suffixes">The suffix syllables to add.</param>
    /// <returns>This builder instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="suffixes"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the array is empty or contains invalid strings.</exception>
    public NpcNameDataBuilder AddFemaleSuffixes(params string[] suffixes)
    {
        ValidateStringArray(suffixes, nameof(suffixes), "Female name suffixes");
        _femaleSuffixes = CombineArrays(_femaleSuffixes, suffixes.ToArray()); // Copy array to ensure immutability
        return this;
    }

    /// <summary>
    /// Adds neutral name prefixes to the extension.
    /// </summary>
    /// <param name="prefixes">The prefix syllables to add.</param>
    /// <returns>This builder instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="prefixes"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the array is empty or contains invalid strings.</exception>
    public NpcNameDataBuilder AddNeutralPrefixes(params string[] prefixes)
    {
        ValidateStringArray(prefixes, nameof(prefixes), "Neutral name prefixes");
        _neutralPrefixes = CombineArrays(_neutralPrefixes, prefixes.ToArray()); // Copy array to ensure immutability
        return this;
    }

    /// <summary>
    /// Adds neutral name cores to the extension.
    /// </summary>
    /// <param name="cores">The core syllables to add.</param>
    /// <returns>This builder instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="cores"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the array is empty or contains invalid strings.</exception>
    public NpcNameDataBuilder AddNeutralCores(params string[] cores)
    {
        ValidateStringArray(cores, nameof(cores), "Neutral name cores");
        _neutralCores = CombineArrays(_neutralCores, cores.ToArray()); // Copy array to ensure immutability
        return this;
    }

    /// <summary>
    /// Adds neutral name suffixes to the extension.
    /// </summary>
    /// <param name="suffixes">The suffix syllables to add.</param>
    /// <returns>This builder instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="suffixes"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the array is empty or contains invalid strings.</exception>
    public NpcNameDataBuilder AddNeutralSuffixes(params string[] suffixes)
    {
        ValidateStringArray(suffixes, nameof(suffixes), "Neutral name suffixes");
        _neutralSuffixes = CombineArrays(_neutralSuffixes, suffixes.ToArray()); // Copy array to ensure immutability
        return this;
    }

    #endregion

    /// <summary>
    /// Builds the NPC name data.
    /// </summary>
    /// <returns>A <see cref="NpcNameData"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when required data is missing for a new theme.</exception>
    internal NpcNameData Build()
    {
        if (_isExtension)
        {
            return new NpcNameData
            {
                Male = new GenderNameData
                {
                    Prefixes = _malePrefixes ?? Array.Empty<string>(),
                    Cores = _maleCores ?? Array.Empty<string>(),
                    Suffixes = _maleSuffixes ?? Array.Empty<string>()
                },
                Female = new GenderNameData
                {
                    Prefixes = _femalePrefixes ?? Array.Empty<string>(),
                    Cores = _femaleCores ?? Array.Empty<string>(),
                    Suffixes = _femaleSuffixes ?? Array.Empty<string>()
                },
                Neutral = new GenderNameData
                {
                    Prefixes = _neutralPrefixes ?? Array.Empty<string>(),
                    Cores = _neutralCores ?? Array.Empty<string>(),
                    Suffixes = _neutralSuffixes ?? Array.Empty<string>()
                }
            };
        }

        // For new themes, all gender data must be complete
        var missingFields = new List<string>();

        if (_malePrefixes == null || _maleCores == null || _maleSuffixes == null)
            missingFields.Add("Male names (prefixes, cores, and suffixes required)");
        if (_femalePrefixes == null || _femaleCores == null || _femaleSuffixes == null)
            missingFields.Add("Female names (prefixes, cores, and suffixes required)");
        if (_neutralPrefixes == null || _neutralCores == null || _neutralSuffixes == null)
            missingFields.Add("Neutral names (prefixes, cores, and suffixes required)");

        if (missingFields.Count > 0)
        {
            throw new ArgumentException(
                $"Missing required NPC name data: {string.Join(", ", missingFields)}. " +
                "All genders must have complete data (prefixes, cores, and suffixes) when creating a new theme.");
        }

        return new NpcNameData
        {
            Male = new GenderNameData
            {
                Prefixes = _malePrefixes ?? Array.Empty<string>(),
                Cores = _maleCores ?? Array.Empty<string>(),
                Suffixes = _maleSuffixes ?? Array.Empty<string>()
            },
            Female = new GenderNameData
            {
                Prefixes = _femalePrefixes ?? Array.Empty<string>(),
                Cores = _femaleCores ?? Array.Empty<string>(),
                Suffixes = _femaleSuffixes ?? Array.Empty<string>()
            },
            Neutral = new GenderNameData
            {
                Prefixes = _neutralPrefixes ?? Array.Empty<string>(),
                Cores = _neutralCores ?? Array.Empty<string>(),
                Suffixes = _neutralSuffixes ?? Array.Empty<string>()
            }
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
    /// Combines two arrays, or returns the new array if the existing is null.
    /// </summary>
    private static string[] CombineArrays(string[]? existing, string[] newItems)
    {
        return existing == null ? newItems : existing.Concat(newItems).ToArray();
    }
}
