using Godot;
using System;

public partial class Lock : RigidBody2D
{
	// Texture to display for the wall
	[Export]
	public Texture2D WallTexture { get; set; }

	// Color of the lock
	[Export]
	public Color Color { get; set; }

	// Number of keys required to open the lock
	[Export(PropertyHint.Range, "1, 99, 1, or_greater")]
	public int NumKeysRequired { get; set; } = 1;

	// Tracks the number of remaining keys needed to open the lock
	private int numKeysRemaining;

	// Stored reference to the label for the number of remaining keys.
	private Label amountLabel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
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

		SetNumKeysRemaining(NumKeysRequired);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// Runs whenever a key is collected. It makes sure the key collected was the correct color before doing anything.
	public void OnKeyCollected(Color color) {
		if (Color == color) {
			// Resolve the effects of the lock
			SetNumKeysRemaining(numKeysRemaining - 1);

			if (numKeysRemaining == 0) {
				Hide();

				GetNode<CollisionShape2D>("CollisionShape").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
			}
		}
	}

	// Sets the number of remaining keys, updating the label
	private void SetNumKeysRemaining(int keys) {
		numKeysRemaining = keys;
		amountLabel.Text = keys.ToString();
	}
}
