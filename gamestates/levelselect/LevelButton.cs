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

	// References to the best score/deaths labels
	private Label bestScoreLabel;
	private Label bestDeathsLabel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		nameLabel = GetNode<Label>("%LevelName");
		nameLabel.Text = LevelResource.LevelName;

		bestScoreLabel = GetNode<Label>("%BestScore");
		bestDeathsLabel = GetNode<Label>("%BestDeaths");

		var saveManager = GetNode<SaveManager>("/root/SaveManager");

		if (saveManager.levelStats.ContainsKey(LevelResource.LevelName)) {
			var stats = saveManager.levelStats[LevelResource.LevelName];

			bestScoreLabel.Text = string.Format("Best Score: {0}", stats.BestScore);
			bestDeathsLabel.Text = string.Format("Fewest Deaths: {0}", stats.BestDeaths);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnMouseEntered() {
		foreach (var label in new Label[] {nameLabel, bestScoreLabel, bestDeathsLabel}) {
			label.AddThemeColorOverride("font_color", new Color(0, 0, 0));
		}
	}

	private void OnMouseExited() {
		foreach (var label in new Label[] {nameLabel, bestScoreLabel, bestDeathsLabel}) {
			label.AddThemeColorOverride("font_color", new Color(255, 255, 255));
		}
	}

	private void OnPressed() {
		EmitSignal(SignalName.LevelButtonPressed, LevelResource);
	}
}
