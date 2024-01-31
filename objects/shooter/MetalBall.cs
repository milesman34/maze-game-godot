using Godot;
using System;

public partial class MetalBall : CharacterBody2D, IShooterProjectile, ICameraZoneListener
{
	// Speed of the metal ball
	private float speed;

	// Angle the ball travels in
	private float angle;

	// Unit direction vector
	private Vector2 unitVector;

	// Reference to the main collision shape
	private CollisionShape2D mainCollisionShape;

	// Track the current camera zone this object is in
	private int cameraZone = -1;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Disable the main collision shape at first
		mainCollisionShape = GetNode<CollisionShape2D>("MainCollisionShape");
		mainCollisionShape.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);

		// Set up something to remove the projectile when another camera zone is entered
		Events.instance.CameraZoneEntered += OnCameraZoneEntered;

		Events.instance.RegisterCameraZone += OnRegisterCameraZone;	

		// Emit the camera zone listener registration signal to make sure camera zones can check if this object is in the camera zone
		Events.instance.EmitSignal(Events.SignalName.RegisterCameraZoneListener, this);	
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var collision = MoveAndCollide(unitVector * (float) delta * speed);

		if (collision != null) {
			QueueFree();
		}
	}

    public override void _ExitTree()
    {
        base._ExitTree();

		Events.instance.CameraZoneEntered -= OnCameraZoneEntered;
		Events.instance.RegisterCameraZone -= OnRegisterCameraZone;
    }

	private void OnAreaBodyEntered(PhysicsBody2D body) {
		// The DamagePlayerComponent takes care of player damage, but we still need the metal ball to disappear when it hits the player
		if (body is Player) {
			QueueFree();
		}
	}

	private void OnAreaBodyExited(PhysicsBody2D body) {
		if (body is Shooter) {
			// Now we can enable the collision shape as it has left the shooter
			mainCollisionShape.SetDeferred(CollisionShape2D.PropertyName.Disabled, false);
		}
	}

	private void OnAreaExited(Area2D area) {
		if (area is CameraZone) {
			// If the metal ball exits the current camera zone, just remove it
			QueueFree();
		}
	}

	private void OnCameraZoneEntered(int ID) {
		if (cameraZone != ID) {
			QueueFree();
		}
	}

	// Handles the registration of a camera zone
	public void OnRegisterCameraZone(CameraZone zone) {
		if (zone.IsVectorInBounds(GlobalPosition)) {
			cameraZone = zone.ID;
		}
	}

    public void SetSpeedAndAngle(float speed, float angle)
    {
        this.speed = speed;
		this.angle = angle;

		unitVector = new Vector2((float) Math.Cos(angle * Math.PI / 180), (float) Math.Sin(angle * Math.PI / 180)).Normalized();
    }

    public void SetCameraZoneID(int ID)
    {
        cameraZone = ID;
    }
}
