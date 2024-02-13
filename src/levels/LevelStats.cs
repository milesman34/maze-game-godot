using Godot;
using System;

/// <summary>
/// Resource that represents the player's best stats on a given level.
/// </summary>
public partial class LevelStats : Resource {
    /// <summary>
    /// The player's highest score.
    /// </summary>
    [Export]
    public int BestScore { get; set; }
    
    /// <summary>
    /// The player's lowest number of deaths.
    /// </summary>
    [Export]
    public int BestDeaths { get; set; }

    /// <summary>
    /// Constructs a LevelStats object.
    /// </summary>
    public LevelStats() : this(0, 0) {}

    /// <summary>
    /// Constructs a LevelStats object.
    /// </summary>
    /// <param name="_score">The player's score</param>
    /// <param name="_deaths">The player's number of deaths</param>
    public LevelStats(int _score, int _deaths) {
        BestScore = _score;
        BestDeaths = _deaths;
    }
}
