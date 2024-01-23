using Godot;
using System;

public partial class LevelButtonsContainer : ScrollContainer
{
	// Information about the available levels
	[Export]
	public LevelInfo LevelInfo { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		foreach(var levelResource in LevelInfo.Levels) {
			GD.Print(levelResource.LevelName);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
