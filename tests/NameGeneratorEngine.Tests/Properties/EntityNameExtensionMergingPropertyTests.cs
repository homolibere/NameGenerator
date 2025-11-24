using CsCheck;
using FluentAssertions;
using NameGeneratorEngine.Enums;
using NameGeneratorEngine.ThemeData;
using NameGeneratorEngine.ThemeData.DataStructures;
using Xunit;

namespace NameGeneratorEngine.Tests.Properties;

/// <summary>
/// Property-based tests for entity name extension merging (City, District, Street, Faction).
/// </summary>
public class EntityNameExtensionMergingPropertyTests
{
    /// <summary>
    /// Feature: custom-theme-registration, Property 24: Entity name extension merging
    /// For any theme extended with additional city name syllables, 
    /// generating city names should include syllables from both original and extended data.
    /// Validates: Requirements 9.5
    /// </summary>
    [Fact]
    public void Property_CityNameExtensionMerging()
    {
        var stringArrayGen = Gen.String[Gen.Char.AlphaNumeric, 3, 10].Array[5, 15];

        Gen.Select(stringArrayGen, stringArrayGen, stringArrayGen)
            .Sample((extensionPrefixes, extensionCores, extensionSuffixes) =>
            {
                // Use a built-in theme as base
                var registry = new ThemeRegistry();
                var baseTheme = registry.GetTheme(Theme.Cyberpunk);

                // Create extension with additional city name data
                var extension = CreateEntityExtension(
                    cityPrefixes: extensionPrefixes,
                    cityCores: extensionCores,
                    citySuffixes: extensionSuffixes);

                registry.RegisterExtension("cyberpunk", extension);

                // Retrieve the merged theme
                var mergedTheme = registry.GetTheme(Theme.Cyberpunk);

                // Verify merged theme contains both base and extension data for city names
                mergedTheme.CityNames.Prefixes.Should().Contain(baseTheme.CityNames.Prefixes);
                mergedTheme.CityNames.Prefixes.Should().Contain(extensionPrefixes);

                mergedTheme.CityNames.Cores.Should().Contain(baseTheme.CityNames.Cores);
                mergedTheme.CityNames.Cores.Should().Contain(extensionCores);

                mergedTheme.CityNames.Suffixes.Should().Contain(baseTheme.CityNames.Suffixes);
                mergedTheme.CityNames.Suffixes.Should().Contain(extensionSuffixes);

                // Verify the merged arrays are longer than the base
                mergedTheme.CityNames.Prefixes.Length.Should().Be(
                    baseTheme.CityNames.Prefixes.Length + extensionPrefixes.Length);
                mergedTheme.CityNames.Cores.Length.Should().Be(
                    baseTheme.CityNames.Cores.Length + extensionCores.Length);
                mergedTheme.CityNames.Suffixes.Length.Should().Be(
                    baseTheme.CityNames.Suffixes.Length + extensionSuffixes.Length);
            }, iter: 100);
    }

    /// <summary>
    /// Feature: custom-theme-registration, Property 24: Entity name extension merging
    /// For any theme extended with additional district name syllables, 
    /// generating district names should include syllables from both original and extended data.
    /// Validates: Requirements 9.5
    /// </summary>
    [Fact]
    public void Property_DistrictNameExtensionMerging()
    {
        var stringArrayGen = Gen.String[Gen.Char.AlphaNumeric, 3, 10].Array[5, 15];

        Gen.Select(stringArrayGen, stringArrayGen)
            .Sample((extensionDescriptors, extensionLocationTypes) =>
            {
                // Use a built-in theme as base
                var registry = new ThemeRegistry();
                var baseTheme = registry.GetTheme(Theme.Elves);

                // Create extension with additional district name data
                var extension = CreateEntityExtension(
                    districtDescriptors: extensionDescriptors,
                    districtLocationTypes: extensionLocationTypes);

                registry.RegisterExtension("elves", extension);

                // Retrieve the merged theme
                var mergedTheme = registry.GetTheme(Theme.Elves);

                // Verify merged theme contains both base and extension data for district names
                mergedTheme.DistrictNames.Descriptors.Should().Contain(baseTheme.DistrictNames.Descriptors);
                mergedTheme.DistrictNames.Descriptors.Should().Contain(extensionDescriptors);

                mergedTheme.DistrictNames.LocationTypes.Should().Contain(baseTheme.DistrictNames.LocationTypes);
                mergedTheme.DistrictNames.LocationTypes.Should().Contain(extensionLocationTypes);

                // Verify the merged arrays are longer than the base
                mergedTheme.DistrictNames.Descriptors.Length.Should().Be(
                    baseTheme.DistrictNames.Descriptors.Length + extensionDescriptors.Length);
                mergedTheme.DistrictNames.LocationTypes.Length.Should().Be(
                    baseTheme.DistrictNames.LocationTypes.Length + extensionLocationTypes.Length);
            }, iter: 100);
    }

