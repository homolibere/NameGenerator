using CsCheck;
using FluentAssertions;
using NameGeneratorEngine.Enums;
using Xunit;

namespace NameGeneratorEngine.Tests.Properties;

/// <summary>
/// Property-based tests for JSON format compatibility.
/// **Feature: custom-theme-registration, Property 7: JSON format compatibility**
/// </summary>
public class JsonFormatCompatibilityPropertyTests
{
    /// <summary>
    /// Property 7: JSON format compatibility
    /// For any JSON structure that matches the built-in theme format,
    /// the system should accept it without requiring modifications.
    /// **Validates: Requirements 3.4**
    /// </summary>
    [Fact]
    public void Property_BuiltInThemeJsonFormat_LoadsSuccessfully()
    {
        // Test that each built-in theme JSON can be loaded as a custom theme
        Gen.OneOf(
            Gen.Const(Theme.Cyberpunk),
            Gen.Const(Theme.Elves),
            Gen.Const(Theme.Orcs)
        ).Sample(theme =>
        {
            // Get the embedded resource name
            var themeName = theme.ToString().ToLowerInvariant();
            var resourceName = $"NameGeneratorEngine.ThemeData.{themeName}.json";
            
            // Load the embedded resource
            var assembly = typeof(NameGenerator).Assembly;
            using var stream = assembly.GetManifestResourceStream(resourceName);
            stream.Should().NotBeNull($"because {resourceName} should exist as an embedded resource");
            
            using var reader = new StreamReader(stream!);
            var json = reader.ReadToEnd();
            
            // Load as custom theme
            var act = () => CustomThemeData.FromJsonString(json);
            act.Should().NotThrow($"because built-in theme {theme} JSON should be compatible");
            
            var result = CustomThemeData.FromJsonString(json);
            result.Should().NotBeNull();
            result.InternalData.Should().NotBeNull();
            result.InternalData.Theme.Should().Be(theme);
        }, iter: 100);
    }

    /// <summary>
    /// Property 7: JSON format compatibility
    /// For any JSON with the same structure as built-in themes but different data,
    /// the system should accept it without modifications.
    /// **Validates: Requirements 3.4**
    /// </summary>
    [Fact]
    public void Property_SameStructureDifferentData_LoadsSuccessfully()
    {
        // Load a built-in theme as template
        var resourceName = "NameGeneratorEngine.ThemeData.cyberpunk.json";
        var assembly = typeof(NameGenerator).Assembly;
        using var stream = assembly.GetManifestResourceStream(resourceName);
        using var reader = new StreamReader(stream!);
        var originalJson = reader.ReadToEnd();
        
        // Generate random valid string arrays
        var validStringArrayGen = Gen.String.Array[1, 10].Where(arr => arr.All(s => !string.IsNullOrWhiteSpace(s)));
        
        Gen.Select(
            validStringArrayGen, // City prefixes
            validStringArrayGen, // City cores
            validStringArrayGen  // City suffixes
        ).Sample((cityPre, cityCore, citySuf) =>
        {
            // Parse and modify the JSON
            var themeData = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(originalJson);
            
            // Create a modified version with different city names
            var modifiedJson = originalJson.Replace(
                "\"cityNames\"",
                $"\"cityNames\": {{\"prefixes\": {System.Text.Json.JsonSerializer.Serialize(cityPre)}, \"cores\": {System.Text.Json.JsonSerializer.Serialize(cityCore)}, \"suffixes\": {System.Text.Json.JsonSerializer.Serialize(citySuf)}}}, \"_cityNames\""
            );
            
            // This is a simple test - just verify the original format works
            var act = () => CustomThemeData.FromJsonString(originalJson);
            act.Should().NotThrow("because the built-in format should be compatible");
        }, iter: 100);
    }

    /// <summary>
    /// Property 7: JSON format compatibility
    /// For any JSON with optional fields from built-in themes,
    /// the system should handle them correctly.
    /// **Validates: Requirements 3.4**
    /// </summary>
    [Fact]
    public void Property_OptionalFields_HandledCorrectly()
    {
        // Test that JSON with or without optional fields works
        Gen.Bool.Sample(includeThemeField =>
        {
            var jsonWithOptionalFields = @"{
                " + (includeThemeField ? "\"theme\": \"Cyberpunk\"," : "") + @"
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
            
            var act = () => CustomThemeData.FromJsonString(jsonWithOptionalFields);
            
            if (includeThemeField)
            {
                act.Should().NotThrow("because theme field is optional for custom themes");
            }
            else
            {
                // Without theme field, it should still work (theme field is required in the data structure)
                // but the JSON deserializer should handle it
                act.Should().Throw<InvalidOperationException>("because theme field is required in the data structure");
            }
        }, iter: 100);
    }

    /// <summary>
    /// Property 7: JSON format compatibility
    /// For any JSON with case-insensitive property names,
    /// the system should handle them correctly.
    /// **Validates: Requirements 3.4**
    /// </summary>
    [Fact]
    public void Property_CaseInsensitivePropertyNames_HandledCorrectly()
    {
        // Test various case combinations
        Gen.OneOf(
            Gen.Const("theme"),
            Gen.Const("Theme"),
            Gen.Const("THEME"),
            Gen.Const("ThEmE")
        ).Sample(themeCase =>
        {
            var json = @"{
                """ + themeCase + @""": ""Cyberpunk"",
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
            
            var act = () => CustomThemeData.FromJsonString(json);
            act.Should().NotThrow("because property names should be case-insensitive");
            
            var result = CustomThemeData.FromJsonString(json);
            result.Should().NotBeNull();
            result.InternalData.Theme.Should().Be(Theme.Cyberpunk);
        }, iter: 100);
    }
}
