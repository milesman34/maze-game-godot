using Godot;
using System;

/// <summary>
/// CameraZones manage a section of the camera, creating the appearance of a room in the level.
/// </summary>
public partial class CameraZone : Area2D
{
	// Current unique ID to be given to a camera zone
	private static int currentID;

	// ID of this camera zone
	public int ID;

	// Position of the camera zone in units
	[Export]
	public Vector2 StartPosition {get; set; }

	// Size of the camera zone in units
	[Export]
	public Vector2 Size { get; set; }

	// Signal for when the player enters a camera zone
	[Signal]
	public delegate void CameraZoneEnteredEventHandler(int ID);

	// Signal for when the player exits a camera zone
	[Signal]
	public delegate void CameraZoneExitedEventHandler(int ID);

	// Signal for when the viewport size is updated
	[Signal]
	public delegate void CameraZoneUpdateEventHandler(int ID);

	// Reference to the camera zone's background
	public ColorRect background;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Get references to certain objects
		background = GetNode<ColorRect>("Background");

		// Set ID of this CameraZone
		ID = currentID;
		currentID++;

		// Set the current position
		Position = StartPosition * Constants.TileSize + Size * Constants.TileSize / 2.0f;

        // Set the size of the collision shape
        var rectSize = new RectangleShape2D
        {
            Size = Size * Constants.TileSize
        };

        GetNode<CollisionShape2D>("CollisionShape").Shape = rectSize;
		
		// Set background position/scale
		CallDeferred(MethodName.UpdateBackgroundSizeAndPosition);
	}

	// Updates the background's size and position. This is separated into a new function so that it can be deferred.
	private void UpdateBackgroundSizeAndPosition() {
		// Seems we just needed to have it relative, based on the background size
		background.Size = Size * Constants.TileSize;
		background.Position = -background.Size / 2.0f;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// Calculates the effective zoom level, based on the viewport size. The zoom level is the same for both axes.
    private float GetEffectiveZoom() {
		var viewportSize = GetViewportRect().Size;

        // Get scale factors for both x and y
        var xScale = viewportSize.X / Size.X;
        var yScale = viewportSize.Y / Size.Y;

        return Math.Min(xScale, yScale) / Constants.TileSize;
    }

	/// <summary>
	/// Calculates the position for the camera to be in, which the Level needs to know.
	/// </summary>
	/// <returns>Camera position</returns>
	public Vector2 GetCameraPosition() {
		// To calculate the position, we want the camera to be in the center of the room
		var myPosition = StartPosition * Constants.TileSize;
		var offsetPosition = myPosition + Size * Constants.TileSize / 2.0f;

		return offsetPosition;
	}

	/// <summary>
	/// Returns the zoom level of the camera as a vector.
	/// </summary>
	/// <returns>Camera zoom level</returns>
	public Vector2 GetCameraZoom() {
        var zoom = GetEffectiveZoom();

        return new Vector2(zoom, zoom);
	}

	// Runs whenever the window resizes
    public void OnWindowResize() {
        EmitSignal(SignalName.CameraZoneUpdate, ID);
    }

	// Runs whenever another body enters the camera zone
	private void OnBodyEntered(Node2D body) {
		if (body is Player) {
			EmitSignal(SignalName.CameraZoneEntered, ID);
		}
	}

	// Runs whenever another body exits the camera zone
	private void OnBodyExited(Node2D body) {
		if (body is Player) {
			EmitSignal(SignalName.CameraZoneExited, ID);
		}
	}

	// Sets the background color
	public void SetBackgroundColor(Color color) {
		background.Color = color;
	}
}
