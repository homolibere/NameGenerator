using NameGeneratorEngine.Enums;

namespace NameGeneratorEngine;

/// <summary>
/// Configuration object for initializing a <see cref="NameGenerator"/> with custom themes and extensions.
/// </summary>
/// <remarks>
/// Use this class to register multiple custom themes and theme extensions in a single configuration,
/// which can then be passed to the <see cref="NameGenerator"/> constructor for initialization.
/// </remarks>
/// <example>
/// <code>
/// var config = new ThemeConfig()
///     .AddTheme("steampunk", steampunkTheme)
///     .ExtendTheme(Theme.Cyberpunk, cyberpunkExtension);
/// 
/// var generator = new NameGenerator(config, seed: 42);
/// </code>
/// </example>
public sealed class ThemeConfig
{
    /// <summary>
    /// Gets the collection of custom themes to be registered.
    /// </summary>
    /// <remarks>
    /// The dictionary key is the theme identifier (case-insensitive),
    /// and the value is the custom theme data.
    /// </remarks>
    public Dictionary<string, CustomThemeData> CustomThemes { get; init; } = new Dictionary<string, CustomThemeData>();

    /// <summary>
    /// Gets the list of theme extensions to be applied.
    /// </summary>
    /// <remarks>
    /// Extensions are applied in the order they appear in this list.
    /// Multiple extensions can be applied to the same base theme.
    /// </remarks>
    public List<ThemeExtension> ThemeExtensions { get; init; } = [];

    /// <summary>
    /// Adds a custom theme to the configuration.
    /// </summary>
    /// <param name="identifier">The unique identifier for the custom theme (case-insensitive).</param>
    /// <param name="themeData">The custom theme data.</param>
    /// <returns>This <see cref="ThemeConfig"/> instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="identifier"/> or <paramref name="themeData"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="identifier"/> is whitespace-only or is a reserved identifier.
    /// </exception>
    /// <remarks>
    /// Reserved identifiers (cyberpunk, elves, orcs) cannot be used for custom themes.
    /// </remarks>
    public ThemeConfig AddTheme(string identifier, CustomThemeData themeData)
    {
        ArgumentNullException.ThrowIfNull(identifier);
        if (string.IsNullOrWhiteSpace(identifier))
            throw new ArgumentException("Theme identifier cannot be whitespace-only.", nameof(identifier));
        ArgumentNullException.ThrowIfNull(themeData);

        CustomThemes[identifier] = themeData;
        return this;
    }

    /// <summary>
    /// Adds a custom theme loaded from a JSON file to the configuration.
    /// </summary>
    /// <param name="identifier">The unique identifier for the custom theme (case-insensitive).</param>
    /// <param name="jsonFilePath">The path to the JSON file containing theme data.</param>
    /// <returns>This <see cref="ThemeConfig"/> instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="identifier"/> or <paramref name="jsonFilePath"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="identifier"/> is whitespace-only or is a reserved identifier.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the file is not found, the JSON is malformed, or the theme data is invalid.
    /// </exception>
    /// <remarks>
    /// This is a convenience method that calls <see cref="CustomThemeData.FromJson(string)"/>
    /// and then <see cref="AddTheme(string, CustomThemeData)"/>.
    /// </remarks>
    public ThemeConfig AddThemeFromJson(string identifier, string jsonFilePath)
    {
        ArgumentNullException.ThrowIfNull(identifier);
        ArgumentNullException.ThrowIfNull(jsonFilePath);

        var themeData = CustomThemeData.FromJson(jsonFilePath);
        return AddTheme(identifier, themeData);
    }

    /// <summary>
    /// Extends a built-in theme with additional data.
    /// </summary>
    /// <param name="baseTheme">The built-in theme to extend.</param>
    /// <param name="extension">The extension data to merge with the base theme.</param>
    /// <returns>This <see cref="ThemeConfig"/> instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="extension"/> is null.</exception>
    /// <remarks>
    /// Multiple extensions can be applied to the same base theme.
    /// Extensions are merged in the order they are added.
    /// </remarks>
    public ThemeConfig ExtendTheme(Theme baseTheme, ThemeExtension extension)
    {
        ArgumentNullException.ThrowIfNull(extension);

        ThemeExtensions.Add(extension);
        return this;
    }

    /// <summary>
    /// Extends a custom theme with additional data.
    /// </summary>
    /// <param name="baseThemeIdentifier">The identifier of the custom theme to extend.</param>
    /// <param name="extension">The extension data to merge with the base theme.</param>
    /// <returns>This <see cref="ThemeConfig"/> instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="baseThemeIdentifier"/> or <paramref name="extension"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="baseThemeIdentifier"/> is whitespace-only.
    /// </exception>
    /// <remarks>
    /// Multiple extensions can be applied to the same base theme.
    /// Extensions are merged in the order they are added.
    /// The base theme must be registered before the extension can be applied.
    /// </remarks>
    public ThemeConfig ExtendTheme(string baseThemeIdentifier, ThemeExtension extension)
    {
        ArgumentNullException.ThrowIfNull(baseThemeIdentifier);
        if (string.IsNullOrWhiteSpace(baseThemeIdentifier))
            throw new ArgumentException("Base theme identifier cannot be whitespace-only.", nameof(baseThemeIdentifier));
        ArgumentNullException.ThrowIfNull(extension);

        ThemeExtensions.Add(extension);
        return this;
    }
}
