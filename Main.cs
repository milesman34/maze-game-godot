using Godot;
using System;

public partial class Main : Node2D
{
	// The active scene to load
	[Export]
	public PackedScene GameScene { get; set; }

	[Export]
	public int test { get; set; }

	// Reference to the current scene
	public IGameState currentGameState;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		currentGameState = GameScene.Instantiate<Game>();
		AddChild((Game) currentGameState);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
