using Godot;
using System;

/// <summary>
/// EndPortals end the level when the player reaches them.
/// </summary>
public partial class EndPortal : Area2D, IGameObject {
	/// <summary>
	/// Signal emitted when the player reaches the end portal.
	/// </summary>
	[Signal]
	public delegate void EndPortalEnteredEventHandler();

	// Speed that the portal rotates at (pixels per second)
	private const double RotationSpeed = 30;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		RotationDegrees += (float) (RotationSpeed * delta);
	}

	public void AttachSignals(Level level) {
		EndPortalEntered += level.OnLevelEnd;
	}

	// Runs when another body enters the portal
	private void OnBodyEntered(PhysicsBody2D body) {
		// Check if the body is the player
		if (body is Player) {
			EmitSignal(SignalName.EndPortalEntered);
		}
	}
}
