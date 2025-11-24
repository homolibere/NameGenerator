using CsCheck;
using FluentAssertions;
using NameGeneratorEngine.Enums;
using Xunit;

namespace NameGeneratorEngine.Tests.Properties;

/// <summary>
/// Property-based tests for session reset functionality.
/// </summary>
public class ResetPropertyTests
{
    /// <summary>
    /// Feature: name-generator-engine, Property 4: Reset enables name reuse
    /// For any generator session, after generating a set of names and then calling ResetSession(), 
    /// it should be possible to generate names that appeared in the previous set (names can be reused after reset).
    /// Validates: Requirements 2.4
    /// </summary>
    [Fact]
    public void Property_ResetEnablesNameReuse()
    {
        // Generate random test scenarios with different seeds, themes, and entity types
        var genSeed = Gen.Int;
        var genTheme = Gen.Int[0, 2].Select(i => (Theme)i); // 0=Cyberpunk, 1=Elves, 2=Orcs
        var genEntityType = Gen.Int[0, 5].Select(i => (EntityType)i); // 0-5 for 6 entity types
        var genCount = Gen.Int[5, 15]; // Number of names to generate

        Gen.Select(genSeed, genTheme, genEntityType, genCount)
            .Sample(tuple =>
            {
                var (seed, theme, entityType, count) = tuple;

                var generator = new NameGenerator(seed);
                var firstSetNames = new List<string>();

                // Generate first set of names
                for (var i = 0; i < count; i++)
                {
                    var name = entityType switch
                    {
                        EntityType.Npc => generator.GenerateNpcName(theme),
                        EntityType.Building => generator.GenerateBuildingName(theme),
                        EntityType.City => generator.GenerateCityName(theme),
                        EntityType.District => generator.GenerateDistrictName(theme),
                        EntityType.Street => generator.GenerateStreetName(theme),
                        EntityType.Faction => generator.GenerateFactionName(theme),
                        _ => throw new InvalidOperationException($"Unknown entity type: {entityType}")
                    };
                    firstSetNames.Add(name);
                }

                // Reset the session
                generator.ResetSession();

                // Verify seed is still the same after reset
                generator.Seed.Should().Be(seed, "seed should remain unchanged after reset");

                var secondSetNames = new List<string>();

                // Generate second set of names
                for (var i = 0; i < count; i++)
                {
                    var name = entityType switch
                    {
                        EntityType.Npc => generator.GenerateNpcName(theme),
                        EntityType.Building => generator.GenerateBuildingName(theme),
                        EntityType.City => generator.GenerateCityName(theme),
                        EntityType.District => generator.GenerateDistrictName(theme),
                        EntityType.Street => generator.GenerateStreetName(theme),
                        EntityType.Faction => generator.GenerateFactionName(theme),
                        _ => throw new InvalidOperationException($"Unknown entity type: {entityType}")
                    };
                    secondSetNames.Add(name);
                }

                // After reset with the same seed, the sequence should be identical
                // This means ALL names from the first set should appear in the second set
                firstSetNames.Should().Equal(secondSetNames,
                    "after reset, the same seed should produce the same sequence of names");
            }, iter: 100);
    }

    /// <summary>
    /// Property test that verifies reset clears duplicate tracking for all entity types.
    /// </summary>
    [Fact]
    public void Property_ResetClearsDuplicateTrackingForAllEntityTypes()
    {
        var genSeed = Gen.Int;
        var genTheme = Gen.Int[0, 2].Select(i => (Theme)i); // 0=Cyberpunk, 1=Elves, 2=Orcs
        var genCount = Gen.Int[3, 8]; // Number of names per entity type

        Gen.Select(genSeed, genTheme, genCount)
            .Sample(tuple =>
            {
                var (seed, theme, count) = tuple;

                var generator = new NameGenerator(seed);

                // Generate names for all entity types
                var firstSetByType = new Dictionary<EntityType, List<string>>();

                foreach (var entityType in Enum.GetValues<EntityType>())
                {
                    var names = new List<string>();
                    for (var i = 0; i < count; i++)
                    {
                        var name = entityType switch
                        {
                            EntityType.Npc => generator.GenerateNpcName(theme),
                            EntityType.Building => generator.GenerateBuildingName(theme),
                            EntityType.City => generator.GenerateCityName(theme),
                            EntityType.District => generator.GenerateDistrictName(theme),
                            EntityType.Street => generator.GenerateStreetName(theme),
                            EntityType.Faction => generator.GenerateFactionName(theme),
                            _ => throw new InvalidOperationException($"Unknown entity type: {entityType}")
                        };
                        names.Add(name);
                    }
                    firstSetByType[entityType] = names;
                }

                // Reset the session
                generator.ResetSession();

                // Generate names again for all entity types
                var secondSetByType = new Dictionary<EntityType, List<string>>();

                foreach (var entityType in Enum.GetValues<EntityType>())
                {
                    var names = new List<string>();
                    for (var i = 0; i < count; i++)
                    {
                        var name = entityType switch
                        {
                            EntityType.Npc => generator.GenerateNpcName(theme),
                            EntityType.Building => generator.GenerateBuildingName(theme),
                            EntityType.City => generator.GenerateCityName(theme),
                            EntityType.District => generator.GenerateDistrictName(theme),
                            EntityType.Street => generator.GenerateStreetName(theme),
                            EntityType.Faction => generator.GenerateFactionName(theme),
                            _ => throw new InvalidOperationException($"Unknown entity type: {entityType}")
                        };
                        names.Add(name);
                    }
                    secondSetByType[entityType] = names;
                }

                // Verify that for each entity type, the sequences are identical after reset
                foreach (var entityType in Enum.GetValues<EntityType>())
                {
                    firstSetByType[entityType].Should().Equal(secondSetByType[entityType],
                        $"after reset, {entityType} names should follow the same sequence");
                }
            }, iter: 100);
    }

