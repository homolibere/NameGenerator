using CsCheck;
using FluentAssertions;
using NameGeneratorEngine.Enums;
using NameGeneratorEngine.ThemeData;
using NameGeneratorEngine.ThemeData.DataStructures;
using Xunit;

namespace NameGeneratorEngine.Tests.Properties;

/// <summary>
/// Property-based tests for NPC name extension merging.
/// </summary>
public class NpcNameExtensionMergingPropertyTests
{
    /// <summary>
    /// Feature: custom-theme-registration, Property 22: NPC name extension merging
    /// For any theme extended with additional NPC name syllables for a specific gender, 
    /// generating NPC names for that gender should include syllables from both original 
    /// and extended data.
    /// Validates: Requirements 9.3
    /// </summary>
    [Fact]
    public void Property_NpcNameExtensionMerging_Male()
    {
        var stringArrayGen = Gen.String[Gen.Char.AlphaNumeric, 3, 10].Array[5, 15];

        Gen.Select(stringArrayGen, stringArrayGen, stringArrayGen)
            .Sample((extensionPrefixes, extensionCores, extensionSuffixes) =>
            {
                // Use a built-in theme as base
                var registry = new ThemeRegistry();
                var baseTheme = registry.GetTheme(Theme.Cyberpunk);

                // Create extension with additional male NPC name data
                var extension = CreateNpcExtension(
                    malePrefixes: extensionPrefixes,
                    maleCores: extensionCores,
                    maleSuffixes: extensionSuffixes);

                registry.RegisterExtension("cyberpunk", extension);

                // Retrieve the merged theme
                var mergedTheme = registry.GetTheme(Theme.Cyberpunk);

                // Verify merged theme contains both base and extension data for male names
                mergedTheme.NpcNames.Male.Prefixes.Should().Contain(baseTheme.NpcNames.Male.Prefixes);
                mergedTheme.NpcNames.Male.Prefixes.Should().Contain(extensionPrefixes);

                mergedTheme.NpcNames.Male.Cores.Should().Contain(baseTheme.NpcNames.Male.Cores);
                mergedTheme.NpcNames.Male.Cores.Should().Contain(extensionCores);

                mergedTheme.NpcNames.Male.Suffixes.Should().Contain(baseTheme.NpcNames.Male.Suffixes);
                mergedTheme.NpcNames.Male.Suffixes.Should().Contain(extensionSuffixes);

                // Verify the merged arrays are longer than the base
                mergedTheme.NpcNames.Male.Prefixes.Length.Should().Be(
                    baseTheme.NpcNames.Male.Prefixes.Length + extensionPrefixes.Length);
                mergedTheme.NpcNames.Male.Cores.Length.Should().Be(
                    baseTheme.NpcNames.Male.Cores.Length + extensionCores.Length);
                mergedTheme.NpcNames.Male.Suffixes.Length.Should().Be(
                    baseTheme.NpcNames.Male.Suffixes.Length + extensionSuffixes.Length);
            }, iter: 100);
    }

    /// <summary>
    /// Feature: custom-theme-registration, Property 22: NPC name extension merging
    /// For any theme extended with additional NPC name syllables for a specific gender, 
    /// generating NPC names for that gender should include syllables from both original 
    /// and extended data.
    /// Validates: Requirements 9.3
    /// </summary>
    [Fact]
    public void Property_NpcNameExtensionMerging_Female()
    {
        var stringArrayGen = Gen.String[Gen.Char.AlphaNumeric, 3, 10].Array[5, 15];

        Gen.Select(stringArrayGen, stringArrayGen, stringArrayGen)
            .Sample((extensionPrefixes, extensionCores, extensionSuffixes) =>
            {
                // Use a built-in theme as base
                var registry = new ThemeRegistry();
                var baseTheme = registry.GetTheme(Theme.Elves);

                // Create extension with additional female NPC name data
                var extension = CreateNpcExtension(
                    femalePrefixes: extensionPrefixes,
                    femaleCores: extensionCores,
                    femaleSuffixes: extensionSuffixes);

                registry.RegisterExtension("elves", extension);

                // Retrieve the merged theme
                var mergedTheme = registry.GetTheme(Theme.Elves);

                // Verify merged theme contains both base and extension data for female names
                mergedTheme.NpcNames.Female.Prefixes.Should().Contain(baseTheme.NpcNames.Female.Prefixes);
                mergedTheme.NpcNames.Female.Prefixes.Should().Contain(extensionPrefixes);

                mergedTheme.NpcNames.Female.Cores.Should().Contain(baseTheme.NpcNames.Female.Cores);
                mergedTheme.NpcNames.Female.Cores.Should().Contain(extensionCores);

                mergedTheme.NpcNames.Female.Suffixes.Should().Contain(baseTheme.NpcNames.Female.Suffixes);
                mergedTheme.NpcNames.Female.Suffixes.Should().Contain(extensionSuffixes);

                // Verify the merged arrays are longer than the base
                mergedTheme.NpcNames.Female.Prefixes.Length.Should().Be(
                    baseTheme.NpcNames.Female.Prefixes.Length + extensionPrefixes.Length);
                mergedTheme.NpcNames.Female.Cores.Length.Should().Be(
                    baseTheme.NpcNames.Female.Cores.Length + extensionCores.Length);
                mergedTheme.NpcNames.Female.Suffixes.Length.Should().Be(
                    baseTheme.NpcNames.Female.Suffixes.Length + extensionSuffixes.Length);
            }, iter: 100);
    }

