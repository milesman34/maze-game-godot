using Godot;
using System;

// Represents the player's stats on a level
public partial class LevelStats : Resource
{
    // Player's best score/deaths
    [Export]
    public int BestScore { get; set; }
    
    [Export]
    public int BestDeaths { get; set; }

    public LevelStats() : this(0, 0) {}

    public LevelStats(int _score, int _deaths) {
        BestScore = _score;
        BestDeaths = _deaths;
    }
}
