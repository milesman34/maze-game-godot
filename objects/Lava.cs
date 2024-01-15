using Godot;
using System;

public partial class Lava : RigidBody2D
{
	// Signal for the player touching the lava
	[Signal]
	public delegate void HitPlayerEventHandler();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
