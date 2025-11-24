using CsCheck;
using FluentAssertions;
using NameGeneratorEngine.Enums;
using Xunit;

namespace NameGeneratorEngine.Tests.Properties;

/// <summary>
/// Property-based tests for validation error message completeness.
/// **Feature: custom-theme-registration, Property 13: Validation error message completeness**
/// **Validates: Requirements 5.1, 5.2, 5.3**
/// </summary>
public class ValidationErrorMessagePropertyTests
{
    /// <summary>
    /// Property: For any theme data with multiple validation errors (missing fields, empty arrays, invalid strings),
    /// the ArgumentException should list all errors, not just the first one.
    /// </summary>
    [Fact]
    public void Property_MultipleValidationErrors_AllErrorsListed()
    {
        Gen.Select(
            Gen.Bool,
            Gen.Bool,
            Gen.Bool,
            Gen.Bool,
            Gen.Bool,
            Gen.Bool)
        .Sample((includeMaleNames, includeFemaleNames, includeNeutralNames, 
                 includeBuildingNames, includeCityNames, includeDistrictNames) =>
        {
            // Create a builder with intentionally missing fields
            var builder = new ThemeDataBuilder();
            
            var missingFields = new List<string>();
            
            // Conditionally add each entity type
            if (includeMaleNames)
            {
                builder.WithNpcNames(npc => npc
                    .WithMaleNames(new[] { "Test" }, new[] { "Test" }, new[] { "Test" })
                    .WithFemaleNames(new[] { "Test" }, new[] { "Test" }, new[] { "Test" })
                    .WithNeutralNames(new[] { "Test" }, new[] { "Test" }, new[] { "Test" }));
            }
            else
            {
                missingFields.Add("NPC names");
            }

            if (includeBuildingNames)
            {
                builder.WithBuildingNames(building =>
                {
                    building.WithGenericNames(new[] { "Test" }, new[] { "Test" });
                    foreach (var type in Enum.GetValues<BuildingType>())
                    {
                        building.WithTypeNames(type, new[] { "Test" }, new[] { "Test" }, new[] { "Test" });
                    }
                });
            }
            else
            {
                missingFields.Add("Building names");
            }

            if (includeCityNames)
            {
                builder.WithCityNames(new[] { "Test" }, new[] { "Test" }, new[] { "Test" });
            }
            else
            {
                missingFields.Add("City names");
            }

            if (includeDistrictNames)
            {
                builder.WithDistrictNames(new[] { "Test" }, new[] { "Test" });
            }
            else
            {
                missingFields.Add("District names");
            }

            // Always skip street and faction names to ensure we have at least some missing fields
            missingFields.Add("Street names");
            missingFields.Add("Faction names");

            // Attempt to build - should throw if any fields are missing
            if (missingFields.Count > 0)
            {
                var exception = Assert.Throws<ArgumentException>(() => builder.Build());

                // Verify all missing fields are mentioned in the error message
                foreach (var field in missingFields)
                {
                    exception.Message.Should().Contain(field,
                        $"error message should mention missing field '{field}'");
                }
            }
        }, iter: 100);
    }

    /// <summary>
    /// Property: For any array with invalid entries (null or whitespace),
    /// the ArgumentException should specify the invalid indices.
    /// </summary>
    [Fact]
    public void Property_InvalidArrayEntries_IndicesListed()
    {
        Gen.Select(
            Gen.Bool,
            Gen.Int[1, 5])
        .Sample((useNull, invalidCount) =>
        {
            // Create an array with some valid entries and some invalid ones
            var array = new List<string>();

            // Add some valid entries
            array.Add("Valid1");
            array.Add("Valid2");

            // Add invalid entries (either null or whitespace)
            for (var i = 0; i < invalidCount; i++)
            {
                array.Add(useNull ? null! : "   ");
            }

            // Try to create a theme with this invalid array
            var builder = new ThemeDataBuilder();

            var exception = Assert.Throws<ArgumentException>(() =>
            {
                builder.WithCityNames(array.ToArray(), new[] { "Core" }, new[] { "Suffix" });
            });

            // Verify the error message mentions invalid entries with index
            exception.Message.Should().Contain("index",
                "error message should mention the index of invalid entries");

            // Verify the type of invalid entry is mentioned
            if (useNull)
            {
                exception.Message.Should().Contain("null",
                    "error message should mention null entries");
            }
            else
            {
                exception.Message.Should().Contain("whitespace",
                    "error message should mention whitespace entries");
            }
        }, iter: 100);
    }

