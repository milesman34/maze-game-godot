using Godot;
using System;

/// <summary>
/// EndPortals mark the end of a level.
/// </summary>
public partial class EndPortal : Area2D, IGameObject
{
	// Level end signal
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

	// Runs when another body enters the portal
	private void OnBodyEntered(RigidBody2D body)
	{
		// Check if the body is the player
		if (body is Player) {
			EmitSignal(SignalName.LevelEnd);
		}
	}

	public void AttachSignals(Level level) {
		LevelEnd += level.OnLevelEnd;
	}
}