    /// <summary>
    /// Property test that verifies multiple resets work correctly.
    /// </summary>
    [Fact]
    public void Property_MultipleResetsWorkCorrectly()
    {
        var genSeed = Gen.Int;
        var genTheme = Gen.Int[0, 2].Select(i => (Theme)i); // 0=Cyberpunk, 1=Elves, 2=Orcs
        var genResetCount = Gen.Int[2, 5]; // Number of resets to perform
        var genNamesPerCycle = Gen.Int[3, 8]; // Names to generate per cycle

        Gen.Select(genSeed, genTheme, genResetCount, genNamesPerCycle)
            .Sample(tuple =>
            {
                var (seed, theme, resetCount, namesPerCycle) = tuple;

                var generator = new NameGenerator(seed);
                var allCycles = new List<List<string>>();

                // Perform multiple reset cycles
                for (var cycle = 0; cycle < resetCount; cycle++)
                {
                    var cycleNames = new List<string>();

                    for (var i = 0; i < namesPerCycle; i++)
                    {
                        cycleNames.Add(generator.GenerateCityName(theme));
                    }

                    allCycles.Add(cycleNames);

                    // Reset for next cycle (except after the last cycle)
                    if (cycle < resetCount - 1)
                    {
                        generator.ResetSession();
                    }
                }

                // Verify all cycles produced identical sequences
                var firstCycle = allCycles[0];
                foreach (var cycle in allCycles.Skip(1))
                {
                    cycle.Should().Equal(firstCycle,
                        "each reset cycle should produce the same sequence of names");
                }
            }, iter: 100);
    }

    /// <summary>
    /// Property test that verifies reset works correctly with mixed entity types and parameters.
    /// </summary>
    [Fact]
    public void Property_ResetWorksWithMixedEntityTypesAndParameters()
    {
        var genSeed = Gen.Int;
        var genTheme = Gen.Int[0, 2].Select(i => (Theme)i); // 0=Cyberpunk, 1=Elves, 2=Orcs

        Gen.Select(genSeed, genTheme)
            .Sample(tuple =>
            {
                var (seed, theme) = tuple;

                var generator = new NameGenerator(seed);
                var firstSequence = new List<string>();

                // Generate a mixed sequence of names
                firstSequence.Add(generator.GenerateNpcName(theme, Gender.Male));
                firstSequence.Add(generator.GenerateBuildingName(theme, BuildingType.Commercial));
                firstSequence.Add(generator.GenerateCityName(theme));
                firstSequence.Add(generator.GenerateNpcName(theme)); // No gender specified
                firstSequence.Add(generator.GenerateDistrictName(theme));
                firstSequence.Add(generator.GenerateBuildingName(theme)); // No type specified
                firstSequence.Add(generator.GenerateStreetName(theme));
                firstSequence.Add(generator.GenerateNpcName(theme, Gender.Female));

                // Reset the session
                generator.ResetSession();

                var secondSequence = new List<string>();

                // Generate the same sequence again
                secondSequence.Add(generator.GenerateNpcName(theme, Gender.Male));
                secondSequence.Add(generator.GenerateBuildingName(theme, BuildingType.Commercial));
                secondSequence.Add(generator.GenerateCityName(theme));
                secondSequence.Add(generator.GenerateNpcName(theme)); // No gender specified
                secondSequence.Add(generator.GenerateDistrictName(theme));
                secondSequence.Add(generator.GenerateBuildingName(theme)); // No type specified
                secondSequence.Add(generator.GenerateStreetName(theme));
                secondSequence.Add(generator.GenerateNpcName(theme, Gender.Female));

                // Verify the sequences are identical
                firstSequence.Should().Equal(secondSequence,
                    "reset should work correctly with mixed entity types and parameters");
            }, iter: 100);
    }
}
