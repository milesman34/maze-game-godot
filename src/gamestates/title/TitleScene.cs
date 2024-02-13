using Godot;
using System;

/// <summary>
/// GameState representing the title screen.
/// </summary>
public partial class TitleScene : CanvasLayer, IGameState {
	/// <summary>
	/// Signal for going to the level select screen.
	/// </summary>
	[Signal]
	public delegate void SwitchToLevelSelectEventHandler();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		// Update background color
		RenderingServer.SetDefaultClearColor(new Color(0, 0, 0));

		// Set the stretch mode to the viewport
		GetTree().Root.ContentScaleMode = Window.ContentScaleModeEnum.CanvasItems;

		// Set up the start button on click
		var startButton = GetNode<Button>("%StartButton");

		startButton.Pressed += () => EmitSignal(SignalName.SwitchToLevelSelect);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {}

    public void AttachSignals(Main main) {
		SwitchToLevelSelect += main.OnSwitchToLevelSelect;
    }
}
