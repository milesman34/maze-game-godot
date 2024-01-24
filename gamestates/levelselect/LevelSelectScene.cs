using Godot;
using System;

public partial class LevelSelectScene : Node2D, IGameState
{	
	// Signal for going back to the title screen
	[Signal]
	public delegate void GoToTitleScreenEventHandler();

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
		GoToTitleScreen += main.OnGoToTitleScreen;
    }

	public void OnLevelButtonPressed(LevelResource resource) {
		Events.instance.EmitSignal(Events.SignalName.SwitchToLevel, resource);
	}

	private void OnBackButtonPressed() {
		EmitSignal(SignalName.GoToTitleScreen);
	}
}
