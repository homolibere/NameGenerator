using CsCheck;
using FluentAssertions;
using NameGeneratorEngine.Enums;
using Xunit;
using System.Text.Json;

namespace NameGeneratorEngine.Tests.Properties;

/// <summary>
/// Property-based tests for validation consistency between JSON and programmatic registration.
/// **Feature: custom-theme-registration, Property 8: Validation consistency**
/// </summary>
public class ValidationConsistencyPropertyTests
{
    /// <summary>
    /// Property 8: Validation consistency
    /// For any invalid theme data, both JSON loading and programmatic registration
    /// should reject it with the same validation rules and error messages.
    /// **Validates: Requirements 3.5**
    /// </summary>
    [Fact]
    public void Property_MissingNpcNames_RejectedConsistently()
    {
        // Test that missing NPC names are rejected both ways
        
        // Programmatic approach
        var builderAct = () =>
        {
            var builder = new ThemeDataBuilder()
                .WithBuildingNames(building =>
                {
                    building.WithGenericNames(new[] { "Build" }, new[] { "ing" });
                    foreach (BuildingType type in Enum.GetValues(typeof(BuildingType)))
                    {
                        building.WithTypeNames(type, new[] { "Pre" }, new[] { "Desc" }, new[] { "Suf" });
                    }
                })
                .WithCityNames(new[] { "City" }, new[] { "Core" }, new[] { "ton" })
                .WithDistrictNames(new[] { "Old" }, new[] { "District" })
                .WithStreetNames(new[] { "Main" }, new[] { "Oak" }, new[] { "Street" })
                .WithFactionNames(new[] { "The" }, new[] { "Guild" }, new[] { "s" });

            return builder.Build();
        };

        builderAct.Should().Throw<ArgumentException>()
            .WithMessage("*Missing required fields*");

        // JSON approach - missing npcNames
        var jsonWithoutNpc = @"{
            ""theme"": ""Cyberpunk"",
            ""buildingNames"": {
                ""genericPrefixes"": [""Build""],
                ""genericSuffixes"": [""ing""],
                ""typeData"": {
                    ""Residential"": { ""prefixes"": [""Home""], ""descriptors"": [""Tower""], ""suffixes"": [""s""] },
                    ""Commercial"": { ""prefixes"": [""Shop""], ""descriptors"": [""Center""], ""suffixes"": [""s""] },
                    ""Industrial"": { ""prefixes"": [""Factory""], ""descriptors"": [""Complex""], ""suffixes"": [""""] },
                    ""Government"": { ""prefixes"": [""City""], ""descriptors"": [""Hall""], ""suffixes"": [""""] },
                    ""Entertainment"": { ""prefixes"": [""Fun""], ""descriptors"": [""Zone""], ""suffixes"": [""""] },
                    ""Medical"": { ""prefixes"": [""Health""], ""descriptors"": [""Center""], ""suffixes"": [""""] },
                    ""Educational"": { ""prefixes"": [""School""], ""descriptors"": [""House""], ""suffixes"": [""""] }
                }
            },
            ""cityNames"": {
                ""prefixes"": [""City""],
                ""cores"": [""Core""],
                ""suffixes"": [""ton""]
            },
            ""districtNames"": {
                ""descriptors"": [""Old""],
                ""locationTypes"": [""District""]
            },
            ""streetNames"": {
                ""prefixes"": [""Main""],
                ""cores"": [""Oak""],
                ""streetSuffixes"": [""Street""]
            },
            ""factionNames"": {
                ""prefixes"": [""The""],
                ""cores"": [""Guild""],
                ""suffixes"": [""s""]
            }
        }";

        var jsonAct = () => CustomThemeData.FromJsonString(jsonWithoutNpc);
        jsonAct.Should().Throw<InvalidOperationException>();
    }

    /// <summary>
    /// Property 8: Validation consistency
    /// For any theme data with empty arrays, both approaches should reject it consistently.
    /// **Validates: Requirements 3.5**
    /// </summary>
    [Fact]
    public void Property_EmptyArrays_RejectedConsistently()
    {
        // Test that empty arrays are rejected both ways
        Gen.OneOf(
            Gen.Const("prefixes"),
            Gen.Const("cores"),
            Gen.Const("suffixes")
        ).Sample(emptyField =>
        {
            // Programmatic approach
            var builderAct = () =>
            {
                var builder = new ThemeDataBuilder();

                var prefixes = emptyField == "prefixes" ? Array.Empty<string>() : new[] { "City" };
                var cores = emptyField == "cores" ? Array.Empty<string>() : new[] { "Core" };
                var suffixes = emptyField == "suffixes" ? Array.Empty<string>() : new[] { "ton" };

                builder.WithCityNames(prefixes, cores, suffixes);
            };

            builderAct.Should().Throw<ArgumentException>()
                .WithMessage("*cannot be empty*");

            // JSON approach
            var jsonWithEmptyArray = @"{
                ""theme"": ""Cyberpunk"",
                ""npcNames"": {
                    ""male"": {
                        ""prefixes"": " + (emptyField == "prefixes" ? "[]" : "[\"M\"]") + @",
                        ""cores"": " + (emptyField == "cores" ? "[]" : "[\"a\"]") + @",
                        ""suffixes"": " + (emptyField == "suffixes" ? "[]" : "[\"le\"]") + @"
                    },
                    ""female"": {
                        ""prefixes"": [""F""],
                        ""cores"": [""e""],
                        ""suffixes"": [""male""]
                    },
                    ""neutral"": {
                        ""prefixes"": [""N""],
                        ""cores"": [""eu""],
                        ""suffixes"": [""tral""]
                    }
                },
                ""buildingNames"": {
                    ""genericPrefixes"": [""Build""],
                    ""genericSuffixes"": [""ing""],
                    ""typeData"": {
                        ""Residential"": { ""prefixes"": [""Home""], ""descriptors"": [""Tower""], ""suffixes"": [""s""] },
                        ""Commercial"": { ""prefixes"": [""Shop""], ""descriptors"": [""Center""], ""suffixes"": [""s""] },
                        ""Industrial"": { ""prefixes"": [""Factory""], ""descriptors"": [""Complex""], ""suffixes"": [""""] },
                        ""Government"": { ""prefixes"": [""City""], ""descriptors"": [""Hall""], ""suffixes"": [""""] },
                        ""Entertainment"": { ""prefixes"": [""Fun""], ""descriptors"": [""Zone""], ""suffixes"": [""""] },
                        ""Medical"": { ""prefixes"": [""Health""], ""descriptors"": [""Center""], ""suffixes"": [""""] },
                        ""Educational"": { ""prefixes"": [""School""], ""descriptors"": [""House""], ""suffixes"": [""""] }
                    }
                },
                ""cityNames"": {
                    ""prefixes"": [""City""],
                    ""cores"": [""Core""],
                    ""suffixes"": [""ton""]
                },
                ""districtNames"": {
                    ""descriptors"": [""Old""],
                    ""locationTypes"": [""District""]
                },
                ""streetNames"": {
                    ""prefixes"": [""Main""],
                    ""cores"": [""Oak""],
                    ""streetSuffixes"": [""Street""]
                },
                ""factionNames"": {
                    ""prefixes"": [""The""],
                    ""cores"": [""Guild""],
                    ""suffixes"": [""s""]
                }
            }";

            var jsonAct = () => CustomThemeData.FromJsonString(jsonWithEmptyArray);
            jsonAct.Should().Throw<InvalidOperationException>()
                .WithMessage("*null or empty*");
        }, iter: 100);
    }

    /// <summary>
    /// Property 8: Validation consistency
    /// For any theme data with null values in arrays, both approaches should reject it consistently.
    /// **Validates: Requirements 3.5**
    /// </summary>
    [Fact]
    public void Property_NullValuesInArrays_RejectedConsistently()
    {
        // Programmatic approach
        var builderAct = () =>
        {
            var builder = new ThemeDataBuilder();
            builder.WithCityNames(new[] { "City", null! }, new[] { "Core" }, new[] { "ton" });
        };

        builderAct.Should().Throw<ArgumentException>()
            .WithMessage("*contains null value*");

        // JSON approach - null values in arrays
        var jsonWithNull = @"{
            ""theme"": ""Cyberpunk"",
            ""npcNames"": {
                ""male"": {
                    ""prefixes"": [""M"", null],
                    ""cores"": [""a""],
                    ""suffixes"": [""le""]
                },
                ""female"": {
                    ""prefixes"": [""F""],
                    ""cores"": [""e""],
                    ""suffixes"": [""male""]
                },
                ""neutral"": {
                    ""prefixes"": [""N""],
                    ""cores"": [""eu""],
                    ""suffixes"": [""tral""]
                }
            },
            ""buildingNames"": {
                ""genericPrefixes"": [""Build""],
                ""genericSuffixes"": [""ing""],
                ""typeData"": {
                    ""Residential"": { ""prefixes"": [""Home""], ""descriptors"": [""Tower""], ""suffixes"": [""s""] },
                    ""Commercial"": { ""prefixes"": [""Shop""], ""descriptors"": [""Center""], ""suffixes"": [""s""] },
                    ""Industrial"": { ""prefixes"": [""Factory""], ""descriptors"": [""Complex""], ""suffixes"": [""""] },
                    ""Government"": { ""prefixes"": [""City""], ""descriptors"": [""Hall""], ""suffixes"": [""""] },
                    ""Entertainment"": { ""prefixes"": [""Fun""], ""descriptors"": [""Zone""], ""suffixes"": [""""] },
                    ""Medical"": { ""prefixes"": [""Health""], ""descriptors"": [""Center""], ""suffixes"": [""""] },
                    ""Educational"": { ""prefixes"": [""School""], ""descriptors"": [""House""], ""suffixes"": [""""] }
                }
            },
            ""cityNames"": {
                ""prefixes"": [""City""],
                ""cores"": [""Core""],
                ""suffixes"": [""ton""]
            },
            ""districtNames"": {
                ""descriptors"": [""Old""],
                ""locationTypes"": [""District""]
            },
            ""streetNames"": {
                ""prefixes"": [""Main""],
                ""cores"": [""Oak""],
                ""streetSuffixes"": [""Street""]
            },
            ""factionNames"": {
                ""prefixes"": [""The""],
                ""cores"": [""Guild""],
                ""suffixes"": [""s""]
            }
        }";

        var jsonAct = () => CustomThemeData.FromJsonString(jsonWithNull);
        jsonAct.Should().Throw<InvalidOperationException>()
            .WithMessage("*contains invalid entries*");
    }

    /// <summary>
    /// Property 8: Validation consistency
    /// For any theme data with whitespace-only strings, both approaches should reject it consistently.
    /// **Validates: Requirements 3.5**
    /// </summary>
    [Fact]
    public void Property_WhitespaceOnlyStrings_RejectedConsistently()
    {
        // Programmatic approach
        var builderAct = () =>
        {
            var builder = new ThemeDataBuilder();
            builder.WithDistrictNames(new[] { "Old", "   " }, new[] { "District" });
        };

        builderAct.Should().Throw<ArgumentException>()
            .WithMessage("*whitespace-only value*");

        // JSON approach - whitespace-only strings
        var jsonWithWhitespace = @"{
            ""theme"": ""Cyberpunk"",
            ""npcNames"": {
                ""male"": {
                    ""prefixes"": [""M""],
                    ""cores"": [""a""],
                    ""suffixes"": [""le""]
                },
                ""female"": {
                    ""prefixes"": [""F""],
                    ""cores"": [""e""],
                    ""suffixes"": [""male""]
                },
                ""neutral"": {
                    ""prefixes"": [""N""],
                    ""cores"": [""eu""],
                    ""suffixes"": [""tral""]
                }
            },
            ""buildingNames"": {
                ""genericPrefixes"": [""Build""],
                ""genericSuffixes"": [""ing""],
                ""typeData"": {
                    ""Residential"": { ""prefixes"": [""Home""], ""descriptors"": [""Tower""], ""suffixes"": [""s""] },
                    ""Commercial"": { ""prefixes"": [""Shop""], ""descriptors"": [""Center""], ""suffixes"": [""s""] },
                    ""Industrial"": { ""prefixes"": [""Factory""], ""descriptors"": [""Complex""], ""suffixes"": [""""] },
                    ""Government"": { ""prefixes"": [""City""], ""descriptors"": [""Hall""], ""suffixes"": [""""] },
                    ""Entertainment"": { ""prefixes"": [""Fun""], ""descriptors"": [""Zone""], ""suffixes"": [""""] },
                    ""Medical"": { ""prefixes"": [""Health""], ""descriptors"": [""Center""], ""suffixes"": [""""] },
                    ""Educational"": { ""prefixes"": [""School""], ""descriptors"": [""House""], ""suffixes"": [""""] }
                }
            },
            ""cityNames"": {
                ""prefixes"": [""City""],
                ""cores"": [""Core""],
                ""suffixes"": [""ton""]
            },
            ""districtNames"": {
                ""descriptors"": [""Old"", ""   ""],
                ""locationTypes"": [""District""]
            },
            ""streetNames"": {
                ""prefixes"": [""Main""],
                ""cores"": [""Oak""],
                ""streetSuffixes"": [""Street""]
            },
            ""factionNames"": {
                ""prefixes"": [""The""],
                ""cores"": [""Guild""],
                ""suffixes"": [""s""]
            }
        }";

        var jsonAct = () => CustomThemeData.FromJsonString(jsonWithWhitespace);
        jsonAct.Should().Throw<InvalidOperationException>()
            .WithMessage("*whitespace*");
    }

    /// <summary>
    /// Property 8: Validation consistency
    /// For any valid complete theme data, both approaches should accept it consistently.
    /// **Validates: Requirements 3.5**
    /// </summary>
    [Fact]
    public void Property_ValidCompleteData_AcceptedConsistently()
    {
        var validStringArrayGen = Gen.String.Array[1, 10].Where(arr => arr.All(s => !string.IsNullOrWhiteSpace(s)));

        Gen.Select(
            validStringArrayGen, // City prefixes
            validStringArrayGen, // City cores
            validStringArrayGen  // City suffixes
        ).Sample((cityPre, cityCore, citySuf) =>
        {
            // Programmatic approach
            var builderAct = () =>
            {
                var builder = new ThemeDataBuilder()
                    .WithNpcNames(npc => npc
                        .WithMaleNames(new[] { "M" }, new[] { "a" }, new[] { "le" })
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
                
                return builder.Build();
            };
            
            builderAct.Should().NotThrow();
            var builderResult = builderAct();
            builderResult.Should().NotBeNull();
            
            // JSON approach
            var themeData = new
            {
                theme = "Cyberpunk",
                npcNames = new
                {
                    male = new { prefixes = new[] { "M" }, cores = new[] { "a" }, suffixes = new[] { "le" } },
                    female = new { prefixes = new[] { "F" }, cores = new[] { "e" }, suffixes = new[] { "male" } },
                    neutral = new { prefixes = new[] { "N" }, cores = new[] { "eu" }, suffixes = new[] { "tral" } }
                },
                buildingNames = new
                {
                    genericPrefixes = new[] { "Build" },
                    genericSuffixes = new[] { "ing" },
                    typeData = new Dictionary<string, object>
                    {
                        ["Residential"] = new { prefixes = new[] { "Home" }, descriptors = new[] { "Tower" }, suffixes = new[] { "s" } },
                        ["Commercial"] = new { prefixes = new[] { "Shop" }, descriptors = new[] { "Center" }, suffixes = new[] { "s" } },
                        ["Industrial"] = new { prefixes = new[] { "Factory" }, descriptors = new[] { "Complex" }, suffixes = new[] { "" } },
                        ["Government"] = new { prefixes = new[] { "City" }, descriptors = new[] { "Hall" }, suffixes = new[] { "" } },
                        ["Entertainment"] = new { prefixes = new[] { "Fun" }, descriptors = new[] { "Zone" }, suffixes = new[] { "" } },
                        ["Medical"] = new { prefixes = new[] { "Health" }, descriptors = new[] { "Center" }, suffixes = new[] { "" } },
                        ["Educational"] = new { prefixes = new[] { "School" }, descriptors = new[] { "House" }, suffixes = new[] { "" } }
                    }
                },
                cityNames = new { prefixes = cityPre, cores = cityCore, suffixes = citySuf },
                districtNames = new { descriptors = new[] { "Old" }, locationTypes = new[] { "District" } },
                streetNames = new { prefixes = new[] { "Main" }, cores = new[] { "Oak" }, streetSuffixes = new[] { "Street" } },
                factionNames = new { prefixes = new[] { "The" }, cores = new[] { "Guild" }, suffixes = new[] { "s" } }
            };
            
            var json = JsonSerializer.Serialize(themeData, new JsonSerializerOptions { WriteIndented = true });
            
            var jsonAct = () => CustomThemeData.FromJsonString(json);
            jsonAct.Should().NotThrow();
            var jsonResult = jsonAct();
            jsonResult.Should().NotBeNull();
            
            // Both should produce equivalent results
            jsonResult.InternalData.CityNames.Prefixes.Should().BeEquivalentTo(cityPre);
            jsonResult.InternalData.CityNames.Cores.Should().BeEquivalentTo(cityCore);
            jsonResult.InternalData.CityNames.Suffixes.Should().BeEquivalentTo(citySuf);
        }, iter: 100);
    }
}
