using Godot;
using System;

/// <summary>
/// GameGUI object is responsible for maintaining the ingame GUI.
/// </summary>
public partial class GameGUI : CanvasLayer {
	/// <summary>
	/// Constant representing the height of the header in pixels (default is 64)
	/// </summary>
	public const int HeaderHeight = 64;

	// Reference to the score label object, set during _Ready()
	private Label scoreLabel;

	// Reference to the deaths label object
	private Label deathsLabel;

	// Reference to the quit confirm modal
	private Control quitConfirmModal;

	// Is the game paused?
	private bool isPaused = false;

	// Is the level finished?
	private bool levelFinished = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		// Set some references
		scoreLabel = GetNode<Label>("%ScoreLabel");
		deathsLabel = GetNode<Label>("%DeathsLabel");
		quitConfirmModal = GetNode<Control>("%QuitConfirmModal");
		quitConfirmModal.ProcessMode = ProcessModeEnum.Always;

		// Hide the quit confirm modal
		quitConfirmModal.Hide();

		Events.Instance.LevelEnd += (string levelName, int score, int deaths) => {
			levelFinished = true;
		};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {}

    public override void _Input(InputEvent @event) {
		// Exit level keybind
        if (@event.IsActionPressed("exit") && !isPaused && !levelFinished) {
			Pause();
		}
    }

    /// <summary>
    /// Sets the contents of the score display in the GUI
    /// </summary>
    /// <param name="score"></param>
    public void SetScore(int score) {
		scoreLabel.Text = string.Format("Score: {0}", score);
	}

	/// <summary>
	/// Sets the contents of the deaths display in the GUI
	/// </summary>
	/// <param name="deaths"></param>
	public void SetDeaths(int deaths) {
		deathsLabel.Text = string.Format("Deaths: {0}", deaths);
	}

	// Pauses the game
	private void Pause() {
		quitConfirmModal.Show();
		GetTree().Paused = true;
		isPaused = true;
	}

	private void OnExitButtonPressed() {
		if (!levelFinished) {
			Pause();
		}
	}

	private void OnNoQuitButtonPressed() {
		quitConfirmModal.Hide();
		GetTree().Paused = false;
		isPaused = false;
	}

	private void OnYesQuitButtonPressed() {
		GetTree().Paused = false;
		isPaused = false;
		Events.Instance.EmitSignal(Events.SignalName.ExitToLevelSelect);
	}
}
