using Godot;
using System;

/// <summary>
/// PositioningComponent lets you snap an element to the correct part of the grid
/// </summary>
public partial class PositioningComponent : Node
{
	/// <summary>
	/// Position to put the parent component
	/// </summary>
	[Export]
	public Vector2 StartPosition {get; set;}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var parent = GetParent();

		if (parent is Node2D) {
			(parent as Node2D).Position = GetStartingPosition();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	/// <summary>
    /// Returns the starting position in pixels. The original position was based on units.
    /// </summary>
    /// <returns>Starting position in pixels</returns>
    public Vector2 GetStartingPosition() {
		return StartPosition * Constants.TileSize + new Vector2(Constants.TileSize, Constants.TileSize) / 2.0f; // Size of tile?
    }
}
