using Godot;
using System;

public partial class Checkpoint : Area2D
{
	[Signal]
	public delegate void ReachedCheckpointEventHandler();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnBodyEntered(RigidBody2D body) {
		if (body is Player) {
			EmitSignal(SignalName.ReachedCheckpoint);
		}
	}
}