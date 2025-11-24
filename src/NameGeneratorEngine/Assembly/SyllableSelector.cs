using NameGeneratorEngine.Foundation;

namespace NameGeneratorEngine.Assembly;

/// <summary>
/// Selects syllables from arrays using seeded random generation.
/// </summary>
internal class SyllableSelector
{
    /// <summary>
    /// Selects a random element from the provided array.
    /// </summary>
    /// <param name="options">The array of options to select from.</param>
    /// <param name="random">The seeded random generator to use.</param>
    /// <returns>A randomly selected element from the array.</returns>
    public string SelectFrom(string[] options, SeededRandom random)
    {
        if (options == null || options.Length == 0)
        {
            return string.Empty;
        }

        int index = random.Next(options.Length);
        return options[index];
    }

    /// <summary>
    /// Selects multiple random elements from the provided array.
    /// </summary>
    /// <param name="options">The array of options to select from.</param>
    /// <param name="count">The number of elements to select.</param>
    /// <param name="random">The seeded random generator to use.</param>
    /// <returns>An array of randomly selected elements.</returns>
    public string[] SelectMultiple(string[] options, int count, SeededRandom random)
    {
        if (options == null || options.Length == 0 || count <= 0)
        {
            return Array.Empty<string>();
        }

        var results = new string[count];
        for (int i = 0; i < count; i++)
        {
            results[i] = SelectFrom(options, random);
        }

        return results;
    }
}
