using CsCheck;
using FluentAssertions;
using NameGeneratorEngine.Enums;
using NameGeneratorEngine.Exceptions;
using Xunit;

namespace NameGeneratorEngine.Tests.Properties;

/// <summary>
/// Property-based tests for pool exhaustion with custom themes.
/// </summary>
public class CustomThemePoolExhaustionPropertyTests
{
    /// <summary>
    /// Feature: custom-theme-registration, Property 12: Pool exhaustion for custom themes
    /// For any custom theme with limited data, attempting to generate more unique names than possible
    /// should throw NamePoolExhaustedException after 1000 attempts.
    /// Validates: Requirements 4.5
    /// </summary>
    [Fact]
    public void Property_CustomThemePoolExhaustionThrowsException()
    {
        // Generate random test scenarios
        var genSeed = Gen.Int;
        var genEntityType = Gen.Int[0, 5].Select(i => (EntityType)i);

        Gen.Select(genSeed, genEntityType)
            .Sample(tuple =>
            {
                var (seed, entityType) = tuple;

                // Create a custom theme with very limited data (only 2 possible combinations)
                var limitedTheme = CreateLimitedCustomTheme();
                var config = new ThemeConfig().AddTheme("limited-theme", limitedTheme);
                var generator = new NameGenerator(config, seed);

                // Try to generate more names than possible
                var generatedNames = new List<string>();
                var generateExcessiveNames = () =>
                {
                    // Try to generate way more names than the limited pool allows
                    for (var i = 0; i < 1500; i++)
                    {
                        var name = entityType switch
                        {
                            EntityType.Npc => generator.GenerateNpcName("limited-theme"),
                            EntityType.Building => generator.GenerateBuildingName("limited-theme"),
                            EntityType.City => generator.GenerateCityName("limited-theme"),
                            EntityType.District => generator.GenerateDistrictName("limited-theme"),
                            EntityType.Street => generator.GenerateStreetName("limited-theme"),
                            EntityType.Faction => generator.GenerateFactionName("limited-theme"),
                            _ => throw new InvalidOperationException($"Unknown entity type: {entityType}")
                        };
                        generatedNames.Add(name);
                    }
                };

                // Verify that NamePoolExhaustedException is thrown
                generateExcessiveNames.Should().Throw<NamePoolExhaustedException>()
                    .Which.EntityType.Should().Be(entityType);
            }, iter: 100);
    }

    /// <summary>
    /// Helper method to create a custom theme with very limited data for pool exhaustion testing.
    /// </summary>
    private static CustomThemeData CreateLimitedCustomTheme()
    {
        var builder = new ThemeDataBuilder();

        // Very limited NPC names (only 2 possible combinations per gender)
        builder.WithNpcNames(npc =>
        {
            npc.WithMaleNames(
                new[] { "A", "B" },
                new[] { "x" },
                new[] { "1" });
            npc.WithFemaleNames(
                new[] { "C", "D" },
                new[] { "y" },
                new[] { "2" });
            npc.WithNeutralNames(
                new[] { "E", "F" },
                new[] { "z" },
                new[] { "3" });
        });

        // Limited building names
        builder.WithBuildingNames(building =>
        {
            building.WithGenericNames(
                new[] { "The", "A" },
                new[] { "Place" });

            foreach (var buildingType in Enum.GetValues<BuildingType>())
            {
                building.WithTypeNames(
                    buildingType,
                    new[] { "X", "Y" },
                    new[] { "desc" },
                    new[] { "1" });
            }
        });

        // Limited city names
        builder.WithCityNames(
            new[] { "New", "Old" },
            new[] { "City" },
            new[] { "ton" });

        // Limited district names
        builder.WithDistrictNames(
            new[] { "North", "South" },
            new[] { "District" });

        // Limited street names
        builder.WithStreetNames(
            new[] { "Main", "First" },
            new[] { "St" },
            new[] { "Street" });

        // Limited faction names
        builder.WithFactionNames(
            new[] { "The", "A" },
            new[] { "Guild" },
            new[] { "Order" });

        return builder.Build();
    }
}
