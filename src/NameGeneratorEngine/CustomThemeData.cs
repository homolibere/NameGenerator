namespace NameGeneratorEngine;

/// <summary>
/// Represents an immutable custom theme that can be registered with the NameGenerator.
/// Custom themes provide complete name generation data for all entity types.
/// </summary>
/// <remarks>
/// Use <see cref="ThemeDataBuilder"/> to construct custom theme data programmatically,
/// or use the static factory methods to load themes from JSON files.
/// </remarks>
public sealed class CustomThemeData
{
    /// <summary>
    /// Gets the internal theme data representation.
    /// </summary>
    internal ThemeData.ThemeData InternalData { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomThemeData"/> class.
    /// </summary>
    /// <param name="data">The internal theme data.</param>
    /// <remarks>
    /// This constructor is internal and should only be called by <see cref="ThemeDataBuilder"/>.
    /// </remarks>
    internal CustomThemeData(ThemeData.ThemeData data)
    {
        InternalData = data ?? throw new ArgumentNullException(nameof(data));
    }

    /// <summary>
    /// Loads custom theme data from a JSON file.
    /// </summary>
    /// <param name="jsonFilePath">The path to the JSON file containing theme data.</param>
    /// <returns>A <see cref="CustomThemeData"/> instance containing the loaded theme data.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="jsonFilePath"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the file is not found, the JSON is malformed, or the theme data is invalid.
    /// </exception>
    /// <remarks>
    /// The JSON file must follow the same format as built-in themes.
    /// See the documentation for the expected JSON structure.
    /// </remarks>
    public static CustomThemeData FromJson(string jsonFilePath)
    {
        ArgumentNullException.ThrowIfNull(jsonFilePath);

        try
        {
            // Read the JSON file
            var jsonContent = File.ReadAllText(jsonFilePath);
            return FromJsonString(jsonContent);
        }
        catch (FileNotFoundException ex)
        {
            throw new InvalidOperationException(
                $"Unable to load custom theme from file '{jsonFilePath}'. " +
                $"The file was not found.", ex);
        }
        catch (DirectoryNotFoundException ex)
        {
            throw new InvalidOperationException(
                $"Unable to load custom theme from file '{jsonFilePath}'. " +
                $"The directory was not found.", ex);
        }
        catch (IOException ex)
        {
            throw new InvalidOperationException(
                $"Unable to load custom theme from file '{jsonFilePath}'. " +
                $"An I/O error occurred: {ex.Message}", ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new InvalidOperationException(
                $"Unable to load custom theme from file '{jsonFilePath}'. " +
                $"Access to the file was denied.", ex);
        }
    }

    /// <summary>
    /// Loads custom theme data from a JSON string.
    /// </summary>
    /// <param name="jsonContent">The JSON string containing theme data.</param>
    /// <returns>A <see cref="CustomThemeData"/> instance containing the loaded theme data.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="jsonContent"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the JSON is malformed or the theme data is invalid.
    /// </exception>
    /// <remarks>
    /// The JSON content must follow the same format as built-in themes.
    /// See the documentation for the expected JSON structure.
    /// </remarks>
    public static CustomThemeData FromJsonString(string jsonContent)
    {
        ArgumentNullException.ThrowIfNull(jsonContent);

        try
        {
            var options = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
            };

            var themeData = System.Text.Json.JsonSerializer.Deserialize<ThemeData.ThemeData>(jsonContent, options);
            
            if (themeData == null)
            {
                throw new InvalidOperationException(
                    "Failed to deserialize custom theme data. " +
                    "The JSON content may be empty or malformed.");
            }

            // Validate the theme data using ThemeProvider's validation logic
            var provider = new ThemeData.ThemeProvider();
            provider.ValidateThemeData(themeData);

            return new CustomThemeData(themeData);
        }
        catch (System.Text.Json.JsonException ex)
        {
            throw new InvalidOperationException(
                $"Failed to parse custom theme data. " +
                $"The JSON content contains invalid syntax or structure: {ex.Message}", ex);
        }
    }
}
