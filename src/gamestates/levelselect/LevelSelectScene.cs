using Godot;
using System;

/// <summary>
/// GameState representing the level selection screen.
/// </summary>
public partial class LevelSelectScene : Node2D, IGameState {
	/// <summary>
	/// Signal for returning to the title screen.
	/// </summary>
	[Signal]
	public delegate void GoToTitleScreenEventHandler();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		// Set the stretch mode to the viewport
		GetTree().Root.ContentScaleMode = Window.ContentScaleModeEnum.CanvasItems;
		GetTree().Root.ContentScaleAspect = Window.ContentScaleAspectEnum.Expand;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {}
	
	public void AttachSignals(Main main) {
		GoToTitleScreen += main.OnGoToTitleScreen;
    }

	private void OnLevelButtonPressed(LevelResource resource) {
		Events.Instance.EmitSignal(Events.SignalName.SwitchToLevel, resource);
	}

	private void OnBackButtonPressed() {
		EmitSignal(SignalName.GoToTitleScreen);
	}
}
