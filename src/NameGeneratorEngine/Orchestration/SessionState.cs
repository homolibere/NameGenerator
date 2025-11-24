using NameGeneratorEngine.Foundation;

namespace NameGeneratorEngine.Orchestration;

/// <summary>
/// Manages the state of a generation session, including the random instance and duplicate tracking.
/// </summary>
internal class SessionState
{
    /// <summary>
    /// Gets the seeded random number generator for this session.
    /// </summary>
    public SeededRandom Random { get; private set; }

    /// <summary>
    /// Gets the duplicate tracker for this session.
    /// </summary>
    public DuplicateTracker Tracker { get; }

    /// <summary>
    /// Initializes a new instance of the SessionState class.
    /// </summary>
    /// <param name="seed">The seed value for the random number generator.</param>
    public SessionState(int seed)
    {
        Random = new SeededRandom(seed);
        Tracker = new DuplicateTracker();
    }

    /// <summary>
    /// Resets the session state, clearing all tracked names and reinitializing the random generator.
    /// </summary>
    public void Reset()
    {
        Tracker.Clear();
        Random = new SeededRandom(Random.Seed);
    }
}
