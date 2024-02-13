using Godot;
using System;

/// <summary>
/// Wall that only blocks against one direction.
/// </summary>
[Tool]
public partial class OneWayWall : RigidBody2D {
	[Export]
	public Direction Direction { get; set; } = Direction.Right;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		RotationDegrees = Direction.GetRotation();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {}
}
