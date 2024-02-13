using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// GUI element used to display all of the LevelButtons.
/// </summary>
public partial class LevelButtonsContainer : ScrollContainer {
	/// <summary>
	/// Reference to the LevelInfo resource which contains all information about levels.
	/// </summary>
	[Export]
	public LevelInfo LevelInfo { get; set; }

	/// <summary>
	/// PackedScene used to instantiate a LevelButton.
	/// </summary>
	[Export]
	public PackedScene LevelButtonScene { get; set; }

	/// <summary>
	/// Signal sent when a level button is pressed, sending the resource
	/// </summary>
	/// <param name="resource">LevelResource representing the level</param>
	[Signal]
	public delegate void LevelButtonPressedEventHandler(LevelResource resource);

	// Keep track of all the LevelButton elements
	// This isn't used yet, but maybe one day I will want to implement the ability to sort levels.
	private List<LevelButton> levelButtons;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		levelButtons = new List<LevelButton>();

		var mainContainer = GetNode<VBoxContainer>("MainContainer");

		foreach(var levelResource in LevelInfo.Levels) {
			var button = LevelButton.InstantiateLevelButton(LevelButtonScene, levelResource, this);
			
			levelButtons.Add(button);
			mainContainer.AddChild(button);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {}
}
