using CsCheck;
using FluentAssertions;
using Xunit;

namespace NameGeneratorEngine.Tests.Properties;

/// <summary>
/// Property-based tests for JSON error handling.
/// **Feature: custom-theme-registration, Property 6: JSON error handling**
/// </summary>
public class JsonErrorHandlingPropertyTests
{
    /// <summary>
    /// Property 6: JSON error handling
    /// For any malformed JSON file or missing file path,
    /// the system should throw InvalidOperationException with details about the error.
    /// **Validates: Requirements 3.3**
    /// </summary>
    [Fact]
    public void Property_MalformedJson_ThrowsInvalidOperationException()
    {
        // Generate various malformed JSON strings
        Gen.OneOf(
            Gen.Const("{"),
            Gen.Const("}"),
            Gen.Const("["),
            Gen.Const("]"),
            Gen.Const("{\"theme\": }"),
            Gen.Const("{\"theme\": \"Cyberpunk\",}"),
            Gen.Const("{\"theme\": \"Cyberpunk\" \"npcNames\": {}}"),
            Gen.Const("not json at all"),
            Gen.Const("{ invalid: json }"),
            Gen.Const("{\"theme\": \"Cyberpunk\", \"npcNames\": [}"),
            Gen.Const("")
        ).Sample(malformedJson =>
        {
            var act = () => CustomThemeData.FromJsonString(malformedJson);
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*Failed to parse custom theme data*");
        }, iter: 100);
    }

    /// <summary>
    /// Property 6: JSON error handling
    /// For any non-existent file path, the system should throw InvalidOperationException
    /// indicating the file was not found.
    /// **Validates: Requirements 3.3**
    /// </summary>
    [Fact]
    public void Property_NonExistentFile_ThrowsInvalidOperationException()
    {
        // Generate random non-existent file paths
        Gen.String.Where(s => !string.IsNullOrWhiteSpace(s) && !File.Exists(s))
            .Sample(nonExistentPath =>
            {
                var act = () => CustomThemeData.FromJson(nonExistentPath);
                act.Should().Throw<InvalidOperationException>()
                    .WithMessage($"*{nonExistentPath}*");
            }, iter: 100);
    }

    /// <summary>
    /// Property 6: JSON error handling
    /// For any JSON with missing required fields, the system should throw InvalidOperationException
    /// with validation error details.
    /// **Validates: Requirements 3.3**
    /// </summary>
    [Fact]
    public void Property_IncompleteJson_ThrowsInvalidOperationException()
    {
        // Generate JSON with missing required fields
        Gen.OneOf(
            Gen.Const("{\"theme\": \"Cyberpunk\"}"),
            Gen.Const("{\"theme\": \"Cyberpunk\", \"npcNames\": {}}"),
            Gen.Const("{\"theme\": \"Cyberpunk\", \"npcNames\": {\"male\": {}}}"),
            Gen.Const("{\"npcNames\": {\"male\": {\"prefixes\": [\"M\"], \"cores\": [\"a\"], \"suffixes\": [\"le\"]}}}")
        ).Sample(incompleteJson =>
        {
            var act = () => CustomThemeData.FromJsonString(incompleteJson);
            act.Should().Throw<InvalidOperationException>();
        }, iter: 100);
    }

    /// <summary>
    /// Property 6: JSON error handling
    /// For null arguments, the system should throw ArgumentNullException.
    /// **Validates: Requirements 3.3**
    /// </summary>
    [Fact]
    public void Property_NullArguments_ThrowsArgumentNullException()
    {
        var actFromJson = () => CustomThemeData.FromJson(null!);
        actFromJson.Should().Throw<ArgumentNullException>();

        var actFromJsonString = () => CustomThemeData.FromJsonString(null!);
        actFromJsonString.Should().Throw<ArgumentNullException>();
    }

    /// <summary>
    /// Property 6: JSON error handling
    /// For any JSON with invalid data types, the system should throw InvalidOperationException.
    /// **Validates: Requirements 3.3**
    /// </summary>
    [Fact]
    public void Property_InvalidDataTypes_ThrowsInvalidOperationException()
    {
        // Generate JSON with wrong data types
        Gen.OneOf(
            Gen.Const("{\"theme\": 123}"),
            Gen.Const("{\"theme\": \"Cyberpunk\", \"npcNames\": \"not an object\"}"),
            Gen.Const("{\"theme\": \"Cyberpunk\", \"npcNames\": {\"male\": \"not an object\"}}"),
            Gen.Const("{\"theme\": \"Cyberpunk\", \"npcNames\": {\"male\": {\"prefixes\": \"not an array\"}}}")
        ).Sample(invalidJson =>
        {
            var act = () => CustomThemeData.FromJsonString(invalidJson);
            act.Should().Throw<InvalidOperationException>();
        }, iter: 100);
    }

    /// <summary>
    /// Property 6: JSON error handling
    /// For any JSON with empty arrays, the system should throw InvalidOperationException
    /// with validation error details.
    /// **Validates: Requirements 3.3**
    /// </summary>
    [Fact]
    public void Property_EmptyArraysInJson_ThrowsInvalidOperationException()
    {
        var jsonWithEmptyArray = @"{
            ""theme"": ""Cyberpunk"",
            ""npcNames"": {
                ""male"": {
                    ""prefixes"": [],
                    ""cores"": [""a""],
                    ""suffixes"": [""le""]
                },
                ""female"": {
                    ""prefixes"": [""F""],
                    ""cores"": [""e""],
                    ""suffixes"": [""male""]
                },
                ""neutral"": {
                    ""prefixes"": [""N""],
                    ""cores"": [""eu""],
                    ""suffixes"": [""tral""]
                }
            },
            ""buildingNames"": {
                ""genericPrefixes"": [""Build""],
                ""genericSuffixes"": [""ing""],
                ""typeData"": {
                    ""Residential"": { ""prefixes"": [""Home""], ""descriptors"": [""Tower""], ""suffixes"": [""s""] },
                    ""Commercial"": { ""prefixes"": [""Shop""], ""descriptors"": [""Center""], ""suffixes"": [""s""] },
                    ""Industrial"": { ""prefixes"": [""Factory""], ""descriptors"": [""Complex""], ""suffixes"": [""""] },
                    ""Government"": { ""prefixes"": [""City""], ""descriptors"": [""Hall""], ""suffixes"": [""""] },
                    ""Entertainment"": { ""prefixes"": [""Fun""], ""descriptors"": [""Zone""], ""suffixes"": [""""] },
                    ""Medical"": { ""prefixes"": [""Health""], ""descriptors"": [""Center""], ""suffixes"": [""""] },
                    ""Educational"": { ""prefixes"": [""School""], ""descriptors"": [""House""], ""suffixes"": [""""] }
                }
            },
            ""cityNames"": {
                ""prefixes"": [""City""],
                ""cores"": [""Core""],
                ""suffixes"": [""ton""]
            },
            ""districtNames"": {
                ""descriptors"": [""Old""],
                ""locationTypes"": [""District""]
            },
            ""streetNames"": {
                ""prefixes"": [""Main""],
                ""cores"": [""Oak""],
                ""streetSuffixes"": [""Street""]
            },
            ""factionNames"": {
                ""prefixes"": [""The""],
                ""cores"": [""Guild""],
                ""suffixes"": [""s""]
            }
        }";

        var act = () => CustomThemeData.FromJsonString(jsonWithEmptyArray);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*null or empty*");
    }
}
