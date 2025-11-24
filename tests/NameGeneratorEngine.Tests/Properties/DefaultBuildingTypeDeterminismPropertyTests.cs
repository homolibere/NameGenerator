using CsCheck;
using FluentAssertions;
using NameGeneratorEngine.Enums;
using Xunit;

namespace NameGeneratorEngine.Tests.Properties;

/// <summary>
/// Property-based tests for default building type selection determinism.
/// </summary>
public class DefaultBuildingTypeDeterminismPropertyTests
{
    /// <summary>
    /// Feature: name-generator-engine, Property 6: Default building type is deterministic
    /// For any seed and theme, calling GenerateBuildingName without specifying a building type 
    /// should produce the same name across multiple executions with the same seed.
    /// Validates: Requirements 5.5
    /// </summary>
    [Fact]
    public void Property_DefaultBuildingTypeIsDeterministic()
    {
        // Generate random test scenarios with different seeds and themes
        var genSeed = Gen.Int;
        var genTheme = Gen.Int[0, 2].Select(i => (Theme)i); // 0=Cyberpunk, 1=Elves, 2=Orcs
        var genCallCount = Gen.Int[5, 20]; // Number of calls to make

        Gen.Select(genSeed, genTheme, genCallCount)
            .Sample(tuple =>
            {
                var (seed, theme, callCount) = tuple;

                // Create two generators with the same seed
                var generator1 = new NameGenerator(seed);
                var generator2 = new NameGenerator(seed);

                var names1 = new List<string>();
                var names2 = new List<string>();

                // Generate names without specifying building type
                for (int i = 0; i < callCount; i++)
                {
                    names1.Add(generator1.GenerateBuildingName(theme));
                    names2.Add(generator2.GenerateBuildingName(theme));
                }

                // Verify all generated names are identical
                names1.Should().Equal(names2,
                    "generators with the same seed should produce identical building names when building type is not specified");
            }, iter: 100);
    }

    /// <summary>
    /// Property test that verifies default building type selection is deterministic across all themes.
    /// </summary>
    [Fact]
    public void Property_DefaultBuildingTypeDeterministicAcrossAllThemes()
    {
        var genSeed = Gen.Int;
        var genCallCount = Gen.Int[3, 10]; // Calls per theme

        Gen.Select(genSeed, genCallCount)
            .Sample(tuple =>
            {
                var (seed, callCount) = tuple;

                // Create two generators with the same seed
                var generator1 = new NameGenerator(seed);
                var generator2 = new NameGenerator(seed);

                // Test all themes
                foreach (var theme in Enum.GetValues<Theme>())
                {
                    var names1 = new List<string>();
                    var names2 = new List<string>();

                    for (int i = 0; i < callCount; i++)
                    {
                        names1.Add(generator1.GenerateBuildingName(theme));
                        names2.Add(generator2.GenerateBuildingName(theme));
                    }

                    names1.Should().Equal(names2,
                        $"generators with the same seed should produce identical building names for {theme} theme when building type is not specified");
                }
            }, iter: 100);
    }

    /// <summary>
    /// Property test that verifies default building type selection remains deterministic 
    /// even when interleaved with other generation calls.
    /// </summary>
    [Fact]
    public void Property_DefaultBuildingTypeDeterministicWithInterleavedCalls()
    {
        var genSeed = Gen.Int;
        var genTheme = Gen.Int[0, 2].Select(i => (Theme)i); // 0=Cyberpunk, 1=Elves, 2=Orcs

        Gen.Select(genSeed, genTheme)
            .Sample(tuple =>
            {
                var (seed, theme) = tuple;

                // Create two generators with the same seed
                var generator1 = new NameGenerator(seed);
                var generator2 = new NameGenerator(seed);

                var names1 = new List<string>();
                var names2 = new List<string>();

                // Interleave building name generation with other entity types
                for (int i = 0; i < 10; i++)
                {
                    names1.Add(generator1.GenerateBuildingName(theme)); // No building type specified
                    names2.Add(generator2.GenerateBuildingName(theme)); // No building type specified

                    // Generate other entity types to ensure state consistency
                    generator1.GenerateDistrictName(theme);
                    generator2.GenerateDistrictName(theme);

                    names1.Add(generator1.GenerateBuildingName(theme)); // No building type specified
                    names2.Add(generator2.GenerateBuildingName(theme)); // No building type specified

                    generator1.GenerateNpcName(theme, Gender.Male);
                    generator2.GenerateNpcName(theme, Gender.Male);
                }

                // Verify all building names are identical
                names1.Should().Equal(names2,
                    "default building type selection should remain deterministic even when interleaved with other generation calls");
            }, iter: 100);
    }
}
