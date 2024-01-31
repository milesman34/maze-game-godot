using Godot;
using System;

// This component can be attached to an object to make it damage the player
public partial class DamagePlayerComponent : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var parent = GetParent();

		if (parent is Area2D) {
			var node = parent as Area2D;

			node.BodyEntered += OnBodyEntered;
		}
	}

	// Emits a player hit signal if the collided node was the player
	private void OnBodyEntered(Node2D body) {
		if (body is Player) {
			Events.instance.EmitSignal(Events.SignalName.PlayerHit);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
