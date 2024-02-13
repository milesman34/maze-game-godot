using Godot;
using System;

/// <summary>
/// This node contains several GlowingLavaTraps as children and sets up their activation delays based on their order.
/// </summary>
public partial class GlowingLavaTrapSequencer : Node2D {
	/// <summary>
	/// What should the base delay from the sequencer be?
	/// </summary>
	[Export]
	public float BaseDelay { get; set; } = 0f;

	/// <summary>
	/// How much should the next trap be delayed compared to the current one?
	/// </summary>
	[Export]
	public float DelayPerTrap { get; set; } = 0.2f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		int countTraps = 0;

		foreach (var node in GetChildren()) {
			if (node is GlowingLavaTrap) {
				var trap = node as GlowingLavaTrap;

				trap.ActivationTimeDelay += BaseDelay + DelayPerTrap * countTraps;
				countTraps++;
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {}
}
