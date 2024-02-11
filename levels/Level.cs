using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Levels handle a level, with all the objects and data needed to make the level playable.
/// </summary>
public partial class Level : Node2D
{
    /// <summary>
    /// The player's starting position in units (can be overridden using StartPosition objects)
    /// </summary>
    [Export]
    private Vector2 StartPosition { get; set; }

    /// <summary>
    /// Signal emitted to set the player's score.
    /// </summary>
    /// <param name="score">The player's current score</param>
    [Signal]
    public delegate void SetScoreEventHandler(int score);

    /// <summary>
    /// Signal emitted to update the player's death count.
    /// </summary>
    /// <param name="deaths">The player's number of deaths</param>
    [Signal]
    public delegate void SetDeathsEventHandler(int deaths);

    /// <summary>
    /// Signal emitted when a key is collected, to update the locks.
    /// </summary>
    /// <param name="color">The color of the collected key</param>
    [Signal]
    public delegate void CollectKeyEventHandler(Color color);

    /// <summary>
    /// Signal emitted when the player reaches a checkpoint.
    /// </summary>
    [Signal]
    public delegate void CheckpointHitEventHandler();

    // The name of the level (to be set externally)
    private string levelName;

    // The priority of the player's start position (the default one is -9999)
    private int startPosPriority = -9999;

    // The player's current score
    private int score = 0;

    // The player's current number of deaths
    private int deaths = 0;

    // Reference to the current camera
    private Camera2D camera;

    // This dictionary maps the IDs of CameraZones to the CameraZones themselves
    private Dictionary<int, CameraZone> cameraZones;

    // Tracks the current CameraZone ID. The ID is null at first
    private int? cameraZoneID = null;

    // Tracks the CameraZone the player was previously in. This exists to solve a bug that occurred when you don't 100% leave a room, and try to go back in.
    // The ID is null at first.
    private int? previousCameraZoneID = null;

    // Did the player finish the level?
    private bool levelFinished = false;

    // Stored reference to the main game
    private GameScene game;

    /// <summary>
    /// Reference to the Game scene.
    /// </summary>
    public GameScene Game {
        get {
            return game;
        }
    }

    // Reference to the Player object
    private Player player;
    
    /// Reference to the level background
    private TextureRect background;

    // Did the player die recently? (last 50 ms)
    private bool recentDeath = false;

    // Tracks the most recent checkpoint the player has reached (checkpoints are whenever the player enters/exits a room, or can be defined manually)
    private Vector2 checkpoint;

    // Track the default background texture
    private Texture2D defaultBackgroundTexture;

    public override void _Ready() {
        // Instantiate the camera zones dictionary
        cameraZones = new Dictionary<int, CameraZone>();

        // The TileMap won't have all the elements as children immediately, so the call needs to be deferred
        CallDeferred(MethodName.AttachSignals);

        // Set camera reference
        camera = GetNode<Camera2D>("Camera");

        // Update position of player
		player = GetNode<Player>("Player");

		player.Position = GetStartingPosition();

        Events.Instance.PlayerHit += OnPlayerHit;

        // Set the starting checkpoint
        checkpoint = player.Position;

        // Set other references
        background = GetNode<TextureRect>("Background");
        defaultBackgroundTexture = background.Texture;
    }

    public override void _ExitTree() {
        base._ExitTree();

        Events.Instance.PlayerHit -= OnPlayerHit;
        Events.Instance.CameraZoneEntered -= OnCameraZoneEntered;
        Events.Instance.CameraZoneExited -= OnCameraZoneExited;
    }

    /// <summary>
    /// InstantiateLevel creates a new level instance from the provided LevelResource which has the needed reference to the main game.
    /// </summary>
    /// <param name="resource">LevelResource containing the level scene to instantiate</param>
    /// <param name="game">Reference to the game</param>
    /// <param name="gui">Reference to the GUI object</param>
    /// <returns>A newly created Level instance</returns>
    public static Level InstantiateLevel(LevelResource resource, GameScene game, GameGUI gui) {
        var level = resource.LevelScene.Instantiate<Level>();
        level.game = game;
        level.levelName = resource.LevelName;

		// Set some signals for the GUI
		level.SetScore += gui.SetScore;
        level.SetDeaths += gui.SetDeaths;

        return level;
    }

