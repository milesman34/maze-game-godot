using Godot;
using System;

/// <summary>
/// Keys help you unlock various Locks in the level.
/// </summary>
public partial class Key : Area2D
{
	[Export]
	public Color Color { get; set; }

	// Signal to emit when the key is collected
	[Signal]
	public delegate void CollectKeyEventHandler(Color color);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<Sprite2D>("Sprite").Modulate = Color;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	/// <summary>
	/// OnBodyEntered runs when a body enters the object. If that body is the player, then it hides the key and emits a signal.
	/// </summary>
	/// <param name="body"></param>
	public void OnBodyEntered(RigidBody2D body) {
		if (body is Player) {
			// Get rid of this key object
			Hide();

			GetNode<CollisionShape2D>("CollisionShape").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);

			// Emit the signal
			EmitSignal(SignalName.CollectKey, Color);
		}
	}
}
