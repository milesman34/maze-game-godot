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

	// Are the player's invincibility frames currently active? This is set to false when teleporting or moving back from a checkpoint,
	// so that the portal doesn't immediately active
	public bool invincibilityFramesActive = false;

	// Reference to the invincibility timer
	private Timer invincibilityTimer;

	// Reference to the collision shape
	public CollisionShape2D collisionShape;

	// Signal emitted when the player is hit
	[Signal]
	public delegate void PlayerHitEventHandler();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Get reference to timer
		invincibilityTimer = GetNode<Timer>("InvincibilityTimer");
		
		// Get reference to the collision shape
		collisionShape = GetNode<CollisionShape2D>("CollisionShape");
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
			var collision = MoveAndCollide(velocity * Speed * (float) delta);

			if (collision != null) {
				var collisionTarget = collision.GetCollider();

				if (collisionTarget is Lava && !invincibilityFramesActive) {
					EmitSignal(SignalName.PlayerHit);
				}
			}
		}
	}

	// Disables the player's invincibility frames
	public void DisableInvincibilityFrames() {
		if (!invincibilityTimer.IsStopped()) {
			invincibilityTimer.Stop();
		}

		invincibilityFramesActive = false;
	}

	// Enables the player's invincibility frames temporarily
	public void EnableInvincibilityFrames(float milliseconds = 100) {
		// Stop the timer if it is already running first
		if (!invincibilityTimer.IsStopped()) {
			invincibilityTimer.Stop();
		}

		invincibilityFramesActive = true;
		invincibilityTimer.Start(milliseconds / 1000);
	}

	private void OnInvincibilityTimerTimeout() {
		invincibilityFramesActive = false;
	}
}
