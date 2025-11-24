using CsCheck;
using FluentAssertions;
using NameGeneratorEngine.Enums;
using Xunit;

namespace NameGeneratorEngine.Tests.Properties;

/// <summary>
/// Property-based tests for name uniqueness within a session.
/// </summary>
public class UniquenessPropertyTests
{
    /// <summary>
    /// Feature: name-generator-engine, Property 2: No duplicate names within entity type in session
    /// For any sequence of name generation calls for a specific entity type within a single session,
    /// all returned names should be unique (no duplicates).
    /// Validates: Requirements 2.2
    /// </summary>
    [Fact]
    public void Property_NoDuplicateNamesWithinEntityTypeInSession()
    {
        // Generate random test scenarios with different seeds and entity types
        var genSeed = Gen.Int;
        var genTheme = Gen.Int[0, 2].Select(i => (Theme)i); // 0=Cyberpunk, 1=Elves, 2=Orcs
        var genEntityType = Gen.Int[0, 4].Select(i => (EntityType)i); // EntityType has 5 values: 0-4
        var genCount = Gen.Int[10, 50]; // Generate between 10 and 50 names

        Gen.Select(genSeed, genTheme, genEntityType, genCount)
            .Sample(tuple =>
            {
                var (seed, theme, entityType, count) = tuple;

                var generator = new NameGenerator(seed);
                var generatedNames = new List<string>();

                // Generate multiple names for the same entity type
                for (var i = 0; i < count; i++)
                {
                    var name = entityType switch
                    {
                        EntityType.Npc => generator.GenerateNpcName(theme),
                        EntityType.Building => generator.GenerateBuildingName(theme),
                        EntityType.City => generator.GenerateCityName(theme),
                        EntityType.District => generator.GenerateDistrictName(theme),
                        EntityType.Street => generator.GenerateStreetName(theme),
                        _ => throw new InvalidOperationException($"Unknown entity type: {entityType}")
                    };

                    generatedNames.Add(name);
                }

                // Verify all names are unique
                var uniqueNames = generatedNames.Distinct().ToList();
                uniqueNames.Should().HaveCount(generatedNames.Count,
                    $"all {entityType} names should be unique within a session");
            }, iter: 100);
    }

    /// <summary>
    /// Property test that verifies uniqueness across different themes for the same entity type.
    /// </summary>
    [Fact]
    public void Property_NoDuplicateNamesAcrossThemesForSameEntityType()
    {
        var genSeed = Gen.Int;
        var genEntityType = Gen.Int[0, 4].Select(i => (EntityType)i); // EntityType has 5 values: 0-4
        var genCount = Gen.Int[5, 15]; // Generate fewer names per theme

        Gen.Select(genSeed, genEntityType, genCount)
            .Sample(tuple =>
            {
                var (seed, entityType, countPerTheme) = tuple;

                var generator = new NameGenerator(seed);
                var allGeneratedNames = new List<string>();

                // Generate names for all themes
                foreach (var theme in Enum.GetValues<Theme>())
                {
                    for (var i = 0; i < countPerTheme; i++)
                    {
                        var name = entityType switch
                        {
                            EntityType.Npc => generator.GenerateNpcName(theme),
                            EntityType.Building => generator.GenerateBuildingName(theme),
                            EntityType.City => generator.GenerateCityName(theme),
                            EntityType.District => generator.GenerateDistrictName(theme),
                            EntityType.Street => generator.GenerateStreetName(theme),
                            _ => throw new InvalidOperationException($"Unknown entity type: {entityType}")
                        };

                        allGeneratedNames.Add(name);
                    }
                }

                // Verify all names are unique across all themes
                var uniqueNames = allGeneratedNames.Distinct().ToList();
                uniqueNames.Should().HaveCount(allGeneratedNames.Count,
                    $"all {entityType} names should be unique within a session, even across different themes");
            }, iter: 100);
    }

    /// <summary>
    /// Property test that verifies uniqueness with mixed entity types and parameters.
    /// </summary>
    [Fact]
    public void Property_NoDuplicateNamesWithMixedParameters()
    {
        var genSeed = Gen.Int;
        var genTheme = Gen.Int[0, 2].Select(i => (Theme)i); // 0=Cyberpunk, 1=Elves, 2=Orcs
        var genGender = Gen.Int[0, 2].Select(i => (Gender)i); // 0=Male, 1=Female, 2=Neutral
        var genBuildingType = Gen.Int[0, 6].Select(i => (BuildingType)i); // 0-6 for 7 building types
        var genCount = Gen.Int[10, 30];

        Gen.Select(genSeed, genTheme, genCount)
            .Sample(tuple =>
            {
                var (seed, theme, count) = tuple;

                var generator = new NameGenerator(seed);

                // Test NPC names with different genders
                var npcNames = new List<string>();
                for (var i = 0; i < count; i++)
                {
                    var gender = (Gender)(i % 3);
                    npcNames.Add(generator.GenerateNpcName(theme, gender));
                }
                npcNames.Distinct().Should().HaveCount(npcNames.Count,
                    "NPC names should be unique regardless of gender parameter");

                // Test building names with different types
                var buildingNames = new List<string>();
                for (var i = 0; i < count; i++)
                {
                    var buildingType = (BuildingType)(i % 7);
                    buildingNames.Add(generator.GenerateBuildingName(theme, buildingType));
                }
                buildingNames.Distinct().Should().HaveCount(buildingNames.Count,
                    "Building names should be unique regardless of building type parameter");
            }, iter: 100);
    }
}
