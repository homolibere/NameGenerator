using CsCheck;
using FluentAssertions;
using Xunit;
using System.Text.Json;

namespace NameGeneratorEngine.Tests.Properties;

/// <summary>
/// Property-based tests for JSON loading functionality.
/// **Feature: custom-theme-registration, Property 5: JSON loading and registration**
/// </summary>
public class JsonLoadingPropertyTests
{
    private static string CreateValidThemeJson(string[] cityPrefixes, string[] cityCores, string[] citySuffixes)
    {
        var themeData = new
        {
            theme = "Cyberpunk",
            npcNames = new
            {
                male = new
                {
                    prefixes = new[] { "M" },
                    cores = new[] { "a" },
                    suffixes = new[] { "le" }
                },
                female = new
                {
                    prefixes = new[] { "F" },
                    cores = new[] { "e" },
                    suffixes = new[] { "male" }
                },
                neutral = new
                {
                    prefixes = new[] { "N" },
                    cores = new[] { "eu" },
                    suffixes = new[] { "tral" }
                }
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
            cityNames = new
            {
                prefixes = cityPrefixes,
                cores = cityCores,
                suffixes = citySuffixes
            },
            districtNames = new
            {
                descriptors = new[] { "Old" },
                locationTypes = new[] { "District" }
            },
            streetNames = new
            {
                prefixes = new[] { "Main" },
                cores = new[] { "Oak" },
                streetSuffixes = new[] { "Street" }
            },
            factionNames = new
            {
                prefixes = new[] { "The" },
                cores = new[] { "Guild" },
                suffixes = new[] { "s" }
            }
        };

        return JsonSerializer.Serialize(themeData, new JsonSerializerOptions { WriteIndented = true });
    }

    /// <summary>
    /// Property 5: JSON loading and registration
    /// For any valid JSON file containing theme data in the correct format,
    /// loading and registering the theme should succeed and make it available for name generation.
    /// **Validates: Requirements 3.1, 3.2**
    /// </summary>
    [Fact]
    public void Property_ValidJsonFile_LoadsSuccessfully()
    {
        // Generate valid theme data and serialize to JSON
        var validStringArrayGen = Gen.String.Array[1, 10].Where(arr => arr.All(s => !string.IsNullOrWhiteSpace(s)));

        Gen.Select(
            validStringArrayGen, // City prefixes
            validStringArrayGen, // City cores
            validStringArrayGen  // City suffixes
        ).Sample((cityPre, cityCore, citySuf) =>
        {
            var json = CreateValidThemeJson(cityPre, cityCore, citySuf);

            // Create a temporary file
            var tempFile = Path.GetTempFileName();
            try
            {
                File.WriteAllText(tempFile, json);

                // Load from JSON file
                var act = () => CustomThemeData.FromJson(tempFile);
                act.Should().NotThrow();

                var result = CustomThemeData.FromJson(tempFile);
                result.Should().NotBeNull();
                result.InternalData.Should().NotBeNull();
                result.InternalData.CityNames.Should().NotBeNull();
                result.InternalData.CityNames.Prefixes.Should().BeEquivalentTo(cityPre);
                result.InternalData.CityNames.Cores.Should().BeEquivalentTo(cityCore);
                result.InternalData.CityNames.Suffixes.Should().BeEquivalentTo(citySuf);
            }
            finally
            {
                // Clean up
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
        }, iter: 100);
    }

    /// <summary>
    /// Property 5: JSON loading and registration
    /// For any valid JSON string containing theme data in the correct format,
    /// loading should succeed and create a valid CustomThemeData instance.
    /// **Validates: Requirements 3.1, 3.2**
    /// </summary>
    [Fact]
    public void Property_ValidJsonString_LoadsSuccessfully()
    {
        // Generate valid theme data and serialize to JSON
        var validStringArrayGen = Gen.String.Array[1, 10].Where(arr => arr.All(s => !string.IsNullOrWhiteSpace(s)));

        Gen.Select(
            validStringArrayGen, // City prefixes
            validStringArrayGen, // City cores
            validStringArrayGen  // City suffixes
        ).Sample((cityPre, cityCore, citySuf) =>
        {
            var json = CreateValidThemeJson(cityPre, cityCore, citySuf);

            // Load from JSON string
            var act = () => CustomThemeData.FromJsonString(json);
            act.Should().NotThrow();

            var result = CustomThemeData.FromJsonString(json);
            result.Should().NotBeNull();
            result.InternalData.Should().NotBeNull();
            result.InternalData.CityNames.Should().NotBeNull();
            result.InternalData.CityNames.Prefixes.Should().BeEquivalentTo(cityPre);
            result.InternalData.CityNames.Cores.Should().BeEquivalentTo(cityCore);
            result.InternalData.CityNames.Suffixes.Should().BeEquivalentTo(citySuf);
        }, iter: 100);
    }
}
