using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// The Player class represents the player, which can be moved around based on the user's inputs.
/// </summary>
public partial class Player : CharacterBody2D {
	/// <summary>
	/// Speed of the player in pixels per second.
	/// </summary>
	[Export]
	public int Speed { get; set; } = 32;

	// Are the player's invincibility frames currently active? This is set to false when teleporting or moving back from a checkpoint,
	// so that the portal doesn't immediately active
	private bool invincibilityFramesActive = false;

	/// <summary>
	/// Are the player's invincibility frames currently active?
	/// </summary>
	public bool InvincibilityFramesActive {
		get {
			return invincibilityFramesActive;
		}
	}

	// Reference to the invincibility timer
	private Timer invincibilityTimer;

	// Reference to the collision shape
	private CollisionShape2D collisionShape;

	// Did the player just die?
	private bool justDied = false;

	// Track the current velocities being applied to the player from each angle of a conveyor
	// To get the actual velocity, we calculate the maximum value from the set for each angle, then apply it and add them all together
	private Dictionary<Vector2, CounterDict<int>> activeConveyorVelocities;

	// Cache the current velocity to add from conveyors
	private Vector2 conveyorVelocity = Vector2.Zero;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		// Get reference to timer
		invincibilityTimer = GetNode<Timer>("InvincibilityTimer");
		
		// Get reference to the collision shape
		collisionShape = GetNode<CollisionShape2D>("CollisionShape");

		// Set up event listeners
		Events.Instance.ConveyorBeltEntered += OnConveyorBeltEntered;
		Events.Instance.ConveyorBeltExited += OnConveyorBeltExited;

		// Set up dictionaries
		activeConveyorVelocities = new Dictionary<Vector2, CounterDict<int>>();
	}

    public override void _ExitTree() {
        base._ExitTree();

		Events.Instance.ConveyorBeltEntered -= OnConveyorBeltEntered;
		Events.Instance.ConveyorBeltExited -= OnConveyorBeltExited;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {
		// Starting velocity
		var playerVelocity = Vector2.Zero;

		if (Input.IsActionPressed("move_left"))
			playerVelocity.X--;

		if (Input.IsActionPressed("move_right"))
			playerVelocity.X++;

		if (Input.IsActionPressed("move_up"))
			playerVelocity.Y--;

		if (Input.IsActionPressed("move_down"))
			playerVelocity.Y++;

		// Is the player moving?
		bool isMoving = playerVelocity.Length() > 0;

		if (isMoving) {
			playerVelocity = playerVelocity.Normalized();
		}

		// Other modifiers to player velocity
		var finalVelocity = (playerVelocity * Speed + conveyorVelocity) * (float) delta;

		if (finalVelocity.Length() > 0) {
			// To get the right velocity, it seems I need to multiply the final velocity by a constant
			// 48 seems to get the right result but idk why
			Velocity = finalVelocity * 48;
			MoveAndSlide();

			var numCollisions = GetSlideCollisionCount();

			for (int i = 0; i < numCollisions; i++) {
				var collisionTarget = GetSlideCollision(i).GetCollider();

				if (collisionTarget is Lava && !invincibilityFramesActive) {
					Events.Instance.EmitSignal(Events.SignalName.PlayerHit);
				}
			}
		}
	}

	// Returns the velocity gotten from conveyors
	// This function is designed to only be called after the player receives a signal that it entered/exited a conveyor
	// This minimizes the amount of calls
	private Vector2 GetConveyorVelocity() {
		return activeConveyorVelocities
			.Select(entry => entry.Key * entry.Value.GetMaxKey())
			.Aggregate(Vector2.Zero, (a, b) => a + b);
	}

    /// <summary>
	/// Disables the player's invincibility frames.
	/// </summary>
    public void DisableInvincibilityFrames() {
		if (!invincibilityTimer.IsStopped()) {
			invincibilityTimer.Stop();
		}

		invincibilityFramesActive = false;
	}

	/// <summary>
	/// Temporarily enables the player's invincibility frames.
	/// </summary>
	/// <param name="milliseconds">How many milliseconds the player should be invincible for</param>
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

	// Runs when a conveyor belt is entered
	private void OnConveyorBeltEntered(Vector2 unitVector, int speed) {
		if (!activeConveyorVelocities.ContainsKey(unitVector)) {
			activeConveyorVelocities[unitVector] = new CounterDict<int>();
		}

		activeConveyorVelocities[unitVector] += speed;

		conveyorVelocity = GetConveyorVelocity();
	}

	// Runs when a conveyor belt is exited
	private void OnConveyorBeltExited(Vector2 unitVector, int speed) {
		activeConveyorVelocities[unitVector] -= speed;

		// Remove the element since I don't think the GetMaxKey method will work well if there are no keys in existence
		if (activeConveyorVelocities[unitVector].NumKeys() == 0) {
			activeConveyorVelocities.Remove(unitVector);
		}

		conveyorVelocity = GetConveyorVelocity();
	}
}
