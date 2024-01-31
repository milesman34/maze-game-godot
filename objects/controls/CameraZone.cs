using Godot;
using System;

/// <summary>
/// CameraZones manage a section of the camera, creating the appearance of a room in the level.
/// </summary>
public partial class CameraZone : Area2D, IGameObject
{
	// Current unique ID to be given to a camera zone
	private static int currentID;

	// ID of this camera zone
	public int ID;

	// Position of the camera zone in units
	[Export]
	public Vector2 StartPosition {get; set; }

	// Size of the camera zone in units
	[Export]
	public Vector2 Size { get; set; }

	// Signal for when the viewport size is updated
	[Signal]
	public delegate void CameraZoneUpdateEventHandler(int ID);

	// Reference to the various background objects
	public Node2D backgroundContainer;

	private ColorRect backgroundLeft, backgroundRight, backgroundTop, backgroundBottom;

	// Position of the top left and bottom right coordinates
	private Vector2 topLeft, bottomRight;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Get references to certain objects
		backgroundContainer = GetNode<Node2D>("BackgroundContainer");

		backgroundLeft = backgroundContainer.GetNode<ColorRect>("BackgroundLeft");
		backgroundRight = backgroundContainer.GetNode<ColorRect>("BackgroundRight");
		backgroundTop = backgroundContainer.GetNode<ColorRect>("BackgroundTop");
		backgroundBottom = backgroundContainer.GetNode<ColorRect>("BackgroundBottom");

		// The surrounding background starts hidden, then the borders are shown when the player enters a room
		backgroundContainer.Hide();

		// Set ID of this CameraZone
		ID = currentID;
		currentID++;

		// Set the current position
		Position = StartPosition * Constants.TileSize + Size * Constants.TileSize / 2.0f;

        // Set the size of the collision shape
        var rectSize = new RectangleShape2D
        {
            Size = Size * Constants.TileSize
        };

        GetNode<CollisionShape2D>("CollisionShape").Shape = rectSize;

		// Get the corner positions
		topLeft = Position - Size * Constants.TileSize / 2;
		bottomRight = Position + Size * Constants.TileSize / 2;

		// Set up the code for helping objects determine which camera zone they are in if registered later
		Events.instance.RegisterCameraZoneListener += OnRegisterCameraZoneListener;

		// Emit the camera zone registration signal for helping objects know which camera zone they belong to
		Events.instance.EmitSignal(Events.SignalName.RegisterCameraZone, this);
		
