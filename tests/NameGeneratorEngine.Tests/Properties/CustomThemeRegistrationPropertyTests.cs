using CsCheck;
using FluentAssertions;
using NameGeneratorEngine.Enums;
using NameGeneratorEngine.ThemeData;
using NameGeneratorEngine.ThemeData.DataStructures;
using Xunit;

namespace NameGeneratorEngine.Tests.Properties;

/// <summary>
/// Property-based tests for custom theme registration and retrieval.
/// </summary>
public class CustomThemeRegistrationPropertyTests
{
    /// <summary>
    /// Feature: custom-theme-registration, Property 1: Custom theme registration and retrieval
    /// For any valid custom theme data, when registered with a unique identifier, 
    /// the theme should be retrievable and usable for name generation.
    /// Validates: Requirements 1.2, 1.3
    /// </summary>
    [Fact]
    public void Property_CustomThemeRegistrationAndRetrieval()
    {
        var identifierGen = Gen.String[Gen.Char.AlphaNumeric, 5, 20];
        var stringArrayGen = Gen.String[Gen.Char.AlphaNumeric, 3, 10].Array[5, 15];

        Gen.Select(identifierGen, stringArrayGen, stringArrayGen, stringArrayGen)
            .Sample((identifier, prefixes, cores, suffixes) =>
            {
                // Create a valid custom theme with complete data
                var themeData = CreateValidThemeData(prefixes, cores, suffixes);
                var registry = new ThemeRegistry();

                // Register the custom theme
                registry.RegisterCustomTheme(identifier, themeData);

                // Retrieve the theme
                var retrievedTheme = registry.GetTheme(identifier);

                // Verify the theme was retrieved successfully
                retrievedTheme.Should().NotBeNull();
                
                // Verify NPC name data is preserved
                retrievedTheme.NpcNames.Male.Prefixes.Should().BeEquivalentTo(prefixes);
                retrievedTheme.NpcNames.Male.Cores.Should().BeEquivalentTo(cores);
                retrievedTheme.NpcNames.Male.Suffixes.Should().BeEquivalentTo(suffixes);

                // Verify the theme is marked as registered
                registry.HasCustomTheme(identifier).Should().BeTrue();

                // Verify the theme appears in registered theme names
                var registeredThemes = registry.GetRegisteredThemeNames();
                registeredThemes.Should().Contain(identifier);
            }, iter: 100);
    }

    /// <summary>
    /// Feature: custom-theme-registration, Property 2: Theme isolation
    /// For any set of registered custom themes with distinct identifiers, 
    /// generating names from one theme should only use data from that specific theme 
    /// and not mix data from other themes.
    /// Validates: Requirements 1.4
    /// </summary>
    [Fact]
    public void Property_ThemeIsolation()
    {
        var identifierGen = Gen.String[Gen.Char.AlphaNumeric, 5, 20];
        var stringArrayGen = Gen.String[Gen.Char.AlphaNumeric, 3, 10].Array[5, 15];

        Gen.Select(identifierGen, identifierGen, stringArrayGen, stringArrayGen)
            .Where(t => t.Item1 != t.Item2) // Ensure distinct identifiers
            .Sample((id1, id2, data1, data2) =>
            {
                // Create two themes with distinct data
                var theme1 = CreateValidThemeData(data1, data1, data1);
                var theme2 = CreateValidThemeData(data2, data2, data2);
                
                var registry = new ThemeRegistry();
                registry.RegisterCustomTheme(id1, theme1);
                registry.RegisterCustomTheme(id2, theme2);

                // Retrieve both themes
                var retrieved1 = registry.GetTheme(id1);
                var retrieved2 = registry.GetTheme(id2);

                // Verify theme 1 data is isolated
                retrieved1.NpcNames.Male.Prefixes.Should().BeEquivalentTo(data1);
                retrieved1.NpcNames.Male.Prefixes.Should().NotContain(data2);

                // Verify theme 2 data is isolated
                retrieved2.NpcNames.Male.Prefixes.Should().BeEquivalentTo(data2);
                retrieved2.NpcNames.Male.Prefixes.Should().NotContain(data1);
            }, iter: 100);
    }

