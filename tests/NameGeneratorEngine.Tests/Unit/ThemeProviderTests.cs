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
        var provider = new NameGeneratorEngine.ThemeData.ThemeProvider();
        var themeData = CreateValidThemeData();

        // Act & Assert
        var act = () => provider.ValidateThemeData(themeData);
        act.Should().NotThrow();
    }

    [Fact]
    public void ValidateThemeData_WithNullNpcMalePrefixes_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var provider = new NameGeneratorEngine.ThemeData.ThemeProvider();
        var baseData = CreateValidThemeData();
        
        var invalidNpcNames = new NpcNameData
        {
            Male = new GenderNameData
            {
                Prefixes = null!,
                Cores = new[] { "core" },
                Suffixes = new[] { "suffix" }
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
        var provider = new NameGeneratorEngine.ThemeData.ThemeProvider();
        var baseData = CreateValidThemeData();
        
        var invalidNpcNames = new NpcNameData
        {
            Male = baseData.NpcNames.Male,
            Female = new GenderNameData
            {
                Prefixes = new[] { "prefix" },
                Cores = Array.Empty<string>(),
                Suffixes = new[] { "suffix" }
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
        var provider = new NameGeneratorEngine.ThemeData.ThemeProvider();
        var baseData = CreateValidThemeData();
        
        var invalidNpcNames = new NpcNameData
        {
            Male = baseData.NpcNames.Male,
            Female = baseData.NpcNames.Female,
            Neutral = new GenderNameData
            {
                Prefixes = new[] { "prefix" },
                Cores = new[] { "core" },
                Suffixes = new[] { "suffix", "   ", "another" }
            }
        };
        
        var themeData = CreateThemeDataWithNpcNames(invalidNpcNames);

        // Act
        var act = () => provider.ValidateThemeData(themeData);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Theme data validation failed*")
            .WithMessage("*NPC Neutral Suffixes contains null or whitespace entries*");
    }

    [Fact]
    public void ValidateThemeData_WithMissingBuildingType_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var provider = new NameGeneratorEngine.ThemeData.ThemeProvider();
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
        var provider = new NameGeneratorEngine.ThemeData.ThemeProvider();
        var baseData = CreateValidThemeData();
        
        var invalidBuildingNames = new BuildingNameData
        {
            TypeData = baseData.BuildingNames.TypeData,
            GenericPrefixes = Array.Empty<string>(),
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
        var provider = new NameGeneratorEngine.ThemeData.ThemeProvider();
        var baseData = CreateValidThemeData();
        
        var invalidCityNames = new CityNameData
        {
            Prefixes = Array.Empty<string>(),
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
        var provider = new NameGeneratorEngine.ThemeData.ThemeProvider();
        var baseData = CreateValidThemeData();
        
        var invalidDistrictNames = new DistrictNameData
        {
            Descriptors = Array.Empty<string>(),
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
        var provider = new NameGeneratorEngine.ThemeData.ThemeProvider();
        var baseData = CreateValidThemeData();
        
        var invalidStreetNames = new StreetNameData
        {
            Prefixes = baseData.StreetNames.Prefixes,
            Cores = baseData.StreetNames.Cores,
            StreetSuffixes = Array.Empty<string>()
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
        var provider = new NameGeneratorEngine.ThemeData.ThemeProvider();
        var baseData = CreateValidThemeData();
        
        // Create multiple validation errors
        var invalidNpcNames = new NpcNameData
        {
            Male = new GenderNameData
            {
                Prefixes = Array.Empty<string>(),
                Cores = new[] { "core" },
                Suffixes = new[] { "suffix" }
            },
            Female = baseData.NpcNames.Female,
            Neutral = baseData.NpcNames.Neutral
        };
        
        var invalidCityNames = new CityNameData
        {
            Prefixes = baseData.CityNames.Prefixes,
            Cores = Array.Empty<string>(),
            Suffixes = baseData.CityNames.Suffixes
        };
        
        var themeData = new NameGeneratorEngine.ThemeData.ThemeData
        {
            Theme = Theme.Cyberpunk,
            NpcNames = invalidNpcNames,
            BuildingNames = baseData.BuildingNames,
            CityNames = invalidCityNames,
            DistrictNames = baseData.DistrictNames,
            StreetNames = baseData.StreetNames
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
            Prefixes = new[] { "Pre" },
            Cores = new[] { "Core" },
            Suffixes = new[] { "Suf" }
        };

        var buildingTypeData = new BuildingTypeData
        {
            Prefixes = new[] { "Build" },
            Descriptors = new[] { "Desc" },
            Suffixes = new[] { "Suf" }
        };

        var buildingTypes = new Dictionary<BuildingType, BuildingTypeData>();
        foreach (BuildingType type in Enum.GetValues<BuildingType>())
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
                GenericPrefixes = new[] { "Generic" },
                GenericSuffixes = new[] { "Building" }
            },
            CityNames = new CityNameData
            {
                Prefixes = new[] { "City" },
                Cores = new[] { "Core" },
                Suffixes = new[] { "Ville" }
            },
            DistrictNames = new DistrictNameData
            {
                Descriptors = new[] { "Old" },
                LocationTypes = new[] { "District" }
            },
            StreetNames = new StreetNameData
            {
                Prefixes = new[] { "Main" },
                Cores = new[] { "Oak" },
                StreetSuffixes = new[] { "Street" }
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
            StreetNames = baseData.StreetNames
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
            StreetNames = baseData.StreetNames
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
            StreetNames = baseData.StreetNames
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
            StreetNames = baseData.StreetNames
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
            StreetNames = streetNames
        };
    }
}
