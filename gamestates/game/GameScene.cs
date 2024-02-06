using Godot;
using System;

/// <summary>
/// GameState for managing the game itself
/// </summary>
public partial class GameScene : Node2D, IGameState {
	/// <summary>
	/// Resource for the level.
	/// </summary>
	[Export]
	public LevelResource LevelResource { get; set; }

	// Reference to the game viewport
	private SubViewport gameViewport;

	/// <summary>
	/// The game viewport is in charge of displaying the game.
	/// </summary>
	public SubViewport GameViewport { 
		get {
			return gameViewport;
		}
	 }

	// Reference to the current instantiated level
	private Level level;
	
	// Reference to the GUI object
	private GameGUI gameGUI;

	// Reference to the GameContainerOffset object, for offsetting the position of the GameContainer
	private Node2D gameContainerOffset;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		// Update background color
		RenderingServer.SetDefaultClearColor(new Color(0, 0, 0));

		// Set the stretch mode to the viewport
		GetTree().Root.ContentScaleMode = Window.ContentScaleModeEnum.Disabled;
		
		// Get a reference to the GUI
		gameGUI = GetNode<GameGUI>("GUI");

		// Trying to get the viewport directly didn't work
		// So I had to use three steps
		gameContainerOffset = GetNode<Node2D>("GameContainerOffset");

		gameViewport = GetNode<SubViewport>("%GameViewport");

		// Connect the on window resize event
		GetTree().Root.SizeChanged += SetupViewportPositionSize;

		// Start the level if the level scene instance exists
		if (LevelResource != null) {
			StartLevel(LevelResource);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {}

	public void AttachSignals(Main main) {}

	/// <summary>
	/// Instantiates and starts a level.
	/// </summary>
	/// <param name="levelResource">Resource for the given level</param>
	public void StartLevel(LevelResource levelResource) {
		// Instantiate the level
		level = Level.InstantiateLevel(levelResource, this, gameGUI);

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
}
