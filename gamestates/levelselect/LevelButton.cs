using Godot;
using System;

/// <summary>
/// LevelButton is a clickable button which takes the player to the given level.
/// </summary>
public partial class LevelButton : Button {
	/// <summary>
	/// Resource for the level this button links to.
	/// </summary>
	[Export]
	public LevelResource LevelResource { get; set; }

	/// <summary>
	/// Signal for when the button is pressed. Provide the LevelResource representing the level to switch to.
	/// </summary>
	/// <param name="resource">LevelResource representing the level</param>
	[Signal]
	public delegate void LevelButtonPressedEventHandler(LevelResource resource);

	// Reference to the level name label
	private Label nameLabel;

	// References to the best score/deaths labels
	private Label bestScoreLabel;
	private Label bestDeathsLabel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		nameLabel = GetNode<Label>("%LevelName");
		nameLabel.Text = LevelResource.LevelName;

		bestScoreLabel = GetNode<Label>("%BestScore");
		bestDeathsLabel = GetNode<Label>("%BestDeaths");

		var saveManager = GetNode<SaveManager>("/root/SaveManager");

		if (saveManager.HasSaveForLevel(LevelResource.LevelName)) {
			var stats = saveManager.GetStatsForLevel(LevelResource.LevelName);

			bestScoreLabel.Text = string.Format("Best Score: {0}", stats.BestScore);
			bestDeathsLabel.Text = string.Format("Fewest Deaths: {0}", stats.BestDeaths);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {}

	/// <summary>
	/// Creates an instance of the LevelButton scene.
	/// </summary>
	/// <param name="buttonScene">PackedScene used to create the button, should be LevelButton.tscn</param>
	/// <param name="resource">LevelResource for this button</param>
	/// <param name="container">Reference to the LevelButtonsContainer</param>
	/// <returns></returns>
	public static LevelButton InstantiateLevelButton(PackedScene buttonScene, LevelResource resource, LevelButtonsContainer container) {
		var button = buttonScene.Instantiate<LevelButton>();
		button.LevelResource = resource;
		button.LevelButtonPressed += level => container.EmitSignal(SignalName.LevelButtonPressed, level);

		return button;
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
