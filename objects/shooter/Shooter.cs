using Godot;
using System;

public partial class Shooter : RigidBody2D
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

	// Scene for the projectile to shoot
	[Export]
	public PackedScene ProjectileScene { get; set; }

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

		// Set up something to enable/disable the shooter when you enter/exit the current room
		Events.instance.CameraZoneEntered += OnCameraZoneEntered;
		Events.instance.CameraZoneExited += OnCameraZoneExited;
		
		Events.instance.RegisterCameraZone += OnRegisterCameraZone;	

		// Emit the camera zone listener registration signal to make sure camera zones can check if this object is in the camera zone
		Events.instance.EmitSignal(Events.SignalName.RegisterCameraZoneListener, this);	
	}

	// Handles the registration of a camera zone
	private void OnRegisterCameraZone(CameraZone zone) {
		if (zone.IsVectorInBounds(GlobalPosition)) {
			cameraZone = zone.ID;
			GD.Print(cameraZone);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void _ExitTree()
    {
        base._ExitTree();

		Events.instance.CameraZoneEntered -= OnCameraZoneEntered;
		Events.instance.CameraZoneExited -= OnCameraZoneExited;
		Events.instance.RegisterCameraZone -= OnRegisterCameraZone;
    }

	private void OnCameraZoneEntered(int ID) {
		if (cameraZone != ID) {
			shooterTimer.Paused = true;
		} else {
			isGameInCurrentCameraZone = true;
		}
	}

	private void OnCameraZoneExited(int ID) {
		// We unpause the timer if the exited camera zone was not the current one and we are in the current one
		if (cameraZone != ID && isGameInCurrentCameraZone) {
			shooterTimer.Paused = false;
		} else if (cameraZone == ID) {
			isGameInCurrentCameraZone = false;
		}
	}

	private void OnShooterTimerTimeout() {
		var projectile = ProjectileScene.Instantiate<IShooterProjectile>();

		projectile.SetSpeedAndAngle(ProjectileSpeed, ShooterAngle);

		AddChild((Node2D) projectile);
	}
}
