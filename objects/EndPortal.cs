using Godot;
using System;

public partial class EndPortal : Area2D
{
	// Signal that ends the level
	[Signal]
	public delegate void LevelEndEventHandler();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnBodyEntered(RigidBody2D body)
	{
		// Check if the body is the player
		if (body is Player) {
			EmitSignal(SignalName.LevelEnd);
		}
	}
}
