 using Godot;
using System;

public partial class Main : Node2D
{
	// The various scenes in the game
	[Export]
	public PackedScene TitleScene { get; set; }
	
	[Export]
	public PackedScene LevelSelectScene { get; set; }

	[Export]
	public PackedScene GameScene { get; set; }

	[Export]
	public PackedScene LevelEndScene { get; set; }

	// Reference to the current scene
	public IGameState currentGameState;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Switch to the title screen
		SwitchToScene<TitleScene>(TitleScene);

		// Connect to the LevelEnd global signal
		Events.instance.LevelEnd += OnLevelEnd;
		Events.instance.SwitchToLevel += SwitchToLevel;
		Events.instance.ExitToLevelSelect += OnExitToLevelSelect;
	}

	// Switches to a difference scene
	private void SwitchToScene<T>(PackedScene newScene) where T : Node {
		if (currentGameState != null) {
			currentGameState.QueueFree();
		}
		
		var scene = newScene.Instantiate<T>();
		AddChild(scene);
		currentGameState = (IGameState) scene;
		currentGameState.AttachSignals(this);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// Runs when the game starts
	public void OnGameStarted() {
		SwitchToScene<LevelSelectScene>(LevelSelectScene);
	}

	// Runs when the player exits out to level select
	public void OnExitToLevelSelect() {
		SwitchToScene<LevelSelectScene>(LevelSelectScene);
	}

	// Runs when the game ends
	public void OnLevelEnd(string name, int score, int deaths) {
		// There is a 0.5 second delay
		GetTree().CreateTimer(0.5).Timeout += () => {
			SwitchToScene<LevelEndScene>(LevelEndScene);

			// Give key information to the level end scene
			var endScene = (LevelEndScene) currentGameState;
			endScene.Score = score;
			endScene.Deaths = deaths;
			endScene.LevelName = name;
		};
	}

	// Switches to a level
	public void SwitchToLevel(LevelResource resource) {
		SwitchToScene<GameScene>(GameScene);
		
		var gameScene = (GameScene) currentGameState;
		gameScene.StartLevel(resource);
	}

	// Runs when the player goes back to the title screen
	public void OnGoToTitleScreen() {
		SwitchToScene<TitleScene>(TitleScene);
	}
}
