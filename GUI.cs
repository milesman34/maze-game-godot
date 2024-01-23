using Godot;
using System;

/// <summary>
/// GUI object is responsible for maintaining the GUI.
/// </summary>
public partial class GUI : CanvasLayer
{
	/// <summary>
	/// Constant representing the height of the header in pixels (default is 32)
	/// </summary>
	public const int HeaderHeight = 64;

	// Reference to the score label object, set during _Ready()
	private Label scoreLabel;

	// Reference to the deaths label object
	private Label deathsLabel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Set some references
		scoreLabel = GetNode<Label>("%ScoreLabel");
		deathsLabel = GetNode<Label>("%DeathsLabel");
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
}
