using Godot;
using System;

// Helps scale a label properly, designed to be used with Labels only
// This is a bit awkward to use but can help make text look better
public partial class TextScalingComponent : Node
{
	// How much it should be scaled by
	[Export]
	public float ScaleFactor = 4;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var parent = GetParent<Label>();

		parent.AddThemeFontSizeOverride("font_size", (int) (parent.GetThemeFontSize("font_size") * ScaleFactor));

		parent.Scale = new Vector2(1 / ScaleFactor, 1 / ScaleFactor);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
