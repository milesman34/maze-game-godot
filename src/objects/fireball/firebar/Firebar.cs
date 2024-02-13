using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Firebars are an obstacle which contain several fireballs spinning around itself.
/// </summary>
[Tool]
public partial class Firebar : Node2D, IGameObject {
	/// <summary>
	/// Fireball scene to create.
	/// </summary>
	[Export]
	public PackedScene FireballScene { get; set; }

	/// <summary>
	/// Angular speed of the firebar's rotation in degrees per second.
	/// </summary>
	[Export]
	public int RotationSpeed { get; set; } = 45;

	/// <summary>
	/// Rotational offset of the firebar in degrees.
	/// </summary>
	[Export]
	public int RotationOffset { get; set; } = 0;

	/// <summary>
	/// The number of fireballs in the firebar.
	/// </summary>
	[Export]
	public int FireballCount { get; set; } = 1;

	/// <summary>
	/// How much should the first fireball be offset from the center? (1 makes it adjacent to the center)
	/// </summary>
	[Export]
	public int FireballOffset { get; set; } = 1;

	/// <summary>
	/// How large should each fireball be (in pixels)?
	/// </summary>
	[Export]
	public int FireballSize { get; set; } = 32;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		for (int offset = FireballOffset; offset < FireballOffset + FireballCount; offset++) {
			var fireball = RotatingFireball.InstantiateFireball(FireballScene, RotationSpeed, RotationOffset, Position, offset, FireballSize);
			AddChild(fireball);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {}

    public void AttachSignals(Level level) {}
}
