using Godot;
using System;

/// <summary>
/// Keys help you unlock various Locks in the level.
/// </summary>
public partial class Key : Area2D, IGameObject
{
	[Export]
	public Color Color { get; set; }

	// Signal to emit when the key is collected
	[Signal]
	public delegate void CollectKeyEventHandler(Color color);

	// Track the states of the keys, meaning if it was collected or not, as well as the saved version
	private bool savedCollected = false;
	private bool currentCollected = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<Sprite2D>("Sprite").Modulate = Color;
		Events.instance.PlayerHit += OnPlayerHit;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// Runs when another body enters the key
	private void OnBodyEntered(PhysicsBody2D body) {
		if (body is Player) {
			// Get rid of this key object
			currentCollected = true;
			Hide();

			GetNode<CollisionShape2D>("CollisionShape").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);

			// Emit the signal
			EmitSignal(SignalName.CollectKey, Color);
		}
	}

	// Runs when the player is hit
	public void OnPlayerHit() {
		// If the key was collected but was not saved, then it needs to be reset
		if (currentCollected && !savedCollected) {
			currentCollected = false;
			Show();

			GetNode<CollisionShape2D>("CollisionShape").SetDeferred(CollisionShape2D.PropertyName.Disabled, false);
		}
	}

	// Runs when a checkpoint is hit
	public void OnCheckpointHit() {
		savedCollected = currentCollected;
	}

	public void AttachSignals(Level level) {
        CollectKey += level.OnKeyCollected;
        level.CheckpointHit += OnCheckpointHit;
	}
}