using FluentAssertions;
using NameGeneratorEngine.Enums;
using Xunit;

namespace NameGeneratorEngine.Tests.Unit;

/// <summary>
/// Unit tests for custom theme registration public API.
/// Tests ThemeDataBuilder, ThemeConfig, and NameGenerator custom theme methods.
/// Requirements: 1.1, 2.1, 6.1, 6.2, 6.3, 7.2, 7.5, 8.1
/// </summary>
public class CustomThemeApiTests
{
    #region ThemeDataBuilder Fluent Interface Tests

    [Fact]
    public void ThemeDataBuilder_Constructor_ShouldReturnBuilderInstance()
    {
        // Act
        var builder = new ThemeDataBuilder();

        // Assert
        builder.Should().NotBeNull();
    }

    [Fact]
    public void ThemeDataBuilder_WithNpcNames_ShouldReturnSameBuilderInstance()
    {
        // Arrange
        var builder = new ThemeDataBuilder();

        // Act
        var result = builder.WithNpcNames(npc => npc
            .WithMaleNames(new[] { "Pre" }, new[] { "Core" }, new[] { "Suf" })
            .WithFemaleNames(new[] { "Pre" }, new[] { "Core" }, new[] { "Suf" })
            .WithNeutralNames(new[] { "Pre" }, new[] { "Core" }, new[] { "Suf" }));

        // Assert
        result.Should().BeSameAs(builder, "fluent interface should return same builder instance");
    }

    [Fact]
    public void ThemeDataBuilder_WithBuildingNames_ShouldReturnSameBuilderInstance()
    {
        // Arrange
        var builder = new ThemeDataBuilder();

        // Act
        var result = builder.WithBuildingNames(building => building
            .WithGenericNames(new[] { "Pre" }, new[] { "Suf" })
            .WithTypeNames(BuildingType.Residential, new[] { "Pre" }, new[] { "Desc" }, new[] { "Suf" })
            .WithTypeNames(BuildingType.Commercial, new[] { "Pre" }, new[] { "Desc" }, new[] { "Suf" })
            .WithTypeNames(BuildingType.Industrial, new[] { "Pre" }, new[] { "Desc" }, new[] { "Suf" })
            .WithTypeNames(BuildingType.Government, new[] { "Pre" }, new[] { "Desc" }, new[] { "Suf" })
            .WithTypeNames(BuildingType.Entertainment, new[] { "Pre" }, new[] { "Desc" }, new[] { "Suf" })
            .WithTypeNames(BuildingType.Medical, new[] { "Pre" }, new[] { "Desc" }, new[] { "Suf" })
            .WithTypeNames(BuildingType.Educational, new[] { "Pre" }, new[] { "Desc" }, new[] { "Suf" }));

        // Assert
        result.Should().BeSameAs(builder, "fluent interface should return same builder instance");
    }

    [Fact]
    public void ThemeDataBuilder_WithCityNames_ShouldReturnSameBuilderInstance()
    {
        // Arrange
        var builder = new ThemeDataBuilder();

        // Act
        var result = builder.WithCityNames(new[] { "Pre" }, new[] { "Core" }, new[] { "Suf" });

        // Assert
        result.Should().BeSameAs(builder, "fluent interface should return same builder instance");
    }

    [Fact]
    public void ThemeDataBuilder_WithDistrictNames_ShouldReturnSameBuilderInstance()
    {
        // Arrange
        var builder = new ThemeDataBuilder();

        // Act
        var result = builder.WithDistrictNames(new[] { "Desc" }, new[] { "Type" });

        // Assert
        result.Should().BeSameAs(builder, "fluent interface should return same builder instance");
    }

    [Fact]
    public void ThemeDataBuilder_WithStreetNames_ShouldReturnSameBuilderInstance()
    {
        // Arrange
        var builder = new ThemeDataBuilder();

        // Act
        var result = builder.WithStreetNames(new[] { "Pre" }, new[] { "Core" }, new[] { "Street" });

        // Assert
        result.Should().BeSameAs(builder, "fluent interface should return same builder instance");
    }

    [Fact]
    public void ThemeDataBuilder_WithFactionNames_ShouldReturnSameBuilderInstance()
    {
        // Arrange
        var builder = new ThemeDataBuilder();

        // Act
        var result = builder.WithFactionNames(new[] { "Pre" }, new[] { "Core" }, new[] { "Suf" });

        // Assert
        result.Should().BeSameAs(builder, "fluent interface should return same builder instance");
    }

