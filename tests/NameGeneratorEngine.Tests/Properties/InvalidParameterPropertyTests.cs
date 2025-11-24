using CsCheck;
using FluentAssertions;
using NameGeneratorEngine.Enums;
using Xunit;

namespace NameGeneratorEngine.Tests.Properties;

/// <summary>
/// Property-based tests for invalid parameter handling.
/// </summary>
public class InvalidParameterPropertyTests
{
    /// <summary>
    /// Feature: name-generator-engine, Property 8: Invalid parameters throw ArgumentException
    /// For any invalid parameter value (such as an undefined enum value or null where not allowed), 
    /// the generation methods should throw ArgumentException before attempting to generate a name.
    /// Validates: Requirements 12.1
    /// </summary>
    [Fact]
    public void Property_InvalidThemeThrowsArgumentException()
    {
        // Generate invalid theme values (outside the valid enum range)
        var genInvalidTheme = Gen.Int.Where(i => !Enum.IsDefined(typeof(Theme), i))
            .Select(i => (Theme)i);

        genInvalidTheme.Sample(invalidTheme =>
        {
            var generator = new NameGenerator(seed: 42);

            // Test all generation methods with invalid theme
            var npcAction = () => generator.GenerateNpcName(invalidTheme);
            var buildingAction = () => generator.GenerateBuildingName(invalidTheme);
            var cityAction = () => generator.GenerateCityName(invalidTheme);
            var districtAction = () => generator.GenerateDistrictName(invalidTheme);
            var streetAction = () => generator.GenerateStreetName(invalidTheme);

            // All should throw ArgumentException
            npcAction.Should().Throw<ArgumentException>()
                .WithMessage("*Invalid theme value*")
                .And.ParamName.Should().Be("theme");

            buildingAction.Should().Throw<ArgumentException>()
                .WithMessage("*Invalid theme value*")
                .And.ParamName.Should().Be("theme");

            cityAction.Should().Throw<ArgumentException>()
                .WithMessage("*Invalid theme value*")
                .And.ParamName.Should().Be("theme");

            districtAction.Should().Throw<ArgumentException>()
                .WithMessage("*Invalid theme value*")
                .And.ParamName.Should().Be("theme");

            streetAction.Should().Throw<ArgumentException>()
                .WithMessage("*Invalid theme value*")
                .And.ParamName.Should().Be("theme");
        }, iter: 100);
    }

    /// <summary>
    /// Property test that verifies invalid gender values throw ArgumentException.
    /// </summary>
    [Fact]
    public void Property_InvalidGenderThrowsArgumentException()
    {
        // Generate invalid gender values (outside the valid enum range)
        var genInvalidGender = Gen.Int.Where(i => !Enum.IsDefined(typeof(Gender), i))
            .Select(i => (Gender)i);

        var genValidTheme = Gen.Int[0, 2].Select(i => (Theme)i);

        Gen.Select(genValidTheme, genInvalidGender)
            .Sample(tuple =>
            {
                var (theme, invalidGender) = tuple;
                var generator = new NameGenerator(seed: 42);

                // Test GenerateNpcName with invalid gender
                var action = () => generator.GenerateNpcName(theme, invalidGender);

                action.Should().Throw<ArgumentException>()
                    .WithMessage("*Invalid gender value*")
                    .And.ParamName.Should().Be("gender");
            }, iter: 100);
    }

    /// <summary>
    /// Property test that verifies invalid building type values throw ArgumentException.
    /// </summary>
    [Fact]
    public void Property_InvalidBuildingTypeThrowsArgumentException()
    {
        // Generate invalid building type values (outside the valid enum range)
        var genInvalidBuildingType = Gen.Int.Where(i => !Enum.IsDefined(typeof(BuildingType), i))
            .Select(i => (BuildingType)i);

        var genValidTheme = Gen.Int[0, 2].Select(i => (Theme)i);

        Gen.Select(genValidTheme, genInvalidBuildingType)
            .Sample(tuple =>
            {
                var (theme, invalidBuildingType) = tuple;
                var generator = new NameGenerator(seed: 42);

                // Test GenerateBuildingName with invalid building type
                var action = () => generator.GenerateBuildingName(theme, invalidBuildingType);

                action.Should().Throw<ArgumentException>()
                    .WithMessage("*Invalid buildingType value*")
                    .And.ParamName.Should().Be("buildingType");
            }, iter: 100);
    }

    /// <summary>
    /// Property test that verifies exception messages contain expected information.
    /// </summary>
    [Fact]
    public void Property_ExceptionMessagesContainExpectedInformation()
    {
        // Generate invalid enum values
        var genInvalidTheme = Gen.Int.Where(i => !Enum.IsDefined(typeof(Theme), i))
            .Select(i => (Theme)i);

        genInvalidTheme.Sample(invalidTheme =>
        {
            var generator = new NameGenerator(seed: 42);

            try
            {
                generator.GenerateCityName(invalidTheme);
                Assert.Fail("Expected ArgumentException to be thrown");
            }
            catch (ArgumentException ex)
            {
                // Verify exception message contains the invalid value
                ex.Message.Should().Contain(((int)invalidTheme).ToString(),
                    "exception message should include the invalid value");

                // Verify exception message contains expected values
                ex.Message.Should().Contain("Expected values",
                    "exception message should list expected values");

                // Verify exception message lists valid theme names
                ex.Message.Should().Contain("Cyberpunk");
                ex.Message.Should().Contain("Elves");
                ex.Message.Should().Contain("Orcs");

                // Verify parameter name is set
                ex.ParamName.Should().Be("theme");
            }
        }, iter: 100);
    }

    /// <summary>
    /// Property test that verifies validation happens before any processing.
    /// This ensures that invalid parameters don't cause side effects.
    /// </summary>
    [Fact]
    public void Property_ValidationHappensBeforeProcessing()
    {
        var genInvalidTheme = Gen.Int.Where(i => !Enum.IsDefined(typeof(Theme), i))
            .Select(i => (Theme)i);

        genInvalidTheme.Sample(invalidTheme =>
        {
            var generator = new NameGenerator(seed: 42);

            // Generate a valid name first to establish state
            var validName = generator.GenerateCityName(Theme.Cyberpunk);
            validName.Should().NotBeNullOrEmpty();

            // Try to generate with invalid theme - should throw immediately
            var action = () => generator.GenerateCityName(invalidTheme);
            action.Should().Throw<ArgumentException>();

            // Verify we can still generate valid names after the exception
            // This proves that validation happened before any state changes
            var anotherValidName = generator.GenerateCityName(Theme.Elves);
            anotherValidName.Should().NotBeNullOrEmpty();
            anotherValidName.Should().NotBe(validName, "should generate a different name");
        }, iter: 100);
    }
}