    /// <summary>
    /// Feature: custom-theme-registration, Property 22: NPC name extension merging
    /// For any theme extended with additional NPC name syllables for a specific gender, 
    /// generating NPC names for that gender should include syllables from both original 
    /// and extended data.
    /// Validates: Requirements 9.3
    /// </summary>
    [Fact]
    public void Property_NpcNameExtensionMerging_Neutral()
    {
        var stringArrayGen = Gen.String[Gen.Char.AlphaNumeric, 3, 10].Array[5, 15];

        Gen.Select(stringArrayGen, stringArrayGen, stringArrayGen)
            .Sample((extensionPrefixes, extensionCores, extensionSuffixes) =>
            {
                // Use a built-in theme as base
                var registry = new ThemeRegistry();
                var baseTheme = registry.GetTheme(Theme.Orcs);

                // Create extension with additional neutral NPC name data
                var extension = CreateNpcExtension(
                    neutralPrefixes: extensionPrefixes,
                    neutralCores: extensionCores,
                    neutralSuffixes: extensionSuffixes);

                registry.RegisterExtension("orcs", extension);

                // Retrieve the merged theme
                var mergedTheme = registry.GetTheme(Theme.Orcs);

                // Verify merged theme contains both base and extension data for neutral names
                mergedTheme.NpcNames.Neutral.Prefixes.Should().Contain(baseTheme.NpcNames.Neutral.Prefixes);
                mergedTheme.NpcNames.Neutral.Prefixes.Should().Contain(extensionPrefixes);

                mergedTheme.NpcNames.Neutral.Cores.Should().Contain(baseTheme.NpcNames.Neutral.Cores);
                mergedTheme.NpcNames.Neutral.Cores.Should().Contain(extensionCores);

                mergedTheme.NpcNames.Neutral.Suffixes.Should().Contain(baseTheme.NpcNames.Neutral.Suffixes);
                mergedTheme.NpcNames.Neutral.Suffixes.Should().Contain(extensionSuffixes);

                // Verify the merged arrays are longer than the base
                mergedTheme.NpcNames.Neutral.Prefixes.Length.Should().Be(
                    baseTheme.NpcNames.Neutral.Prefixes.Length + extensionPrefixes.Length);
                mergedTheme.NpcNames.Neutral.Cores.Length.Should().Be(
                    baseTheme.NpcNames.Neutral.Cores.Length + extensionCores.Length);
                mergedTheme.NpcNames.Neutral.Suffixes.Length.Should().Be(
                    baseTheme.NpcNames.Neutral.Suffixes.Length + extensionSuffixes.Length);
            }, iter: 100);
    }

