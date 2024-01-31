using Godot;
using System;

public partial class MetalBall : CharacterBody2D, IShooterProjectile
{
	// Speed of the metal ball
	private float speed;

	// Angle the ball travels in
	private float angle;

	// Unit direction vector
	private Vector2 unitVector;

	// Reference to the main collision shape
	private CollisionShape2D mainCollisionShape;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Disable the main collision shape at first
		mainCollisionShape = GetNode<CollisionShape2D>("MainCollisionShape");
		mainCollisionShape.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var collision = MoveAndCollide(unitVector * (float) delta * speed);

		if (collision != null) {
			QueueFree();
		}
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

}
