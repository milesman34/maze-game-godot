using Godot;
using System;

/// <summary>
/// The Main object manages the entire game.
/// </summary>
public partial class GameScene : Node2D, IGameState
{
	// Scene for the level, to be instantiated later
	[Export]
	public PackedScene LevelScene { get; set; }

	// Reference to the game viewport
	public SubViewport gameViewport;

	// Reference to the current instantiated level
	private Level level;
	
	// Reference to the GUI object
	private GameGUI GameGUI;

	// Reference to the GameContainerOffset object, for offsetting the position of the GameContainer
	private Node2D gameContainerOffset;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Update background color
		RenderingServer.SetDefaultClearColor(new Color(0, 0, 0));
		
		// Get a reference to the GUI
		GameGUI = GetNode<GameGUI>("GUI");

		// Trying to get the viewport directly didn't work
		// So I had to use three steps
		gameContainerOffset = GetNode<Node2D>("GameContainerOffset");

		gameViewport = GetNode<SubViewport>("%GameViewport");

		// Connect the on window resize event
		GetTree().Root.SizeChanged += SetupViewportPositionSize;

		// Start the level if the level scene instance exists
		if (LevelScene != null) {
			StartLevel(LevelScene);
		}
	}

	// Starts playing a level
	public void StartLevel(PackedScene levelScene) {
		// Instantiate the level
		level = Level.InstantiateLevelScene(levelScene, this, GameGUI);

		gameViewport.AddChild(level);
		SetupViewportPositionSize();
	}

	// Sets up the correct size and position for the main game viewport. There is a 32-pixel gap on the top and bottom.
	private void SetupViewportPositionSize() {
		var mainViewportSize = GetViewportRect().Size;
	
		gameViewport.Size = (Vector2I) new Vector2(mainViewportSize.X, mainViewportSize.Y - GameGUI.HeaderHeight * 2);

		// Offset node is used to offset the viewport's position
		gameContainerOffset.Position = new Vector2(0, GameGUI.HeaderHeight);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	public void AttachSignals(Main main) {
    }
}
