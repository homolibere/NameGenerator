using CsCheck;
using FluentAssertions;
using NameGeneratorEngine.Enums;
using NameGeneratorEngine.ThemeData;
using NameGeneratorEngine.ThemeData.DataStructures;
using Xunit;

namespace NameGeneratorEngine.Tests.Properties;

/// <summary>
/// Property-based tests for partial extension validation.
/// </summary>
public class PartialExtensionValidationPropertyTests
{
    /// <summary>
    /// Feature: custom-theme-registration, Property 25: Partial extension validation
    /// For any theme extension containing only some entity types (e.g., only NPC names), 
    /// the system should accept it and only validate the provided arrays, 
    /// not require complete theme data.
    /// Validates: Requirements 10.2, 10.3
    /// </summary>
    [Fact]
    public void Property_PartialExtension_OnlyNpcNames()
    {
        var stringArrayGen = Gen.String[Gen.Char.AlphaNumeric, 3, 10].Array[5, 15];

        stringArrayGen.Sample(npcData =>
            {
                // Create extension with only NPC name data (all other entity types empty)
                var extension = CreatePartialExtension(npcData: npcData);

                var registry = new ThemeRegistry();
                
                // Should not throw - partial extensions are valid
                var act = () => registry.RegisterExtension("cyberpunk", extension);
                act.Should().NotThrow();

                // Verify the extension is applied
                var mergedTheme = registry.GetTheme(Theme.Cyberpunk);
                mergedTheme.NpcNames.Male.Prefixes.Should().Contain(npcData);
            }, iter: 100);
    }

    /// <summary>
    /// Feature: custom-theme-registration, Property 25: Partial extension validation
    /// For any theme extension containing only some entity types (e.g., only building names), 
    /// the system should accept it and only validate the provided arrays, 
    /// not require complete theme data.
    /// Validates: Requirements 10.2, 10.3
    /// </summary>
    [Fact]
    public void Property_PartialExtension_OnlyBuildingNames()
    {
        var stringArrayGen = Gen.String[Gen.Char.AlphaNumeric, 3, 10].Array[5, 15];

        stringArrayGen.Sample(buildingData =>
            {
                // Create extension with only building name data (all other entity types empty)
                var extension = CreatePartialExtension(buildingData: buildingData);

                var registry = new ThemeRegistry();
                
                // Should not throw - partial extensions are valid
                var act = () => registry.RegisterExtension("elves", extension);
                act.Should().NotThrow();

                // Verify the extension is applied
                var mergedTheme = registry.GetTheme(Theme.Elves);
                mergedTheme.BuildingNames.GenericPrefixes.Should().Contain(buildingData);
            }, iter: 100);
    }

    /// <summary>
    /// Feature: custom-theme-registration, Property 25: Partial extension validation
    /// For any theme extension containing only some entity types (e.g., only city names), 
    /// the system should accept it and only validate the provided arrays, 
    /// not require complete theme data.
    /// Validates: Requirements 10.2, 10.3
    /// </summary>
    [Fact]
    public void Property_PartialExtension_OnlyCityNames()
    {
        var stringArrayGen = Gen.String[Gen.Char.AlphaNumeric, 3, 10].Array[5, 15];

        stringArrayGen.Sample(cityData =>
            {
                // Create extension with only city name data (all other entity types empty)
                var extension = CreatePartialExtension(cityData: cityData);

                var registry = new ThemeRegistry();
                
                // Should not throw - partial extensions are valid
                var act = () => registry.RegisterExtension("orcs", extension);
                act.Should().NotThrow();

                // Verify the extension is applied
                var mergedTheme = registry.GetTheme(Theme.Orcs);
                mergedTheme.CityNames.Prefixes.Should().Contain(cityData);
            }, iter: 100);
    }

    /// <summary>
    /// Feature: custom-theme-registration, Property 25: Partial extension validation
    /// For any theme extension containing only some entity types (e.g., only district names), 
    /// the system should accept it and only validate the provided arrays, 
    /// not require complete theme data.
    /// Validates: Requirements 10.2, 10.3
    /// </summary>
    [Fact]
    public void Property_PartialExtension_OnlyDistrictNames()
    {
        var stringArrayGen = Gen.String[Gen.Char.AlphaNumeric, 3, 10].Array[5, 15];

        stringArrayGen.Sample(districtData =>
            {
                // Create extension with only district name data (all other entity types empty)
                var extension = CreatePartialExtension(districtData: districtData);

                var registry = new ThemeRegistry();
                
                // Should not throw - partial extensions are valid
                var act = () => registry.RegisterExtension("cyberpunk", extension);
                act.Should().NotThrow();

                // Verify the extension is applied
                var mergedTheme = registry.GetTheme(Theme.Cyberpunk);
                mergedTheme.DistrictNames.Descriptors.Should().Contain(districtData);
            }, iter: 100);
    }

    /// <summary>
    /// Feature: custom-theme-registration, Property 25: Partial extension validation
    /// For any theme extension containing only some entity types (e.g., only street names), 
    /// the system should accept it and only validate the provided arrays, 
    /// not require complete theme data.
    /// Validates: Requirements 10.2, 10.3
    /// </summary>
    [Fact]
    public void Property_PartialExtension_OnlyStreetNames()
    {
        var stringArrayGen = Gen.String[Gen.Char.AlphaNumeric, 3, 10].Array[5, 15];

        stringArrayGen.Sample(streetData =>
            {
                // Create extension with only street name data (all other entity types empty)
                var extension = CreatePartialExtension(streetData: streetData);

                var registry = new ThemeRegistry();
                
                // Should not throw - partial extensions are valid
                var act = () => registry.RegisterExtension("elves", extension);
                act.Should().NotThrow();

                // Verify the extension is applied
                var mergedTheme = registry.GetTheme(Theme.Elves);
                mergedTheme.StreetNames.Prefixes.Should().Contain(streetData);
            }, iter: 100);
    }

