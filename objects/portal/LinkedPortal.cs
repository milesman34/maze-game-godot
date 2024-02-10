using Godot;
using System;

/// <summary>
/// LinkedPortals are a utility for creating portals, which ensure the 2 positions are linked to each other.
/// </summary>
[Tool]
public partial class LinkedPortal : Node2D, IGameObject {
	/// <summary>
	/// The color of the portal.
	/// </summary>
	[Export]
	public Color Color { get; set; } = new Color(255, 255, 255);

	/// <summary>
	/// The first position of the linked portal.
	/// </summary>
	[Export]
	public Vector2 Position1 { get; set; }
	
	/// <summary>
	/// The second position of the linked portal.
	/// </summary>
	[Export]
	public Vector2 Position2 { get; set; }

	/// <summary>
	/// The portal scene used to instantiate the portals.
	/// </summary>
	[Export]
	public PackedScene PortalScene { get; set; }

	// References to the two portals
	private Portal portal1, portal2;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		portal1 = PortalScene.Instantiate<Portal>();
		portal2 = PortalScene.Instantiate<Portal>();

		portal1.Color = Color;
		portal2.Color = Color;

		// Position is relative to the location of the linked portal
		portal1.Position = Position1 * Constants.TileSize - Position + Constants.TileVector / 2.0f;
		portal2.Position = Position2 * Constants.TileSize - Position + Constants.TileVector / 2.0f;

		portal1.Target = Position2;
		portal2.Target = Position1;
		
		AddChild(portal1);
		AddChild(portal2);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {}

    public void AttachSignals(Level level) {
        portal1.AttachSignals(level);
		portal2.AttachSignals(level);
    }

}
