namespace NameGeneratorEngine.Foundation;

/// <summary>
/// Wrapper around System.Random that provides seeded randomization for deterministic generation.
/// </summary>
internal class SeededRandom
{
    private readonly Random _random;

    /// <summary>
    /// Gets the seed value used to initialize this random number generator.
    /// </summary>
    public int Seed { get; }

    /// <summary>
    /// Initializes a new instance of the SeededRandom class with the specified seed.
    /// </summary>
    /// <param name="seed">The seed value for the random number generator.</param>
    public SeededRandom(int seed)
    {
        Seed = seed;
        _random = new Random(seed);
    }

    /// <summary>
    /// Returns a non-negative random integer that is less than the specified maximum.
    /// </summary>
    /// <param name="maxValue">The exclusive upper bound of the random number to be generated.</param>
    /// <returns>A random integer between 0 (inclusive) and maxValue (exclusive).</returns>
    public int Next(int maxValue)
    {
        return _random.Next(maxValue);
    }

    /// <summary>
    /// Returns a random integer within a specified range.
    /// </summary>
    /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
    /// <param name="maxValue">The exclusive upper bound of the random number returned.</param>
    /// <returns>A random integer between minValue (inclusive) and maxValue (exclusive).</returns>
    public int Next(int minValue, int maxValue)
    {
        return _random.Next(minValue, maxValue);
    }
}
