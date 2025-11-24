using CsCheck;
using FluentAssertions;
using NameGeneratorEngine.Enums;
using Xunit;

namespace NameGeneratorEngine.Tests.Properties;

/// <summary>
/// Property-based tests for custom theme duplicate prevention.
/// </summary>
public class CustomThemeDuplicatePreventionPropertyTests
{
    /// <summary>
    /// Feature: custom-theme-registration, Property 9: Custom theme duplicate prevention
    /// For any custom theme, generating multiple names should never produce duplicates within the same session,
    /// consistent with built-in theme behavior.
    /// Validates: Requirements 4.2
    /// </summary>
    [Fact]
    public void Property_CustomThemeNoDuplicatesWithinSession()
    {
        // Generate random test scenarios
        var genSeed = Gen.Int;
        var genEntityType = Gen.Int[0, 5].Select(i => (EntityType)i); // EntityType has 6 values: 0-5
        var genCount = Gen.Int[10, 50]; // Generate between 10 and 50 names

        Gen.Select(genSeed, genEntityType, genCount)
            .Sample(tuple =>
            {
                var (seed, entityType, count) = tuple;

                // Create a simple custom theme with sufficient data
                var customTheme = CreateSimpleCustomTheme();
                
                var config = new ThemeConfig()
                    .AddTheme("test-theme", customTheme);
                
                var generator = new NameGenerator(config, seed);
                var generatedNames = new List<string>();

                // Generate multiple names for the same entity type using custom theme
                for (var i = 0; i < count; i++)
                {
                    var name = entityType switch
                    {
                        EntityType.Npc => generator.GenerateNpcName("test-theme"),
                        EntityType.Building => generator.GenerateBuildingName("test-theme"),
                        EntityType.City => generator.GenerateCityName("test-theme"),
                        EntityType.District => generator.GenerateDistrictName("test-theme"),
                        EntityType.Street => generator.GenerateStreetName("test-theme"),
                        EntityType.Faction => generator.GenerateFactionName("test-theme"),
                        _ => throw new InvalidOperationException($"Unknown entity type: {entityType}")
                    };

                    generatedNames.Add(name);
                }

                // Verify all names are unique
                var uniqueNames = generatedNames.Distinct().ToList();
                uniqueNames.Should().HaveCount(generatedNames.Count,
                    $"all {entityType} names from custom theme should be unique within a session");
            }, iter: 100);
    }

    /// <summary>
    /// Property test that verifies custom themes maintain uniqueness across multiple entity types.
    /// </summary>
    [Fact]
    public void Property_CustomThemeNoDuplicatesAcrossEntityTypes()
    {
        var genSeed = Gen.Int;
        var genCount = Gen.Int[5, 15]; // Generate fewer names per entity type

        Gen.Select(genSeed, genCount)
            .Sample(tuple =>
            {
                var (seed, countPerType) = tuple;

                var customTheme = CreateSimpleCustomTheme();
                var config = new ThemeConfig()
                    .AddTheme("test-theme", customTheme);
                
                var generator = new NameGenerator(config, seed);
                var allGeneratedNames = new Dictionary<EntityType, List<string>>();

                // Generate names for all entity types
                foreach (var entityType in Enum.GetValues<EntityType>())
                {
                    var names = new List<string>();
                    for (var i = 0; i < countPerType; i++)
                    {
                        var name = entityType switch
                        {
                            EntityType.Npc => generator.GenerateNpcName("test-theme"),
                            EntityType.Building => generator.GenerateBuildingName("test-theme"),
                            EntityType.City => generator.GenerateCityName("test-theme"),
                            EntityType.District => generator.GenerateDistrictName("test-theme"),
                            EntityType.Street => generator.GenerateStreetName("test-theme"),
                            EntityType.Faction => generator.GenerateFactionName("test-theme"),
                            _ => throw new InvalidOperationException($"Unknown entity type: {entityType}")
                        };

                        names.Add(name);
                    }
                    allGeneratedNames[entityType] = names;
                }

                // Verify uniqueness within each entity type
                foreach (var kvp in allGeneratedNames)
                {
                    var uniqueNames = kvp.Value.Distinct().ToList();
                    uniqueNames.Should().HaveCount(kvp.Value.Count,
                        $"all {kvp.Key} names from custom theme should be unique within a session");
                }
            }, iter: 100);
    }

    /// <summary>
    /// Helper method to create a simple custom theme with sufficient variety for testing.
    /// </summary>
    private static CustomThemeData CreateSimpleCustomTheme()
    {
        var builder = new ThemeDataBuilder();

        // Add NPC names with sufficient variety
        builder.WithNpcNames(npc =>
        {
            npc.WithMaleNames(
                GenerateTestArray("M", 30),
                GenerateTestArray("core", 30),
                GenerateTestArray("son", 30));
            npc.WithFemaleNames(
                GenerateTestArray("F", 30),
                GenerateTestArray("core", 30),
                GenerateTestArray("dottir", 30));
            npc.WithNeutralNames(
                GenerateTestArray("N", 30),
                GenerateTestArray("core", 30),
                GenerateTestArray("x", 30));
        });

        // Add building names
        builder.WithBuildingNames(building =>
        {
            building.WithGenericNames(
                GenerateTestArray("The", 30),
                GenerateTestArray("Building", 30));

            foreach (var buildingType in Enum.GetValues<BuildingType>())
            {
                building.WithTypeNames(
                    buildingType,
                    GenerateTestArray($"{buildingType}", 30),
                    GenerateTestArray("desc", 30),
                    GenerateTestArray("suffix", 30));
            }
        });

        // Add city names
        builder.WithCityNames(
            GenerateTestArray("City", 30),
            GenerateTestArray("core", 30),
            GenerateTestArray("ville", 30));

        // Add district names
        builder.WithDistrictNames(
            GenerateTestArray("Old", 30),
            GenerateTestArray("District", 30));

        // Add street names
        builder.WithStreetNames(
            GenerateTestArray("Main", 30),
            GenerateTestArray("core", 30),
            new[] { "Street", "Avenue", "Boulevard", "Road", "Way", "Lane", "Drive", "Court", "Place", "Circle" });

        // Add faction names
        builder.WithFactionNames(
            GenerateTestArray("The", 30),
            GenerateTestArray("core", 30),
            GenerateTestArray("Guild", 30));

        return builder.Build();
    }

    /// <summary>
    /// Generates an array of test strings with unique suffixes.
    /// </summary>
    private static string[] GenerateTestArray(string prefix, int count)
    {
        return Enumerable.Range(1, count)
            .Select(i => $"{prefix}{i}")
            .ToArray();
    }
}
