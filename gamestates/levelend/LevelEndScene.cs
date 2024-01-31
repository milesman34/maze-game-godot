using Godot;
using System;

public partial class LevelEndScene : Node2D, IGameState
{
	[Export]
	public int Score { get; set; } = 0;

	[Export]
	public int Deaths { get; set; } = 0;

	// Name of the current level
	[Export]
	public string LevelName { get; set; }

	// Reference to the level info
	[Export]
	public LevelInfo LevelInfo { get; set; }

	// References to the labels
	private Label scoreLabel;
	private Label deathsLabel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Update background color
		RenderingServer.SetDefaultClearColor(new Color(0, 0, 0));

		// Set the stretch mode to the viewport
		GetTree().Root.ContentScaleMode = Window.ContentScaleModeEnum.CanvasItems;

		// Set up references
		scoreLabel = GetNode<Label>("%ScoreLabel");
		deathsLabel = GetNode<Label>("%DeathsLabel");

		// Figure out the best score
		var saveManager = GetNode<SaveManager>("/root/SaveManager");

		// Set up the labels
		scoreLabel.Text = GetButtonText("Score", Score, saveManager.levelStats[LevelName].BestScore, saveManager.newBestScore);
		deathsLabel.Text = GetButtonText("Deaths", Deaths, saveManager.levelStats[LevelName].BestDeaths, saveManager.newBestDeaths);
	}

	// Returns the text to display for a button
	private string GetButtonText(string buttonText, int current, int best, bool newBest) {
		if (newBest) {
			return string.Format("{0}: {1} (New Best!)", buttonText, current);
		} else {
			return string.Format("{0}: {1} (Best: {2})", buttonText, current, best);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public void AttachSignals(Main main) {
    }

	private void OnReplayButtonPressed() {
		// Find the corresponding level if it exists
		foreach (var levelResource in LevelInfo.Levels) {
			if (levelResource.LevelName == LevelName) {
				Events.instance.EmitSignal(Events.SignalName.SwitchToLevel, levelResource);
				break;
			}
		}
	}

	private void OnExitButtonPressed() {
		// Switch back to the level select scene
		Events.instance.EmitSignal(Events.SignalName.ExitToLevelSelect);
	}
}
