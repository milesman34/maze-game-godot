using Godot;
using System;

/// <summary>
/// This tile can be placed on a level to set the starting position. If more than one starting position exists, it uses priority as a tiebreaker (higher = better).
/// </summary>
public partial class StartPosition : Node2D, IGameObject {
	/// <summary>
	/// The priority of this start position (higher = better).
	/// </summary>
	[Export]
	public int Priority = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {}

    public void AttachSignals(Level level) {
		level.RegisterStartPosition(this);
	}
}
