using CsCheck;
using FluentAssertions;
using NameGeneratorEngine.Enums;
using Xunit;

namespace NameGeneratorEngine.Tests.Properties;

/// <summary>
/// Property-based tests for default gender selection determinism.
/// </summary>
public class DefaultGenderDeterminismPropertyTests
{
    /// <summary>
    /// Feature: name-generator-engine, Property 5: Default gender selection is deterministic
    /// For any seed and theme, calling GenerateNpcName without specifying gender should 
    /// produce the same name across multiple executions with the same seed (gender selection is deterministic).
    /// Validates: Requirements 4.5
    /// </summary>
    [Fact]
    public void Property_DefaultGenderSelectionIsDeterministic()
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

                // Generate names without specifying gender
                for (int i = 0; i < callCount; i++)
                {
                    names1.Add(generator1.GenerateNpcName(theme));
                    names2.Add(generator2.GenerateNpcName(theme));
                }

                // Verify all generated names are identical
                names1.Should().Equal(names2,
                    "generators with the same seed should produce identical NPC names when gender is not specified");
            }, iter: 100);
    }

    /// <summary>
    /// Property test that verifies default gender selection is deterministic across all themes.
    /// </summary>
    [Fact]
    public void Property_DefaultGenderDeterministicAcrossAllThemes()
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
                        names1.Add(generator1.GenerateNpcName(theme));
                        names2.Add(generator2.GenerateNpcName(theme));
                    }

                    names1.Should().Equal(names2,
                        $"generators with the same seed should produce identical NPC names for {theme} theme when gender is not specified");
                }
            }, iter: 100);
    }

    /// <summary>
    /// Property test that verifies default gender selection remains deterministic 
    /// even when interleaved with other generation calls.
    /// </summary>
    [Fact]
    public void Property_DefaultGenderDeterministicWithInterleavedCalls()
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

                // Interleave NPC name generation with other entity types
                for (int i = 0; i < 10; i++)
                {
                    names1.Add(generator1.GenerateNpcName(theme)); // No gender specified
                    names2.Add(generator2.GenerateNpcName(theme)); // No gender specified

                    // Generate other entity types to ensure state consistency
                    generator1.GenerateCityName(theme);
                    generator2.GenerateCityName(theme);

                    names1.Add(generator1.GenerateNpcName(theme)); // No gender specified
                    names2.Add(generator2.GenerateNpcName(theme)); // No gender specified

                    generator1.GenerateStreetName(theme);
                    generator2.GenerateStreetName(theme);
                }

                // Verify all NPC names are identical
                names1.Should().Equal(names2,
                    "default gender selection should remain deterministic even when interleaved with other generation calls");
            }, iter: 100);
    }
}
