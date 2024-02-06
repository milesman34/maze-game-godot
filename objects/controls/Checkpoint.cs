using Godot;
using System;

/// <summary>
/// Checkpoints save the player's checkpoint when reached.
/// </summary>
public partial class Checkpoint : Area2D, IGameObject {
	/// <summary>
	/// This signal is emitted when a checkpoint is reached.
	/// </summary>
	[Signal]
	public delegate void ReachedCheckpointEventHandler();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {}

	public void AttachSignals(Level level) {
		ReachedCheckpoint += level.OnReachedCheckpoint;
	}

	private void OnBodyEntered(PhysicsBody2D body) {
		if (body is Player) {
			EmitSignal(SignalName.ReachedCheckpoint);
		}
	}
}