		// Set background position/scale
		CallDeferred(MethodName.UpdateBackgroundSizeAndPosition);
	}

    public override void _ExitTree()
    {
        base._ExitTree();

		Events.instance.RegisterCameraZoneListener -= OnRegisterCameraZoneListener;
    }

    // Returns if a point is in the camera zone
    public bool IsVectorInBounds(Vector2 vector) {
		return vector.X >= topLeft.X && vector.X <= bottomRight.X && vector.Y >= topLeft.Y && vector.Y <= bottomRight.Y;
	}

	// Returns the size of the camera zone in pixels
	public Vector2 GetCameraZoneSize() {
		return Size * Constants.TileSize;
	}

	// Returns the position of the camera zone in pixels (based on where it is to have it in the center of the screen)
	public Vector2 GetCameraZonePosition() {
		return -GetCameraZoneSize() / 2.0f;
	}

	// Updates the background's size and position. This is separated into a new function so that it can be deferred.
	private void UpdateBackgroundSizeAndPosition() {
		// Seems we just needed to have it relative, based on the background size
		var zoneSize = GetCameraZoneSize();

		// Let's be realistic about the maximum hypothetical size that an object in the room could extend past it
		var ExtendFactor = 10;

		// These positions seem to be based on the top left coordinate
		backgroundLeft.Size = new Vector2(zoneSize.X * ExtendFactor, zoneSize.Y * ExtendFactor * 3);
		backgroundLeft.Position = new Vector2(-zoneSize.X * ExtendFactor - zoneSize.X / 2f, -zoneSize.Y * ExtendFactor * 1.5f);
		backgroundRight.Size = new Vector2(zoneSize.X * ExtendFactor, zoneSize.Y * ExtendFactor * 3);
		backgroundRight.Position = new Vector2(zoneSize.X / 2.0f, -zoneSize.Y / 2.0f);
		backgroundTop.Size = new Vector2(zoneSize.X, zoneSize.Y * ExtendFactor);
		backgroundTop.Position = new Vector2(-zoneSize.X / 2.0f, -zoneSize.Y * ExtendFactor - zoneSize.Y / 2.0f);
		backgroundBottom.Size = new Vector2(zoneSize.X, zoneSize.Y * ExtendFactor);
		backgroundBottom.Position = new Vector2(-zoneSize.X / 2.0f, zoneSize.Y / 2.0f);

		// backgroundLeft.Color = new Color(255, 0, 0);
		// backgroundRight.Color = new Color(0, 255, 0);
		// backgroundTop.Color = new Color(0, 0, 255);
		// backgroundBottom.Color = new Color(255, 0, 255);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// Calculates the effective zoom level, based on the viewport size. The zoom level is the same for both axes.
    private float GetEffectiveZoom() {
		var viewportSize = GetViewportRect().Size;

        // Get scale factors for both x and y
        var xScale = viewportSize.X / Size.X;
        var yScale = viewportSize.Y / Size.Y;

        return Math.Min(xScale, yScale) / Constants.TileSize;
    }

	/// <summary>
	/// Calculates the position for the camera to be in, which the Level needs to know.
	/// </summary>
	/// <returns>Camera position</returns>
	public Vector2 GetCameraPosition() {
		// To calculate the position, we want the camera to be in the center of the room
		var myPosition = StartPosition * Constants.TileSize;
		var offsetPosition = myPosition + Size * Constants.TileSize / 2.0f;

		return offsetPosition;
	}

	/// <summary>
	/// Returns the zoom level of the camera as a vector.
	/// </summary>
	/// <returns>Camera zoom level</returns>
	public Vector2 GetCameraZoom() {
        var zoom = GetEffectiveZoom();

        return new Vector2(zoom, zoom);
	}

	// Runs whenever the window resizes
    public void OnWindowResize() {
        EmitSignal(SignalName.CameraZoneUpdate, ID);
		UpdateBackgroundSizeAndPosition();
    }

	// Runs whenever another body enters the camera zone
	private void OnBodyEntered(Node2D body) {
		if (body is Player) {
			SetSurroundingVisibility(true);

			Events.instance.EmitSignal(Events.SignalName.CameraZoneEntered, ID);
		}
	}

	// Runs whenever another body exits the camera zone
	private void OnBodyExited(Node2D body) {
		if (body is Player) {
			SetSurroundingVisibility(false);
			
			Events.instance.EmitSignal(Events.SignalName.CameraZoneExited, ID);
		}
	}

	// Sets the background color
	public void SetBackgroundColor(Color color) {
		// background.Color = color;
	}

	// Sets if the surrounding background is visible
	public void SetSurroundingVisibility(bool visible) {
		if (visible) {
			backgroundContainer.Show();
		} else {
			backgroundContainer.Hide();
		}
	}

	private void OnRegisterCameraZoneListener(Node2D node) {
		if (node is ICameraZoneListener) {
			var listener = node as ICameraZoneListener;

			if (IsVectorInBounds(node.GlobalPosition)) {
				listener.SetCameraZoneID(ID);
			}
		}
	}

	public void AttachSignals(Level level) {
		// Set the CameraZone in the dictionary
		level.SetCameraZone(ID, this);

        // Signals for updating the camera zone
        CameraZoneUpdate += level.OnCameraZoneUpdate;

        // Also attach the signal for the game viewport changing size
        level.mainGame.gameViewport.SizeChanged += OnWindowResize;
	}
}
