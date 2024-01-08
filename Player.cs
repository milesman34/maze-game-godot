using Godot;
using System;

public partial class Player : RigidBody2D
{
	[Export]
	public int Speed { get; set; } = 32;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var velocity = Vector2.Zero;

		if (Input.IsActionPressed("move_left"))
			velocity.X--;

		if (Input.IsActionPressed("move_right"))
			velocity.X++;

		if (Input.IsActionPressed("move_up"))
			velocity.Y--;

		if (Input.IsActionPressed("move_down"))
			velocity.Y++;

		// Normalize velocity vector
		if (velocity.Length() > 0) {
			velocity = velocity.Normalized();

			MoveAndCollide(velocity * Speed * (float) delta);
		}
	}
}
