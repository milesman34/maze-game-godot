using Godot;
using System;

/// <summary>
/// Locks block the player until enough keys of the given color are collected.
/// </summary>
[Tool]
public partial class Lock : RigidBody2D, IGameObject, IHasSaveData {
	/// <summary>
	/// LockState keeps track of the current state of the lock
	/// </summary>
	private class LockState {
		private int keysRemaining;
		private bool enabled;

		/// <summary>
		/// How many remaining keys are required to open the lock?
		/// </summary>
		public int KeysRemaining { 
			get {
				return keysRemaining;
			}
			
			set {
				keysRemaining = value;
			}
		}
		
		/// <summary>
		/// Is the lock enabled or not?
		/// </summary>
		public bool Enabled {
			get {
				return enabled;
			}

			set {
				enabled = value;
			}
		}

		public LockState(int remaining, bool enabled = true) {
			keysRemaining = remaining;
			this.enabled = enabled;
		}

		/// <summary>
		/// Creates a copy of the current LockState, as a distinct object.
		/// </summary>
		/// <returns></returns>
		public LockState Copy() {
			return new LockState(keysRemaining, enabled);
		}

        public override bool Equals(object obj) {
			if (!(obj is LockState)) {
				return false;
			} else {
				var lockState = obj as LockState;

				return keysRemaining == lockState.keysRemaining && enabled == lockState.enabled;
			}
        }

        public override int GetHashCode() {
            return HashCode.Combine(keysRemaining, enabled);
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

	public void AttachSignals(Level level) {
        level.CollectKey += OnKeyCollected;
	}

    public void SaveCurrentState() {
        savedState = currentState.Copy();
    }

    public void ReloadCurrentState() {
		if (currentState != savedState) {
			currentState = savedState.Copy();

			SetNumKeysRemaining(currentState.KeysRemaining);

			// If the lock was disabled but not in the saved state, we need to re-enable it
			if (savedState.Enabled) {
				Show();

				GetNode<CollisionShape2D>("CollisionShape").SetDeferred(CollisionShape2D.PropertyName.Disabled, false);
			}
		}
    }

	// Sets the number of remaining keys, updating the label
	private void SetNumKeysRemaining(int keys) {
		numKeysRemaining = keys;
		amountLabel.Text = keys.ToString();
		currentState.KeysRemaining = keys;
	}

    // Runs whenever a key is collected. It makes sure the key collected was the correct color before doing anything.
    private void OnKeyCollected(Color color) {
		if (Color == color) {
			// Resolve the effects of the lock
			SetNumKeysRemaining(numKeysRemaining - 1);

			if (numKeysRemaining == 0) {
				Hide();
				currentState.Enabled = false;

				GetNode<CollisionShape2D>("CollisionShape").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
			}
		}
	}
}
