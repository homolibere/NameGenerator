using CsCheck;
using FluentAssertions;
using NameGeneratorEngine.Enums;
using NameGeneratorEngine.ThemeData;
using NameGeneratorEngine.ThemeData.DataStructures;
using Xunit;

namespace NameGeneratorEngine.Tests.Properties;

/// <summary>
/// Property-based tests for building name extension merging.
/// </summary>
public class BuildingNameExtensionMergingPropertyTests
{
    /// <summary>
    /// Feature: custom-theme-registration, Property 23: Building name extension merging
    /// For any theme extended with additional building name data for a specific building type, 
    /// generating building names for that type should include data from both original 
    /// and extended sources.
    /// Validates: Requirements 9.4
    /// </summary>
    [Fact]
    public void Property_BuildingNameExtensionMerging_GenericData()
    {
        var stringArrayGen = Gen.String[Gen.Char.AlphaNumeric, 3, 10].Array[5, 15];

        Gen.Select(stringArrayGen, stringArrayGen)
            .Sample((extensionPrefixes, extensionSuffixes) =>
            {
                // Use a built-in theme as base
                var registry = new ThemeRegistry();
                var baseTheme = registry.GetTheme(Theme.Cyberpunk);

                // Create extension with additional generic building name data
                var extension = CreateBuildingExtension(
                    genericPrefixes: extensionPrefixes,
                    genericSuffixes: extensionSuffixes);

                registry.RegisterExtension("cyberpunk", extension);

                // Retrieve the merged theme
                var mergedTheme = registry.GetTheme(Theme.Cyberpunk);

                // Verify merged theme contains both base and extension data for generic building names
                mergedTheme.BuildingNames.GenericPrefixes.Should().Contain(baseTheme.BuildingNames.GenericPrefixes);
                mergedTheme.BuildingNames.GenericPrefixes.Should().Contain(extensionPrefixes);

                mergedTheme.BuildingNames.GenericSuffixes.Should().Contain(baseTheme.BuildingNames.GenericSuffixes);
                mergedTheme.BuildingNames.GenericSuffixes.Should().Contain(extensionSuffixes);

                // Verify the merged arrays are longer than the base
                mergedTheme.BuildingNames.GenericPrefixes.Length.Should().Be(
                    baseTheme.BuildingNames.GenericPrefixes.Length + extensionPrefixes.Length);
                mergedTheme.BuildingNames.GenericSuffixes.Length.Should().Be(
                    baseTheme.BuildingNames.GenericSuffixes.Length + extensionSuffixes.Length);
            }, iter: 100);
    }

    /// <summary>
    /// Feature: custom-theme-registration, Property 23: Building name extension merging
    /// For any theme extended with additional building name data for a specific building type, 
    /// generating building names for that type should include data from both original 
    /// and extended sources.
    /// Validates: Requirements 9.4
    /// </summary>
    [Fact]
    public void Property_BuildingNameExtensionMerging_TypeSpecificData()
    {
        var stringArrayGen = Gen.String[Gen.Char.AlphaNumeric, 3, 10].Array[5, 15];
        var buildingTypeGen = Gen.Int[0, 6].Select(i => (BuildingType)i);

        Gen.Select(buildingTypeGen, stringArrayGen, stringArrayGen, stringArrayGen)
            .Sample((buildingType, extensionPrefixes, extensionDescriptors, extensionSuffixes) =>
            {
                // Use a built-in theme as base
                var registry = new ThemeRegistry();
                var baseTheme = registry.GetTheme(Theme.Elves);

                // Create extension with additional type-specific building name data
                var extension = CreateBuildingExtension(
                    buildingType: buildingType,
                    typePrefixes: extensionPrefixes,
                    typeDescriptors: extensionDescriptors,
                    typeSuffixes: extensionSuffixes);

                registry.RegisterExtension("elves", extension);

                // Retrieve the merged theme
                var mergedTheme = registry.GetTheme(Theme.Elves);

                // Verify merged theme contains both base and extension data for the specific building type
                var baseTypeData = baseTheme.BuildingNames.TypeData[buildingType];
                var mergedTypeData = mergedTheme.BuildingNames.TypeData[buildingType];

                mergedTypeData.Prefixes.Should().Contain(baseTypeData.Prefixes);
                mergedTypeData.Prefixes.Should().Contain(extensionPrefixes);

                mergedTypeData.Descriptors.Should().Contain(baseTypeData.Descriptors);
                mergedTypeData.Descriptors.Should().Contain(extensionDescriptors);

                mergedTypeData.Suffixes.Should().Contain(baseTypeData.Suffixes);
                mergedTypeData.Suffixes.Should().Contain(extensionSuffixes);

                // Verify the merged arrays are longer than the base
                mergedTypeData.Prefixes.Length.Should().Be(
                    baseTypeData.Prefixes.Length + extensionPrefixes.Length);
                mergedTypeData.Descriptors.Length.Should().Be(
                    baseTypeData.Descriptors.Length + extensionDescriptors.Length);
                mergedTypeData.Suffixes.Length.Should().Be(
                    baseTypeData.Suffixes.Length + extensionSuffixes.Length);
            }, iter: 100);
    }