    [Fact]
    public void ThemeDataBuilder_MethodChaining_ShouldWorkCorrectly()
    {
        // Arrange & Act
        var act = () => new ThemeDataBuilder()
            .WithNpcNames(npc => npc
                .WithMaleNames(new[] { "Pre" }, new[] { "Core" }, new[] { "Suf" })
                .WithFemaleNames(new[] { "Pre" }, new[] { "Core" }, new[] { "Suf" })
                .WithNeutralNames(new[] { "Pre" }, new[] { "Core" }, new[] { "Suf" }))
            .WithBuildingNames(building => building
                .WithGenericNames(new[] { "Pre" }, new[] { "Suf" })
                .WithTypeNames(BuildingType.Residential, new[] { "Pre" }, new[] { "Desc" }, new[] { "Suf" })
                .WithTypeNames(BuildingType.Commercial, new[] { "Pre" }, new[] { "Desc" }, new[] { "Suf" })
                .WithTypeNames(BuildingType.Industrial, new[] { "Pre" }, new[] { "Desc" }, new[] { "Suf" })
                .WithTypeNames(BuildingType.Government, new[] { "Pre" }, new[] { "Desc" }, new[] { "Suf" })
                .WithTypeNames(BuildingType.Entertainment, new[] { "Pre" }, new[] { "Desc" }, new[] { "Suf" })
                .WithTypeNames(BuildingType.Medical, new[] { "Pre" }, new[] { "Desc" }, new[] { "Suf" })
                .WithTypeNames(BuildingType.Educational, new[] { "Pre" }, new[] { "Desc" }, new[] { "Suf" }))
            .WithCityNames(new[] { "Pre" }, new[] { "Core" }, new[] { "Suf" })
            .WithDistrictNames(new[] { "Desc" }, new[] { "Type" })
            .WithStreetNames(new[] { "Pre" }, new[] { "Core" }, new[] { "Street" })
            .WithFactionNames(new[] { "Pre" }, new[] { "Core" }, new[] { "Suf" })
            .Build();

        // Assert
        act.Should().NotThrow("method chaining should work correctly");
    }

    [Fact]
    public void ThemeDataBuilder_Extend_WithBuiltInTheme_ShouldReturnBuilderInstance()
    {
        // Act
        var builder = ThemeDataBuilder.Extend(Theme.Cyberpunk);

        // Assert
        builder.Should().NotBeNull();
    }

    [Fact]
    public void ThemeDataBuilder_Extend_WithStringIdentifier_ShouldReturnBuilderInstance()
    {
        // Act
        var builder = ThemeDataBuilder.Extend("custom-theme");

        // Assert
        builder.Should().NotBeNull();
    }

    #endregion

    #region ThemeConfig Construction Tests

