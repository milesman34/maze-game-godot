using System;
using Godot;

/// <summary>
/// The Events autoloaded node handles global events.
/// </summary>
public partial class Events : Node {
    /// <summary>
    /// Static reference to the Events instance.
    /// </summary>
    public static Events Instance = new Events();

    /// <summary>
    /// Signal emitted when the level ends.
    /// </summary>
    /// <param name="levelName">Name of the completed level</param>
    /// <param name="score">The player's score in the level</param>
    /// <param name="deaths">How many times the player died</param>
    [Signal]
    public delegate void LevelEndEventHandler(string levelName, int score, int deaths);

    /// <summary>
    /// Signal emitted to switch to a level.
    /// </summary>
    /// <param name="level">Resource for the level to switch to</param>
    [Signal]
    public delegate void SwitchToLevelEventHandler(LevelResource level);

    /// <summary>
    /// Signal emitted to exit to the level select menu.
    /// </summary>
    [Signal]
    public delegate void ExitToLevelSelectEventHandler();

    /// <summary>
    /// Signal emitted when the player is hit.
    /// </summary>
    [Signal]
    public delegate void PlayerHitEventHandler();

    /// <summary>
    /// Signal emitted when a checkpoint is reached.
    /// </summary>
    [Signal]
    public delegate void ReachedCheckpointEventHandler();

    /// <summary>
    /// Signal emitted when a camera zone is entered.
    /// </summary>
    /// <param name="ID">ID of the camera zone</param>
    [Signal]
    public delegate void CameraZoneEnteredEventHandler(int ID);

    /// <summary>
    /// Signal emitted when a camera zone is exited.
    /// </summary>
    /// <param name="ID">ID of the camera zone</param>
    [Signal]
    public delegate void CameraZoneExitedEventHandler(int ID);

    // To handle instantiating the camera zone ID, it will require 2 events, one for when the camera zone is registered, and one for when the object is registered.
    // For the camera zone registration event, objects can add a listener to do the needed things
    // For the object registration event, camera zones can add a listener to do the needed things
    // The listener must be of the type ICameraZoneListener

    /// <summary>
    /// Signal emitted when a camera zone is registered.
    /// </summary>
    /// <param name="zone">The CameraZone to register</param>
    [Signal]
    public delegate void RegisterCameraZoneEventHandler(CameraZone zone);

    /// <summary>
    /// Signal emitted when a camera zone listener is registered.
    /// </summary>
    /// <param name="node">The CameraZoneListener to register</param>
    [Signal]
    public delegate void RegisterCameraZoneListenerEventHandler(Node2D node);

    /// <summary>
    /// Signal emitted when a conveyor belt is entered.
    /// </summary>
    /// <param name="unitVector">Unit vector representing the conveyor's angle</param>
    /// <param name="velocity">The conveyor's velocity</param>
    [Signal]
    public delegate void ConveyorBeltEnteredEventHandler(Vector2 unitVector, int velocity);

    /// <summary>
    /// Signal emitted when a conveyor belt is exited.
    /// </summary>
    /// <param name="unitVector">Unit vector representing the conveyor's angle</param>
    /// <param name="velocity">The conveyor's velocity</param>
    [Signal]
    public delegate void ConveyorBeltExitedEventHandler(Vector2 unitVector, int velocity);
    
    /// <summary>
    /// Constructs the Events object.
    /// </summary>
    public Events() {}
}