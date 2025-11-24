using FluentAssertions;
using NameGeneratorEngine.Enums;
using NameGeneratorEngine.ThemeData.DataStructures;
using Xunit;

namespace NameGeneratorEngine.Tests.Unit;

/// <summary>
/// Unit tests for ThemeProvider class, focusing on validation logic.
/// </summary>
public class ThemeProviderTests
{
    [Fact]
    public void ValidateThemeData_WithValidData_ShouldNotThrow()
    {
        // Arrange
        var provider = new ThemeData.ThemeProvider();
        var themeData = CreateValidThemeData();

        // Act & Assert
        var act = () => provider.ValidateThemeData(themeData);
        act.Should().NotThrow();
    }

    [Fact]
    public void ValidateThemeData_WithNullNpcMalePrefixes_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var provider = new ThemeData.ThemeProvider();
        var baseData = CreateValidThemeData();
        
        var invalidNpcNames = new NpcNameData
        {
            Male = new GenderNameData
            {
                Prefixes = null!,
                Cores = ["core"],
                Suffixes = ["suffix"]
            },
            Female = baseData.NpcNames.Female,
            Neutral = baseData.NpcNames.Neutral
        };
        
        var themeData = CreateThemeDataWithNpcNames(invalidNpcNames);

        // Act
        var act = () => provider.ValidateThemeData(themeData);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Theme data validation failed*")
            .WithMessage("*NPC Male Prefixes is null or empty*");
    }

    [Fact]
    public void ValidateThemeData_WithEmptyNpcFemaleCores_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var provider = new ThemeData.ThemeProvider();
        var baseData = CreateValidThemeData();
        
        var invalidNpcNames = new NpcNameData
        {
            Male = baseData.NpcNames.Male,
            Female = new GenderNameData
            {
                Prefixes = ["prefix"],
                Cores = [
                ],
                Suffixes = ["suffix"]
            },
            Neutral = baseData.NpcNames.Neutral
        };
        
        var themeData = CreateThemeDataWithNpcNames(invalidNpcNames);

        // Act
        var act = () => provider.ValidateThemeData(themeData);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Theme data validation failed*")
            .WithMessage("*NPC Female Cores is null or empty*");
    }

    [Fact]
    public void ValidateThemeData_WithWhitespaceInNpcNeutralSuffixes_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var provider = new ThemeData.ThemeProvider();
        var baseData = CreateValidThemeData();
        
        var invalidNpcNames = new NpcNameData
        {
            Male = baseData.NpcNames.Male,
            Female = baseData.NpcNames.Female,
            Neutral = new GenderNameData
            {
                Prefixes = ["prefix"],
                Cores = ["core"],
                Suffixes = ["suffix", "   ", "another"]
            }
        };
        
        var themeData = CreateThemeDataWithNpcNames(invalidNpcNames);

        // Act
        var act = () => provider.ValidateThemeData(themeData);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Theme data validation failed*")
            .WithMessage("*NPC Neutral Suffixes contains invalid entries*");
    }

    [Fact]
    public void ValidateThemeData_WithMissingBuildingType_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var provider = new ThemeData.ThemeProvider();
        var baseData = CreateValidThemeData();
        
        // Remove one building type
        var incompleteTypeData = new Dictionary<BuildingType, BuildingTypeData>(baseData.BuildingNames.TypeData);
        incompleteTypeData.Remove(BuildingType.Residential);
        
        var invalidBuildingNames = new BuildingNameData
        {
            TypeData = incompleteTypeData,
            GenericPrefixes = baseData.BuildingNames.GenericPrefixes,
            GenericSuffixes = baseData.BuildingNames.GenericSuffixes
        };
        
        var themeData = CreateThemeDataWithBuildingNames(invalidBuildingNames);

        // Act
        var act = () => provider.ValidateThemeData(themeData);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Theme data validation failed*")
            .WithMessage("*Building type 'Residential' is missing from theme data*");
    }

    [Fact]
    public void ValidateThemeData_WithEmptyBuildingGenericPrefixes_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var provider = new ThemeData.ThemeProvider();
        var baseData = CreateValidThemeData();
        
        var invalidBuildingNames = new BuildingNameData
        {
            TypeData = baseData.BuildingNames.TypeData,
            GenericPrefixes = [
            ],
            GenericSuffixes = baseData.BuildingNames.GenericSuffixes
        };
        
        var themeData = CreateThemeDataWithBuildingNames(invalidBuildingNames);

        // Act
        var act = () => provider.ValidateThemeData(themeData);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Theme data validation failed*")
            .WithMessage("*Building Generic Prefixes is null or empty*");
    }

    [Fact]
    public void ValidateThemeData_WithEmptyCityPrefixes_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var provider = new ThemeData.ThemeProvider();
        var baseData = CreateValidThemeData();
        
        var invalidCityNames = new CityNameData
        {
            Prefixes = [
            ],
            Cores = baseData.CityNames.Cores,
            Suffixes = baseData.CityNames.Suffixes
        };
        
        var themeData = CreateThemeDataWithCityNames(invalidCityNames);

        // Act
        var act = () => provider.ValidateThemeData(themeData);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Theme data validation failed*")
            .WithMessage("*City Prefixes is null or empty*");
    }

    [Fact]
    public void ValidateThemeData_WithEmptyDistrictDescriptors_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var provider = new ThemeData.ThemeProvider();
        var baseData = CreateValidThemeData();
        
        var invalidDistrictNames = new DistrictNameData
        {
            Descriptors = [
            ],
            LocationTypes = baseData.DistrictNames.LocationTypes
        };
        
        var themeData = CreateThemeDataWithDistrictNames(invalidDistrictNames);

        // Act
        var act = () => provider.ValidateThemeData(themeData);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Theme data validation failed*")
            .WithMessage("*District Descriptors is null or empty*");
    }

    [Fact]
    public void ValidateThemeData_WithEmptyStreetSuffixes_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var provider = new ThemeData.ThemeProvider();
        var baseData = CreateValidThemeData();
        
        var invalidStreetNames = new StreetNameData
        {
            Prefixes = baseData.StreetNames.Prefixes,
            Cores = baseData.StreetNames.Cores,
            StreetSuffixes = [
            ]
        };
        
        var themeData = CreateThemeDataWithStreetNames(invalidStreetNames);

        // Act
        var act = () => provider.ValidateThemeData(themeData);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Theme data validation failed*")
            .WithMessage("*Street Suffixes is null or empty*");
    }

    [Fact]
    public void ValidateThemeData_WithMultipleErrors_ShouldIncludeAllErrorsInMessage()
    {
        // Arrange
        var provider = new ThemeData.ThemeProvider();
        var baseData = CreateValidThemeData();
        
        // Create multiple validation errors
        var invalidNpcNames = new NpcNameData
        {
            Male = new GenderNameData
            {
                Prefixes = [
                ],
                Cores = ["core"],
                Suffixes = ["suffix"]
            },
            Female = baseData.NpcNames.Female,
            Neutral = baseData.NpcNames.Neutral
        };
        
        var invalidCityNames = new CityNameData
        {
            Prefixes = baseData.CityNames.Prefixes,
            Cores = [
            ],
            Suffixes = baseData.CityNames.Suffixes
        };
        
        var themeData = new NameGeneratorEngine.ThemeData.ThemeData
        {
            Theme = Theme.Cyberpunk,
            NpcNames = invalidNpcNames,
            BuildingNames = baseData.BuildingNames,
            CityNames = invalidCityNames,
            DistrictNames = baseData.DistrictNames,
            StreetNames = baseData.StreetNames,
            FactionNames = baseData.FactionNames
        };

        // Act
        var act = () => provider.ValidateThemeData(themeData);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Theme data validation failed*")
            .WithMessage("*NPC Male Prefixes is null or empty*")
            .WithMessage("*City Cores is null or empty*");
    }

    private static NameGeneratorEngine.ThemeData.ThemeData CreateValidThemeData()
    {
        var genderData = new GenderNameData
        {
            Prefixes = ["Pre"],
            Cores = ["Core"],
            Suffixes = ["Suf"]
        };

        var buildingTypeData = new BuildingTypeData
        {
            Prefixes = ["Build"],
            Descriptors = ["Desc"],
            Suffixes = ["Suf"]
        };

        var buildingTypes = new Dictionary<BuildingType, BuildingTypeData>();
        foreach (var type in Enum.GetValues<BuildingType>())
        {
            buildingTypes[type] = buildingTypeData;
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
                TypeData = buildingTypes,
                GenericPrefixes = ["Generic"],
                GenericSuffixes = ["Building"]
            },
            CityNames = new CityNameData
            {
                Prefixes = ["City"],
                Cores = ["Core"],
                Suffixes = ["Ville"]
            },
            DistrictNames = new DistrictNameData
            {
                Descriptors = ["Old"],
                LocationTypes = ["District"]
            },
            StreetNames = new StreetNameData
            {
                Prefixes = ["Main"],
                Cores = ["Oak"],
                StreetSuffixes = ["Street"]
            },
            FactionNames = new FactionNameData
            {
                Prefixes = ["Mega"],
                Cores = ["Corp"],
                Suffixes = [""]
            }
        };
    }

    private static NameGeneratorEngine.ThemeData.ThemeData CreateThemeDataWithNpcNames(NpcNameData npcNames)
    {
        var baseData = CreateValidThemeData();
        return new NameGeneratorEngine.ThemeData.ThemeData
        {
            Theme = baseData.Theme,
            NpcNames = npcNames,
            BuildingNames = baseData.BuildingNames,
            CityNames = baseData.CityNames,
            DistrictNames = baseData.DistrictNames,
            StreetNames = baseData.StreetNames,
            FactionNames = baseData.FactionNames
        };
    }

    private static NameGeneratorEngine.ThemeData.ThemeData CreateThemeDataWithBuildingNames(BuildingNameData buildingNames)
    {
        var baseData = CreateValidThemeData();
        return new NameGeneratorEngine.ThemeData.ThemeData
        {
            Theme = baseData.Theme,
            NpcNames = baseData.NpcNames,
            BuildingNames = buildingNames,
            CityNames = baseData.CityNames,
            DistrictNames = baseData.DistrictNames,
            StreetNames = baseData.StreetNames,
            FactionNames = baseData.FactionNames
        };
    }

    private static NameGeneratorEngine.ThemeData.ThemeData CreateThemeDataWithCityNames(CityNameData cityNames)
    {
        var baseData = CreateValidThemeData();
        return new NameGeneratorEngine.ThemeData.ThemeData
        {
            Theme = baseData.Theme,
            NpcNames = baseData.NpcNames,
            BuildingNames = baseData.BuildingNames,
            CityNames = cityNames,
            DistrictNames = baseData.DistrictNames,
            StreetNames = baseData.StreetNames,
            FactionNames = baseData.FactionNames
        };
    }

    private static NameGeneratorEngine.ThemeData.ThemeData CreateThemeDataWithDistrictNames(DistrictNameData districtNames)
    {
        var baseData = CreateValidThemeData();
        return new NameGeneratorEngine.ThemeData.ThemeData
        {
            Theme = baseData.Theme,
            NpcNames = baseData.NpcNames,
            BuildingNames = baseData.BuildingNames,
            CityNames = baseData.CityNames,
            DistrictNames = districtNames,
            StreetNames = baseData.StreetNames,
            FactionNames = baseData.FactionNames
        };
    }

    private static NameGeneratorEngine.ThemeData.ThemeData CreateThemeDataWithStreetNames(StreetNameData streetNames)
    {
        var baseData = CreateValidThemeData();
        return new NameGeneratorEngine.ThemeData.ThemeData
        {
            Theme = baseData.Theme,
            NpcNames = baseData.NpcNames,
            BuildingNames = baseData.BuildingNames,
            CityNames = baseData.CityNames,
            DistrictNames = baseData.DistrictNames,
            StreetNames = streetNames,
            FactionNames = baseData.FactionNames
        };
    }
}
