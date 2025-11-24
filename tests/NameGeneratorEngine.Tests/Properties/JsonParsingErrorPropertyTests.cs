using CsCheck;
using FluentAssertions;
using Xunit;

namespace NameGeneratorEngine.Tests.Properties;

/// <summary>
/// Property-based tests for JSON parsing error details.
/// **Feature: custom-theme-registration, Property 15: JSON parsing error details**
/// **Validates: Requirements 5.5**
/// </summary>
public class JsonParsingErrorPropertyTests
{
    /// <summary>
    /// Property: For any malformed JSON content, the InvalidOperationException should include
    /// the underlying JSON parsing error details.
    /// </summary>
    [Fact]
    public void Property_MalformedJson_ErrorDetailsIncluded()
    {
        Gen.OneOf(
            Gen.Const("{ invalid json"),
            Gen.Const("{ \"npcNames\": { \"male\": { \"prefixes\": [1, 2, 3] } } }"), // Wrong type
            Gen.Const("{ \"npcNames\": null }"), // Null required field
            Gen.Const("{ \"theme\": \"InvalidTheme\" }"), // Invalid enum value
            Gen.Const(""), // Empty JSON
            Gen.Const("null"), // Null JSON
            Gen.Const("{ \"npcNames\": { \"male\": { \"prefixes\": [ }"), // Incomplete array
            Gen.Const("{ \"npcNames\": { \"male\": { \"prefixes\": [\"test\"] } } }")) // Missing required fields
        .Sample(malformedJson =>
        {
            // Attempt to load from malformed JSON string
            var exception = Assert.Throws<InvalidOperationException>(() =>
            {
                CustomThemeData.FromJsonString(malformedJson);
            });
            
            // Verify the exception message mentions JSON parsing/deserialization
            var mentionsJsonError = 
                exception.Message.Contains("JSON", StringComparison.OrdinalIgnoreCase) ||
                exception.Message.Contains("parse", StringComparison.OrdinalIgnoreCase) ||
                exception.Message.Contains("deserialize", StringComparison.OrdinalIgnoreCase);
            
            mentionsJsonError.Should().BeTrue(
                "error message should mention JSON parsing or deserialization");
            
            // Verify the exception has an inner exception with details
            // OR the message contains error details
            var hasDetails = 
                exception.InnerException != null ||
                exception.Message.Length > 50; // Detailed message
            
            hasDetails.Should().BeTrue(
                "error should include underlying error details either in inner exception or message");
        }, iter: 100);
    }

    /// <summary>
    /// Property: For any file-based JSON loading error, the InvalidOperationException should
    /// include the file path in the error message.
    /// </summary>
    [Fact]
    public void Property_FileLoadingError_FilePathIncluded()
    {
        Gen.String.Sample(randomPath =>
        {
            // Generate a path that doesn't exist
            var nonExistentPath = $"/nonexistent/path/{randomPath}.json";
            
            // Attempt to load from non-existent file
            var exception = Assert.Throws<InvalidOperationException>(() =>
            {
                CustomThemeData.FromJson(nonExistentPath);
            });
            
            // Verify the exception message mentions the file path
            exception.Message.Should().Contain(nonExistentPath,
                "error message should include the file path that failed to load");
            
            // Verify the exception mentions file-related error
            var mentionsFileError = 
                exception.Message.Contains("file", StringComparison.OrdinalIgnoreCase) ||
                exception.Message.Contains("load", StringComparison.OrdinalIgnoreCase);
            
            mentionsFileError.Should().BeTrue(
                "error message should mention file loading");
        }, iter: 100);
    }

    /// <summary>
    /// Property: For any JSON with invalid theme data structure, the InvalidOperationException
    /// should provide details about what is invalid.
    /// </summary>
    [Fact]
    public void Property_InvalidThemeData_ValidationDetailsIncluded()
    {
        // Create JSON with valid structure but invalid data (empty arrays)
        var invalidJson = @"{
            ""theme"": ""Cyberpunk"",
            ""npcNames"": {
                ""male"": { ""prefixes"": [], ""cores"": [], ""suffixes"": [] },
                ""female"": { ""prefixes"": [], ""cores"": [], ""suffixes"": [] },
                ""neutral"": { ""prefixes"": [], ""cores"": [], ""suffixes"": [] }
            },
            ""buildingNames"": {
                ""genericPrefixes"": [],
                ""genericSuffixes"": [],
                ""typeData"": {}
            },
            ""cityNames"": { ""prefixes"": [], ""cores"": [], ""suffixes"": [] },
            ""districtNames"": { ""descriptors"": [], ""locationTypes"": [] },
            ""streetNames"": { ""prefixes"": [], ""cores"": [], ""streetSuffixes"": [] },
            ""factionNames"": { ""prefixes"": [], ""cores"": [], ""suffixes"": [] }
        }";
        
        Gen.Const(invalidJson).Sample(json =>
        {
            // Attempt to load from JSON with invalid data
            var exception = Assert.Throws<InvalidOperationException>(() =>
            {
                CustomThemeData.FromJsonString(json);
            });
            
            // Verify the exception message mentions validation
            var mentionsValidation = 
                exception.Message.Contains("validation", StringComparison.OrdinalIgnoreCase) ||
                exception.Message.Contains("invalid", StringComparison.OrdinalIgnoreCase) ||
                exception.Message.Contains("empty", StringComparison.OrdinalIgnoreCase);
            
            mentionsValidation.Should().BeTrue(
                "error message should mention validation or what is invalid");
            
            // Verify the exception provides details about what failed
            exception.Message.Length.Should().BeGreaterThan(50,
                "error message should provide detailed information about the validation failure");
        }, iter: 100);
    }

    /// <summary>
    /// Property: For any JSON parsing exception, the error message should be descriptive
    /// and help developers understand what went wrong.
    /// </summary>
    [Fact]
    public void Property_JsonError_DescriptiveMessage()
    {
        Gen.OneOf(
            Gen.Const("{ \"invalid\": "),
            Gen.Const("{ \"npcNames\": { \"male\": { \"prefixes\": [1, 2] } } }"),
            Gen.Const("not json at all"),
            Gen.Const("{ \"theme\": 123 }"))
        .Sample(invalidJson =>
        {
            // Attempt to load from invalid JSON
            var exception = Assert.Throws<InvalidOperationException>(() =>
            {
                CustomThemeData.FromJsonString(invalidJson);
            });
            
            // Verify the exception message is descriptive (not just "An error occurred")
            exception.Message.Should().NotBe("An error occurred",
                "error message should be descriptive, not generic");
            
            exception.Message.Length.Should().BeGreaterThan(20,
                "error message should provide meaningful information");
            
            // Verify the message mentions what operation failed
            var mentionsOperation = 
                exception.Message.Contains("parse", StringComparison.OrdinalIgnoreCase) ||
                exception.Message.Contains("load", StringComparison.OrdinalIgnoreCase) ||
                exception.Message.Contains("deserialize", StringComparison.OrdinalIgnoreCase) ||
                exception.Message.Contains("theme", StringComparison.OrdinalIgnoreCase);
            
            mentionsOperation.Should().BeTrue(
                "error message should mention what operation failed");
        }, iter: 100);
    }
}
