using Godot;
using System;

// This tile can be placed on a Level to set the starting position. If more than one have been placed, then it uses the priority as a tiebreaker
public partial class StartPosition : Node2D, IGameObject
{
	[Export]
	public int Priority = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public void AttachSignals(Level level) {
		level.RegisterStartPosition(this);
	}

}
