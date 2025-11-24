using CsCheck;
using FluentAssertions;
using NameGeneratorEngine.Enums;
using Xunit;

namespace NameGeneratorEngine.Tests.Properties;

/// <summary>
/// Property-based tests for session reset with custom themes.
/// </summary>
public class CustomThemeSessionResetPropertyTests
{
    /// <summary>
    /// Feature: custom-theme-registration, Property 11: Session reset for custom themes
    /// For any custom theme, after generating names and calling ResetSession(), previously generated names
    /// should be available for generation again.
    /// Validates: Requirements 4.4
    /// </summary>
    [Fact]
    public void Property_CustomThemeSessionResetAllowsNameReuse()
    {
        // Generate random test scenarios
        var genSeed = Gen.Int;
        var genEntityType = Gen.Int[0, 5].Select(i => (EntityType)i); // EntityType has 6 values: 0-5
        var genCount = Gen.Int[5, 15]; // Generate between 5 and 15 names

        Gen.Select(genSeed, genEntityType, genCount)
            .Sample(tuple =>
            {
                var (seed, entityType, count) = tuple;

                var customTheme = CreateSimpleCustomTheme();
                var config = new ThemeConfig().AddTheme("test-theme", customTheme);
                var generator = new NameGenerator(config, seed);

                // Generate names in first session
                var firstSessionNames = new List<string>();
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
                    firstSessionNames.Add(name);
                }

                // Reset session
                generator.ResetSession();

                // Generate names in second session
                var secondSessionNames = new List<string>();
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
                    secondSessionNames.Add(name);
                }

                // Verify that the same names are generated after reset (determinism maintained)
                secondSessionNames.Should().Equal(firstSessionNames,
                    $"session reset should allow name reuse and maintain determinism for {entityType} names in custom themes");
            }, iter: 100);
    }

    /// <summary>
    /// Property test that verifies session reset clears tracking for all entity types.
    /// </summary>
    [Fact]
    public void Property_CustomThemeSessionResetClearsAllEntityTypes()
    {
        var genSeed = Gen.Int;
        var genCount = Gen.Int[3, 8];

        Gen.Select(genSeed, genCount)
            .Sample(tuple =>
            {
                var (seed, count) = tuple;

                var customTheme = CreateSimpleCustomTheme();
                var config = new ThemeConfig().AddTheme("test-theme", customTheme);
                var generator = new NameGenerator(config, seed);

                // Generate names for all entity types
                var firstSessionNames = new Dictionary<EntityType, List<string>>();
                foreach (var entityType in Enum.GetValues<EntityType>())
                {
                    var names = new List<string>();
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
                        names.Add(name);
                    }
                    firstSessionNames[entityType] = names;
                }

                // Reset session
                generator.ResetSession();

                // Generate names again for all entity types
                var secondSessionNames = new Dictionary<EntityType, List<string>>();
                foreach (var entityType in Enum.GetValues<EntityType>())
                {
                    var names = new List<string>();
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
                        names.Add(name);
                    }
                    secondSessionNames[entityType] = names;
                }

                // Verify all entity types produce the same names after reset
                foreach (var entityType in Enum.GetValues<EntityType>())
                {
                    secondSessionNames[entityType].Should().Equal(firstSessionNames[entityType],
                        $"session reset should clear tracking for {entityType} in custom themes");
                }
            }, iter: 100);
    }

    /// <summary>
    /// Helper method to create a simple custom theme with sufficient variety for testing.
    /// </summary>
    private static CustomThemeData CreateSimpleCustomTheme()
    {
        var builder = new ThemeDataBuilder();

        builder.WithNpcNames(npc =>
        {
            npc.WithMaleNames(
                GenerateTestArray("M", 20),
                GenerateTestArray("core", 20),
                GenerateTestArray("son", 20));
            npc.WithFemaleNames(
                GenerateTestArray("F", 20),
                GenerateTestArray("core", 20),
                GenerateTestArray("dottir", 20));
            npc.WithNeutralNames(
                GenerateTestArray("N", 20),
                GenerateTestArray("core", 20),
                GenerateTestArray("x", 20));
        });

        builder.WithBuildingNames(building =>
        {
            building.WithGenericNames(
                GenerateTestArray("The", 20),
                GenerateTestArray("Building", 20));

            foreach (var buildingType in Enum.GetValues<BuildingType>())
            {
                building.WithTypeNames(
                    buildingType,
                    GenerateTestArray($"{buildingType}", 20),
                    GenerateTestArray("desc", 20),
                    GenerateTestArray("suffix", 20));
            }
        });

        builder.WithCityNames(
            GenerateTestArray("City", 20),
            GenerateTestArray("core", 20),
            GenerateTestArray("ville", 20));

        builder.WithDistrictNames(
            GenerateTestArray("Old", 20),
            GenerateTestArray("District", 20));

        builder.WithStreetNames(
            GenerateTestArray("Main", 20),
            GenerateTestArray("core", 20),
            new[] { "Street", "Avenue", "Boulevard", "Road", "Way", "Lane", "Drive", "Court", "Place", "Circle" });

        builder.WithFactionNames(
            GenerateTestArray("The", 20),
            GenerateTestArray("core", 20),
            GenerateTestArray("Guild", 20));

        return builder.Build();
    }

    private static string[] GenerateTestArray(string prefix, int count)
    {
        return Enumerable.Range(1, count)
            .Select(i => $"{prefix}{i}")
            .ToArray();
    }
}
