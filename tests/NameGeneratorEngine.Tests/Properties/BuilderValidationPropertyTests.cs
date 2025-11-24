using CsCheck;
using FluentAssertions;
using NameGeneratorEngine.Enums;
using Xunit;

namespace NameGeneratorEngine.Tests.Properties;

/// <summary>
/// Property-based tests for theme data builder validation.
/// **Feature: custom-theme-registration, Property 3: Theme data validation completeness**
/// </summary>
public class BuilderValidationPropertyTests
{
    /// <summary>
    /// Property 3: Theme data validation completeness
    /// For any theme data with missing required fields, empty arrays, or invalid string values,
    /// the validation system should reject it and throw ArgumentException.
    /// **Validates: Requirements 1.5, 2.3, 2.4**
    /// </summary>
    [Fact]
    public void Property_MissingRequiredFields_ThrowsArgumentException()
    {
        // Test that building without all required fields throws ArgumentException
        Gen.Select(
            Gen.Bool,
            Gen.Bool,
            Gen.Bool,
            Gen.Bool,
            Gen.Bool,
            Gen.Bool
        ).Sample((includeNpc, includeBuilding, includeCity, includeDistrict, includeStreet, includeFaction) =>
        {
            // Skip the case where all fields are included (that's valid)
            if (includeNpc && includeBuilding && includeCity && includeDistrict && includeStreet && includeFaction)
                return;

            var builder = new ThemeDataBuilder();

            if (includeNpc)
            {
                builder.WithNpcNames(npc => npc
                    .WithMaleNames(new[] { "M" }, new[] { "a" }, new[] { "le" })
                    .WithFemaleNames(new[] { "F" }, new[] { "e" }, new[] { "male" })
                    .WithNeutralNames(new[] { "N" }, new[] { "eu" }, new[] { "tral" }));
            }

            if (includeBuilding)
            {
                builder.WithBuildingNames(building =>
                {
                    building.WithGenericNames(new[] { "Build" }, new[] { "ing" });
                    foreach (BuildingType type in Enum.GetValues(typeof(BuildingType)))
                    {
                        building.WithTypeNames(type, new[] { "Pre" }, new[] { "Desc" }, new[] { "Suf" });
                    }
                });
            }

            if (includeCity)
            {
                builder.WithCityNames(new[] { "City" }, new[] { "Core" }, new[] { "ton" });
            }

            if (includeDistrict)
            {
                builder.WithDistrictNames(new[] { "Old" }, new[] { "District" });
            }

            if (includeStreet)
            {
                builder.WithStreetNames(new[] { "Main" }, new[] { "Oak" }, new[] { "Street" });
            }

            if (includeFaction)
            {
                builder.WithFactionNames(new[] { "The" }, new[] { "Guild" }, new[] { "s" });
            }

            // Attempting to build should throw ArgumentException
            var act = () => builder.Build();
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Missing required fields*");
        }, iter: 100);
    }

    /// <summary>
    /// Property 3: Theme data validation completeness
    /// For any array that is empty, the validation system should reject it and throw ArgumentException.
    /// **Validates: Requirements 1.5, 2.3, 2.4**
    /// </summary>
    [Fact]
    public void Property_EmptyArrays_ThrowsArgumentException()
    {
        // Test NPC names with empty arrays
        Gen.OneOf(
            Gen.Const("prefixes"),
            Gen.Const("cores"),
            Gen.Const("suffixes")
        ).Sample(emptyField =>
        {
            var builder = new ThemeDataBuilder();

            var act = () => builder.WithNpcNames(npc =>
            {
                var prefixes = emptyField == "prefixes" ? Array.Empty<string>() : new[] { "Pre" };
                var cores = emptyField == "cores" ? Array.Empty<string>() : new[] { "Core" };
                var suffixes = emptyField == "suffixes" ? Array.Empty<string>() : new[] { "Suf" };

                npc.WithMaleNames(prefixes, cores, suffixes);
            });

            act.Should().Throw<ArgumentException>()
                .WithMessage("*cannot be empty*");
        }, iter: 100);
    }

