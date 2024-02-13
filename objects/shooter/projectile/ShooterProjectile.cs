using Godot;
using System;

/// <summary>
/// This object represents a projectile that has been shot by a Shooter.
/// </summary>
public partial class ShooterProjectile : CharacterBody2D, ICameraZoneListener {
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
	private Projectile projectileResource;

	// Reference to the sprite
	private Sprite2D sprite;

	// Speed that the projectile rotates at (pixels per second)
	private const double RotationSpeed = 30;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		// Set the texture
		sprite = GetNode<Sprite2D>("Sprite");
		
		sprite.Texture = projectileResource.ProjectileTexture;

		// Disable the main collision shape at first
		mainCollisionShape = GetNode<CollisionShape2D>("MainCollisionShape");
		mainCollisionShape.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);

		Events.Instance.PlayerHit += OnPlayerHit;
	}

    public override void _ExitTree() {
        base._ExitTree();
		Events.Instance.PlayerHit -= OnPlayerHit;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {
		// Handle rotation first
		if (projectileResource.Rotates) {
			RotationDegrees += (float) (RotationSpeed * delta);
		}

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

    public void SetCameraZoneID(int ID) {
        cameraZone = ID;
    }

	/// <summary>
	/// Creates an instance of a Projectile.
	/// </summary>
	/// <param name="scene">Reference to the ShooterProjectile scene to instantiate the projectile</param>
	/// <param name="projectileResource">Projectile resource to use/param>
	/// <param name="speed">Speed of the projectile</param>
	/// <param name="angle">Angle of the projectile</param>
	/// <returns></returns>
	public static ShooterProjectile InstantiateProjectile(PackedScene scene, Projectile projectileResource, float speed, float angle) {
		var projectile = scene.Instantiate<ShooterProjectile>();
		projectile.projectileResource = projectileResource;
		projectile.speed = speed;
		projectile.angle = angle;

		// Set up the unit vector
		projectile.unitVector = new Vector2((float) Math.Cos(angle * Math.PI / 180), (float) Math.Sin(angle * Math.PI / 180)).Normalized();

		return projectile;
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

    public void OnCameraZoneEntered(int ID) {
		if (cameraZone != ID) {
			QueueFree();
		}
    }

    public void OnCameraZoneExited(int ID) {
		if (cameraZone == ID) {
			QueueFree();
		}
	}
}
