using Godot;
using System;

/// <summary>
/// GUI object is responsible for maintaining the GUI.
/// </summary>
public partial class GameGUI : CanvasLayer
{
	/// <summary>
	/// Constant representing the height of the header in pixels (default is 32)
	/// </summary>
	public const int HeaderHeight = 64;

	// Reference to the score label object, set during _Ready()
	private Label scoreLabel;

	// Reference to the deaths label object
	private Label deathsLabel;

	// Reference to the quit confirm modal
	private Control quitConfirmModal;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Set some references
		scoreLabel = GetNode<Label>("%ScoreLabel");
		deathsLabel = GetNode<Label>("%DeathsLabel");
		quitConfirmModal = GetNode<Control>("%QuitConfirmModal");

		// Hide the quit confirm modal
		quitConfirmModal.Hide();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// Sets the contents of the score display in the GUI.
	public void SetScore(int score) {
		scoreLabel.Text = string.Format("Score: {0}", score);
	}

	// Sets the contents of the deaths display in the GUI.
	public void SetDeaths(int deaths) {
		deathsLabel.Text = string.Format("Deaths: {0}", deaths);
	}

	private void OnExitButtonPressed() {
		quitConfirmModal.Show();
	}

	private void OnNoQuitButtonPressed() {
		quitConfirmModal.Hide();
	}

	private void OnYesQuitButtonPressed() {
		Events.instance.EmitSignal(Events.SignalName.ExitToLevelSelect);
	}
}
