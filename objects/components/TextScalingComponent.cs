using Godot;
using System;

/// <summary>
/// This component can be attached to a Label object to make it hopefully scale better.
/// It is a bit awkward to use, for best results you should make sure the label object isn't too wide compared to the text.
/// </summary>
public partial class TextScalingComponent : Node {
	/// <summary>
	/// How many times should the text be scaled?
	/// </summary>
	[Export]
	public float ScaleFactor = 4;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		var parent = GetParent<Label>();

		parent.AddThemeFontSizeOverride("font_size", (int) (parent.GetThemeFontSize("font_size") * ScaleFactor));

		parent.Scale = new Vector2(1 / ScaleFactor, 1 / ScaleFactor);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {}
}
