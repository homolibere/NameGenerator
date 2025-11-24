using FluentAssertions;
using NameGeneratorEngine.Enums;
using Xunit;

namespace NameGeneratorEngine.Tests.Unit;

/// <summary>
/// Unit tests for NameGenerator API surface.
/// Tests constructor behavior, seed property, method signatures, and basic functionality.
/// </summary>
public class NameGeneratorTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_WithSeed_ShouldUseThatSeed()
    {
        // Arrange
        var expectedSeed = 12345;

        // Act
        var generator = new NameGenerator(expectedSeed);

        // Assert
        generator.Seed.Should().Be(expectedSeed);
    }

    [Fact]
    public void Constructor_WithoutSeed_ShouldGenerateRandomSeed()
    {
        // Act
        var generator1 = new NameGenerator();
        var generator2 = new NameGenerator();

        // Assert
        generator1.Seed.Should().NotBe(0);
        generator2.Seed.Should().NotBe(0);
        // Note: There's a tiny chance these could be equal, but it's extremely unlikely
    }

    [Fact]
    public void Constructor_WithNullSeed_ShouldGenerateRandomSeed()
    {
        // Act
        var generator = new NameGenerator(null);

        // Assert
        generator.Seed.Should().NotBe(0);
    }

    #endregion

    #region Seed Property Tests

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(42)]
    [InlineData(999999)]
    [InlineData(int.MaxValue)]
    public void Seed_ShouldReturnConstructorSeedValue(int seed)
    {
        // Arrange
        var generator = new NameGenerator(seed);

        // Act
        var actualSeed = generator.Seed;

        // Assert
        actualSeed.Should().Be(seed);
    }

    #endregion

    #region Generation Method Existence Tests

    [Fact]
    public void GenerateNpcName_WithThemeAndGender_ShouldReturnString()
    {
        // Arrange
        var generator = new NameGenerator(42);

        // Act
        var name = generator.GenerateNpcName(Theme.Cyberpunk, Gender.Male);

        // Assert
        name.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void GenerateNpcName_WithThemeOnly_ShouldReturnString()
    {
        // Arrange
        var generator = new NameGenerator(42);

        // Act
        var name = generator.GenerateNpcName(Theme.Elves);

        // Assert
        name.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void GenerateBuildingName_WithThemeAndType_ShouldReturnString()
    {
        // Arrange
        var generator = new NameGenerator(42);

        // Act
        var name = generator.GenerateBuildingName(Theme.Orcs, BuildingType.Commercial);

        // Assert
        name.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void GenerateBuildingName_WithThemeOnly_ShouldReturnString()
    {
        // Arrange
        var generator = new NameGenerator(42);

        // Act
        var name = generator.GenerateBuildingName(Theme.Cyberpunk);

        // Assert
        name.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void GenerateCityName_WithTheme_ShouldReturnString()
    {
        // Arrange
        var generator = new NameGenerator(42);

        // Act
        var name = generator.GenerateCityName(Theme.Elves);

        // Assert
        name.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void GenerateDistrictName_WithTheme_ShouldReturnString()
    {
        // Arrange
        var generator = new NameGenerator(42);

        // Act
        var name = generator.GenerateDistrictName(Theme.Orcs);

        // Assert
        name.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void GenerateStreetName_WithTheme_ShouldReturnString()
    {
        // Arrange
        var generator = new NameGenerator(42);

        // Act
        var name = generator.GenerateStreetName(Theme.Cyberpunk);

        // Assert
        name.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void GenerateFactionName_WithTheme_ShouldReturnString()
    {
        // Arrange
        var generator = new NameGenerator(42);

        // Act
        var name = generator.GenerateFactionName(Theme.Cyberpunk);

        // Assert
        name.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void ResetSession_ShouldExist()
    {
        // Arrange
        var generator = new NameGenerator(42);

        // Act & Assert
        var act = () => generator.ResetSession();
        act.Should().NotThrow();
    }

    #endregion

    #region Theme Compatibility Tests

    [Theory]
    [InlineData(Theme.Cyberpunk)]
    [InlineData(Theme.Elves)]
    [InlineData(Theme.Orcs)]
    public void GenerateNpcName_WithAllThemes_ShouldReturnString(Theme theme)
    {
        // Arrange
        var generator = new NameGenerator(42);

        // Act
        var name = generator.GenerateNpcName(theme);

        // Assert
        name.Should().NotBeNullOrWhiteSpace();
    }

    [Theory]
    [InlineData(Theme.Cyberpunk)]
    [InlineData(Theme.Elves)]
    [InlineData(Theme.Orcs)]
    public void GenerateBuildingName_WithAllThemes_ShouldReturnString(Theme theme)
    {
        // Arrange
        var generator = new NameGenerator(42);

        // Act
        var name = generator.GenerateBuildingName(theme);

        // Assert
        name.Should().NotBeNullOrWhiteSpace();
    }

    [Theory]
    [InlineData(Theme.Cyberpunk)]
    [InlineData(Theme.Elves)]
    [InlineData(Theme.Orcs)]
    public void GenerateCityName_WithAllThemes_ShouldReturnString(Theme theme)
    {
        // Arrange
        var generator = new NameGenerator(42);

        // Act
        var name = generator.GenerateCityName(theme);

        // Assert
        name.Should().NotBeNullOrWhiteSpace();
    }

    [Theory]
    [InlineData(Theme.Cyberpunk)]
    [InlineData(Theme.Elves)]
    [InlineData(Theme.Orcs)]
    public void GenerateDistrictName_WithAllThemes_ShouldReturnString(Theme theme)
    {
        // Arrange
        var generator = new NameGenerator(42);

        // Act
        var name = generator.GenerateDistrictName(theme);

        // Assert
        name.Should().NotBeNullOrWhiteSpace();
    }

    [Theory]
    [InlineData(Theme.Cyberpunk)]
    [InlineData(Theme.Elves)]
    [InlineData(Theme.Orcs)]
    public void GenerateStreetName_WithAllThemes_ShouldReturnString(Theme theme)
    {
        // Arrange
        var generator = new NameGenerator(42);

        // Act
        var name = generator.GenerateStreetName(theme);

        // Assert
        name.Should().NotBeNullOrWhiteSpace();
    }

    [Theory]
    [InlineData(Theme.Cyberpunk)]
    [InlineData(Theme.Elves)]
    [InlineData(Theme.Orcs)]
    public void GenerateFactionName_WithAllThemes_ShouldReturnString(Theme theme)
    {
        // Arrange
        var generator = new NameGenerator(42);

        // Act
        var name = generator.GenerateFactionName(theme);

        // Assert
        name.Should().NotBeNullOrWhiteSpace();
    }

    #endregion

    #region Gender Parameter Tests

    [Theory]
    [InlineData(Gender.Male)]
    [InlineData(Gender.Female)]
    [InlineData(Gender.Neutral)]
    public void GenerateNpcName_WithAllGenders_ShouldReturnString(Gender gender)
    {
        // Arrange
        var generator = new NameGenerator(42);

        // Act
        var name = generator.GenerateNpcName(Theme.Cyberpunk, gender);

        // Assert
        name.Should().NotBeNullOrWhiteSpace();
    }

    [Theory]
    [InlineData(Theme.Cyberpunk, Gender.Male)]
    [InlineData(Theme.Cyberpunk, Gender.Female)]
    [InlineData(Theme.Cyberpunk, Gender.Neutral)]
    [InlineData(Theme.Elves, Gender.Male)]
    [InlineData(Theme.Elves, Gender.Female)]
    [InlineData(Theme.Elves, Gender.Neutral)]
    [InlineData(Theme.Orcs, Gender.Male)]
    [InlineData(Theme.Orcs, Gender.Female)]
    [InlineData(Theme.Orcs, Gender.Neutral)]
    public void GenerateNpcName_WithAllThemeAndGenderCombinations_ShouldReturnString(Theme theme, Gender gender)
    {
        // Arrange
        var generator = new NameGenerator(42);

        // Act
        var name = generator.GenerateNpcName(theme, gender);

        // Assert
        name.Should().NotBeNullOrWhiteSpace();
    }

    #endregion

    #region Building Type Parameter Tests

    [Theory]
    [InlineData(BuildingType.Residential)]
    [InlineData(BuildingType.Commercial)]
    [InlineData(BuildingType.Industrial)]
    [InlineData(BuildingType.Government)]
    [InlineData(BuildingType.Entertainment)]
    [InlineData(BuildingType.Medical)]
    [InlineData(BuildingType.Educational)]
    public void GenerateBuildingName_WithAllBuildingTypes_ShouldReturnString(BuildingType buildingType)
    {
        // Arrange
        var generator = new NameGenerator(42);

        // Act
        var name = generator.GenerateBuildingName(Theme.Cyberpunk, buildingType);

        // Assert
        name.Should().NotBeNullOrWhiteSpace();
    }

    [Theory]
    [InlineData(Theme.Cyberpunk, BuildingType.Residential)]
    [InlineData(Theme.Cyberpunk, BuildingType.Commercial)]
    [InlineData(Theme.Cyberpunk, BuildingType.Industrial)]
    [InlineData(Theme.Cyberpunk, BuildingType.Government)]
    [InlineData(Theme.Cyberpunk, BuildingType.Entertainment)]
    [InlineData(Theme.Cyberpunk, BuildingType.Medical)]
    [InlineData(Theme.Cyberpunk, BuildingType.Educational)]
    [InlineData(Theme.Elves, BuildingType.Residential)]
    [InlineData(Theme.Elves, BuildingType.Commercial)]
    [InlineData(Theme.Elves, BuildingType.Industrial)]
    [InlineData(Theme.Elves, BuildingType.Government)]
    [InlineData(Theme.Elves, BuildingType.Entertainment)]
    [InlineData(Theme.Elves, BuildingType.Medical)]
    [InlineData(Theme.Elves, BuildingType.Educational)]
    [InlineData(Theme.Orcs, BuildingType.Residential)]
    [InlineData(Theme.Orcs, BuildingType.Commercial)]
    [InlineData(Theme.Orcs, BuildingType.Industrial)]
    [InlineData(Theme.Orcs, BuildingType.Government)]
    [InlineData(Theme.Orcs, BuildingType.Entertainment)]
    [InlineData(Theme.Orcs, BuildingType.Medical)]
    [InlineData(Theme.Orcs, BuildingType.Educational)]
    public void GenerateBuildingName_WithAllThemeAndTypeCombinations_ShouldReturnString(Theme theme, BuildingType buildingType)
    {
        // Arrange
        var generator = new NameGenerator(42);

        // Act
        var name = generator.GenerateBuildingName(theme, buildingType);

        // Assert
        name.Should().NotBeNullOrWhiteSpace();
    }

    #endregion

    #region Faction Name Tests

    [Fact]
    public void GenerateFactionName_ShouldGenerateUniqueNamesWithinSession()
    {
        // Arrange
        var generator = new NameGenerator(42);
        var generatedNames = new HashSet<string>();
        const int nameCount = 50;

        // Act
        for (var i = 0; i < nameCount; i++)
        {
            var name = generator.GenerateFactionName(Theme.Cyberpunk);
            generatedNames.Add(name);
        }

        // Assert
        generatedNames.Should().HaveCount(nameCount, "all faction names should be unique within a session");
    }

    [Fact]
    public void GenerateFactionName_WithSameSeed_ShouldProduceDeterministicResults()
    {
        // Arrange
        const int seed = 12345;
        var generator1 = new NameGenerator(seed);
        var generator2 = new NameGenerator(seed);

        // Act
        var name1 = generator1.GenerateFactionName(Theme.Elves);
        var name2 = generator2.GenerateFactionName(Theme.Elves);

        // Assert
        name1.Should().Be(name2, "same seed should produce identical faction names");
    }

    [Fact]
    public void GenerateFactionName_WithSameSeedMultipleCalls_ShouldProduceDeterministicSequence()
    {
        // Arrange
        const int seed = 99999;
        var generator1 = new NameGenerator(seed);
        var generator2 = new NameGenerator(seed);
        var names1 = new List<string>();
        var names2 = new List<string>();

        // Act
        for (var i = 0; i < 10; i++)
        {
            names1.Add(generator1.GenerateFactionName(Theme.Orcs));
            names2.Add(generator2.GenerateFactionName(Theme.Orcs));
        }

        // Assert
        names1.Should().Equal(names2, "same seed should produce identical faction name sequences");
    }

    #endregion
}
