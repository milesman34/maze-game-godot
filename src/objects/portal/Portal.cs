using Godot;
using System;

/// <summary>
/// Portals let the player teleport to a new location upon hitting them.
/// </summary>
[Tool]
public partial class Portal : Area2D, IGameObject {
	/// <summary>
	/// The color of the portal.
	/// </summary>
	[Export]
	public Color Color { get; set; } = new Color(255, 255, 255);

	/// <summary>
	/// The position (in units) that the player is teleported to.
	/// </summary>
	[Export]
	public Vector2 Target { get; set; }

	/// <summary>
	/// Signal emitted when the player enters a portal.
	/// </summary>
	/// <param name="target">The target location the player is teleported to.</param>
	[Signal]
	public delegate void PortalEnteredEventHandler(Vector2 target);

	/// <summary>
	/// Signal emitted when the player exits a portal.
	/// </summary>
	[Signal]
	public delegate void PortalExitedEventHandler();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		var sprite = GetNode<Sprite2D>("Sprite");

		sprite.Modulate = Color;

		GetNode<AnimationPlayer>("AnimationPlayer").Play("Portal");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {}

	public void AttachSignals(Level level) {
        PortalEntered += level.OnPortalEntered;
        PortalExited += level.OnPortalExited;
	}

	// Runs when another body enters the portal
	private void OnBodyEntered(Node2D body) {
		if (body is Player) {
			EmitSignal(SignalName.PortalEntered, Target);
		}
	}

	// Runs when another body exits the portal
	private void OnBodyExited(Node2D body) {
		if (body is Player) {
			EmitSignal(SignalName.PortalExited);
		}
	}
}
