using Godot;
using System;

/// <summary>
/// Coins are collectible objects which increase the player's score. They can be customized with a color and a value.
/// </summary>
public partial class Coin : Area2D
{
	/// <summary>
	/// Color to paint the coin with
	/// </summary>
	[Export]
	public Color CoinColor { get; set; }

	/// <summary>
	/// Value of the coin, meaning how many points you get for collecting it
	/// </summary>
	[Export]
	public int Value { get; set; } = 1;

	/// <summary>
	/// Coins emit this signal when collected
	/// </summary>
	/// <param name="value">Value of the coin</param>
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

	/// <summary>
	/// This function runs when another RigidBody2D enters the coin's area. It detects if that body is the Player and removes the coin if it is.
	/// </summary>
	/// <param name="body">The RigidBody2D entering the coin's area</param>
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
