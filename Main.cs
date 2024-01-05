using Godot;
using System;

public partial class Main : Node2D
{
	// Scene for the Level
	[Export]
	public PackedScene LevelScene { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Update background color
		RenderingServer.SetDefaultClearColor(new Color(0, 0, 0));

		// Instantiate the level
		var level = LevelScene.Instantiate<Level>();

		AddChild(level);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
}
