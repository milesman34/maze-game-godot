using Godot;
using System;

public partial class Shooter : RigidBody2D, ICameraZoneListener, IGameObject
{
	// Wall texture to render
	[Export]
	public Texture2D WallTexture { get; set; }

	// Angle the shooter is rotated (relative to facing to the right)
	[Export]
	public float ShooterAngle { get; set; } = 0;

	// Delay in seconds before the first shot is fired
	[Export]
	public float FiringDelay { get; set; } = 0;

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

	// Reference to the delay timer
	private Timer delayTimer;

	// Has the delay timer been triggered yet?
	private bool delayTimerTriggered = false;

	// Track the current camera zone this object is in
	private int cameraZone = -1;

	// Are we in the current camera zone ID?
	private bool isGameInCurrentCameraZone = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<Sprite2D>("WallSprite").Texture = WallTexture;

		// Change only the rotation for the sprite itself, not the wall sprite
		GetNode<Sprite2D>("ShooterSprite").RotationDegrees = ShooterAngle;

		// Set up references
		shooterTimer = GetNode<Timer>("ShooterTimer");
		delayTimer = GetNode<Timer>("DelayTimer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// Sets up the shooter timer
	private void SetUpShooterTimer() {
		shooterTimer.WaitTime = FiringRate;

		shooterTimer.Start();
	}

	private void OnDelayTimerTimeout() {
		ShootProjectile();
		SetUpShooterTimer();
	}

	private void OnShooterTimerTimeout() {
		ShootProjectile();
	}

	// Shoots a projectile
	private void ShootProjectile() {
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
			delayTimer.Paused = true;
		} else { // Set that we are in the current room
			isGameInCurrentCameraZone = true;

			if (!delayTimerTriggered) {
				delayTimerTriggered = true;

				// We can set up the delay timer now
				if (FiringDelay > 0) {
					delayTimer.WaitTime = FiringDelay;
					delayTimer.Start();
				} else {
					CallDeferred(MethodName.ShootProjectile);
					SetUpShooterTimer();
				}
			}
		}
    }

    public void OnCameraZoneExited(int ID)
    {
        if (cameraZone != ID && isGameInCurrentCameraZone) { // We are leaving a different room to enter this room, so unpause the shooter
			shooterTimer.Paused = false;
			delayTimer.Paused = false;
		} else if (cameraZone == ID) { // We are leaving this room
			isGameInCurrentCameraZone = false;
		}
    }

    public virtual void AttachSignals(Level level)
    {
        
    }
}
