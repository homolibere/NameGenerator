using CsCheck;
using FluentAssertions;
using NameGeneratorEngine.Enums;
using Xunit;

namespace NameGeneratorEngine.Tests.Properties;

/// <summary>
/// Property-based tests for custom theme error handling.
/// </summary>
public class CustomThemeErrorHandlingPropertyTests
{
    /// <summary>
    /// Feature: custom-theme-registration, Property 17: Unregistered theme error handling
    /// For any unregistered theme identifier, attempting to generate names should throw ArgumentException
    /// listing all available themes.
    /// Validates: Requirements 7.4
    /// </summary>
    [Fact]
    public void Property_UnregisteredThemeThrowsArgumentExceptionWithAvailableThemes()
    {
        var genSeed = Gen.Int;
        var genEntityType = Gen.Int[0, 5].Select(i => (EntityType)i);

        Gen.Select(genSeed, genEntityType)
            .Sample(tuple =>
            {
                var (seed, entityType) = tuple;

                var generator = new NameGenerator(seed);
                var unregisteredTheme = "nonexistent-theme";

                // Try to generate a name with an unregistered theme
                var generateWithUnregisteredTheme = () =>
                {
                    _ = entityType switch
                    {
                        EntityType.Npc => generator.GenerateNpcName(unregisteredTheme),
                        EntityType.Building => generator.GenerateBuildingName(unregisteredTheme),
                        EntityType.City => generator.GenerateCityName(unregisteredTheme),
                        EntityType.District => generator.GenerateDistrictName(unregisteredTheme),
                        EntityType.Street => generator.GenerateStreetName(unregisteredTheme),
                        EntityType.Faction => generator.GenerateFactionName(unregisteredTheme),
                        _ => throw new InvalidOperationException($"Unknown entity type: {entityType}")
                    };
                };

                // Verify ArgumentException is thrown with available themes listed
                generateWithUnregisteredTheme.Should().Throw<ArgumentException>()
                    .WithMessage("*not registered*")
                    .WithMessage("*Available themes*")
                    .WithMessage("*Cyberpunk*")
                    .WithMessage("*Elves*")
                    .WithMessage("*Orcs*");
            }, iter: 100);
    }

    /// <summary>
    /// Property test that verifies error message includes custom themes in available list.
    /// </summary>
    [Fact]
    public void Property_UnregisteredThemeErrorIncludesCustomThemes()
    {
        var genSeed = Gen.Int;
        var genDummy = Gen.Int; // Dummy generator for Select

        Gen.Select(genSeed, genDummy)
            .Sample(tuple =>
            {
                var (seed, _) = tuple;
                var customTheme = CreateSimpleCustomTheme();
                var config = new ThemeConfig()
                    .AddTheme("custom1", customTheme)
                    .AddTheme("custom2", customTheme);
                
                var generator = new NameGenerator(config, seed);
                var unregisteredTheme = "nonexistent-theme";

                // Try to generate a name with an unregistered theme
                var generateWithUnregisteredTheme = () =>
                {
                    generator.GenerateNpcName(unregisteredTheme);
                };

                // Verify ArgumentException includes both built-in and custom themes
                generateWithUnregisteredTheme.Should().Throw<ArgumentException>()
                    .WithMessage("*not registered*")
                    .WithMessage("*Available themes*")
                    .WithMessage("*Cyberpunk*")
                    .WithMessage("*custom1*")
                    .WithMessage("*custom2*");
            }, iter: 100);
    }

    private static CustomThemeData CreateSimpleCustomTheme()
    {
        var builder = new ThemeDataBuilder();

        builder.WithNpcNames(npc =>
        {
            npc.WithMaleNames(
                new[] { "A", "B", "C" },
                new[] { "x", "y", "z" },
                new[] { "1", "2", "3" });
            npc.WithFemaleNames(
                new[] { "D", "E", "F" },
                new[] { "x", "y", "z" },
                new[] { "1", "2", "3" });
            npc.WithNeutralNames(
                new[] { "G", "H", "I" },
                new[] { "x", "y", "z" },
                new[] { "1", "2", "3" });
        });

        builder.WithBuildingNames(building =>
        {
            building.WithGenericNames(
                new[] { "The", "A", "An" },
                new[] { "Building", "Place", "Center" });

            foreach (var buildingType in Enum.GetValues<BuildingType>())
            {
                building.WithTypeNames(
                    buildingType,
                    new[] { "X", "Y", "Z" },
                    new[] { "desc1", "desc2", "desc3" },
                    new[] { "1", "2", "3" });
            }
        });

        builder.WithCityNames(
            new[] { "New", "Old", "Great" },
            new[] { "City", "Town", "Ville" },
            new[] { "ton", "burg", "ford" });

        builder.WithDistrictNames(
            new[] { "North", "South", "East" },
            new[] { "District", "Quarter", "Ward" });

        builder.WithStreetNames(
            new[] { "Main", "First", "Second" },
            new[] { "St", "Ave", "Blvd" },
            new[] { "Street", "Avenue", "Boulevard" });

        builder.WithFactionNames(
            new[] { "The", "A", "An" },
            new[] { "Guild", "Order", "Council" },
            new[] { "of Power", "of Light", "of Shadow" });

        return builder.Build();
    }
}