    /// <summary>
    /// Feature: custom-theme-registration, Property 25: Partial extension validation
    /// For any theme extension containing only some entity types (e.g., only faction names), 
    /// the system should accept it and only validate the provided arrays, 
    /// not require complete theme data.
    /// Validates: Requirements 10.2, 10.3
    /// </summary>
    [Fact]
    public void Property_PartialExtension_OnlyFactionNames()
    {
        var stringArrayGen = Gen.String[Gen.Char.AlphaNumeric, 3, 10].Array[5, 15];

        stringArrayGen.Sample(factionData =>
            {
                // Create extension with only faction name data (all other entity types empty)
                var extension = CreatePartialExtension(factionData: factionData);

                var registry = new ThemeRegistry();
                
                // Should not throw - partial extensions are valid
                var act = () => registry.RegisterExtension("orcs", extension);
                act.Should().NotThrow();

                // Verify the extension is applied
                var mergedTheme = registry.GetTheme(Theme.Orcs);
                mergedTheme.FactionNames.Prefixes.Should().Contain(factionData);
            }, iter: 100);
    }

    /// <summary>
    /// Feature: custom-theme-registration, Property 25: Partial extension validation
    /// Verifies that extensions with multiple entity types (but not all) are also valid.
    /// Validates: Requirements 10.2, 10.3
    /// </summary>
    [Fact]
    public void Property_PartialExtension_MultipleEntityTypes()
    {
        var stringArrayGen = Gen.String[Gen.Char.AlphaNumeric, 3, 10].Array[5, 15];

        Gen.Select(stringArrayGen, stringArrayGen)
            .Sample((npcData, cityData) =>
            {
                // Create extension with only NPC and city data (other entity types empty)
                var extension = CreatePartialExtension(npcData: npcData, cityData: cityData);

                var registry = new ThemeRegistry();
                
                // Should not throw - partial extensions are valid
                var act = () => registry.RegisterExtension("cyberpunk", extension);
                act.Should().NotThrow();

                // Verify both extensions are applied
                var mergedTheme = registry.GetTheme(Theme.Cyberpunk);
                mergedTheme.NpcNames.Male.Prefixes.Should().Contain(npcData);
                mergedTheme.CityNames.Prefixes.Should().Contain(cityData);
            }, iter: 100);
    }

    /// <summary>
    /// Feature: custom-theme-registration, Property 25: Partial extension validation
    /// Verifies that extensions with empty arrays for some entity types are accepted.
    /// Validates: Requirements 10.2, 10.3
    /// </summary>
    [Fact]
    public void Property_PartialExtension_EmptyArraysAccepted()
    {
        var stringArrayGen = Gen.String[Gen.Char.AlphaNumeric, 3, 10].Array[5, 15];

        stringArrayGen.Sample(npcData =>
            {
                // Create extension with NPC data and explicitly empty arrays for other types
                var emptyArray = Array.Empty<string>();
                var extension = CreatePartialExtension(
                    npcData: npcData,
                    buildingData: emptyArray,
                    cityData: emptyArray,
                    districtData: emptyArray,
                    streetData: emptyArray,
                    factionData: emptyArray);

                var registry = new ThemeRegistry();
                
                // Should not throw - empty arrays are valid for extensions
                var act = () => registry.RegisterExtension("elves", extension);
                act.Should().NotThrow();

                // Verify the NPC extension is applied
                var mergedTheme = registry.GetTheme(Theme.Elves);
                mergedTheme.NpcNames.Male.Prefixes.Should().Contain(npcData);
            }, iter: 100);
    }

    /// <summary>
    /// Helper method to create partial extension data.
    /// </summary>
    private NameGeneratorEngine.ThemeData.ThemeData CreatePartialExtension(
        string[]? npcData = null,
        string[]? buildingData = null,
        string[]? cityData = null,
        string[]? districtData = null,
        string[]? streetData = null,
        string[]? factionData = null)
    {
        // Use empty arrays as defaults
        var emptyArray = Array.Empty<string>();

        // Create gender data for NPC names
        var genderData = new GenderNameData
        {
            Prefixes = npcData ?? emptyArray,
            Cores = npcData ?? emptyArray,
            Suffixes = npcData ?? emptyArray
        };

        // Create building type data
        var buildingTypeData = new Dictionary<BuildingType, BuildingTypeData>();
        foreach (var type in Enum.GetValues<BuildingType>())
        {
            buildingTypeData[type] = new BuildingTypeData
            {
                Prefixes = buildingData ?? emptyArray,
                Descriptors = buildingData ?? emptyArray,
                Suffixes = buildingData ?? emptyArray
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
                GenericPrefixes = buildingData ?? emptyArray,
                GenericSuffixes = buildingData ?? emptyArray,
                TypeData = buildingTypeData
            },
            CityNames = new CityNameData
            {
                Prefixes = cityData ?? emptyArray,
                Cores = cityData ?? emptyArray,
                Suffixes = cityData ?? emptyArray
            },
            DistrictNames = new DistrictNameData
            {
                Descriptors = districtData ?? emptyArray,
                LocationTypes = districtData ?? emptyArray
            },
            StreetNames = new StreetNameData
            {
                Prefixes = streetData ?? emptyArray,
                Cores = streetData ?? emptyArray,
                StreetSuffixes = streetData ?? emptyArray
            },
            FactionNames = new FactionNameData
            {
                Prefixes = factionData ?? emptyArray,
                Cores = factionData ?? emptyArray,
                Suffixes = factionData ?? emptyArray
            }
        };
    }
}
