using Godot;
using System;

/// <summary>
/// Wall that only blocks against one direction.
/// </summary>
[Tool]
public partial class OneWayWall : RigidBody2D {
	/// <summary>
	/// Direction the wall is rotated
	/// </summary>
	[Export]
	public Direction Direction { get; set; } = Direction.Up;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		// To make the one-way collisions work, I had to make the default rotation be up, so now I have to add 90 to this.
		RotationDegrees = Direction.GetRotation() + 90;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {}
}