    /// <summary>
    /// Feature: custom-theme-registration, Property 23: Building name extension merging
    /// Verifies that both generic and type-specific data can be extended simultaneously.
    /// Validates: Requirements 9.4
    /// </summary>
    [Fact]
    public void Property_BuildingNameExtensionMerging_GenericAndTypeSpecific()
    {
        var stringArrayGen = Gen.String[Gen.Char.AlphaNumeric, 3, 10].Array[5, 15];

        Gen.Select(stringArrayGen, stringArrayGen, stringArrayGen)
            .Sample((genericData, residentialData, commercialData) =>
            {
                // Use a built-in theme as base
                var registry = new ThemeRegistry();
                var baseTheme = registry.GetTheme(Theme.Orcs);

                // Create extension with both generic and type-specific data
                var extension = CreateBuildingExtension(
                    genericPrefixes: genericData,
                    genericSuffixes: genericData,
                    buildingType: BuildingType.Residential,
                    typePrefixes: residentialData,
                    typeDescriptors: residentialData,
                    typeSuffixes: residentialData);

                // Add another type-specific extension
                var extension2 = CreateBuildingExtension(
                    buildingType: BuildingType.Commercial,
                    typePrefixes: commercialData,
                    typeDescriptors: commercialData,
                    typeSuffixes: commercialData);

                registry.RegisterExtension("orcs", extension);
                registry.RegisterExtension("orcs", extension2);

                // Retrieve the merged theme
                var mergedTheme = registry.GetTheme(Theme.Orcs);

                // Verify generic data is merged
                mergedTheme.BuildingNames.GenericPrefixes.Should().Contain(baseTheme.BuildingNames.GenericPrefixes);
                mergedTheme.BuildingNames.GenericPrefixes.Should().Contain(genericData);

                // Verify residential type data is merged
                var mergedResidential = mergedTheme.BuildingNames.TypeData[BuildingType.Residential];
                mergedResidential.Prefixes.Should().Contain(baseTheme.BuildingNames.TypeData[BuildingType.Residential].Prefixes);
                mergedResidential.Prefixes.Should().Contain(residentialData);

                // Verify commercial type data is merged
                var mergedCommercial = mergedTheme.BuildingNames.TypeData[BuildingType.Commercial];
                mergedCommercial.Prefixes.Should().Contain(baseTheme.BuildingNames.TypeData[BuildingType.Commercial].Prefixes);
                mergedCommercial.Prefixes.Should().Contain(commercialData);
            }, iter: 100);
    }

