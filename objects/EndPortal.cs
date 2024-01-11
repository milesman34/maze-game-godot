using Godot;
using System;

/// <summary>
/// EndPortals mark the end of a level.
/// </summary>
public partial class EndPortal : Area2D
{
	/// <summary>
	/// EndPortals emit this signal when touched, to end the level.
	/// </summary>
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

	/// <summary>
	/// This function runs when another RigidBody2D enters the area. It ends the level if the body in question is the Player.
	/// </summary>
	/// <param name="body">RigidBody2D that entered the area</param>
	private void OnBodyEntered(RigidBody2D body)
	{
		// Check if the body is the player
		if (body is Player) {
			EmitSignal(SignalName.LevelEnd);
		}
	}
}
