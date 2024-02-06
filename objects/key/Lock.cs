using Godot;
using System;

/// <summary>
/// Locks block the player until enough keys of the given color are collected.
/// </summary>
public partial class Lock : RigidBody2D, IGameObject {
	// LockState keeps track of the current state of the lock
	private class LockState {
		public int keysRemaining;
		public bool enabled;

		public LockState(int remaining, bool enabled = true) {
			keysRemaining = remaining;
			this.enabled = enabled;
		}
	}

	/// <summary>
	/// Texture to display for the wall.
	/// </summary>
	[Export]
	public Texture2D WallTexture { get; set; }

	/// <summary>
	/// Color of the lock.
	/// </summary>
	[Export]
	public Color Color { get; set; }

	/// <summary>
	/// The number of keys required to open the lock.
	/// </summary>
	[Export(PropertyHint.Range, "1, 99, 1, or_greater")]
	public int NumKeysRequired { get; set; } = 1;

	// Tracks the number of remaining keys needed to open the lock
	private int numKeysRemaining;

	// Stored reference to the label for the number of remaining keys.
	private Label amountLabel;

	// Current/saved states
	private LockState currentState;
	private LockState savedState;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		Events.Instance.PlayerHit += OnPlayerHit;

		// Set up references
		amountLabel = GetNode<Label>("AmountLabel");

		// I want the text to actually look good
		// 12px is the target size but is too pixellated
		// So instead I set the font size to 48px and then scale down the label by 4x
		amountLabel.Scale = new Vector2(0.25f, 0.25f);

		// Set wall sprite texture
		GetNode<Sprite2D>("WallSprite").Texture = WallTexture;
		
		// Set lock color
		GetNode<Sprite2D>("LockSprite").Modulate = Color;

		// Set up the states
		currentState = new LockState(NumKeysRequired);
		savedState = new LockState(NumKeysRequired);

		SetNumKeysRemaining(NumKeysRequired);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {}

    public override void _ExitTree() {
        base._ExitTree();

		Events.Instance.PlayerHit -= OnPlayerHit;
    }

	public void AttachSignals(Level level) {
        level.CollectKey += OnKeyCollected;
        level.CheckpointHit += OnCheckpointHit;
	}

	// Sets the number of remaining keys, updating the label
	private void SetNumKeysRemaining(int keys) {
		numKeysRemaining = keys;
		amountLabel.Text = keys.ToString();
		currentState.keysRemaining = keys;
	}

    // Runs whenever a key is collected. It makes sure the key collected was the correct color before doing anything.
    private void OnKeyCollected(Color color) {
		if (Color == color) {
			// Resolve the effects of the lock
			SetNumKeysRemaining(numKeysRemaining - 1);

			if (numKeysRemaining == 0) {
				Hide();
				currentState.enabled = false;

				GetNode<CollisionShape2D>("CollisionShape").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
			}
		}
	}

	// Runs when the player is hit
	private void OnPlayerHit() {
		SetNumKeysRemaining(savedState.keysRemaining);

		// If the lock was disabled but not in the saved state, we need to re-enable it
		if (savedState.enabled && !currentState.enabled) {
			currentState.enabled = true;
			Show();

			GetNode<CollisionShape2D>("CollisionShape").SetDeferred(CollisionShape2D.PropertyName.Disabled, false);
		}
	}

	// Runs when a checkpoint is hit
	private void OnCheckpointHit() {
		savedState = new LockState(currentState.keysRemaining, currentState.enabled);
	}
}