    /// <summary>
    /// Feature: custom-theme-registration, Property 22: NPC name extension merging
    /// Verifies that all three genders can be extended simultaneously.
    /// Validates: Requirements 9.3
    /// </summary>
    [Fact]
    public void Property_NpcNameExtensionMerging_AllGenders()
    {
        var stringArrayGen = Gen.String[Gen.Char.AlphaNumeric, 3, 10].Array[5, 15];

        // Generate three sets of data (one per gender)
        Gen.Select(stringArrayGen, stringArrayGen, stringArrayGen)
            .Sample((maleData, femaleData, neutralData) =>
            {
                // Use a built-in theme as base
                var registry = new ThemeRegistry();
                var baseTheme = registry.GetTheme(Theme.Cyberpunk);

                // Create extension with data for all genders
                var extension = CreateNpcExtension(
                    malePrefixes: maleData,
                    maleCores: maleData,
                    maleSuffixes: maleData,
                    femalePrefixes: femaleData,
                    femaleCores: femaleData,
                    femaleSuffixes: femaleData,
                    neutralPrefixes: neutralData,
                    neutralCores: neutralData,
                    neutralSuffixes: neutralData);

                registry.RegisterExtension("cyberpunk", extension);

                // Retrieve the merged theme
                var mergedTheme = registry.GetTheme(Theme.Cyberpunk);

                // Verify all genders have merged data
                // Male
                mergedTheme.NpcNames.Male.Prefixes.Should().Contain(baseTheme.NpcNames.Male.Prefixes);
                mergedTheme.NpcNames.Male.Prefixes.Should().Contain(maleData);
                
                // Female
                mergedTheme.NpcNames.Female.Prefixes.Should().Contain(baseTheme.NpcNames.Female.Prefixes);
                mergedTheme.NpcNames.Female.Prefixes.Should().Contain(femaleData);
                
                // Neutral
                mergedTheme.NpcNames.Neutral.Prefixes.Should().Contain(baseTheme.NpcNames.Neutral.Prefixes);
                mergedTheme.NpcNames.Neutral.Prefixes.Should().Contain(neutralData);
            }, iter: 100);
    }

    /// <summary>
    /// Helper method to create NPC name extension data.
    /// </summary>
    private NameGeneratorEngine.ThemeData.ThemeData CreateNpcExtension(
        string[]? malePrefixes = null,
        string[]? maleCores = null,
        string[]? maleSuffixes = null,
        string[]? femalePrefixes = null,
        string[]? femaleCores = null,
        string[]? femaleSuffixes = null,
        string[]? neutralPrefixes = null,
        string[]? neutralCores = null,
        string[]? neutralSuffixes = null)
    {
        // Use empty arrays as defaults
        var emptyArray = Array.Empty<string>();

        var maleData = new GenderNameData
        {
            Prefixes = malePrefixes ?? emptyArray,
            Cores = maleCores ?? emptyArray,
            Suffixes = maleSuffixes ?? emptyArray
        };

        var femaleData = new GenderNameData
        {
            Prefixes = femalePrefixes ?? emptyArray,
            Cores = femaleCores ?? emptyArray,
            Suffixes = femaleSuffixes ?? emptyArray
        };

        var neutralData = new GenderNameData
        {
            Prefixes = neutralPrefixes ?? emptyArray,
            Cores = neutralCores ?? emptyArray,
            Suffixes = neutralSuffixes ?? emptyArray
        };

        // Create minimal valid theme data (extensions don't need complete data)
        var buildingTypeData = new Dictionary<BuildingType, BuildingTypeData>();
        foreach (var type in Enum.GetValues<BuildingType>())
        {
            buildingTypeData[type] = new BuildingTypeData
            {
                Prefixes = emptyArray,
                Descriptors = emptyArray,
                Suffixes = emptyArray
            };
        }

        return new NameGeneratorEngine.ThemeData.ThemeData
        {
            Theme = Theme.Cyberpunk,
            NpcNames = new NpcNameData
            {
                Male = maleData,
                Female = femaleData,
                Neutral = neutralData
            },
            BuildingNames = new BuildingNameData
            {
                GenericPrefixes = emptyArray,
                GenericSuffixes = emptyArray,
                TypeData = buildingTypeData
            },
            CityNames = new CityNameData
            {
                Prefixes = emptyArray,
                Cores = emptyArray,
                Suffixes = emptyArray
            },
            DistrictNames = new DistrictNameData
            {
                Descriptors = emptyArray,
                LocationTypes = emptyArray
            },
            StreetNames = new StreetNameData
            {
                Prefixes = emptyArray,
                Cores = emptyArray,
                StreetSuffixes = emptyArray
            },
            FactionNames = new FactionNameData
            {
                Prefixes = emptyArray,
                Cores = emptyArray,
                Suffixes = emptyArray
            }
        };
    }
}
