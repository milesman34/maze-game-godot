    using Godot;
using System;

public partial class Level : Node2D
{
    // Start position is unscaled, so based off tile indexes
    [Export]
    private Vector2 StartPosition {get; set; } = new Vector2(1, 1);

    // How much the level should be zoomed in
    [Export]
    public float LevelZoom { get; set; } = 1;

    // Track the current score
    public int score = 0;

    public override void _Ready()
    {
        // Update position of player
		Player player = GetNode<Player>("Player");

		player.Position = GetStartingPosition();

		// Update camera zoom
		Camera2D camera = GetNode<Camera2D>("Camera");

		camera.Zoom = new Vector2(LevelZoom, LevelZoom);
		camera.Position /= new Vector2(LevelZoom, LevelZoom); // Division is needed so that the camera is centered on the actual area

        // The TileMap won't have all the elements as children immediately, so the call needs to be deferred
        CallDeferred(MethodName.AttachSignals);
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
}