using Godot;
using System;

/// <summary>
/// Keys help you unlock various Locks in the level.
/// </summary>
[Tool]
public partial class Key : Area2D, IGameObject, IHasSaveData {
	/// <summary>
	/// The color of the key.
	/// </summary>
	[Export]
	public Color Color { get; set; }

	/// <summary>
	/// Signal emitted when the key is collected.
	/// </summary>
	/// <param name="color">Color of the key</param>
	[Signal]
	public delegate void CollectKeyEventHandler(Color color);

	// Track the state of the key (if it was collected)
	private bool savedState = false;
	private bool currentState = false;

	// Reference to the level
	private Level level;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		GetNode<Sprite2D>("Sprite").Modulate = Color;
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {}

	public void AttachSignals(Level level) {
        CollectKey += level.OnKeyCollected;
		this.level = level;
	}

    public void SaveCurrentState() {
        savedState = currentState;
    }

    public void ReloadCurrentState() {
		if (currentState != savedState) {
			currentState = savedState;
		}

		// Show the key again if it was not saved as collected
		if (!currentState) {
			Show();

			GetNode<CollisionShape2D>("CollisionShape").SetDeferred(CollisionShape2D.PropertyName.Disabled, false);
		}
    }

	// Runs when another body enters the key
	private void OnBodyEntered(PhysicsBody2D body) {
		if (body is Player && !level.HadRecentDeath) {
			// Get rid of this key object
			currentState = true;
			Hide();

			GetNode<CollisionShape2D>("CollisionShape").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);

			// Emit the signal
			EmitSignal(SignalName.CollectKey, Color);
		}
	}
}