    /// <summary>
    /// Feature: custom-theme-registration, Property 24: Entity name extension merging
    /// For any theme extended with additional street name syllables, 
    /// generating street names should include syllables from both original and extended data.
    /// Validates: Requirements 9.5
    /// </summary>
    [Fact]
    public void Property_StreetNameExtensionMerging()
    {
        var stringArrayGen = Gen.String[Gen.Char.AlphaNumeric, 3, 10].Array[5, 15];

        Gen.Select(stringArrayGen, stringArrayGen, stringArrayGen)
            .Sample((extensionPrefixes, extensionCores, extensionSuffixes) =>
            {
                // Use a built-in theme as base
                var registry = new ThemeRegistry();
                var baseTheme = registry.GetTheme(Theme.Orcs);

                // Create extension with additional street name data
                var extension = CreateEntityExtension(
                    streetPrefixes: extensionPrefixes,
                    streetCores: extensionCores,
                    streetSuffixes: extensionSuffixes);

                registry.RegisterExtension("orcs", extension);

                // Retrieve the merged theme
                var mergedTheme = registry.GetTheme(Theme.Orcs);

                // Verify merged theme contains both base and extension data for street names
                mergedTheme.StreetNames.Prefixes.Should().Contain(baseTheme.StreetNames.Prefixes);
                mergedTheme.StreetNames.Prefixes.Should().Contain(extensionPrefixes);

                mergedTheme.StreetNames.Cores.Should().Contain(baseTheme.StreetNames.Cores);
                mergedTheme.StreetNames.Cores.Should().Contain(extensionCores);

                mergedTheme.StreetNames.StreetSuffixes.Should().Contain(baseTheme.StreetNames.StreetSuffixes);
                mergedTheme.StreetNames.StreetSuffixes.Should().Contain(extensionSuffixes);

                // Verify the merged arrays are longer than the base
                mergedTheme.StreetNames.Prefixes.Length.Should().Be(
                    baseTheme.StreetNames.Prefixes.Length + extensionPrefixes.Length);
                mergedTheme.StreetNames.Cores.Length.Should().Be(
                    baseTheme.StreetNames.Cores.Length + extensionCores.Length);
                mergedTheme.StreetNames.StreetSuffixes.Length.Should().Be(
                    baseTheme.StreetNames.StreetSuffixes.Length + extensionSuffixes.Length);
            }, iter: 100);
    }

    /// <summary>
    /// Feature: custom-theme-registration, Property 24: Entity name extension merging
    /// For any theme extended with additional faction name syllables, 
    /// generating faction names should include syllables from both original and extended data.
    /// Validates: Requirements 9.5
    /// </summary>
    [Fact]
    public void Property_FactionNameExtensionMerging()
    {
        var stringArrayGen = Gen.String[Gen.Char.AlphaNumeric, 3, 10].Array[5, 15];

        Gen.Select(stringArrayGen, stringArrayGen, stringArrayGen)
            .Sample((extensionPrefixes, extensionCores, extensionSuffixes) =>
            {
                // Use a built-in theme as base
                var registry = new ThemeRegistry();
                var baseTheme = registry.GetTheme(Theme.Cyberpunk);

                // Create extension with additional faction name data
                var extension = CreateEntityExtension(
                    factionPrefixes: extensionPrefixes,
                    factionCores: extensionCores,
                    factionSuffixes: extensionSuffixes);

                registry.RegisterExtension("cyberpunk", extension);

                // Retrieve the merged theme
                var mergedTheme = registry.GetTheme(Theme.Cyberpunk);

                // Verify merged theme contains both base and extension data for faction names
                mergedTheme.FactionNames.Prefixes.Should().Contain(baseTheme.FactionNames.Prefixes);
                mergedTheme.FactionNames.Prefixes.Should().Contain(extensionPrefixes);

                mergedTheme.FactionNames.Cores.Should().Contain(baseTheme.FactionNames.Cores);
                mergedTheme.FactionNames.Cores.Should().Contain(extensionCores);

                mergedTheme.FactionNames.Suffixes.Should().Contain(baseTheme.FactionNames.Suffixes);
                mergedTheme.FactionNames.Suffixes.Should().Contain(extensionSuffixes);

                // Verify the merged arrays are longer than the base
                mergedTheme.FactionNames.Prefixes.Length.Should().Be(
                    baseTheme.FactionNames.Prefixes.Length + extensionPrefixes.Length);
                mergedTheme.FactionNames.Cores.Length.Should().Be(
                    baseTheme.FactionNames.Cores.Length + extensionCores.Length);
                mergedTheme.FactionNames.Suffixes.Length.Should().Be(
                    baseTheme.FactionNames.Suffixes.Length + extensionSuffixes.Length);
            }, iter: 100);
    }

