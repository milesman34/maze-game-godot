using Godot;
using System;

public partial class Shooter : RigidBody2D, ICameraZoneListener
{
	// Wall texture to render
	[Export]
	public Texture2D WallTexture { get; set; }

	// Angle the shooter is rotated (relative to facing to the right)
	[Export]
	public float ShooterAngle { get; set; } = 0;

	// How many seconds per projectile?
	[Export]
	public float FiringRate { get; set; } = 5;

	// Speed of the projectile in pixels per second
	[Export]
	public float ProjectileSpeed { get; set; } = 64;

	// Scene for the projectile
	[Export]
	public PackedScene ProjectileScene { get; set; }

	// Resource for the projectile
	[Export]
	public Projectile Projectile { get; set; }

	// Reference to the shooter timer
	private Timer shooterTimer;

	// Track the current camera zone this object is in
	private int cameraZone = -1;

	// Are we in the current camera zone ID?
	private bool isGameInCurrentCameraZone = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<Sprite2D>("WallSprite").Texture = WallTexture;

		// Change only the rotation for the sprite itself, not the wall sprite
		GetNode<Sprite2D>("ShooterSprite").RotationDegrees = ShooterAngle - 90;

		// Set up references
		shooterTimer = GetNode<Timer>("ShooterTimer");

		// Set up shooter timer
		shooterTimer.WaitTime = FiringRate;

		shooterTimer.Start();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnShooterTimerTimeout() {
		var projectile = ProjectileScene.Instantiate<ShooterProjectile>();

		projectile.projectileResource = Projectile;

		projectile.SetSpeedAndAngle(ProjectileSpeed, ShooterAngle);

		AddChild(projectile);
	}

    public void SetCameraZoneID(int ID)
    {
        cameraZone = ID;
    }

    public void OnCameraZoneEntered(int ID)
    {
        if (cameraZone != ID) { // We are entering a new room, so pause the shooter
			shooterTimer.Paused = true;
		} else { // Set that we are in the current room
			isGameInCurrentCameraZone = true;
		}
    }

    public void OnCameraZoneExited(int ID)
    {
        if (cameraZone != ID && isGameInCurrentCameraZone) { // We are leaving a different room to enter this room, so unpause the shooter
			shooterTimer.Paused = false;
		} else if (cameraZone == ID) { // We are leaving this room
			isGameInCurrentCameraZone = false;
		}
    }

}