    /// <summary>
    /// Feature: custom-theme-registration, Property 14: Theme identifier conflict detection
    /// For any registered theme identifier, attempting to register another theme 
    /// with the same identifier (case-insensitive) should throw InvalidOperationException.
    /// Validates: Requirements 5.4
    /// </summary>
    [Fact]
    public void Property_ThemeIdentifierConflictDetection()
    {
        var identifierGen = Gen.String[Gen.Char.AlphaNumeric, 5, 20];
        var stringArrayGen = Gen.String[Gen.Char.AlphaNumeric, 3, 10].Array[5, 15];

        Gen.Select(identifierGen, stringArrayGen)
            .Sample((identifier, data) =>
            {
                var theme1 = CreateValidThemeData(data, data, data);
                var theme2 = CreateValidThemeData(data, data, data);
                
                var registry = new ThemeRegistry();
                registry.RegisterCustomTheme(identifier, theme1);

                // Attempt to register another theme with the same identifier
                var act = () => registry.RegisterCustomTheme(identifier, theme2);

                // Should throw InvalidOperationException
                act.Should().Throw<InvalidOperationException>()
                    .WithMessage($"*{identifier}*already been registered*");
            }, iter: 100);
    }

    /// <summary>
    /// Feature: custom-theme-registration, Property 16: String identifier case-insensitivity
    /// For any registered custom theme, accessing it with different case variations 
    /// of the identifier should return the same theme data.
    /// Validates: Requirements 7.3
    /// </summary>
    [Fact]
    public void Property_StringIdentifierCaseInsensitivity()
    {
        var identifierGen = Gen.String[Gen.Char.AlphaNumeric, 5, 20];
        var stringArrayGen = Gen.String[Gen.Char.AlphaNumeric, 3, 10].Array[5, 15];

        Gen.Select(identifierGen, stringArrayGen)
            .Sample((identifier, data) =>
            {
                var theme = CreateValidThemeData(data, data, data);
                var registry = new ThemeRegistry();
                registry.RegisterCustomTheme(identifier, theme);

                // Access with different case variations
                var lower = registry.GetTheme(identifier.ToLowerInvariant());
                var upper = registry.GetTheme(identifier.ToUpperInvariant());
                var original = registry.GetTheme(identifier);

                // All should return equivalent data
                lower.NpcNames.Male.Prefixes.Should().BeEquivalentTo(data);
                upper.NpcNames.Male.Prefixes.Should().BeEquivalentTo(data);
                original.NpcNames.Male.Prefixes.Should().BeEquivalentTo(data);

                // HasCustomTheme should also be case-insensitive
                registry.HasCustomTheme(identifier.ToLowerInvariant()).Should().BeTrue();
                registry.HasCustomTheme(identifier.ToUpperInvariant()).Should().BeTrue();
            }, iter: 100);
    }

    /// <summary>
    /// Feature: custom-theme-registration, Property 21: Theme extension merging
    /// For any built-in or custom theme extended with additional data, 
    /// generating names should produce results using both the original theme data 
    /// and the extension data.
    /// Validates: Requirements 9.1, 9.2
    /// </summary>
    [Fact]
    public void Property_ThemeExtensionMerging()
    {
        var stringArrayGen = Gen.String[Gen.Char.AlphaNumeric, 3, 10].Array[5, 15];

        Gen.Select(stringArrayGen, stringArrayGen)
            .Sample((baseData, extensionData) =>
            {
                // Use a built-in theme as base
                var registry = new ThemeRegistry();
                var baseTheme = registry.GetTheme(Theme.Cyberpunk);

                // Create extension with additional data
                var extension = CreateValidThemeData(extensionData, extensionData, extensionData);
                registry.RegisterExtension("cyberpunk", extension);

                // Retrieve the merged theme
                var mergedTheme = registry.GetTheme(Theme.Cyberpunk);

                // Verify merged theme contains both base and extension data
                mergedTheme.NpcNames.Male.Prefixes.Should().Contain(baseTheme.NpcNames.Male.Prefixes);
                mergedTheme.NpcNames.Male.Prefixes.Should().Contain(extensionData);

                // Verify the merged array is longer than the base
                mergedTheme.NpcNames.Male.Prefixes.Length.Should().BeGreaterThan(baseTheme.NpcNames.Male.Prefixes.Length);
            }, iter: 100);
    }

