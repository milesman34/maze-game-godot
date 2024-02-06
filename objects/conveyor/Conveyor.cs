using Godot;
using System;

/// <summary>
/// Conveyors move the player forwards when they touch them.
/// </summary>
[Tool]
public partial class Conveyor : Area2D, IGameObject {
	/// <summary>
	/// How much should the player's speed be affected by the conveyor (pixels per second)?
	/// </summary>
	[Export]
	public int Speed { get; set; } = 8;

	/// <summary>
	/// How much should the conveyor be rotated by?
	/// </summary>
	[Export]
	public int ConveyorRotation { get; set; } = 0;

	// Pre-calculated unit vector for velocity
	private Vector2 unitVector;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		GetNode<Sprite2D>("OverlaySprite").RotationDegrees = ConveyorRotation;

		// Calculates the unit vector
		unitVector = new Vector2((float) Math.Cos(ConveyorRotation * Math.PI / 180), (float) Math.Sin(ConveyorRotation * Math.PI / 180)).Normalized();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {}

    public void AttachSignals(Level level) {}

	private void OnBodyEntered(PhysicsBody2D body) {
		if (body is Player) {
			Events.Instance.EmitSignal(Events.SignalName.ConveyorBeltEntered, unitVector, Speed);
		}
	}

	private void OnBodyExited(PhysicsBody2D body) {
		if (body is Player) {
			Events.Instance.EmitSignal(Events.SignalName.ConveyorBeltExited, unitVector, Speed);
		}
	}
}
