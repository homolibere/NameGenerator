using NameGeneratorEngine.Enums;

namespace NameGeneratorEngine.Exceptions;

/// <summary>
/// Exception thrown when the name generator cannot produce a unique name after the maximum number of attempts.
/// </summary>
public class NamePoolExhaustedException : Exception
{
    /// <summary>
    /// Gets the entity type for which name generation failed.
    /// </summary>
    public EntityType EntityType { get; }

    /// <summary>
    /// Gets the theme being used for name generation.
    /// </summary>
    public Theme Theme { get; }

    /// <summary>
    /// Gets the theme identifier being used for name generation (for custom themes).
    /// </summary>
    public string? ThemeIdentifier { get; }

    /// <summary>
    /// Gets the number of attempts made before exhaustion.
    /// </summary>
    public int Attempts { get; }

    /// <summary>
    /// Initializes a new instance of the NamePoolExhaustedException class.
    /// </summary>
    /// <param name="entityType">The entity type for which generation failed.</param>
    /// <param name="theme">The theme being used.</param>
    /// <param name="attempts">The number of attempts made.</param>
    public NamePoolExhaustedException(EntityType entityType, Theme theme, int attempts)
        : base($"Unable to generate unique {entityType} name for {theme} theme after {attempts} attempts. " +
               $"Consider resetting the session or using a different seed.")
    {
        EntityType = entityType;
        Theme = theme;
        ThemeIdentifier = null;
        Attempts = attempts;
    }

    /// <summary>
    /// Initializes a new instance of the NamePoolExhaustedException class for custom themes.
    /// </summary>
    /// <param name="entityType">The entity type for which generation failed.</param>
    /// <param name="themeIdentifier">The theme identifier being used.</param>
    /// <param name="attempts">The number of attempts made.</param>
    public NamePoolExhaustedException(EntityType entityType, string themeIdentifier, int attempts)
        : base($"Unable to generate unique {entityType} name for '{themeIdentifier}' theme after {attempts} attempts. " +
               $"Consider resetting the session or using a different seed.")
    {
        EntityType = entityType;
        Theme = default; // Not applicable for custom themes
        ThemeIdentifier = themeIdentifier;
        Attempts = attempts;
    }

    /// <summary>
    /// Initializes a new instance of the NamePoolExhaustedException class with a generic theme object.
    /// </summary>
    /// <param name="entityType">The entity type for which generation failed.</param>
    /// <param name="themeObject">The theme object (Theme enum or string identifier).</param>
    /// <param name="attempts">The number of attempts made.</param>
    internal NamePoolExhaustedException(EntityType entityType, object themeObject, int attempts)
        : base($"Unable to generate unique {entityType} name for '{themeObject}' theme after {attempts} attempts. " +
               $"Consider resetting the session or using a different seed.")
    {
        EntityType = entityType;
        
        switch (themeObject)
        {
            case Theme themeEnum:
                Theme = themeEnum;
                ThemeIdentifier = null;
                break;
            case string identifier:
                Theme = default;
                ThemeIdentifier = identifier;
                break;
            default:
                Theme = default;
                ThemeIdentifier = themeObject?.ToString();
                break;
        }
        
        Attempts = attempts;
    }
}
