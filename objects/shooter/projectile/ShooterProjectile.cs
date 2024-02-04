using Godot;
using System;

public partial class ShooterProjectile : CharacterBody2D, ICameraZoneListener
{
	// Speed of the projectile
	private float speed;

	// Angle the projectile travels in
	private float angle;

	// Unit direction vector
	private Vector2 unitVector;

	// Reference to the main collision shape
	private CollisionShape2D mainCollisionShape;

	// Current camera zone
	private int cameraZone = -1;

	// Resource used for the projectile
	public Projectile projectileResource;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Set the texture
		GetNode<Sprite2D>("Sprite").Texture = projectileResource.ProjectileTexture;

		// Disable the main collision shape at first
		mainCollisionShape = GetNode<CollisionShape2D>("MainCollisionShape");
		mainCollisionShape.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);

		Events.instance.PlayerHit += OnPlayerHit;
	}

    public override void _ExitTree()
    {
        base._ExitTree();
		Events.instance.PlayerHit -= OnPlayerHit;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		if (projectileResource.CollidesWithWalls) {
			// Use MoveAndCollide here to detect collisions
			var collision = MoveAndCollide(unitVector * (float) delta * speed);

			if (collision != null) {
				QueueFree();
			}
		} else {
			// Ignore collisions with walls
			Position += unitVector * (float) delta * speed;
		}
	}

	// Probably best to get rid of any active projectiles when the player gets hit, so that they can't immediately get hit again
	private void OnPlayerHit() {
		QueueFree();
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

    public void OnCameraZoneEntered(int ID)
    {
		if (cameraZone != ID) {
			QueueFree();
		}
    }

    public void OnCameraZoneExited(int ID)
    {
    }

}