    [Fact]
    public void ThemeConfig_Constructor_ShouldInitializeEmptyCollections()
    {
        // Act
        var config = new ThemeConfig();

        // Assert
        config.CustomThemes.Should().NotBeNull().And.BeEmpty();
        config.ThemeExtensions.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void ThemeConfig_AddTheme_ShouldReturnSameConfigInstance()
    {
        // Arrange
        var config = new ThemeConfig();
        var themeData = CreateValidCustomTheme();

        // Act
        var result = config.AddTheme("test-theme", themeData);

        // Assert
        result.Should().BeSameAs(config, "fluent interface should return same config instance");
    }

    [Fact]
    public void ThemeConfig_AddTheme_ShouldAddThemeToCollection()
    {
        // Arrange
        var config = new ThemeConfig();
        var themeData = CreateValidCustomTheme();

        // Act
        config.AddTheme("test-theme", themeData);

        // Assert
        config.CustomThemes.Should().ContainKey("test-theme");
        config.CustomThemes["test-theme"].Should().BeSameAs(themeData);
    }

    [Fact]
    public void ThemeConfig_ExtendTheme_WithBuiltInTheme_ShouldReturnSameConfigInstance()
    {
        // Arrange
        var config = new ThemeConfig();
        var extension = CreateValidExtension();

        // Act
        var result = config.ExtendTheme(Theme.Cyberpunk, extension);

        // Assert
        result.Should().BeSameAs(config, "fluent interface should return same config instance");
    }

    [Fact]
    public void ThemeConfig_ExtendTheme_WithStringIdentifier_ShouldReturnSameConfigInstance()
    {
        // Arrange
        var config = new ThemeConfig();
        var extension = CreateValidExtension();

        // Act
        var result = config.ExtendTheme("custom-theme", extension);

        // Assert
        result.Should().BeSameAs(config, "fluent interface should return same config instance");
    }

    [Fact]
    public void ThemeConfig_ExtendTheme_ShouldAddExtensionToCollection()
    {
        // Arrange
        var config = new ThemeConfig();
        var extension = CreateValidExtension();

        // Act
        config.ExtendTheme(Theme.Cyberpunk, extension);

        // Assert
        config.ThemeExtensions.Should().HaveCount(1);
        config.ThemeExtensions[0].Should().BeSameAs(extension);
    }

    [Fact]
    public void ThemeConfig_MethodChaining_ShouldWorkCorrectly()
    {
        // Arrange
        var theme1 = CreateValidCustomTheme();
        var theme2 = CreateValidCustomTheme();
        var extension1 = CreateValidExtension();
        var extension2 = CreateValidExtension();

        // Act
        var config = new ThemeConfig()
            .AddTheme("theme1", theme1)
            .AddTheme("theme2", theme2)
            .ExtendTheme(Theme.Cyberpunk, extension1)
            .ExtendTheme("theme1", extension2);

        // Assert
        config.CustomThemes.Should().HaveCount(2);
        config.ThemeExtensions.Should().HaveCount(2);
    }

    #endregion

    #region NameGenerator Constructor Tests

    [Fact]
    public void NameGenerator_Constructor_WithNullConfig_ShouldNotThrow()
    {
        // Act
        var act = () => new NameGenerator(null, seed: 42);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void NameGenerator_Constructor_WithEmptyConfig_ShouldNotThrow()
    {
        // Arrange
        var config = new ThemeConfig();

        // Act
        var act = () => new NameGenerator(config, seed: 42);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void NameGenerator_Constructor_WithValidConfig_ShouldNotThrow()
    {
        // Arrange
        var config = new ThemeConfig()
            .AddTheme("test-theme", CreateValidCustomTheme());

        // Act
        var act = () => new NameGenerator(config, seed: 42);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void NameGenerator_Constructor_WithInvalidThemeData_ShouldThrowArgumentException()
    {
        // Arrange & Act - Creating invalid theme should throw during Build()
        var act = () => CreateInvalidCustomTheme();

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Missing required building name data*");
    }

    #endregion

    #region String Identifier Generation Methods Tests

    [Fact]
    public void GenerateNpcName_WithStringIdentifier_ShouldReturnString()
    {
        // Arrange
        var config = new ThemeConfig()
            .AddTheme("test-theme", CreateValidCustomTheme());
        var generator = new NameGenerator(config, seed: 42);

        // Act
        var name = generator.GenerateNpcName("test-theme");

        // Assert
        name.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void GenerateBuildingName_WithStringIdentifier_ShouldReturnString()
    {
        // Arrange
        var config = new ThemeConfig()
            .AddTheme("test-theme", CreateValidCustomTheme());
        var generator = new NameGenerator(config, seed: 42);

        // Act
        var name = generator.GenerateBuildingName("test-theme");

        // Assert
        name.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void GenerateCityName_WithStringIdentifier_ShouldReturnString()
    {
        // Arrange
        var config = new ThemeConfig()
            .AddTheme("test-theme", CreateValidCustomTheme());
        var generator = new NameGenerator(config, seed: 42);

        // Act
        var name = generator.GenerateCityName("test-theme");

        // Assert
        name.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void GenerateDistrictName_WithStringIdentifier_ShouldReturnString()
    {
        // Arrange
        var config = new ThemeConfig()
            .AddTheme("test-theme", CreateValidCustomTheme());
        var generator = new NameGenerator(config, seed: 42);

        // Act
        var name = generator.GenerateDistrictName("test-theme");

        // Assert
        name.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void GenerateStreetName_WithStringIdentifier_ShouldReturnString()
    {
        // Arrange
        var config = new ThemeConfig()
            .AddTheme("test-theme", CreateValidCustomTheme());
        var generator = new NameGenerator(config, seed: 42);

        // Act
        var name = generator.GenerateStreetName("test-theme");

        // Assert
        name.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void GenerateFactionName_WithStringIdentifier_ShouldReturnString()
    {
        // Arrange
        var config = new ThemeConfig()
            .AddTheme("test-theme", CreateValidCustomTheme());
        var generator = new NameGenerator(config, seed: 42);

        // Act
        var name = generator.GenerateFactionName("test-theme");

        // Assert
        name.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void GenerateNpcName_WithBuiltInThemeString_ShouldReturnString()
    {
        // Arrange
        var generator = new NameGenerator(seed: 42);

        // Act
        var name = generator.GenerateNpcName("cyberpunk");

        // Assert
        name.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void GenerateNpcName_WithCaseInsensitiveIdentifier_ShouldReturnString()
    {
        // Arrange
        var config = new ThemeConfig()
            .AddTheme("test-theme", CreateValidCustomTheme());
        var generator = new NameGenerator(config, seed: 42);

        // Act - Generate one name with each case variation
        var name1 = generator.GenerateNpcName("test-theme");

        // Reset to avoid pool exhaustion with limited test data
        generator.ResetSession();
        var name2 = generator.GenerateNpcName("TEST-THEME");

        generator.ResetSession();
        var name3 = generator.GenerateNpcName("Test-Theme");

        // Assert
        name1.Should().NotBeNullOrWhiteSpace();
        name2.Should().NotBeNullOrWhiteSpace();
        name3.Should().NotBeNullOrWhiteSpace();
    }

    #endregion

    #region GetAvailableThemes Tests

    [Fact]
    public void GetAvailableThemes_WithNoCustomThemes_ShouldReturnBuiltInThemes()
    {
        // Arrange
        var generator = new NameGenerator(seed: 42);

        // Act
        var themes = generator.GetAvailableThemes();

        // Assert
        themes.Should().Contain("Cyberpunk");
        themes.Should().Contain("Elves");
        themes.Should().Contain("Orcs");
    }

    [Fact]
    public void GetAvailableThemes_WithCustomThemes_ShouldReturnAllThemes()
    {
        // Arrange
        var config = new ThemeConfig()
            .AddTheme("custom1", CreateValidCustomTheme())
            .AddTheme("custom2", CreateValidCustomTheme());
        var generator = new NameGenerator(config, seed: 42);

        // Act
        var themes = generator.GetAvailableThemes();

        // Assert
        themes.Should().Contain("Cyberpunk");
        themes.Should().Contain("Elves");
        themes.Should().Contain("Orcs");
        themes.Should().Contain("custom1");
        themes.Should().Contain("custom2");
    }

    #endregion

    #region Error Message Format Tests

    [Fact]
    public void ThemeDataBuilder_WithNullArray_ShouldThrowWithDescriptiveMessage()
    {
        // Arrange
        var builder = new ThemeDataBuilder();

        // Act
        var act = () => builder.WithCityNames(null!, new[] { "Core" }, new[] { "Suf" });

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("*City name prefixes cannot be null*");
    }

    [Fact]
    public void ThemeDataBuilder_WithEmptyArray_ShouldThrowWithDescriptiveMessage()
    {
        // Arrange
        var builder = new ThemeDataBuilder();

        // Act
        var act = () => builder.WithCityNames(Array.Empty<string>(), new[] { "Core" }, new[] { "Suf" });

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*City name prefixes cannot be empty*");
    }

    [Fact]
    public void ThemeDataBuilder_WithWhitespaceString_ShouldThrowWithDescriptiveMessage()
    {
        // Arrange
        var builder = new ThemeDataBuilder();

        // Act
        var act = () => builder.WithCityNames(new[] { "Pre", "   ", "Fix" }, new[] { "Core" }, new[] { "Suf" });

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*City name prefixes contains whitespace-only value*");
    }

    [Fact]
    public void ThemeDataBuilder_Build_WithMissingFields_ShouldThrowWithDescriptiveMessage()
    {
        // Arrange
        var builder = new ThemeDataBuilder()
            .WithCityNames(new[] { "Pre" }, new[] { "Core" }, new[] { "Suf" });

        // Act
        var act = () => builder.Build();

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Missing required fields*")
            .WithMessage("*NPC names*")
            .WithMessage("*Building names*");
    }

    [Fact]
    public void ThemeConfig_AddTheme_WithNullIdentifier_ShouldThrowWithDescriptiveMessage()
    {
        // Arrange
        var config = new ThemeConfig();
        var themeData = CreateValidCustomTheme();

        // Act
        var act = () => config.AddTheme(null!, themeData);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("identifier");
    }

    [Fact]
    public void ThemeConfig_AddTheme_WithWhitespaceIdentifier_ShouldThrowWithDescriptiveMessage()
    {
        // Arrange
        var config = new ThemeConfig();
        var themeData = CreateValidCustomTheme();

        // Act
        var act = () => config.AddTheme("   ", themeData);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Theme identifier cannot be whitespace-only*");
    }

    [Fact]
    public void GenerateNpcName_WithUnregisteredTheme_ShouldThrowWithAvailableThemes()
    {
        // Arrange
        var generator = new NameGenerator(seed: 42);

        // Act
        var act = () => generator.GenerateNpcName("unknown-theme");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*unknown-theme*")
            .WithMessage("*Available themes*");
    }

    #endregion

    #region Edge Cases Tests

    [Fact]
    public void ThemeDataBuilder_WithNullInArray_ShouldThrowArgumentException()
    {
        // Arrange
        var builder = new ThemeDataBuilder();

        // Act
        var act = () => builder.WithCityNames(new[] { "Pre", null!, "Fix" }, new[] { "Core" }, new[] { "Suf" });

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*contains null value*");
    }

    [Fact]
    public void ThemeConfig_AddTheme_WithNullThemeData_ShouldThrowArgumentNullException()
    {
        // Arrange
        var config = new ThemeConfig();

        // Act
        var act = () => config.AddTheme("test", null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("themeData");
    }

    [Fact]
    public void ThemeConfig_ExtendTheme_WithNullExtension_ShouldThrowArgumentNullException()
    {
        // Arrange
        var config = new ThemeConfig();

        // Act
        var act = () => config.ExtendTheme(Theme.Cyberpunk, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("extension");
    }

    [Fact]
    public void NameGenerator_RegisterCustomTheme_WithNullIdentifier_ShouldThrowArgumentNullException()
    {
        // Arrange
        var generator = new NameGenerator(seed: 42);
        var themeData = CreateValidCustomTheme();

        // Act
        var act = () => generator.RegisterCustomTheme(null!, themeData);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("identifier");
    }

    [Fact]
    public void NameGenerator_RegisterCustomTheme_WithWhitespaceIdentifier_ShouldThrowArgumentException()
    {
        // Arrange
        var generator = new NameGenerator(seed: 42);
        var themeData = CreateValidCustomTheme();

        // Act
        var act = () => generator.RegisterCustomTheme("   ", themeData);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Theme identifier cannot be whitespace-only*");
    }

    [Fact]
    public void NameGenerator_GenerateNpcName_WithNullStringIdentifier_ShouldThrowArgumentNullException()
    {
        // Arrange
        var generator = new NameGenerator(seed: 42);

        // Act
        var act = () => generator.GenerateNpcName((string)null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("themeIdentifier");
    }

    #endregion

    #region Backward Compatibility Tests

    [Fact]
    public void NameGenerator_ExistingEnumBasedMethods_ShouldStillWork()
    {
        // Arrange
        var generator = new NameGenerator(seed: 42);

        // Act & Assert
        generator.GenerateNpcName(Theme.Cyberpunk).Should().NotBeNullOrWhiteSpace();
        generator.GenerateBuildingName(Theme.Elves).Should().NotBeNullOrWhiteSpace();
        generator.GenerateCityName(Theme.Orcs).Should().NotBeNullOrWhiteSpace();
        generator.GenerateDistrictName(Theme.Cyberpunk).Should().NotBeNullOrWhiteSpace();
        generator.GenerateStreetName(Theme.Elves).Should().NotBeNullOrWhiteSpace();
        generator.GenerateFactionName(Theme.Orcs).Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void NameGenerator_WithoutConfig_ShouldWorkWithBuiltInThemes()
    {
        // Arrange
        var generator = new NameGenerator(seed: 42);

        // Act
        var name1 = generator.GenerateNpcName(Theme.Cyberpunk);
        var name2 = generator.GenerateBuildingName(Theme.Elves);
        var name3 = generator.GenerateCityName(Theme.Orcs);

        // Assert
        name1.Should().NotBeNullOrWhiteSpace();
        name2.Should().NotBeNullOrWhiteSpace();
        name3.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void NameGenerator_ResetSession_ShouldWorkWithCustomThemes()
    {
        // Arrange
        var config = new ThemeConfig()
            .AddTheme("test-theme", CreateValidCustomTheme());
        var generator = new NameGenerator(config, seed: 42);
        var name1 = generator.GenerateNpcName("test-theme");

        // Act
        generator.ResetSession();
        var name2 = generator.GenerateNpcName("test-theme");

        // Assert
        name1.Should().Be(name2, "reset should allow name reuse with same seed");
    }

    #endregion

    #region Helper Methods

    private static CustomThemeData CreateValidCustomTheme()
    {
        return new ThemeDataBuilder()
            .WithNpcNames(npc => npc
                .WithMaleNames(new[] { "Test" }, new[] { "Core" }, new[] { "Suf" })
                .WithFemaleNames(new[] { "Test" }, new[] { "Core" }, new[] { "Suf" })
                .WithNeutralNames(new[] { "Test" }, new[] { "Core" }, new[] { "Suf" }))
            .WithBuildingNames(building => building
                .WithGenericNames(new[] { "Test" }, new[] { "Building" })
                .WithTypeNames(BuildingType.Residential, new[] { "Test" }, new[] { "Desc" }, new[] { "Suf" })
                .WithTypeNames(BuildingType.Commercial, new[] { "Test" }, new[] { "Desc" }, new[] { "Suf" })
                .WithTypeNames(BuildingType.Industrial, new[] { "Test" }, new[] { "Desc" }, new[] { "Suf" })
                .WithTypeNames(BuildingType.Government, new[] { "Test" }, new[] { "Desc" }, new[] { "Suf" })
                .WithTypeNames(BuildingType.Entertainment, new[] { "Test" }, new[] { "Desc" }, new[] { "Suf" })
                .WithTypeNames(BuildingType.Medical, new[] { "Test" }, new[] { "Desc" }, new[] { "Suf" })
                .WithTypeNames(BuildingType.Educational, new[] { "Test" }, new[] { "Desc" }, new[] { "Suf" }))
            .WithCityNames(new[] { "Test" }, new[] { "Core" }, new[] { "Ville" })
            .WithDistrictNames(new[] { "Old" }, new[] { "District" })
            .WithStreetNames(new[] { "Test" }, new[] { "Oak" }, new[] { "Street" })
            .WithFactionNames(new[] { "Test" }, new[] { "Corp" }, new[] { "Suf" })
            .Build();
    }

    private static CustomThemeData CreateInvalidCustomTheme()
    {
        // Create a theme with missing building types (invalid)
        return new ThemeDataBuilder()
            .WithNpcNames(npc => npc
                .WithMaleNames(new[] { "Test" }, new[] { "Core" }, new[] { "Suf" })
                .WithFemaleNames(new[] { "Test" }, new[] { "Core" }, new[] { "Suf" })
                .WithNeutralNames(new[] { "Test" }, new[] { "Core" }, new[] { "Suf" }))
            .WithBuildingNames(building => building
                .WithGenericNames(new[] { "Test" }, new[] { "Building" })
                .WithTypeNames(BuildingType.Residential, new[] { "Test" }, new[] { "Desc" }, new[] { "Suf" }))
            .WithCityNames(new[] { "Test" }, new[] { "Core" }, new[] { "Ville" })
            .WithDistrictNames(new[] { "Old" }, new[] { "District" })
            .WithStreetNames(new[] { "Test" }, new[] { "Oak" }, new[] { "Street" })
            .WithFactionNames(new[] { "Test" }, new[] { "Corp" }, new[] { "Suf" })
            .Build();
    }

    private static ThemeExtension CreateValidExtension()
    {
        return ThemeDataBuilder.Extend(Theme.Cyberpunk)
            .WithNpcNames(npc => npc
                .AddMalePrefixes("Extra"))
            .BuildExtension();
    }

    #endregion
}
