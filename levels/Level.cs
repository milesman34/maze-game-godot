using Godot;
using System;
using System.Collections.Generic;
using System.Threading;

/// <summary>
/// Levels handle a level, with all the objects and data needed to make the level playable.
/// </summary>
public partial class Level : Node2D
{
    // The player's starting position in units
    [Export]
    private Vector2 StartPosition { get; set; }

    // The player's current score
    public int score = 0;

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
    public Main mainGame;

    // Reference to the Player object
    private Player player;
    
    /// Reference to the level background
    private ColorRect background;

    // Signal that updates the game's current score
    [Signal]
    public delegate void SetScoreEventHandler(int score);

    // Signal that runs when a key is collected, to send this message to the locks
    [Signal]
    public delegate void CollectKeyEventHandler(Color color);

    // Track what keys have been collected
    private Dictionary<Color, int> collectedKeys;

    /// <summary>
    /// InstantiateLevelScene creates a new level instance from the provided LevelScene which has the needed reference to the main game.
    /// </summary>
    /// <param name="levelScene">Level Scene to instantiate.</param>
    /// <param name="game">Reference to the game.</param>
    /// <param name="gui">Reference to the GUI object.</param>
    /// <returns>A newly created Level instance</returns>
    public static Level InstantiateLevelScene(PackedScene levelScene, Main game, GUI gui) {
        var level = levelScene.Instantiate<Level>();
        level.mainGame = game;

		// Set some signals for the GUI
		level.SetScore += gui.SetScore;

        return level;
    }

    public override void _Ready()
    {
        // Instantiate the camera zones dictionary
        cameraZones = new Dictionary<int, CameraZone>();

        // Instantiate the keys dictionary
        collectedKeys = new Dictionary<Color, int>();

        // The TileMap won't have all the elements as children immediately, so the call needs to be deferred
        CallDeferred(MethodName.AttachSignals);

        // Set camera reference
        camera = GetNode<Camera2D>("Camera");

        // Update position of player
		player = GetNode<Player>("Player");

		player.Position = GetStartingPosition();

        // Set other references
        background = GetNode<ColorRect>("Background");
    }

    // Returns the starting position in pixels. The original position was based on units.
    private Vector2 GetStartingPosition() {
        return StartPosition * Constants.TileSize + new Vector2(Constants.TileSize, Constants.TileSize) / 2.0f; // Size of tile?
    }

    // Attaches all the signals for the objects node recursively
    private void AttachSignalsForObjectsNode(Node parentNode) {
        foreach(var node in parentNode.GetChildren()) {
            // We check the class name specifically for Node, since other classes subclass Node
            if (node.GetClass() == "Node") {
                AttachSignalsForObjectsNode(node);
            } else if (node is Portal) {
                var portal = node as Portal;

                portal.PortalEntered += OnPortalEntered;
                portal.PortalExited += OnPortalExited;
            } else if (node is Key) {
                (node as Key).CollectKey += OnKeyCollected;
            } else if (node is Lock) {
                CollectKey += (node as Lock).OnKeyCollected;
            }
        }
    }

    // Attaches signals to key elements in the tilemap.
    private void AttachSignals() {
        var tileMap = GetNode<TileMap>("TileMap");

        foreach (var node in tileMap.GetChildren()) {
            if (node is Coin) {
                (node as Coin).CollectCoin += OnAddScore;
            } else if (node is EndPortal) {
                (node as EndPortal).LevelEnd += OnLevelEnd;
            }
        }

        // Now attach the signals from the camera zones
        var zonesNode = GetNode<Node>("CameraZones");

        foreach(var node in zonesNode.GetChildren()) {
            var cameraZone = node as CameraZone;

            // Set the CameraZone in the dictionary
            cameraZones[cameraZone.ID] = cameraZone;

            // Signals for entering/exiting the camera zone
            cameraZone.CameraZoneEntered += OnCameraZoneEntered;
            cameraZone.CameraZoneExited += OnCameraZoneExited;

            // Signals for updating the camera zone
            cameraZone.CameraZoneUpdate += ID => {
                if (ID == cameraZoneID) {
                    UpdateCamera(ID);
                }
            };

            // Also attach the signal for the game viewport changing size
            mainGame.gameViewport.SizeChanged += cameraZone.OnWindowResize;
        }

        // Now attach the signals from the other objects
        var objectsNode = GetNode<Node>("Objects");

        AttachSignalsForObjectsNode(objectsNode);
    }

    // Runs when a coin emits a CollectCoin signal
    private void OnAddScore(int value) {
        score += value;
        EmitSignal(SignalName.SetScore, score);
    }

    // Runs when an end portal emits a LevelEnd signal
    public void OnLevelEnd() {
        GD.Print("Finished level!");

        // Hide the player
        levelFinished = true;
        player.QueueFree();
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
        }
    }

    // Switches to the camera zone with the given ID
    private void SwitchToCameraZone(int ID) {
        // We need to update the background color to hide rooms the player is not in
        if (cameraZoneID is not null) { // The cameraID is set to -1 upon loading, so check if the key exists
            cameraZones[(int) cameraZoneID].SetBackgroundColor(new Color(0, 0, 0));
        }

        cameraZoneID = ID;

        // Update the background color of the new camera zone
        cameraZones[ID].SetBackgroundColor(new Color(0, 0, 0, 0.0f));

        // Update the camera
        UpdateCamera(ID);

        // Update background for the level itself
        background.Size = cameraZones[ID].background.Size;

        // Add the two positions together since the background object's position is relative to the camera zone
        background.Position = cameraZones[ID].background.Position + cameraZones[ID].Position;
    }

    // Updates the camera to have the position/zoom provided from the current CameraZone
    private void UpdateCamera(int ID) {
        var zone = cameraZones[ID];

        camera.Position = zone.GetCameraPosition();
        camera.Zoom = zone.GetCameraZoom();
    }

    // Runs when a portal is entered by the player
    private void OnPortalEntered(Vector2 target) {
        // If the player just teleported here then they are safe from further teleportation until they leave the portal
        if (!player.justTeleported) {
            player.justTeleported = true;

            // We need to update the GlobalPosition directly
            // It seems the regular Position is updated in the physics process
            // So updating that could cause the player to snap back here due to the physics process
            player.GlobalPosition = target * Constants.TileSize + Constants.TileVector / 2.0f;
        }
    }

    
    // Runs when a portal is exited by the player
    private void OnPortalExited() {
        player.justTeleported = false;
    }

    // Runs when a key is collected by the player
    private void OnKeyCollected(Color color) {
        // Add to the number of keys
        collectedKeys[color] = collectedKeys.GetValueOrDefault(color, 0) + 1;

        // Propogate the signal to any locks
        EmitSignal(SignalName.CollectKey, color);
    }
}