    /// <summary>
    /// Feature: custom-theme-registration, Property 24: Entity name extension merging
    /// Verifies that all entity types can be extended simultaneously.
    /// Validates: Requirements 9.5
    /// </summary>
    [Fact]
    public void Property_AllEntityTypesExtensionMerging()
    {
        var stringArrayGen = Gen.String[Gen.Char.AlphaNumeric, 3, 10].Array[5, 15];

        Gen.Select(stringArrayGen, stringArrayGen, stringArrayGen, stringArrayGen)
            .Sample((cityData, districtData, streetData, factionData) =>
            {
                // Use a built-in theme as base
                var registry = new ThemeRegistry();
                var baseTheme = registry.GetTheme(Theme.Elves);

                // Create extension with data for all entity types
                var extension = CreateEntityExtension(
                    cityPrefixes: cityData,
                    cityCores: cityData,
                    citySuffixes: cityData,
                    districtDescriptors: districtData,
                    districtLocationTypes: districtData,
                    streetPrefixes: streetData,
                    streetCores: streetData,
                    streetSuffixes: streetData,
                    factionPrefixes: factionData,
                    factionCores: factionData,
                    factionSuffixes: factionData);

                registry.RegisterExtension("elves", extension);

                // Retrieve the merged theme
                var mergedTheme = registry.GetTheme(Theme.Elves);

                // Verify all entity types have merged data
                // City
                mergedTheme.CityNames.Prefixes.Should().Contain(baseTheme.CityNames.Prefixes);
                mergedTheme.CityNames.Prefixes.Should().Contain(cityData);

                // District
                mergedTheme.DistrictNames.Descriptors.Should().Contain(baseTheme.DistrictNames.Descriptors);
                mergedTheme.DistrictNames.Descriptors.Should().Contain(districtData);

                // Street
                mergedTheme.StreetNames.Prefixes.Should().Contain(baseTheme.StreetNames.Prefixes);
                mergedTheme.StreetNames.Prefixes.Should().Contain(streetData);

                // Faction
                mergedTheme.FactionNames.Prefixes.Should().Contain(baseTheme.FactionNames.Prefixes);
                mergedTheme.FactionNames.Prefixes.Should().Contain(factionData);
            }, iter: 100);
    }

    /// <summary>
    /// Helper method to create entity name extension data.
    /// </summary>
    private NameGeneratorEngine.ThemeData.ThemeData CreateEntityExtension(
        string[]? cityPrefixes = null,
        string[]? cityCores = null,
        string[]? citySuffixes = null,
        string[]? districtDescriptors = null,
        string[]? districtLocationTypes = null,
        string[]? streetPrefixes = null,
        string[]? streetCores = null,
        string[]? streetSuffixes = null,
        string[]? factionPrefixes = null,
        string[]? factionCores = null,
        string[]? factionSuffixes = null)
    {
        // Use empty arrays as defaults
        var emptyArray = Array.Empty<string>();

        // Create minimal valid theme data (extensions don't need complete data)
        var genderData = new GenderNameData
        {
            Prefixes = emptyArray,
            Cores = emptyArray,
            Suffixes = emptyArray
        };

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
                Male = genderData,
                Female = genderData,
                Neutral = genderData
            },
            BuildingNames = new BuildingNameData
            {
                GenericPrefixes = emptyArray,
                GenericSuffixes = emptyArray,
                TypeData = buildingTypeData
            },
            CityNames = new CityNameData
            {
                Prefixes = cityPrefixes ?? emptyArray,
                Cores = cityCores ?? emptyArray,
                Suffixes = citySuffixes ?? emptyArray
            },
            DistrictNames = new DistrictNameData
            {
                Descriptors = districtDescriptors ?? emptyArray,
                LocationTypes = districtLocationTypes ?? emptyArray
            },
            StreetNames = new StreetNameData
            {
                Prefixes = streetPrefixes ?? emptyArray,
                Cores = streetCores ?? emptyArray,
                StreetSuffixes = streetSuffixes ?? emptyArray
            },
            FactionNames = new FactionNameData
            {
                Prefixes = factionPrefixes ?? emptyArray,
                Cores = factionCores ?? emptyArray,
                Suffixes = factionSuffixes ?? emptyArray
            }
        };
    }
}
