using Godot;
using System;

/// <summary>
/// Markers are decorative objects which can be used to display a color somewhere.
/// </summary>
public partial class Marker : Node2D {
	/// <summary>
	/// The color of the marker.
	/// </summary>
	[Export]
	public Color Color { get; set; } = new Color(255, 255, 255);

	// Reference to the ColorRect
	private ColorRect colorRect;
 
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		colorRect = GetNode<ColorRect>("ColorRect");
		colorRect.Color = Color;

		// Figure out if we actually need to update the positioning to deal with it being off center, since the size is only 32x32
		// Manual positioning should not be affected
		var shouldUpdate = false;

		foreach (var child in GetChildren()) {
			if (child is PositioningComponent) {
				shouldUpdate = true;
				break;
			}
		}

		if (shouldUpdate) {
			Position += new Vector2(-8, -8);
		}

		// Apparently "Current Animation" doesn't actually play the animation
		// So just do it in code instead
		// This animation makes the marker's brightness change over time.
		GetNode<AnimationPlayer>("AnimationPlayer").Play("Marker");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {}
}
