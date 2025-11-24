namespace NameGeneratorEngine;

/// <summary>
/// Represents an immutable extension to an existing theme.
/// Theme extensions allow adding additional name generation data to built-in or custom themes
/// without replacing the entire theme.
/// </summary>
/// <remarks>
/// Use <see cref="ThemeDataBuilder.Extend(Enums.Theme)"/> or 
/// <see cref="ThemeDataBuilder.Extend(string)"/> to create theme extensions.
/// Extensions only need to provide data for the entity types being extended.
/// </remarks>
public sealed class ThemeExtension
{
    /// <summary>
    /// Gets the identifier of the base theme being extended.
    /// </summary>
    internal string BaseThemeIdentifier { get; }

    /// <summary>
    /// Gets the extension data to be merged with the base theme.
    /// </summary>
    internal ThemeData.ThemeData ExtensionData { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ThemeExtension"/> class.
    /// </summary>
    /// <param name="baseThemeIdentifier">The identifier of the base theme being extended.</param>
    /// <param name="extensionData">The extension data to be merged with the base theme.</param>
    /// <remarks>
    /// This constructor is internal and should only be called by <see cref="ThemeDataBuilder"/>.
    /// </remarks>
    internal ThemeExtension(string baseThemeIdentifier, ThemeData.ThemeData extensionData)
    {
        if (string.IsNullOrWhiteSpace(baseThemeIdentifier))
            throw new ArgumentException("Base theme identifier cannot be null or whitespace.", nameof(baseThemeIdentifier));

        BaseThemeIdentifier = baseThemeIdentifier;
        ExtensionData = extensionData ?? throw new ArgumentNullException(nameof(extensionData));
    }
}
