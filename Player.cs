using Godot;
using System;

/// <summary>
/// The Player class represents the player, which can be moved around based on the user's inputs.
/// </summary>
public partial class Player : RigidBody2D
{
	// Current speed of the player in pixels per second
	[Export]
	public int Speed { get; set; } = 32;

	// Did the player just teleport? This is used to prevent teleport loops with portals.
	public bool justTeleported = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Starting velocity
		var velocity = Vector2.Zero;

		if (Input.IsActionPressed("move_left"))
			velocity.X--;

		if (Input.IsActionPressed("move_right"))
			velocity.X++;

		if (Input.IsActionPressed("move_up"))
			velocity.Y--;

		if (Input.IsActionPressed("move_down"))
			velocity.Y++;

		// Normalize velocity vector, so that they can't go faster by moving diagonally
		if (velocity.Length() > 0) {
			velocity = velocity.Normalized();

			// Call move and collide to ensure proper collision with walls/other obstacles
			MoveAndCollide(velocity * Speed * (float) delta);
		}
	}
}
