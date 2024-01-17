using Godot;
using System;

public partial class Fireball : Area2D, IGameObject
{	
	// Signal called when the fireball hits the player
	[Signal]
	public delegate void PlayerHitEventHandler();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnBodyEntered(RigidBody2D body) {
		if (body is Player) {
			EmitSignal(SignalName.PlayerHit);
		}
	}

	public void AttachSignals(Level level) {
		PlayerHit += level.OnPlayerHit;
	}
}
