using Godot;
using System;

public partial class LevelSelectScene : Node2D, IGameState
{	
	// Signal to switch to a level
	[Signal]
	public delegate void SwitchToLevelEventHandler(LevelResource resource);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Set the stretch mode to the viewport
		GetTree().Root.ContentScaleMode = Window.ContentScaleModeEnum.CanvasItems;
		GetTree().Root.ContentScaleAspect = Window.ContentScaleAspectEnum.Expand;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void AttachSignals(Main main) {
		SwitchToLevel += main.SwitchToLevel;
    }

	public void OnSwitchToLevel(LevelResource resource) {
		EmitSignal(SignalName.SwitchToLevel, resource);
	}
}
