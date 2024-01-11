using Godot;
using System;

/// <summary>
/// The Main object manages the entire game.
/// </summary>
public partial class Main : Node2D
{
	/// <summary>
	/// Scene for the level, to be instantiated later
	/// </summary>
	[Export]
	public PackedScene LevelScene { get; set; }

	/// <summary>
	/// Reference to the game viewport, this is set during _Ready()
	/// </summary>
	public SubViewport gameViewport;

	/// <summary>
	/// Reference to the current instantiated level.
	/// </summary>
	private Level level;
	
	/// <summary>
	/// Reference to the GUI object.
	/// </summary>
	private GUI GUI;

	/// <summary>
	/// Reference to the GameContainerOffset object, for offsetting the position of the GameContainer.
	/// </summary>
	private Node2D gameContainerOffset;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Update background color
		RenderingServer.SetDefaultClearColor(new Color(0, 0, 0));
		
		// Get a reference to the GUI
		GUI = GetNode<GUI>("GUI");

		// Instantiate the level
		level = Level.InstantiateLevelScene(LevelScene, this, GUI);

		// Trying to get the viewport directly didn't work
		// So I had to use three steps
		gameContainerOffset = GetNode<Node2D>("GameContainerOffset");

		gameViewport = gameContainerOffset.GetNode<SubViewportContainer>("GameContainer").GetNode<SubViewport>("Viewport");

		gameViewport.AddChild(level);

		SetupViewportPositionSize();

		// Connect the on window resize event
		GetTree().Root.SizeChanged += SetupViewportPositionSize;
	}

	/// <summary>
	/// Sets up the correct size and position for the main game viewport. There is a 32-pixel gap on the top and bottom.
	/// </summary>
	private void SetupViewportPositionSize() {
		var mainViewportSize = GetViewportRect().Size;
	
		gameViewport.Size = (Vector2I) new Vector2(mainViewportSize.X, mainViewportSize.Y - GUI.HeaderHeight * 2);

		// Offset node is used to offset the viewport's position
		gameContainerOffset.Position = new Vector2(0, GUI.HeaderHeight);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
}
