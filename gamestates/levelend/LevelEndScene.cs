using Godot;
using System;

public partial class LevelEndScene : Node2D, IGameState
{
	// Score
	private int _score = 0;

	[Export]
	public int Score { get => _score; set {
		_score = value;

		if (scoreLabel != null) {
			scoreLabel.Text = string.Format("Score: {0}", _score);
		}
	}}

	// Number of deaths
	private int _deaths = 0;

	[Export]
	public int Deaths { get => _deaths; set {
		_deaths = value;

		if (deathsLabel != null) {
			deathsLabel.Text = string.Format("Deaths: {0}", _deaths);
		}
	}}

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
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public void AttachSignals(Main main) {
    }

}