    // Returns the starting position in pixels. The original position was based on units.
    private Vector2 GetStartingPosition() {
        return StartPosition * Constants.TileSize + new Vector2(Constants.TileSize, Constants.TileSize) / 2.0f; // Size of tile?
    }

    /// <summary>
    /// Returns the player's current position, returning (0, 0) if the player doesn't exist.
    /// </summary>
    /// <returns></returns>
    public Vector2 GetPlayerPosition() {
        try {
            return player.GlobalPosition;
        } catch (ObjectDisposedException) {
            // I couldn't figure out a good way to check if the player doesn't exist, so using try/catch seems like my best bet
            return Vector2.Zero;
        }
    }

    // Attaches all the signals for the objects node recursively
    private void AttachSignalsForObjectsNode(Node parentNode) {
        foreach(var node in parentNode.GetChildren()) {
            // We check the class name specifically for Node, since other classes subclass Node
            if (node.GetClass() == "Node") {
                AttachSignalsForObjectsNode(node);
            } else if (node is IGameObject) {
                (node as IGameObject).AttachSignals(this);
            }
        }
    }

    // Attaches signals to key elements in the tilemap.
    private void AttachSignals() {
        var tileMap = GetNode<TileMap>("TileMap");

        foreach (var node in tileMap.GetChildren()) {
            if (node is IGameObject) {
                (node as IGameObject).AttachSignals(this);
            }
        }

        // Now attach the signals from the camera zones
        var zonesNode = GetNode<Node>("CameraZones");

        foreach(var node in zonesNode.GetChildren()) {
            (node as CameraZone).AttachSignals(this);
        }

        // Connect to global camera zone events
        Events.Instance.CameraZoneEntered += OnCameraZoneEntered;
        Events.Instance.CameraZoneExited += OnCameraZoneExited;

        // Now attach the signals from the other objects
        var objectsNode = GetNode<Node>("Objects");

        AttachSignalsForObjectsNode(objectsNode);
    }

    // Switches to the camera zone with the given ID
    private void SwitchToCameraZone(int ID) {
        // We need to update the background color to hide rooms the player is not in
        if (cameraZoneID is not null) { // The cameraID is set to -1 upon loading, so check if the key exists
            cameraZones[(int) cameraZoneID].SetSurroundingVisibility(false);
        }

        cameraZoneID = ID;

        // Update the background color of the new camera zone
        cameraZones[ID].SetSurroundingVisibility(true);

        // Update the camera
        UpdateCamera(ID);

        // Update background for the level itself (add some extra on the sides)
        background.Size = cameraZones[ID].GetCameraZoneSize();

        // Add the two positions together since the background object's position is relative to the camera zone
        background.Position = cameraZones[ID].GetCameraZonePosition() + cameraZones[ID].Position;

        // Set up the background texture
        var texture = cameraZones[ID].BackgroundTexture;

        background.Texture = texture == null ? defaultBackgroundTexture : texture;
    }

    // Updates the camera to have the position/zoom provided from the current CameraZone
    private void UpdateCamera(int ID) {
        var zone = cameraZones[ID];

        camera.Position = zone.GetCameraPosition();
        camera.Zoom = zone.GetCameraZoom();
    }

    // Updates the checkpoint to the new position
    private void UpdateCheckpoint(Vector2 position) {
        checkpoint = position;
        EmitSignal(SignalName.CheckpointHit);
    }

    /// <summary>
    /// Updates the camera zones dictionary to contain this camera zone.
    /// </summary>
    /// <param name="zone">The camera zone to add.</param>
    public void AddCameraZone(CameraZone zone) {
        cameraZones[zone.ID] = zone;
    }

