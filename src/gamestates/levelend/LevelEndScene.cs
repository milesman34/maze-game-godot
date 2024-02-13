using Godot;
using System;

/// <summary>
/// GameState for the level end screen
/// </summary>
public partial class LevelEndScene : Node2D, IGameState {
	/// <summary>
	/// The score the player attained in the level.
	/// </summary>
	[Export]
	public int Score { get; set; } = 0;

	/// <summary>
	/// The number of times the player died in the level.
	/// </summary>
	[Export]
	public int Deaths { get; set; } = 0;

	/// <summary>
	/// The name of the current level.
	/// </summary>
	[Export]
	public string LevelName { get; set; }

	/// <summary>
	/// Reference to the LevelInfo object, containing a list of all of the LevelResources
	/// </summary>
	[Export]
	public LevelInfo LevelInfo { get; set; }

	// References to the labels
	private Label scoreLabel;
	private Label deathsLabel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
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
		scoreLabel.Text = GetButtonText("Score", Score, saveManager.GetBestScore(LevelName), saveManager.NewBestScore);
		deathsLabel.Text = GetButtonText("Deaths", Deaths, saveManager.GetFewestDeaths(LevelName), saveManager.NewBestDeaths);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {}

    public void AttachSignals(Main main) {}

	// Returns the text to display for a button
	private string GetButtonText(string buttonText, int current, int best, bool newBest) {
		if (newBest) {
			return string.Format("{0}: {1} (New Best!)", buttonText, current);
		} else {
			return string.Format("{0}: {1} (Best: {2})", buttonText, current, best);
		}
	}

	private void OnReplayButtonPressed() {
		// Find the corresponding level if it exists
		foreach (var levelResource in LevelInfo.Levels) {
			if (levelResource.LevelName == LevelName) {
				Events.Instance.EmitSignal(Events.SignalName.SwitchToLevel, levelResource);
				break;
			}
		}
	}

	private void OnExitButtonPressed() {
		// Switch back to the level select scene
		Events.Instance.EmitSignal(Events.SignalName.ExitToLevelSelect);
	}
}
