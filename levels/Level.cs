    using Godot;
using System;
using System.Threading;

public partial class Level : Node2D
{
    // Start position is unscaled, so based off tile indexes
    [Export]
    private Vector2 StartPosition {get; set; } = new Vector2(1, 1);

    // Track the current score
    public int score = 0;
    
    // Track instances of some nodes
    private Camera2D camera;

    public override void _Ready()
    {
        // Update position of player
		Player player = GetNode<Player>("Player");

		player.Position = GetStartingPosition();

		// Update camera zoom
		camera = GetNode<Camera2D>("Camera");

		UpdateCameraPositionAndZoom();

        // The TileMap won't have all the elements as children immediately, so the call needs to be deferred
        CallDeferred(MethodName.AttachSignals);

        // Connect our on screen resize function
        GetTree().Root.SizeChanged += OnWindowResize; 
    }

    // Calculate the effective zoom level (factoring in viewport size)
    private float GetEffectiveZoom(Vector2 viewportSize) {
        // Ok to calculate this we want to just base it off whatever the size is
        var tileMapSize = GetNode<TileMap>("TileMap").GetUsedRect().Size * Constants.UnitSize;

        // Get scale factors for both x and y
        var xScale = viewportSize.X / tileMapSize.X;
        var yScale = viewportSize.Y / tileMapSize.Y;

        return Math.Min(xScale, yScale);
    }

    // Updates the position and zoom level of the camera
    private void UpdateCameraPositionAndZoom() {
        var viewportSize = GetViewportRect().Size;

        var zoom = GetEffectiveZoom(viewportSize);

        camera.Zoom = new Vector2(zoom, zoom);

        // Division is needed so that the camera is centered on the actual area
        camera.Position = new Vector2(viewportSize.X / 2.0f / zoom, viewportSize.Y / 2.0f / zoom);
    }

    // Gets the starting position
    public Vector2 GetStartingPosition() {
        return StartPosition * Constants.UnitSize + new Vector2(Constants.UnitSize, Constants.UnitSize) / 2.0f; // Size of tile?
    }

    // Attaches signals to key elements in the tilemap
    public void AttachSignals() {
        var tileMap = GetNode<TileMap>("TileMap");

        foreach (var node in tileMap.GetChildren()) {
            if (node is Coin) {
                (node as Coin).CollectCoin += OnCollectCoin;
            } else if (node is EndPortal) {
                (node as EndPortal).LevelEnd += OnLevelEnd;
            }
        }
    }

    // Runs when a coin is collected
    public void OnCollectCoin(int value) {
        score += value;
        GD.Print(score);
    }

    // Runs when the level ends
    public void OnLevelEnd() {
        GD.Print("Finished level!");

        // Hide the player
        GetNode<Player>("Player").QueueFree();
    }

    // Runs whenever the level resizes
    public void OnWindowResize() {
        UpdateCameraPositionAndZoom();
    }
}