using CsCheck;
using FluentAssertions;
using NameGeneratorEngine.Enums;
using Xunit;

namespace NameGeneratorEngine.Tests.Properties;

/// <summary>
/// Property-based tests for faction name organizational structure validation.
/// </summary>
public class FactionOrganizationalStructurePropertyTests
{
    // Valid organizational structure terms for each theme based on the theme JSON files
    private static readonly Dictionary<Theme, HashSet<string>> ValidOrganizationalTerms = new Dictionary<Theme, HashSet<string>>
    {
        {
            Theme.Cyberpunk,
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Corp", "Syndicate", "Collective", "Alliance", "Cartel", "Consortium",
                "Federation", "Union", "League", "Network", "Coalition", "Assembly",
                "Council", "Order", "Guild", "Society", "Brotherhood", "Sisterhood",
                "Circle", "Ring"
            }
        },
        {
            Theme.Elves,
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Council", "Circle", "Order", "Assembly", "Conclave", "Covenant",
                "Alliance", "League", "Union", "Fellowship", "Brotherhood", "Sisterhood",
                "Society", "Guild", "Collective", "Congregation", "Gathering", "Synod",
                "Tribunal", "Court"
            }
        },
        {
            Theme.Orcs,
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "Clan", "Horde", "Warband", "Tribe", "Pack", "Gang", "Mob", "Crew",
                "Band", "Legion", "Army", "Host", "Force", "Troop", "Company",
                "Regiment", "Battalion", "Squad", "Unit", "Group"
            }
        }
    };

    /// <summary>
    /// Feature: name-generator-engine, Property 9: Faction names contain organizational structure terms
    /// For any generated faction name in any theme, the name should contain at least one 
    /// organizational structure term appropriate to that theme (e.g., "Corp", "Syndicate", 
    /// "Collective" for Cyberpunk; "Council", "Circle", "Order" for Elves; "Clan", "Horde", 
    /// "Warband" for Orcs).
    /// Validates: Requirements 8A.1, 8A.2, 8A.3, 8A.4, 8A.5
    /// </summary>
    [Fact]
    public void Property_FactionNamesContainOrganizationalStructureTerms()
    {
        // Generate random seeds and test all themes
        Gen.Int.Sample(seed =>
        {
            var generator = new NameGenerator(seed);

            // Test each theme
            foreach (var theme in Enum.GetValues<Theme>())
            {
                // Generate multiple faction names to ensure consistency
                for (var i = 0; i < 10; i++)
                {
                    var factionName = generator.GenerateFactionName(theme);

                    // Verify the faction name is not null or empty
                    factionName.Should().NotBeNullOrWhiteSpace(
                        $"faction name should not be null or empty for theme {theme}");

                    // Check if the faction name contains at least one valid organizational term for this theme
                    var validTerms = ValidOrganizationalTerms[theme];
                    var containsValidTerm = validTerms.Any(term =>
                        factionName.Contains(term, StringComparison.OrdinalIgnoreCase));

                    containsValidTerm.Should().BeTrue(
                        $"faction name '{factionName}' for theme {theme} should contain one of the valid organizational terms: " +
                        $"{string.Join(", ", validTerms)}");
                }
            }
        }, iter: 100); // Run 100 iterations as specified in the design document
    }

    /// <summary>
    /// Additional property test that verifies faction names always contain organizational terms
    /// across random generation scenarios.
    /// </summary>
    [Fact]
    public void Property_AllFactionNamesHaveOrganizationalTerms()
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
                var validTerms = ValidOrganizationalTerms[theme];

                // Generate multiple faction names
                var factionNames = new List<string>();
                for (var i = 0; i < count; i++)
                {
                    factionNames.Add(generator.GenerateFactionName(theme));
                }

                // Verify all generated names have valid organizational terms
                foreach (var factionName in factionNames)
                {
                    var hasValidTerm = validTerms.Any(term =>
                        factionName.Contains(term, StringComparison.OrdinalIgnoreCase));

                    hasValidTerm.Should().BeTrue(
                        $"faction name '{factionName}' for theme {theme} must contain a valid organizational term");
                }

                // Verify we generated the expected number of names
                factionNames.Should().HaveCount(count,
                    "should generate the requested number of faction names");

                // Verify all names are unique (within this session)
                factionNames.Should().OnlyHaveUniqueItems(
                    "faction names should be unique within a session");
            }, iter: 100);
    }
}
