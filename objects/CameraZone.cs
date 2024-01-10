using Godot;
using System;

public partial class CameraZone : Area2D
{
	// Unique ID for each CameraZone
	static int currentID;
	public int ID;

	// Position of the camera zone (in units)
	[Export]
	public Vector2 StartPosition {get; set; }

	// Size of the camera zone (in units)
	[Export]
	public Vector2 Size { get; set; }

	// Signal to send to the level when the camera zone is entered
	[Signal]
	public delegate void CameraZoneEnteredEventHandler(int ID);

	// Signal to send to the level when the camera zone is exited
	[Signal]
	public delegate void CameraZoneExitedEventHandler(int ID);

	// Signal to get the level to update its camera if the ID matches
	[Signal]
	public delegate void CameraZoneUpdateEventHandler(int ID);

	// Getter for the background
	public ColorRect BackgroundObject { get { return GetNode<ColorRect>("Background"); }}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Set ID of this CameraZone
		ID = currentID;
		currentID++;

		// Set position
		Position = StartPosition * Constants.UnitSize + Size * Constants.UnitSize / 2.0f;

        // Set size of collision shape
        var rectSize = new RectangleShape2D
        {
            Size = Size * Constants.UnitSize
        };

        GetNode<CollisionShape2D>("CollisionShape").Shape = rectSize;
		
		// Set background position/scale
		CallDeferred(MethodName.UpdateBackgroundSizeAndPosition);
	}

	// Updates the background's size and position
	// This function exists to allow it to be deferred
	private void UpdateBackgroundSizeAndPosition() {
		var background = GetNode<ColorRect>("Background");

		// Seems we just needed to have it relative, based on the background size
		background.Size = Size * Constants.UnitSize;
		background.Position = -background.Size / 2.0f;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    // Calculate the effective zoom level (factoring in viewport size)
    private float GetEffectiveZoom() {
		var viewportSize = GetViewportRect().Size;

        // Get scale factors for both x and y
        var xScale = viewportSize.X / Size.X;
        var yScale = viewportSize.Y / Size.Y;

        return Math.Min(xScale, yScale);
    }

	// Returns the position of the camera
	public Vector2 GetCameraPosition() {
		// To calculate the position, we want the camera to be in the center of the room
		var myPosition = StartPosition * Constants.UnitSize;
		var offsetPosition = myPosition + Size * Constants.UnitSize / 2.0f;

		return offsetPosition;
	}

	// Returns the zoom level of the camera
	public Vector2 GetCameraZoom() {
        var zoom = GetEffectiveZoom();

        return new Vector2(zoom, zoom) / Constants.UnitSize;
	}

    // Runs whenever the level resizes
    public void OnWindowResize() {
        EmitSignal(SignalName.CameraZoneUpdate, ID);
    }

	// Runs when the camera zone is entered
	public void OnBodyEntered(Node2D body) {
		EmitSignal(SignalName.CameraZoneEntered, ID);
	}

	// Runs when the camera zone is exited
	public void OnBodyExited(Node2D body) {
		EmitSignal(SignalName.CameraZoneExited, ID);
	}

	// Update the background color
	public void SetBackgroundColor(Color color) {
		GetNode<ColorRect>("Background").Color = color;
	}
}