    /// <summary>
    /// Property 3: Theme data validation completeness
    /// For any array containing null values, the validation system should reject it and throw ArgumentException.
    /// **Validates: Requirements 1.5, 2.3, 2.4**
    /// </summary>
    [Fact]
    public void Property_NullValuesInArrays_ThrowsArgumentException()
    {
        // Test arrays with null values
        Gen.Int[1, 5].Sample(arrayLength =>
        {
            Gen.Int[0, arrayLength - 1].Sample(nullIndex =>
            {
                var builder = new ThemeDataBuilder();

                var array = new string[arrayLength];
                for (var i = 0; i < arrayLength; i++)
                {
                    array[i] = i == nullIndex ? null! : "Valid";
                }

                var act = () => builder.WithCityNames(array, new[] { "Core" }, new[] { "Suf" });

                act.Should().Throw<ArgumentException>()
                    .WithMessage("*contains null value*");
            });
        }, iter: 100);
    }

    /// <summary>
    /// Property 3: Theme data validation completeness
    /// For any array containing whitespace-only strings, the validation system should reject it and throw ArgumentException.
    /// **Validates: Requirements 1.5, 2.3, 2.4**
    /// </summary>
    [Fact]
    public void Property_WhitespaceOnlyStringsInArrays_ThrowsArgumentException()
    {
        // Test arrays with whitespace-only strings
        Gen.Select(
            Gen.Int[1, 5],
            Gen.OneOf(Gen.Const(" "), Gen.Const("  "), Gen.Const("\t"), Gen.Const("\n"), Gen.Const("   \t  "))
        ).Sample((arrayLength, whitespace) =>
        {
            Gen.Int[0, arrayLength - 1].Sample(whitespaceIndex =>
            {
                var builder = new ThemeDataBuilder();

                var array = new string[arrayLength];
                for (var i = 0; i < arrayLength; i++)
                {
                    array[i] = i == whitespaceIndex ? whitespace : "Valid";
                }

                var act = () => builder.WithDistrictNames(array, new[] { "District" });

                act.Should().Throw<ArgumentException>()
                    .WithMessage("*whitespace-only value*");
            });
        }, iter: 100);
    }

    /// <summary>
    /// Property 3: Theme data validation completeness
    /// For any complete and valid theme data, the builder should successfully create a CustomThemeData instance.
    /// **Validates: Requirements 1.5, 2.3, 2.4**
    /// </summary>
    [Fact]
    public void Property_ValidCompleteThemeData_BuildsSuccessfully()
    {
        // Generate valid string arrays
        var validStringArrayGen = Gen.String.Array[1, 10].Where(arr => arr.All(s => !string.IsNullOrWhiteSpace(s)));

        Gen.Select(
            validStringArrayGen, // NPC male prefixes
            validStringArrayGen, // NPC male cores
            validStringArrayGen, // NPC male suffixes
            validStringArrayGen, // City prefixes
            validStringArrayGen, // City cores
            validStringArrayGen  // City suffixes
        ).Sample((npcMalePre, npcMaleCore, npcMaleSuf, cityPre, cityCore, citySuf) =>
        {
            var builder = new ThemeDataBuilder()
                .WithNpcNames(npc => npc
                    .WithMaleNames(npcMalePre, npcMaleCore, npcMaleSuf)
                    .WithFemaleNames(new[] { "F" }, new[] { "e" }, new[] { "male" })
                    .WithNeutralNames(new[] { "N" }, new[] { "eu" }, new[] { "tral" }))
                .WithBuildingNames(building =>
                {
                    building.WithGenericNames(new[] { "Build" }, new[] { "ing" });
                    foreach (BuildingType type in Enum.GetValues(typeof(BuildingType)))
                    {
                        building.WithTypeNames(type, new[] { "Pre" }, new[] { "Desc" }, new[] { "Suf" });
                    }
                })
                .WithCityNames(cityPre, cityCore, citySuf)
                .WithDistrictNames(new[] { "Old" }, new[] { "District" })
                .WithStreetNames(new[] { "Main" }, new[] { "Oak" }, new[] { "Street" })
                .WithFactionNames(new[] { "The" }, new[] { "Guild" }, new[] { "s" });

            var act = () => builder.Build();
            act.Should().NotThrow();

            var result = builder.Build();
            result.Should().NotBeNull();
            result.InternalData.Should().NotBeNull();
        }, iter: 100);
    }

