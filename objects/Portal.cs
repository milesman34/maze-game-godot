using Godot;
using System;

/// <summary>
/// Portals let the player teleport to a new location upon hitting them.
/// </summary>
public partial class Portal : Area2D
{
	/// <summary>
	/// Color of the portal.
	/// </summary>
	[Export]
	public Color Color { get; set; } = new Color(255, 255, 255);

	/// <summary>
	/// Position in units that the portal teleports the player to.
	/// </summary>
	[Export]
	public Vector2 Target { get; set; }

	/// <summary>
	/// PortalEntered signal is emitted when the player enters the portal.
	/// </summary>
	/// <param name="target">Target position to teleport to</param>
	[Signal]
	public delegate void PortalEnteredEventHandler(Vector2 target);

	/// <summary>
	/// PortalExited signal is emitted when the player exits the portal.
	/// </summary>
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

	/// <summary>
	/// OnBodyEntered is called when the portal is entered by a body.
	/// </summary>
	/// <param name="body">Body that enters the portal.</param>
	public void OnBodyEntered(Node2D body) {
		if (body is Player) {
			EmitSignal(SignalName.PortalEntered, Target);
		}
	}

	/// <summary>
	/// OnBodyExited is called when the portal is exited by a body.
	/// </summary>
	/// <param name="body">Body that exits the portal.</param>
	public void OnBodyExited(Node2D body) {
		if (body is Player) {
			EmitSignal(SignalName.PortalExited);
		}
	}
}
