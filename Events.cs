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

    // Signal that runs when a camera zone is entered
    [Signal]
    public delegate void CameraZoneEnteredEventHandler(int ID);

    // Signal that runs when a camera zone is exited
    [Signal]
    public delegate void CameraZoneExitedEventHandler(int ID);
    
    // Constructor
    public Events() {}
}