    /// <summary>
    /// Feature: custom-theme-registration, Property 26: Multiple extension merging
    /// For any theme with multiple extensions applied in sequence, 
    /// generating names should include data from the base theme and all extensions, 
    /// with later extensions adding to earlier ones.
    /// Validates: Requirements 10.5
    /// </summary>
    [Fact]
    public void Property_MultipleExtensionMerging()
    {
        var stringArrayGen = Gen.String[Gen.Char.AlphaNumeric, 3, 10].Array[5, 15];

        Gen.Select(stringArrayGen, stringArrayGen, stringArrayGen)
            .Sample((ext1Data, ext2Data, ext3Data) =>
            {
                var registry = new ThemeRegistry();
                var baseTheme = registry.GetTheme(Theme.Elves);

                // Register three extensions
                var ext1 = CreateValidThemeData(ext1Data, ext1Data, ext1Data);
                var ext2 = CreateValidThemeData(ext2Data, ext2Data, ext2Data);
                var ext3 = CreateValidThemeData(ext3Data, ext3Data, ext3Data);

                registry.RegisterExtension("elves", ext1);
                registry.RegisterExtension("elves", ext2);
                registry.RegisterExtension("elves", ext3);

                // Retrieve the merged theme
                var mergedTheme = registry.GetTheme(Theme.Elves);

                // Verify merged theme contains base and all extension data
                mergedTheme.NpcNames.Male.Prefixes.Should().Contain(baseTheme.NpcNames.Male.Prefixes);
                mergedTheme.NpcNames.Male.Prefixes.Should().Contain(ext1Data);
                mergedTheme.NpcNames.Male.Prefixes.Should().Contain(ext2Data);
                mergedTheme.NpcNames.Male.Prefixes.Should().Contain(ext3Data);

                // Verify the total length is correct
                var expectedLength = baseTheme.NpcNames.Male.Prefixes.Length + 
                                   ext1Data.Length + ext2Data.Length + ext3Data.Length;
                mergedTheme.NpcNames.Male.Prefixes.Length.Should().Be(expectedLength);
            }, iter: 100);
    }

    /// <summary>
    /// Helper method to create valid theme data for testing.
    /// </summary>
    private NameGeneratorEngine.ThemeData.ThemeData CreateValidThemeData(string[] prefixes, string[] cores, string[] suffixes)
    {
        var genderData = new GenderNameData
        {
            Prefixes = prefixes,
            Cores = cores,
            Suffixes = suffixes
        };

        var buildingTypeData = new Dictionary<BuildingType, BuildingTypeData>();
        foreach (var type in Enum.GetValues<BuildingType>())
        {
            buildingTypeData[type] = new BuildingTypeData
            {
                Prefixes = prefixes,
                Descriptors = cores,
                Suffixes = suffixes
            };
        }

        return new NameGeneratorEngine.ThemeData.ThemeData
        {
            Theme = Theme.Cyberpunk, // Doesn't matter for custom themes
            NpcNames = new NpcNameData
            {
                Male = genderData,
                Female = genderData,
                Neutral = genderData
            },
            BuildingNames = new BuildingNameData
            {
                GenericPrefixes = prefixes,
                GenericSuffixes = suffixes,
                TypeData = buildingTypeData
            },
            CityNames = new CityNameData
            {
                Prefixes = prefixes,
                Cores = cores,
                Suffixes = suffixes
            },
            DistrictNames = new DistrictNameData
            {
                Descriptors = prefixes,
                LocationTypes = suffixes
            },
            StreetNames = new StreetNameData
            {
                Prefixes = prefixes,
                Cores = cores,
                StreetSuffixes = suffixes
            },
            FactionNames = new FactionNameData
            {
                Prefixes = prefixes,
                Cores = cores,
                Suffixes = suffixes
            }
        };
    }
}
