using Godot;
using System;

/// <summary>
/// This component can be attached to an object to make it damage the player.
/// </summary>
public partial class DamagePlayerComponent : Node {
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		var parent = GetParent();

		if (parent is Area2D) {
			var node = parent as Area2D;

			node.BodyEntered += OnBodyEntered;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {}

	// Emits a player hit signal if the collided node was the player
	private void OnBodyEntered(Node2D body) {
		if (body is Player) {
			Events.Instance.EmitSignal(Events.SignalName.PlayerHit);
		}
	}
}
