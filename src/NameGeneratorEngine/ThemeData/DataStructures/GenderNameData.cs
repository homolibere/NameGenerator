using System.Text.Json.Serialization;

namespace NameGeneratorEngine.ThemeData.DataStructures;

/// <summary>
/// Represents name data for a specific gender, containing syllable arrays for name construction.
/// </summary>
internal class GenderNameData
{
    /// <summary>
    /// Gets the array of prefix syllables used at the start of names.
    /// </summary>
    [JsonPropertyName("prefixes")]
    required public string[] Prefixes { get; init; }

    /// <summary>
    /// Gets the array of core syllables used in the middle of names.
    /// </summary>
    [JsonPropertyName("cores")]
    required public string[] Cores { get; init; }

    /// <summary>
    /// Gets the array of suffix syllables used at the end of names.
    /// </summary>
    [JsonPropertyName("suffixes")]
    required public string[] Suffixes { get; init; }
}
