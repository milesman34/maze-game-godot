using Godot;
using System;
using System.Collections.Generic;

// Firebars are a type of spinning obstacle which can have various rotating fireball obstacles around them.
public partial class Firebar : Node2D, IGameObject
{
	// Fireball scene to use
	[Export]
	public PackedScene FireballScene { get; set; }

	// Angular speed of the firebar's rotation (degrees per second) (positive is clockwise, negative is counterclockwise)
	[Export]
	public int RotationSpeed { get; set; } = 45;

	// Rotational offset of the firebar in degrees
	[Export]
	public int RotationOffset { get; set; } = 0;

	// Number of fireballs in the firebar
	[Export]
	public int FireballCount { get; set; } = 1;

	// Positional offset of the fireballs from the center (by default the first fireball is one unit from the center)
	[Export]
	public int FireballOffset { get; set; } = 1;

	// How much space each fireball should have (in pixels)
	[Export]
	public int FireballSize { get; set; } = 32;

	// Keep track of all the fireballs
	private List<RotatingFireball> fireballs;

	// Signal emitted when the player is hit by one of the fireballs
	[Signal]
	public delegate void PlayerHitEventHandler();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		fireballs = new List<RotatingFireball>();

		for (int offset = FireballOffset; offset < FireballOffset + FireballCount; offset++) {
			var fireball = RotatingFireball.CreateFireball(FireballScene, RotationSpeed, RotationOffset, Position, offset, FireballSize);
			AddChild(fireball);
			fireballs.Add(fireball);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public void AttachSignals(Level level)
    {
        foreach (var node in GetChildren()) {
			if (node is RotatingFireball) {
				(node as RotatingFireball).PlayerHit += () => EmitSignal(SignalName.PlayerHit);
			}
		}

		PlayerHit += level.OnPlayerHit;
    }

}
