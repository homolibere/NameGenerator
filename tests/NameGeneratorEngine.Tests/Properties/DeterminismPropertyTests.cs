using CsCheck;
using FluentAssertions;
using NameGeneratorEngine.Enums;
using Xunit;

namespace NameGeneratorEngine.Tests.Properties;

/// <summary>
/// Property-based tests for deterministic name generation.
/// </summary>
public class DeterminismPropertyTests
{
    /// <summary>
    /// Feature: name-generator-engine, Property 1: Deterministic generation with same seed
    /// For any seed value, creating two NameGenerator instances with that seed and calling 
    /// the same sequence of generation methods (same themes, entity types, and parameters) 
    /// should produce identical sequences of names.
    /// Validates: Requirements 1.2
    /// </summary>
    [Fact]
    public void Property_DeterministicGenerationWithSameSeed()
    {
        // Generate random test scenarios with different seeds and generation sequences
        Gen.Int.Sample(seed =>
        {
            // Create two generators with the same seed
            var generator1 = new NameGenerator(seed);
            var generator2 = new NameGenerator(seed);

            // Verify seed property returns correct value
            generator1.Seed.Should().Be(seed);
            generator2.Seed.Should().Be(seed);

            // Generate a sequence of names using various methods and parameters
            var names1 = new List<string>();
            var names2 = new List<string>();

            // Test NPC names with different themes and genders
            foreach (var theme in Enum.GetValues<Theme>())
            {
                // With explicit gender
                foreach (var gender in Enum.GetValues<Gender>())
                {
                    names1.Add(generator1.GenerateNpcName(theme, gender));
                    names2.Add(generator2.GenerateNpcName(theme, gender));
                }

                // Without explicit gender (should be deterministic)
                names1.Add(generator1.GenerateNpcName(theme));
                names2.Add(generator2.GenerateNpcName(theme));
            }

            // Test building names with different themes and types
            foreach (var theme in Enum.GetValues<Theme>())
            {
                // With explicit building type
                foreach (var buildingType in Enum.GetValues<BuildingType>())
                {
                    names1.Add(generator1.GenerateBuildingName(theme, buildingType));
                    names2.Add(generator2.GenerateBuildingName(theme, buildingType));
                }

                // Without explicit building type (should be deterministic)
                names1.Add(generator1.GenerateBuildingName(theme));
                names2.Add(generator2.GenerateBuildingName(theme));
            }

            // Test city names
            foreach (var theme in Enum.GetValues<Theme>())
            {
                names1.Add(generator1.GenerateCityName(theme));
                names2.Add(generator2.GenerateCityName(theme));
            }

            // Test district names
            foreach (var theme in Enum.GetValues<Theme>())
            {
                names1.Add(generator1.GenerateDistrictName(theme));
                names2.Add(generator2.GenerateDistrictName(theme));
            }

            // Test street names
            foreach (var theme in Enum.GetValues<Theme>())
            {
                names1.Add(generator1.GenerateStreetName(theme));
                names2.Add(generator2.GenerateStreetName(theme));
            }

            // Verify all generated names are identical
            names1.Should().Equal(names2, "generators with the same seed should produce identical sequences");
        }, iter: 100); // Run 100 iterations as specified in the design document
    }

    /// <summary>
    /// Property test that verifies determinism with a more complex random sequence of operations.
    /// This tests that any arbitrary sequence of generation calls produces identical results.
    /// </summary>
    [Fact]
    public void Property_DeterministicGenerationWithRandomSequence()
    {
        // Generate random seeds and random sequences of operations
        var genSeed = Gen.Int;
        var genTheme = Gen.Int[0, 2].Select(i => (Theme)i); // 0=Cyberpunk, 1=Elves, 2=Orcs
        var genGender = Gen.Int[0, 2].Select(i => (Gender)i); // 0=Male, 1=Female, 2=Neutral
        var genBuildingType = Gen.Int[0, 6].Select(i => (BuildingType)i); // 0-6 for 7 building types
        var genOperationType = Gen.Int[0, 4]; // 0=NPC, 1=Building, 2=City, 3=District, 4=Street

        // Create a generator for sequences of operations
        var genOperationSequence = Gen.Int[5, 20].SelectMany(count =>
            Gen.Select(genOperationType, genTheme, genGender, genBuildingType)
                .Array[count]);

        Gen.Select(genSeed, genOperationSequence)
            .Sample(tuple =>
            {
                var (seed, operations) = tuple;

                // Create two generators with the same seed
                var generator1 = new NameGenerator(seed);
                var generator2 = new NameGenerator(seed);

                var names1 = new List<string>();
                var names2 = new List<string>();

                // Execute the same sequence of operations on both generators
                foreach (var (opType, theme, gender, buildingType) in operations)
                {
                    string name1, name2;

                    switch (opType)
                    {
                        case 0: // NPC with gender
                            name1 = generator1.GenerateNpcName(theme, gender);
                            name2 = generator2.GenerateNpcName(theme, gender);
                            break;
                        case 1: // Building with type
                            name1 = generator1.GenerateBuildingName(theme, buildingType);
                            name2 = generator2.GenerateBuildingName(theme, buildingType);
                            break;
                        case 2: // City
                            name1 = generator1.GenerateCityName(theme);
                            name2 = generator2.GenerateCityName(theme);
                            break;
                        case 3: // District
                            name1 = generator1.GenerateDistrictName(theme);
                            name2 = generator2.GenerateDistrictName(theme);
                            break;
                        case 4: // Street
                            name1 = generator1.GenerateStreetName(theme);
                            name2 = generator2.GenerateStreetName(theme);
                            break;
                        default:
                            throw new InvalidOperationException($"Unknown operation type: {opType}");
                    }

                    names1.Add(name1);
                    names2.Add(name2);
                }

                // Verify all generated names are identical
                names1.Should().Equal(names2, 
                    "generators with the same seed should produce identical sequences regardless of operation order");
            }, iter: 100);
    }
}
