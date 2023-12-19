using Godot;
using System;

public partial class Coin : Area2D
{
	// Color to paint the coin
	[Export]
	public Color CoinColor { get; set; }

	// Value of the coin
	[Export]
	public int Value { get; set; } = 1;

	// Signal to emit when collected
	[Signal]
	public delegate void CollectCoinEventHandler(int value);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var sprite = GetNode<Sprite2D>("Sprite");

		sprite.Modulate = new Color(CoinColor);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnBodyEntered(RigidBody2D body) {
		if (body is Player) {
			// Get rid of this coin object
			Hide();

			GetNode<CollisionShape2D>("CollisionShape").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);

			// Emit the signal
			EmitSignal(SignalName.CollectCoin, Value);
		}
	}
}
