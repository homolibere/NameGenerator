using CsCheck;
using FluentAssertions;
using NameGeneratorEngine.Enums;
using Xunit;

namespace NameGeneratorEngine.Tests.Properties;

/// <summary>
/// Property-based tests for custom theme determinism.
/// </summary>
public class CustomThemeDeterminismPropertyTests
{
    /// <summary>
    /// Feature: custom-theme-registration, Property 10: Custom theme determinism
    /// For any two NameGenerator instances with the same seed and the same registered custom theme,
    /// generating names should produce identical sequences.
    /// Validates: Requirements 4.3
    /// </summary>
    [Fact]
    public void Property_CustomThemeDeterministicGeneration()
    {
        // Generate random test scenarios
        var genSeed = Gen.Int;
        var genEntityType = Gen.Int[0, 5].Select(i => (EntityType)i); // EntityType has 6 values: 0-5
        var genCount = Gen.Int[5, 20]; // Generate between 5 and 20 names

        Gen.Select(genSeed, genEntityType, genCount)
            .Sample(tuple =>
            {
                var (seed, entityType, count) = tuple;

                // Create a custom theme
                var customTheme = CreateSimpleCustomTheme();
                
                // Create two generators with the same seed and custom theme
                var config1 = new ThemeConfig().AddTheme("test-theme", customTheme);
                var config2 = new ThemeConfig().AddTheme("test-theme", customTheme);
                
                var generator1 = new NameGenerator(config1, seed);
                var generator2 = new NameGenerator(config2, seed);

                // Generate names from both generators
                var names1 = new List<string>();
                var names2 = new List<string>();

                for (var i = 0; i < count; i++)
                {
                    var name1 = entityType switch
                    {
                        EntityType.Npc => generator1.GenerateNpcName("test-theme"),
                        EntityType.Building => generator1.GenerateBuildingName("test-theme"),
                        EntityType.City => generator1.GenerateCityName("test-theme"),
                        EntityType.District => generator1.GenerateDistrictName("test-theme"),
                        EntityType.Street => generator1.GenerateStreetName("test-theme"),
                        EntityType.Faction => generator1.GenerateFactionName("test-theme"),
                        _ => throw new InvalidOperationException($"Unknown entity type: {entityType}")
                    };

                    var name2 = entityType switch
                    {
                        EntityType.Npc => generator2.GenerateNpcName("test-theme"),
                        EntityType.Building => generator2.GenerateBuildingName("test-theme"),
                        EntityType.City => generator2.GenerateCityName("test-theme"),
                        EntityType.District => generator2.GenerateDistrictName("test-theme"),
                        EntityType.Street => generator2.GenerateStreetName("test-theme"),
                        EntityType.Faction => generator2.GenerateFactionName("test-theme"),
                        _ => throw new InvalidOperationException($"Unknown entity type: {entityType}")
                    };

                    names1.Add(name1);
                    names2.Add(name2);
                }

                // Verify both generators produced identical sequences
                names1.Should().Equal(names2,
                    $"generators with the same seed should produce identical {entityType} name sequences for custom themes");
            }, iter: 100);
    }

    /// <summary>
    /// Property test that verifies determinism with mixed entity types and parameters.
    /// </summary>
    [Fact]
    public void Property_CustomThemeDeterministicWithMixedParameters()
    {
        var genSeed = Gen.Int;
        var genCount = Gen.Int[5, 15];

        Gen.Select(genSeed, genCount)
            .Sample(tuple =>
            {
                var (seed, count) = tuple;

                var customTheme = CreateSimpleCustomTheme();
                
                var config1 = new ThemeConfig().AddTheme("test-theme", customTheme);
                var config2 = new ThemeConfig().AddTheme("test-theme", customTheme);
                
                var generator1 = new NameGenerator(config1, seed);
                var generator2 = new NameGenerator(config2, seed);

                var names1 = new List<string>();
                var names2 = new List<string>();

                // Generate mixed entity types with parameters
                for (var i = 0; i < count; i++)
                {
                    var gender = (Gender)(i % 3);
                    var buildingType = (BuildingType)(i % 7);

                    // Generator 1
                    names1.Add(generator1.GenerateNpcName("test-theme", gender));
                    names1.Add(generator1.GenerateBuildingName("test-theme", buildingType));
                    names1.Add(generator1.GenerateCityName("test-theme"));

                    // Generator 2
                    names2.Add(generator2.GenerateNpcName("test-theme", gender));
                    names2.Add(generator2.GenerateBuildingName("test-theme", buildingType));
                    names2.Add(generator2.GenerateCityName("test-theme"));
                }

                names1.Should().Equal(names2,
                    "generators with the same seed should produce identical sequences even with mixed parameters");
            }, iter: 100);
    }

    /// <summary>
    /// Property test that verifies determinism across multiple custom themes.
    /// </summary>
    [Fact]
    public void Property_CustomThemeDeterministicAcrossMultipleThemes()
    {
        var genSeed = Gen.Int;
        var genCount = Gen.Int[3, 10];

        Gen.Select(genSeed, genCount)
            .Sample(tuple =>
            {
                var (seed, count) = tuple;

                var theme1 = CreateSimpleCustomTheme();
                var theme2 = CreateSimpleCustomTheme();
                
                var config1 = new ThemeConfig()
                    .AddTheme("theme1", theme1)
                    .AddTheme("theme2", theme2);
                
                var config2 = new ThemeConfig()
                    .AddTheme("theme1", theme1)
                    .AddTheme("theme2", theme2);
                
                var generator1 = new NameGenerator(config1, seed);
                var generator2 = new NameGenerator(config2, seed);

                var names1 = new List<string>();
                var names2 = new List<string>();

                // Alternate between themes
                for (var i = 0; i < count; i++)
                {
                    var themeId = i % 2 == 0 ? "theme1" : "theme2";

                    names1.Add(generator1.GenerateNpcName(themeId));
                    names1.Add(generator1.GenerateCityName(themeId));

                    names2.Add(generator2.GenerateNpcName(themeId));
                    names2.Add(generator2.GenerateCityName(themeId));
                }

                names1.Should().Equal(names2,
                    "generators with the same seed should produce identical sequences across multiple custom themes");
            }, iter: 100);
    }

    /// <summary>
    /// Helper method to create a simple custom theme with sufficient variety for testing.
    /// </summary>
    private static CustomThemeData CreateSimpleCustomTheme()
    {
        var builder = new ThemeDataBuilder();

        // Add NPC names
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

        // Add building names
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

        // Add city names
        builder.WithCityNames(
            GenerateTestArray("City", 20),
            GenerateTestArray("core", 20),
            GenerateTestArray("ville", 20));

        // Add district names
        builder.WithDistrictNames(
            GenerateTestArray("Old", 20),
            GenerateTestArray("District", 20));

        // Add street names
        builder.WithStreetNames(
            GenerateTestArray("Main", 20),
            GenerateTestArray("core", 20),
            new[] { "Street", "Avenue", "Boulevard", "Road", "Way", "Lane", "Drive", "Court", "Place", "Circle" });

        // Add faction names
        builder.WithFactionNames(
            GenerateTestArray("The", 20),
            GenerateTestArray("core", 20),
            GenerateTestArray("Guild", 20));

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
