using Godot;
using System;

/// <summary>
/// This is a special type of fireball which rotates around a central pivot
/// </summary>
[Tool]
public partial class RotatingFireball : Fireball
{
	// Pivot position for rotation
	private Vector2 pivot;

	// How much (in units) is the fireball offset from the center?
	private int centerOffset;

	// What is the starting angle in degrees?
	private int startingAngle;

	// What is the rotational speed in degrees?
	private int rotationSpeed;

	// Size of the fireball in pixels
	private int size;

	// Track the total elapsed time in seconds
	private double elapsedTime;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		Position = new Vector2(size * centerOffset, 0);

		// Set up the size of the fireball
		float scaleFactor = ((float) size) / Constants.TileSize * 0.8f;
		Scale = new Vector2(scaleFactor, scaleFactor);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		elapsedTime += delta;

		// First set the sprite rotation
		var rotation = GetRotation();

		RotationDegrees = rotation;

		// Now set the actual position
		var offset = centerOffset * size;

		Position = new Vector2(
            (float) (offset * Math.Cos(rotation * Math.PI / 180)),
            (float) (offset * Math.Sin(rotation * Math.PI / 180))
        );
	}

	/// <summary>
	/// Creates a fireball object using the given parameters.
	/// </summary>
	/// <param name="fireballScene">Scene used to instantiate the fireball</param>
	/// <param name="rotationSpeed">Rotational speed of the fireball in degrees per second</param>
	/// <param name="rotationOffset">How much should the rotation be offset?</param>
	/// <param name="pivot">What point is the fireball rotating around?</param>
	/// <param name="centerOffset">How much should this fireball be offset from the central pivot?</param>
	/// <param name="size">Size of the fireball</param>
	/// <returns></returns>
	public static RotatingFireball InstantiateFireball(PackedScene fireballScene, int rotationSpeed, int rotationOffset, Vector2 pivot, int centerOffset, int size) {
		var fireball = fireballScene.Instantiate<RotatingFireball>();
		fireball.pivot = pivot;
		fireball.centerOffset = centerOffset;
		fireball.startingAngle = rotationOffset;
		fireball.rotationSpeed = rotationSpeed;
		fireball.size = size;

		return fireball;
	}

	// Returns the current rotation
	private float GetRotation() {
		return (float) (startingAngle + rotationSpeed * elapsedTime);
	}
}
