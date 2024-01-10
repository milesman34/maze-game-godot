using Godot;
using System;
using System.Collections.Generic;
using System.Threading;

public partial class Level : Node2D
{
    // Starting position
    [Export]
    private Vector2 StartPosition { get; set; }

    // Track the current score
    public int score = 0;

    // Reference to current camera
    private Camera2D camera;

    // Map camera zone IDs to camera zones
    private Dictionary<int, CameraZone> cameraZones;

    // Track the current camera ID
    private int cameraID = -1;

    // Since there is a bug if you don't 100% leave a room, it'd be best to track the room you came from
    private int previousCameraZone = -1;

    // Is the level finished?
    private bool levelFinished = false;

    // Stored reference to the main game (Main)
    public Main mainGame { get; set; }

    public override void _Ready()
    {
        // Instantiate camera zones dict
        cameraZones = new Dictionary<int, CameraZone>();

        // The TileMap won't have all the elements as children immediately, so the call needs to be deferred
        CallDeferred(MethodName.AttachSignals);

        // Set camera reference
        camera = GetNode<Camera2D>("Camera");

        // Update position of player
		Player player = GetNode<Player>("Player");

		player.Position = GetStartingPosition();
    }

    // Gets the starting position (based on actual pixels, since the one we provide is based on tiles)
    public Vector2 GetStartingPosition() {
        return StartPosition * Constants.UnitSize + new Vector2(Constants.UnitSize, Constants.UnitSize) / 2.0f; // Size of tile?
    }

    // Attaches signals to key elements in the tilemap
    public void AttachSignals() {
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
            cameraZones[cameraZone.ID] = cameraZone;
            cameraZone.CameraZoneEntered += OnCameraZoneEntered;
            cameraZone.CameraZoneExited += OnCameraZoneExited;

            cameraZone.CameraZoneUpdate += ID => {
                if (ID == cameraID) {
                    UpdateCamera(ID);
                }
            };

            // Also attach the signal for the game viewport changing size
            mainGame.viewport.SizeChanged += cameraZone.OnWindowResize;
        }
    }

    // Adds to the score
    private void OnAddScore(int value) {
        score += value;
        // GD.Print(score);
    }

    // Runs when the level ends
    public void OnLevelEnd() {
        GD.Print("Finished level!");

        // Hide the player
        levelFinished = true;
        GetNode<Player>("Player").QueueFree();
    }

    // Runs when the player enters a new camera zone
    private void OnCameraZoneEntered(int ID) {
        previousCameraZone = cameraID;
        SwitchToCameraZone(ID);
    }

    // Runs when the player exits a camera zone
    private void OnCameraZoneExited(int ID) {
        // Check if the room we are exiting is the previous one we were in
        // If it is, then we need to switch back to that room as we are no longer in the current room
        // This switch is done manually, as we never fully left the collision area for the original room
        if (ID == cameraID && previousCameraZone >= 0 && !levelFinished) {
            SwitchToCameraZone(previousCameraZone);
        }
    }

    // Switches to the camera zone with the given ID
    private void SwitchToCameraZone(int ID) {
        // We need to update the background color to hide rooms the player is not in
        if (cameraZones.ContainsKey(cameraID)) { // The cameraID is set to -1 upon loading, so check if the key exists
            cameraZones[cameraID].SetBackgroundColor(new Color(0, 0, 0));
        }

        cameraID = ID;

        cameraZones[ID].SetBackgroundColor(new Color(0, 0, 0, 0.0f));

        UpdateCamera(ID);

        // Update background for the level itself
        var background = GetNode<ColorRect>("Background");

        background.Size = cameraZones[ID].BackgroundObject.Size;

        // Add the two positions together since the background object's position is relative to the camera zone
        background.Position = cameraZones[ID].BackgroundObject.Position + cameraZones[ID].Position;
    }

    // Updates the camera to have the position/zoom of the one with the current ID
    private void UpdateCamera(int ID) {
        var zone = cameraZones[ID];

        camera.Position = zone.GetCameraPosition();
        camera.Zoom = zone.GetCameraZoom();
    }
}