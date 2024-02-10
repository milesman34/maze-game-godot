using Godot;
using System;

/// <summary>
/// Shooters shoot projectiles at the player.
/// </summary>
public partial class Shooter : RigidBody2D, ICameraZoneListener, IGameObject
{
	/// <summary>
	/// Wall texture to render.
	/// </summary>
	[Export]
	public Texture2D WallTexture { get; set; }

	/// <summary>
	/// Angle the shooter is rotated by.
	/// </summary>
	[Export]
	public float ShooterAngle { get; set; } = 0;

	/// <summary>
	/// Delay (in seconds) before the first shot is fired.
	/// </summary>
	[Export]
	public float FiringDelay { get; set; } = 0;

	/// <summary>
	/// How many seconds should the shooter take per projectile fired?
	/// </summary>
	[Export]
	public float FiringRate { get; set; } = 5;

	/// <summary>
	/// Speed of the projectile in pixels per second.
	/// </summary>
	[Export]
	public float ProjectileSpeed { get; set; } = 64;

	/// <summary>
	/// Scene used to instantiate the projectile.
	/// </summary>
	[Export]
	public PackedScene ProjectileScene { get; set; }

	/// <summary>
	/// Resource used for the projectile.
	/// </summary>
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
	public override void _Ready() {
		GetNode<Sprite2D>("WallSprite").Texture = WallTexture;

		// Change only the rotation for the sprite itself, not the wall sprite
		GetNode<Sprite2D>("ShooterSprite").RotationDegrees = ShooterAngle;

		// Set up references
		shooterTimer = GetNode<Timer>("ShooterTimer");
		delayTimer = GetNode<Timer>("DelayTimer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {}

    public virtual void AttachSignals(Level level) {}

    public void SetCameraZoneID(int ID) {
        cameraZone = ID;
    }

	// Sets up the shooter timer
	private void SetUpShooterTimer() {
		shooterTimer.WaitTime = FiringRate;

		shooterTimer.Start();
	}

	// Shoots a projectile
	private void ShootProjectile() {
		var projectile = ShooterProjectile.InstantiateProjectile(ProjectileScene, Projectile, ProjectileSpeed, ShooterAngle);

		AddChild(projectile);
	}

	private void OnDelayTimerTimeout() {
		ShootProjectile();
		SetUpShooterTimer();
	}

	private void OnShooterTimerTimeout() {
		ShootProjectile();
	}

    public void OnCameraZoneEntered(int ID) {
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

    public void OnCameraZoneExited(int ID) {
        if (cameraZone != ID && isGameInCurrentCameraZone) { // We are leaving a different room to enter this room, so unpause the shooter
			shooterTimer.Paused = false;
			delayTimer.Paused = false;
		} else if (cameraZone == ID) { // We are leaving this room
			isGameInCurrentCameraZone = false;
		}
    }
}