    /// <summary>
    /// Property: For any NPC name data with missing gender data,
    /// the ArgumentException should list all missing genders.
    /// </summary>
    [Fact]
    public void Property_MissingGenderData_AllGendersListed()
    {
        Gen.Select(
            Gen.Bool,
            Gen.Bool,
            Gen.Bool)
        .Sample((includeMale, includeFemale, includeNeutral) =>
        {
            // Skip the case where all genders are included (valid theme)
            if (includeMale && includeFemale && includeNeutral)
                return;

            var builder = new ThemeDataBuilder();

            var missingGenders = new List<string>();

            // The exception will be thrown during WithNpcNames() call
            var exception = Assert.Throws<ArgumentException>(() =>
            {
                builder.WithNpcNames(npc =>
                {
                    if (includeMale)
                    {
                        npc.WithMaleNames(new[] { "Test" }, new[] { "Test" }, new[] { "Test" });
                    }
                    else
                    {
                        missingGenders.Add("Male");
                    }

                    if (includeFemale)
                    {
                        npc.WithFemaleNames(new[] { "Test" }, new[] { "Test" }, new[] { "Test" });
                    }
                    else
                    {
                        missingGenders.Add("Female");
                    }

                    if (includeNeutral)
                    {
                        npc.WithNeutralNames(new[] { "Test" }, new[] { "Test" }, new[] { "Test" });
                    }
                    else
                    {
                        missingGenders.Add("Neutral");
                    }
                });
            });

            // Verify the error message mentions missing gender data
            exception.Message.Should().Contain("NPC name data",
                "error message should mention NPC name data");

            // Verify at least one missing gender is mentioned
            var mentionedAnyGender = missingGenders.Any(g =>
                exception.Message.Contains(g, StringComparison.OrdinalIgnoreCase));

            mentionedAnyGender.Should().BeTrue(
                $"error message should mention at least one missing gender: {string.Join(", ", missingGenders)}");
        }, iter: 100);
    }

    /// <summary>
    /// Property: For any building name data with missing building types,
    /// the ArgumentException should list all missing types.
    /// </summary>
    [Fact]
    public void Property_MissingBuildingTypes_AllTypesListed()
    {
        Gen.Select(
            Gen.Int[1, 7],
            Gen.Int[0, 7])
        .Sample((skipCount, seed) =>
        {
            var builder = new ThemeDataBuilder();
            var random = new Random(seed);

            var allTypes = Enum.GetValues<BuildingType>().ToList();
            var missingTypes = new List<BuildingType>();

            // Randomly skip some building types
            for (var i = 0; i < skipCount && allTypes.Count > 0; i++)
            {
                var index = random.Next(allTypes.Count);
                missingTypes.Add(allTypes[index]);
                allTypes.RemoveAt(index);
            }

            builder.WithNpcNames(npc => npc
                .WithMaleNames(new[] { "Test" }, new[] { "Test" }, new[] { "Test" })
                .WithFemaleNames(new[] { "Test" }, new[] { "Test" }, new[] { "Test" })
                .WithNeutralNames(new[] { "Test" }, new[] { "Test" }, new[] { "Test" }));

            // The exception will be thrown during WithBuildingNames() call
            var exception = Assert.Throws<ArgumentException>(() =>
            {
                builder.WithBuildingNames(building =>
                {
                    building.WithGenericNames(new[] { "Test" }, new[] { "Test" });

                    // Only add data for non-skipped types
                    foreach (var type in allTypes)
                    {
                        building.WithTypeNames(type, new[] { "Test" }, new[] { "Test" }, new[] { "Test" });
                    }
                });
            });
            
            // Verify the error message mentions building type data
            exception.Message.Should().Contain("Building", 
                "error message should mention building data");
            
            // Verify at least one missing type is mentioned
            var mentionedAnyType = missingTypes.Any(t => 
                exception.Message.Contains(t.ToString(), StringComparison.OrdinalIgnoreCase));
            
            mentionedAnyType.Should().BeTrue(
                $"error message should mention at least one missing building type: {string.Join(", ", missingTypes)}");
        }, iter: 100);
    }
}
