using Godot;
using System;

public partial class Main : Node2D
{
	// Scene for the Level
	[Export]
	public PackedScene LevelScene { get; set; }

	// Reference to the viewport
	public SubViewport viewport;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Update background color
		RenderingServer.SetDefaultClearColor(new Color(0, 0, 0));

		// Instantiate the level
		var level = LevelScene.Instantiate<Level>();

		// Set the reference to the game object
		level.mainGame = this;

		// Get a reference to the GUI
		var GUI = GetNode<GUI>("GUI");

		// Set some signals for the GUI
		level.SetScore += GUI.SetScore;

		// Trying to get the viewport directly didn't work
		// So I had to use three steps
		viewport = GetNode<Node2D>("GameContainerOffset").GetNode<SubViewportContainer>("GameContainer").GetNode<SubViewport>("Viewport");

		viewport.AddChild(level);

		SetupViewportPositionSize();

		// Connect the on window resize event
		GetTree().Root.SizeChanged += SetupViewportPositionSize;
	}

	// Sets up the proper position and size of the main game viewport
	private void SetupViewportPositionSize() {
		var mainViewportSize = GetViewportRect().Size;
	
		viewport.Size = (Vector2I) new Vector2(mainViewportSize.X, mainViewportSize.Y - GUI.HeaderHeight * 2);

		// Offset node is used to offset the viewport's position
		var offsetNode = GetNode<Node2D>("GameContainerOffset");

		offsetNode.Position = new Vector2(0, GUI.HeaderHeight);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
}
