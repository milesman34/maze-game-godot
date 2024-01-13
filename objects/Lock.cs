using Godot;
using System;

public partial class Lock : RigidBody2D
{
	/// <summary>
	/// Texture to display for the Wall.
	/// </summary>
	[Export]
	public Texture2D WallTexture { get; set; }

	/// <summary>
	/// The color of the Lock.
	/// </summary>
	[Export]
	public Color Color { get; set; }

	/// <summary>
	/// Number of keys required to open the lock.
	/// </summary>
	[Export(PropertyHint.Range, "1, 99, 1")]
	public int NumKeysRequired { get; set; } = 1;

	[ExportCategory("Start Position")]

	/// <summary>
	/// Should the node's transform position be overridden with the start position here?
	/// </summary>
	[Export]
	public bool OverrideStartPos { get; set; } = false;

	/// <summary>
	/// Starting position of the Portal in units. This is technically optional but is useful as an override to avoid having to figure out the location manually.
	/// </summary>
	[Export]
	public Vector2 StartPosition { get; set; } = new Vector2(0, 0);

	/// <summary>
	/// Number of keys needed to open this lock.
	/// </summary>
	private int numKeysRemaining;

	/// <summary>
	/// Stored reference to the label for the number of remaining keys.
	/// </summary>
	private Label amountLabel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Set up references
		amountLabel = GetNode<Label>("AmountLabel");

		// If the portal is set to override position, then override it using the provided position
		Position = GetStartingPosition();

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

	/// <summary>
    /// Returns the starting position in pixels. The original position was based on units.
    /// </summary>
    /// <returns>Starting position in pixels</returns>
    public Vector2 GetStartingPosition() {
        if (OverrideStartPos) {
			return StartPosition * Constants.TileSize + new Vector2(Constants.TileSize, Constants.TileSize) / 2.0f; // Size of tile?
		} else {
			return Position;
		}
    }

	/// <summary>
	/// OnKeyCollected runs when the Level sends a signal with the key that was collected. It decrements the number of remaining keys left and hides the lock if the amount left is 0.
	/// </summary>
	/// <param name="color">The color of the key that was collected</param>
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

	/// <summary>
	/// Sets the number of remaining keys, updating the label.
	/// </summary>
	/// <param name="keys">New number of keys</param>
	private void SetNumKeysRemaining(int keys) {
		numKeysRemaining = keys;
		amountLabel.Text = keys.ToString();
	}
}
