using CsCheck;
using FluentAssertions;
using NameGeneratorEngine.Enums;
using Xunit;

namespace NameGeneratorEngine.Tests.Properties;

/// <summary>
/// Property-based tests for street name suffix validation.
/// </summary>
public class StreetSuffixPropertyTests
{
    // Valid street suffixes for each theme based on the theme JSON files
    private static readonly Dictionary<Theme, HashSet<string>> ValidStreetSuffixes = new Dictionary<Theme, HashSet<string>>
    {
        {
            Theme.Cyberpunk,
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Street", "Avenue", "Boulevard", "Road", "Drive", "Way", "Path", "Lane",
                "Court", "Place", "Terrace", "Circle", "Loop", "Plaza", "Square", "Parkway",
                "Highway", "Expressway", "Freeway", "Corridor"
            }
        },
        {
            Theme.Elves,
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Path", "Way", "Lane", "Walk", "Trail", "Road", "Avenue", "Boulevard",
                "Drive", "Court", "Place", "Terrace", "Circle", "Grove", "Glade", "Green",
                "Commons", "Promenade", "Esplanade", "Passage"
            }
        },
        {
            Theme.Orcs,
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Way", "Path", "Road", "Trail", "Track", "Route", "Pass", "Passage",
                "Alley", "Lane", "Street", "Avenue", "Boulevard", "Drive", "Court",
                "Place", "Row", "Walk", "Run", "March"
            }
        }
    };

    /// <summary>
    /// Feature: name-generator-engine, Property 7: Street names contain valid suffixes
    /// For any generated street name in any theme, the name should contain at least one 
    /// of the valid street suffixes defined for that theme (Street, Avenue, Boulevard, 
    /// Road, Way, Path, etc.).
    /// Validates: Requirements 8.5
    /// </summary>
    [Fact]
    public void Property_StreetNamesContainValidSuffixes()
    {
        // Generate random seeds and test all themes
        Gen.Int.Sample(seed =>
        {
            var generator = new NameGenerator(seed);

            // Test each theme
            foreach (var theme in Enum.GetValues<Theme>())
            {
                // Generate multiple street names to ensure consistency
                for (var i = 0; i < 10; i++)
                {
                    var streetName = generator.GenerateStreetName(theme);

                    // Verify the street name is not null or empty
                    streetName.Should().NotBeNullOrWhiteSpace(
                        $"street name should not be null or empty for theme {theme}");

                    // Check if the street name contains at least one valid suffix for this theme
                    var validSuffixes = ValidStreetSuffixes[theme];
                    var containsValidSuffix = validSuffixes.Any(suffix =>
                        streetName.EndsWith(suffix, StringComparison.OrdinalIgnoreCase));

                    containsValidSuffix.Should().BeTrue(
                        $"street name '{streetName}' for theme {theme} should end with one of the valid suffixes: " +
                        $"{string.Join(", ", validSuffixes)}");
                }
            }
        }, iter: 100); // Run 100 iterations as specified in the design document
    }

    /// <summary>
    /// Additional property test that verifies street names always end with a suffix
    /// (no street names should be generated without a proper suffix).
    /// </summary>
    [Fact]
    public void Property_AllStreetNamesHaveSuffixes()
    {
        // Generate random test scenarios
        var genSeed = Gen.Int;
        var genTheme = Gen.Int[0, 2].Select(i => (Theme)i); // Theme enum has values 0-2 (Cyberpunk, Elves, Orcs)
        var genCount = Gen.Int[5, 20]; // Generate between 5 and 20 names per test

        Gen.Select(genSeed, genTheme, genCount)
            .Sample(tuple =>
            {
                var (seed, theme, count) = tuple;
                var generator = new NameGenerator(seed);
                var validSuffixes = ValidStreetSuffixes[theme];

                // Generate multiple street names
                var streetNames = new List<string>();
                for (var i = 0; i < count; i++)
                {
                    streetNames.Add(generator.GenerateStreetName(theme));
                }

                // Verify all generated names have valid suffixes
                foreach (var streetName in streetNames)
                {
                    var hasValidSuffix = validSuffixes.Any(suffix =>
                        streetName.EndsWith(suffix, StringComparison.OrdinalIgnoreCase));

                    hasValidSuffix.Should().BeTrue(
                        $"street name '{streetName}' for theme {theme} must end with a valid suffix");
                }

                // Verify we generated the expected number of names
                streetNames.Should().HaveCount(count,
                    "should generate the requested number of street names");

                // Verify all names are unique (within this session)
                streetNames.Should().OnlyHaveUniqueItems(
                    "street names should be unique within a session");
            }, iter: 100);
    }
}
