using Godot;
using System;

/// <summary>
/// Keys help you unlock various Locks in the level.
/// </summary>
public partial class Key : Area2D
{
	[Export]
	public Color Color { get; set; }

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

	// Signal to emit when the key is collected
	[Signal]
	public delegate void CollectKeyEventHandler(Color color);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<Sprite2D>("Sprite").Modulate = Color;

		// If the portal is set to override position, then override it using the provided position
		Position = GetStartingPosition();
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
        if (OverrideStartPos) {
			return StartPosition * Constants.TileSize + new Vector2(Constants.TileSize, Constants.TileSize) / 2.0f; // Size of tile?
		} else {
			return Position;
		}
    }

	/// <summary>
	/// OnBodyEntered runs when a body enters the object. If that body is the player, then it hides the key and emits a signal.
	/// </summary>
	/// <param name="body"></param>
	public void OnBodyEntered(RigidBody2D body) {
		if (body is Player) {
			// Get rid of this key object
			Hide();

			GetNode<CollisionShape2D>("CollisionShape").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);

			// Emit the signal
			EmitSignal(SignalName.CollectKey, Color);
		}
	}
}
