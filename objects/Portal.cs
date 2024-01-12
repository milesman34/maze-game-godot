using Godot;
using System;

/// <summary>
/// Portals let the player teleport to a new location upon hitting them.
/// </summary>
public partial class Portal : Area2D
{
	/// <summary>
	/// Color of the portal.
	/// </summary>
	[Export]
	public Color Color { get; set; } = new Color(255, 255, 255);

	/// <summary>
	/// Position in units that the portal teleports the player to.
	/// </summary>
	[Export]
	public Vector2 Target { get; set; }

	[ExportCategory("Start Position")]

	/// <summary>
	/// Should the node's transform position be overridden with the start position here?
	/// </summary>
	[Export]
	public bool OverrideStartPos { get; set; } = false;

	/// <summary>
	/// Starting position of the Portal in units. This is technically optional but is useful as an override to avoid having to figure out the location manually.
	/// </summary>
	[Export]
	public Vector2 StartPosition { get; set; } = new Vector2(0, 0);

	[Signal]
	public delegate void PortalEnteredEventHandler(Vector2 target);

	[Signal]
	public delegate void PortalExitedEventHandler();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var sprite = GetNode<Sprite2D>("Sprite");

		sprite.Modulate = Color;

		// If the portal is set to override position, then override it using the provided position
		if (OverrideStartPos) {
			Position = GetStartingPosition();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	/// <summary>
    /// Returns the starting position in pixels. The original position was based on units.
    /// </summary>
    /// <returns>Starting position in pixels</returns>
    public Vector2 GetStartingPosition() {
        return StartPosition * Constants.TileSize + new Vector2(Constants.TileSize, Constants.TileSize) / 2.0f; // Size of tile?
    }

	/// <summary>
	/// OnBodyEntered is called when the portal is entered by a body.
	/// </summary>
	/// <param name="body">Body that enters the portal.</param>
	public void OnBodyEntered(Node2D body) {
		if (body is Player) {
			EmitSignal(SignalName.PortalEntered, Target);
		}
	}

	public void OnBodyExited(Node2D body) {
		if (body is Player) {
			EmitSignal(SignalName.PortalExited);
		}
	}
}