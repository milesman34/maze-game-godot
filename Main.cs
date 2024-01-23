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
	public int test { get; set; }

	// Reference to the current scene
	public IGameState currentGameState;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Switch to the title screen
		SwitchToScene<TitleScene>(TitleScene);
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

	// Switches to a level
	public void SwitchToLevel(LevelResource resource) {
		SwitchToScene<GameScene>(GameScene);
		
		var gameScene = (GameScene) currentGameState;
		gameScene.StartLevel(resource.LevelScene);
	}
}
