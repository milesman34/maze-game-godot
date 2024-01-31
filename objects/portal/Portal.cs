using Godot;
using System;

/// <summary>
/// Portals let the player teleport to a new location upon hitting them.
/// </summary>
public partial class Portal : Area2D, IGameObject
{
	// Color of the portal
	[Export]
	public Color Color { get; set; } = new Color(255, 255, 255);

	// Position in units that the player is teleported to
	[Export]
	public Vector2 Target { get; set; }

	// Signal for when the player enters the portal
	[Signal]
	public delegate void PortalEnteredEventHandler(Vector2 target);

	// Signal for when the player exits the portal
	[Signal]
	public delegate void PortalExitedEventHandler();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var sprite = GetNode<Sprite2D>("Sprite");

		sprite.Modulate = Color;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
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

	public void AttachSignals(Level level) {
        PortalEntered += level.OnPortalEntered;
        PortalExited += level.OnPortalExited;
	}
}
