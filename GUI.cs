using Godot;
using System;

public partial class GUI : CanvasLayer
{
	// Constant representing the height of the header
	public const int HeaderHeight = 32;

	// Reference to the score label object
	private Label scoreLabel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Set some references
		scoreLabel = GetNode<Control>("Header").GetNode<Label>("ScoreLabel");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// Sets the score displayed by the GUI
	public void SetScore(int score) {
		scoreLabel.Text = string.Format("Score: {0}", score);
	}
}