    /// <summary>
    /// Feature: custom-theme-registration, Property 23: Building name extension merging
    /// Verifies that multiple extensions to the same building type accumulate correctly.
    /// Validates: Requirements 9.4
    /// </summary>
    [Fact]
    public void Property_BuildingNameExtensionMerging_MultipleExtensionsSameType()
    {
        var stringArrayGen = Gen.String[Gen.Char.AlphaNumeric, 3, 10].Array[5, 15];

        Gen.Select(stringArrayGen, stringArrayGen, stringArrayGen)
            .Sample((ext1Data, ext2Data, ext3Data) =>
            {
                // Use a built-in theme as base
                var registry = new ThemeRegistry();
                var baseTheme = registry.GetTheme(Theme.Cyberpunk);

                // Create three extensions for the same building type
                var ext1 = CreateBuildingExtension(
                    buildingType: BuildingType.Industrial,
                    typePrefixes: ext1Data,
                    typeDescriptors: ext1Data,
                    typeSuffixes: ext1Data);

                var ext2 = CreateBuildingExtension(
                    buildingType: BuildingType.Industrial,
                    typePrefixes: ext2Data,
                    typeDescriptors: ext2Data,
                    typeSuffixes: ext2Data);

                var ext3 = CreateBuildingExtension(
                    buildingType: BuildingType.Industrial,
                    typePrefixes: ext3Data,
                    typeDescriptors: ext3Data,
                    typeSuffixes: ext3Data);

                registry.RegisterExtension("cyberpunk", ext1);
                registry.RegisterExtension("cyberpunk", ext2);
                registry.RegisterExtension("cyberpunk", ext3);

                // Retrieve the merged theme
                var mergedTheme = registry.GetTheme(Theme.Cyberpunk);

                // Verify all extensions are merged
                var baseIndustrial = baseTheme.BuildingNames.TypeData[BuildingType.Industrial];
                var mergedIndustrial = mergedTheme.BuildingNames.TypeData[BuildingType.Industrial];

                mergedIndustrial.Prefixes.Should().Contain(baseIndustrial.Prefixes);
                mergedIndustrial.Prefixes.Should().Contain(ext1Data);
                mergedIndustrial.Prefixes.Should().Contain(ext2Data);
                mergedIndustrial.Prefixes.Should().Contain(ext3Data);

                // Verify the total length is correct
                var expectedLength = baseIndustrial.Prefixes.Length + 
                                   ext1Data.Length + ext2Data.Length + ext3Data.Length;
                mergedIndustrial.Prefixes.Length.Should().Be(expectedLength);
            }, iter: 100);
    }

    /// <summary>
    /// Helper method to create building name extension data.
    /// </summary>
    private NameGeneratorEngine.ThemeData.ThemeData CreateBuildingExtension(
        string[]? genericPrefixes = null,
        string[]? genericSuffixes = null,
        BuildingType? buildingType = null,
        string[]? typePrefixes = null,
        string[]? typeDescriptors = null,
        string[]? typeSuffixes = null)
    {
        // Use empty arrays as defaults
        var emptyArray = Array.Empty<string>();

        var buildingTypeData = new Dictionary<BuildingType, BuildingTypeData>();

        // If a specific building type is provided, add its data
        if (buildingType.HasValue)
        {
            buildingTypeData[buildingType.Value] = new BuildingTypeData
            {
                Prefixes = typePrefixes ?? emptyArray,
                Descriptors = typeDescriptors ?? emptyArray,
                Suffixes = typeSuffixes ?? emptyArray
            };
        }

        // Fill in empty data for other building types
        foreach (var type in Enum.GetValues<BuildingType>())
        {
            if (!buildingTypeData.ContainsKey(type))
            {
                buildingTypeData[type] = new BuildingTypeData
                {
                    Prefixes = emptyArray,
                    Descriptors = emptyArray,
                    Suffixes = emptyArray
                };
            }
        }

        // Create minimal valid theme data (extensions don't need complete data)
        var genderData = new GenderNameData
        {
            Prefixes = emptyArray,
            Cores = emptyArray,
            Suffixes = emptyArray
        };

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
                GenericPrefixes = genericPrefixes ?? emptyArray,
                GenericSuffixes = genericSuffixes ?? emptyArray,
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
