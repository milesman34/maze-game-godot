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

    // To handle instantiating the camera zone ID, it will require 2 events, one for when the camera zone is registered, and one for when the object is registered.
    // For the camera zone registration event, objects can add a listener to do the needed things
    // For the object registration event, camera zones can add a listener to do the needed things
    // The listener must be of the type ICameraZoneListener
    [Signal]
    public delegate void RegisterCameraZoneEventHandler(CameraZone zone);

    [Signal]
    public delegate void RegisterCameraZoneListenerEventHandler(Node2D node);
    
    // Constructor
    public Events() {}
}