using Godot;
using System;

public partial class LevelSelectScene : CanvasLayer, IGameState
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Set the stretch mode to the viewport
		GetTree().Root.ContentScaleMode = Window.ContentScaleModeEnum.CanvasItems;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void AttachSignals(Main main) {
		
    }
}
