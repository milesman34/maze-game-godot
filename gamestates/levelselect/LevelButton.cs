using Godot;
using System;

public partial class LevelButton : Button
{
	[Export]
	public LevelResource LevelResource { get; set; }

	// Signal for when the button is pressed
	[Signal]
	public delegate void LevelButtonPressedEventHandler(LevelResource resource);

	// Reference to the level name label
	private Label nameLabel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		nameLabel = GetNode<Label>("%LevelName");
		nameLabel.Text = LevelResource.LevelName;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnMouseEntered() {
		nameLabel.AddThemeColorOverride("font_color", new Color(0, 0, 0));
	}

	private void OnMouseExited() {
		nameLabel.AddThemeColorOverride("font_color", new Color(255, 255, 255));
	}

	private void OnPressed() {
		EmitSignal(SignalName.LevelButtonPressed, LevelResource);
	}
}