    /// <summary>
    /// Property 3: Theme data validation completeness
    /// For any extension with at least one entity type, the builder should successfully create a ThemeExtension instance.
    /// **Validates: Requirements 1.5, 2.3, 2.4**
    /// </summary>
    [Fact]
    public void Property_ValidExtensionData_BuildsSuccessfully()
    {
        // Test that extensions with partial data build successfully
        Gen.OneOf(
            Gen.Const(Theme.Cyberpunk),
            Gen.Const(Theme.Elves),
            Gen.Const(Theme.Orcs)
        ).Sample(baseTheme =>
        {
            var builder = ThemeDataBuilder.Extend(baseTheme)
                .WithNpcNames(npc => npc
                    .AddMalePrefixes("Extra", "More", "Additional"));

            var act = () => builder.BuildExtension();
            act.Should().NotThrow();

            var result = builder.BuildExtension();
            result.Should().NotBeNull();
            result.BaseThemeIdentifier.Should().NotBeNullOrWhiteSpace();
            result.ExtensionData.Should().NotBeNull();
        }, iter: 100);
    }

    /// <summary>
    /// Property 3: Theme data validation completeness
    /// For any extension with no entity data, the builder should throw ArgumentException.
    /// **Validates: Requirements 1.5, 2.3, 2.4**
    /// </summary>
    [Fact]
    public void Property_EmptyExtension_ThrowsArgumentException()
    {
        // Test that extensions with no data throw ArgumentException
        Gen.OneOf(
            Gen.Const(Theme.Cyberpunk),
            Gen.Const(Theme.Elves),
            Gen.Const(Theme.Orcs)
        ).Sample(baseTheme =>
        {
            var builder = ThemeDataBuilder.Extend(baseTheme);

            var act = () => builder.BuildExtension();
            act.Should().Throw<ArgumentException>()
                .WithMessage("*At least one entity type must be provided*");
        }, iter: 100);
    }

    /// <summary>
    /// Property 3: Theme data validation completeness
    /// Calling Build() on an extension builder should throw InvalidOperationException.
    /// **Validates: Requirements 1.5, 2.3, 2.4**
    /// </summary>
    [Fact]
    public void Property_BuildOnExtensionBuilder_ThrowsInvalidOperationException()
    {
        Gen.OneOf(
            Gen.Const(Theme.Cyberpunk),
            Gen.Const(Theme.Elves),
            Gen.Const(Theme.Orcs)
        ).Sample(baseTheme =>
        {
            var builder = ThemeDataBuilder.Extend(baseTheme)
                .WithCityNames(new[] { "Extra" }, new[] { "City" }, new[] { "ton" });

            var act = () => builder.Build();
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*Cannot call Build() on an extension builder*");
        }, iter: 100);
    }

    /// <summary>
    /// Property 3: Theme data validation completeness
    /// Calling BuildExtension() on a new theme builder should throw InvalidOperationException.
    /// **Validates: Requirements 1.5, 2.3, 2.4**
    /// </summary>
    [Fact]
    public void Property_BuildExtensionOnNewThemeBuilder_ThrowsInvalidOperationException()
    {
        var builder = new ThemeDataBuilder()
            .WithCityNames(new[] { "City" }, new[] { "Core" }, new[] { "ton" });

        var act = () => builder.BuildExtension();
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot call BuildExtension() on a new theme builder*");
    }
}
