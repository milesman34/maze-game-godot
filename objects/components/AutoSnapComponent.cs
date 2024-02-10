using Godot;
using System;

/// <summary>
/// This component makes the parent auto-snap to the grid
/// </summary>
public partial class AutoSnapComponent : Node {
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		var parent = GetParent();

		if (parent is Node2D) {
			var position = (parent as Node2D).Position;

			// This apparently works? The tiles are usually 1 pixel off anyway for whatever idiotic reason
			(parent as Node2D).Position = new Vector2(position.X, position.Y + 17 - position.Y % 32);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {}
}
