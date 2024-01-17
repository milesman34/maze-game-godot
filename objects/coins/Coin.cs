using Godot;
using System;

/// <summary>
/// Coins are collectible objects which increase the player's score. They can be customized with a color and a value.
/// </summary>
public partial class Coin : Area2D, IGameObject
{
	// Color to paint the coin with
	[Export]
	public Color CoinColor { get; set; }

	// Value of the coin, meaning how many points you get for collecting it
	[Export]
	public int Value { get; set; } = 1;

	// Coin collection signal
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

	// Runs when another body enters the coin
	private void OnBodyEntered(RigidBody2D body) {
		if (body is Player) {
			// Get rid of this coin object
			Hide();

			GetNode<CollisionShape2D>("CollisionShape").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);

			// Emit the signal
			EmitSignal(SignalName.CollectCoin, Value);
		}
	}

	public void AttachSignals(Level level) {
		CollectCoin += level.OnAddScore;
	}
}
