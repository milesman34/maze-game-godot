using Godot;
using System;
using System.Collections.Generic;

public partial class LevelButtonsContainer : ScrollContainer
{
	// Information about the available levels
	[Export]
	public LevelInfo LevelInfo { get; set; }

	// Level Button scene type
	[Export]
	public PackedScene LevelButtonScene { get; set; }

	// Signal for switching to a level
	[Signal]
	public delegate void SwitchToLevelEventHandler(LevelResource resource);

	// Keep track of all the LevelButton elements
	public List<LevelButton> levelButtons;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		levelButtons = new List<LevelButton>();

		var mainContainer = GetNode<VBoxContainer>("MainContainer");

		foreach(var levelResource in LevelInfo.Levels) {
			var button = LevelButtonScene.Instantiate<LevelButton>();
			button.LevelResource = levelResource;
			button.LevelButtonPressed += level => EmitSignal(SignalName.SwitchToLevel, level);
			
			levelButtons.Add(button);
			mainContainer.AddChild(button);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
