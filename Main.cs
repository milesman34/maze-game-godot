using Godot;
using System;

/// <summary>
/// The main object that controls the game.
/// </summary>
public partial class Main : Node2D {
	/// <summary>
	/// Scene for the title screen.
	/// </summary>
	[Export]
	public PackedScene TitleScene { get; set; }
	
	/// <summary>
	/// Scene for the level select screen.
	/// </summary>
	[Export]
	public PackedScene LevelSelectScene { get; set; }

	/// <summary>
	/// Scene for the game scene.
	/// </summary>
	[Export]
	public PackedScene GameScene { get; set; }

	/// <summary>
	/// Scene for the level end screen.
	/// </summary>
	[Export]
	public PackedScene LevelEndScene { get; set; }

	// Reference to the current scene
	private IGameState currentGameState;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		// Switch to the title screen
		SwitchToScene<TitleScene>(TitleScene);

		// Connect to the LevelEnd global signal
		Events.Instance.LevelEnd += OnLevelEnd;
		Events.Instance.SwitchToLevel += SwitchToLevel;
		Events.Instance.ExitToLevelSelect += OnSwitchToLevelSelect;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {}

	// Switches to a difference scene (you can pass a function to be called before adding the scene)
	private void SwitchToScene<T>(PackedScene newScene, Action<T> preAddFunction) where T : Node {
		if (currentGameState != null) {
			((Node) currentGameState).QueueFree();
		}
		
		var scene = newScene.Instantiate<T>();
		preAddFunction(scene);
		AddChild(scene);
		currentGameState = (IGameState) scene;
		currentGameState.AttachSignals(this);
	}

	// Overload that doesn't require a function
	private void SwitchToScene<T>(PackedScene newScene) where T : Node {
		SwitchToScene(newScene, (T scene) => {});
	}

	// Switches to a level
	private void SwitchToLevel(LevelResource resource) {
		SwitchToScene<GameScene>(GameScene);
		
		var gameScene = (GameScene) currentGameState;
		gameScene.StartLevel(resource);
	}

	/// <summary>
	/// Function called when the player goes back to the title screen.
	/// </summary>
	public void OnGoToTitleScreen() {
		SwitchToScene<TitleScene>(TitleScene);
	}

	/// <summary>
	/// Function called when the player switches to the level select screen.
	/// </summary>
	public void OnSwitchToLevelSelect() {
		SwitchToScene<LevelSelectScene>(LevelSelectScene);
	}

	// Runs when the game ends
	private void OnLevelEnd(string name, int score, int deaths) {
		// There is a 0.5 second delay
		GetTree().CreateTimer(0.5).Timeout += () => {
			SwitchToScene(LevelEndScene, (LevelEndScene scene) => {
				scene.Score = score;
				scene.Deaths = deaths;
				scene.LevelName = name;
			});
		};
	}
}
