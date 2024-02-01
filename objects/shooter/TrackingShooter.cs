using Godot;
using System;

// Variant of the shooter that tracks the player and shoots in their direction
public partial class TrackingShooter : Shooter
{
	// Reference to the parent level
	private Level level;

	// Reference to the shooter sprite
	private Sprite2D shooterSprite;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();

		// Set up references
		shooterSprite = GetNode<Sprite2D>("ShooterSprite");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);

		var position = level.GetPlayerPosition();
		
		var difference = position - GlobalPosition;

		ShooterAngle = (float) (Math.Atan2(difference.Y, difference.X) * 180 / Math.PI);

		shooterSprite.RotationDegrees = ShooterAngle - 90;
	}

	public override void AttachSignals(Level level) {
		this.level = level;
	}
}
