using System;
using Godot;

// This autoloaded node handles some global events
public partial class Events : Node {
    // Static reference to the Events singleton
    public static Events instance = new Events();

    // Runs when the level ends
    [Signal]
    public delegate void LevelEndEventHandler(string levelName, int score, int deaths);

    // Runs to switch to a level
    [Signal]
    public delegate void SwitchToLevelEventHandler(LevelResource level);

    // Runs to quit out back to the level select scene
    [Signal]
    public delegate void ExitToLevelSelectEventHandler();

    // Signal that runs when the player is hit
    [Signal]
    public delegate void PlayerHitEventHandler();
    
    // Constructor
    public Events() {}
}