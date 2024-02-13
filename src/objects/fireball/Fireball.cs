using Godot;
using System;

/// <summary>
/// Fireballs are objects that kill the player when they touch them.
/// </summary>
[Tool]
public partial class Fireball : Area2D, IGameObject {
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {}

	public void AttachSignals(Level level) {}
}