    /// <summary>
    /// Registers a start position for the levle.
    /// </summary>
    /// <param name="startPosObject">The StartPosition object</param>
    public void RegisterStartPosition(StartPosition startPosObject) {
        if (startPosObject.Priority > startPosPriority) {
            // Update the position
            // It should be fine as long as the level creator doesn't do something stupid like putting a start position somewhere where the player immediately dies
            player.Position = startPosObject.Position;
            UpdateCheckpoint(player.Position);
            startPosPriority = startPosObject.Priority;
        } 
    }

    /// <summary>
    /// This function runs when something adds to the score.
    /// </summary>
    /// <param name="value">How much the score should be increased by</param>
    public void OnAddScore(int value) {
        score += value;
        EmitSignal(SignalName.SetScore, score);
    }

    /// <summary>
    /// This function runs when something sends a signal to end the level. The order works like this to ensure that the level can take care of cleaning everything up before needing to do anything else.
    /// </summary>
    public void OnLevelEnd() {
        // Hide the player
        levelFinished = true;
        player.QueueFree();

        // Send the level end event
        Events.Instance.EmitSignal(Events.SignalName.LevelEnd, levelName, score, deaths);
    }

    // Runs when a camera zone is entered by the player
    private void OnCameraZoneEntered(int ID) {
        previousCameraZoneID = cameraZoneID;
        SwitchToCameraZone(ID);
    }

    // Runs when a camera zone is exited by the player
    private void OnCameraZoneExited(int ID) {
        // Check if the room we are exiting is the previous one we were in
        // If it is, then we need to switch back to that room as we are no longer in the current room
        // This switch is done manually, as we never fully left the collision area for the original room
        // We also don't want to switch camera zones when the level is finished
        if (ID == cameraZoneID && previousCameraZoneID != null && !levelFinished) {
            SwitchToCameraZone((int) previousCameraZoneID);
        } else if (levelFinished) {
            // Re-hide the surrounding area
            cameraZones[ID].SetSurroundingVisibility(true);
        }
        
        // Update player checkpoint
        UpdateCheckpoint(player.Position);
    }

    /// <summary>
    /// This function runs when a camera zone is updated, usually via window resizing.
    /// </summary>
    /// <param name="ID">ID of the camera zone being updated</param>
    public void OnCameraZoneUpdate(int ID) {
        if (ID == cameraZoneID) {
            UpdateCamera(ID);
        }
    }

    /// <summary>
    /// This function runs when the player enters a portal.
    /// </summary>
    /// <param name="target">Position to teleport the player to.</param>
    public void OnPortalEntered(Vector2 target) {
        // If the player just teleported here then they are safe from further teleportation until they leave the portal
        if (!player.InvincibilityFramesActive) {
            player.EnableInvincibilityFrames();

            // We need to update the GlobalPosition directly
            // It seems the regular Position is updated in the physics process
            // So updating that could cause the player to snap back here due to the physics process
            player.GlobalPosition = target * Constants.TileSize + Constants.TileVector / 2.0f;
        }
    }
    
    /// <summary>
    /// This function runs when the player exits a portal.
    /// </summary>
    public void OnPortalExited() {
        player.DisableInvincibilityFrames();
    }

    /// <summary>
    /// This function runs when a key is collected by the player.
    /// </summary>
    /// <param name="color">Color of the key</param>
    public void OnKeyCollected(Color color) {
        // Propogate the signal to any locks
        EmitSignal(SignalName.CollectKey, color);
    }

    // Runs when the player is hit by an obstacle
    private void OnPlayerHit() {
        // Hmm, how do I deal with this? Maybe I can add a timer after each death
        if (!recentDeath) {
            deaths++;
            recentDeath = true;

            player.EnableInvincibilityFrames();

            player.Position = checkpoint;

            EmitSignal(SignalName.SetDeaths, deaths);

            GetTree().CreateTimer(0.05).Timeout += () => {
                recentDeath = false;
            };
        }
    }

    /// <summary>
    /// This function runs when a checkpoint is reached.
    /// </summary>
    public void OnReachedCheckpoint() {
        UpdateCheckpoint(player.Position);
    }
}