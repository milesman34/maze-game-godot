using Godot;
using System;

/// <summary>
/// CameraZones manage a section of the camera, creating the appearance of a room in the level.
/// </summary>
public partial class CameraZone : Area2D, IGameObject
{
	/// <summary>
	/// Position of the camera zone (in units).
	/// </summary>
	[Export]
	public Vector2 StartPosition {get; set; }

	/// <summary>
	/// Size of the camera zone (in units).
	/// </summary>
	[Export]
	public Vector2 Size { get; set; }
	
	/// <summary>
	/// Texture to display for the background for this room.
	/// </summary>
	[Export]
	public Texture2D BackgroundTexture { get; set; }

	/// <summary>
	/// Signal sent out when the camera zone is updated (when the window is resized usually).
	/// </summary>
	/// <param name="ID">ID of this camera zone</param>
	[Signal]
	public delegate void CameraZoneUpdateEventHandler(int ID);

	// Current unique ID to be given to a camera zone
	private static int currentID;

	// ID of this camera zone
	protected int id;

	/// <summary>
	/// This camera zone's ID
	/// </summary>
	public int ID { 
		get {
			return id;
		}
	}

	// Reference to the various background objects
	protected Node2D backgroundContainer;

	protected ColorRect backgroundLeft, backgroundRight, backgroundTop, backgroundBottom;

	// Position of the top left and bottom right coordinates
	protected Vector2 topLeft, bottomRight;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		// Get references to certain objects
		backgroundContainer = GetNode<Node2D>("BackgroundContainer");

		backgroundLeft = backgroundContainer.GetNode<ColorRect>("BackgroundLeft");
		backgroundRight = backgroundContainer.GetNode<ColorRect>("BackgroundRight");
		backgroundTop = backgroundContainer.GetNode<ColorRect>("BackgroundTop");
		backgroundBottom = backgroundContainer.GetNode<ColorRect>("BackgroundBottom");

		// The surrounding background starts hidden, then the borders are shown when the player enters a room
		backgroundContainer.Hide();

		// Set ID of this CameraZone
		id = currentID;
		currentID++;

		// Set the current position
		Position = StartPosition * Constants.TileSize + Size * Constants.TileSize / 2.0f;

        // Set the size of the collision shape
        var rectSize = new RectangleShape2D {
            Size = Size * Constants.TileSize
        };

        GetNode<CollisionShape2D>("CollisionShape").Shape = rectSize;

		// Get the corner positions
		topLeft = Position - Size * Constants.TileSize / 2;
		bottomRight = Position + Size * Constants.TileSize / 2;

		// Set up the code for helping objects determine which camera zone they are in if registered later
		Events.Instance.RegisterCameraZoneListener += OnRegisterCameraZoneListener;

		// Emit the camera zone registration signal for helping objects know which camera zone they belong to
		Events.Instance.EmitSignal(Events.SignalName.RegisterCameraZone, this);
		
		// Set background position/scale
		CallDeferred(MethodName.UpdateBackgroundSizeAndPosition);
	}

    public override void _ExitTree() {
        base._ExitTree();

		Events.Instance.RegisterCameraZoneListener -= OnRegisterCameraZoneListener;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {}

	public void AttachSignals(Level level) {
		// Set the CameraZone in the dictionary
		level.AddCameraZone(this);

        // Signals for updating the camera zone
        CameraZoneUpdate += level.OnCameraZoneUpdate;

        // Also attach the signal for the game viewport changing size
        level.Game.GameViewport.SizeChanged += OnWindowResize;
	}

    /// <summary>
	/// Returns if a position vector is contained within the bounds of the camera zone.
	/// </summary>
	/// <param name="vector">The point in question</param>
	/// <returns></returns>
    public bool IsVectorInBounds(Vector2 vector) {
		return vector.X >= topLeft.X && vector.X <= bottomRight.X && vector.Y >= topLeft.Y && vector.Y <= bottomRight.Y;
	}

	/// <summary>
	/// Returns the size of the camera zone in pixels.
	/// </summary>
	/// <returns></returns>
	public virtual Vector2 GetCameraZoneSize() {
		return Size * Constants.TileSize;
	}

	/// <summary>
	/// Returns the position of the camera zone in pixels (based on the position of its center).
	/// </summary>
	/// <returns></returns>
	public Vector2 GetCameraZonePosition() {
		return -GetCameraZoneSize() / 2.0f;
	}

	// Updates the background's size and position. This is separated into a new function so that it can be deferred.
	protected virtual void UpdateBackgroundSizeAndPosition() {
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
	}

	// Calculates the effective zoom level, based on the viewport size. The zoom level is the same for both axes.
    protected virtual float GetEffectiveZoom() {
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
	public virtual Vector2 GetCameraPosition() {
		// To calculate the position, we want the camera to be in the center of the room
		var myPosition = StartPosition * Constants.TileSize;
		var offsetPosition = myPosition + Size * Constants.TileSize / 2.0f;

		return offsetPosition;
	}

	/// <summary>
	/// Returns the zoom level of the camera as a vector.
	/// </summary>
	/// <returns>Camera zoom level</returns>
	public virtual Vector2 GetCameraZoom() {
        var zoom = GetEffectiveZoom();

        return new Vector2(zoom, zoom);
	}

	/// <summary>
	/// Sets if the surrounding background is visible. If it is visible, then it blocks the visibility of other parts of the level.
	/// </summary>
	/// <param name="visible">Should the surrounding background be visible, blocking the visibility of other parts of the level?</param>
	public void SetSurroundingVisibility(bool visible) {
		if (visible) {
			backgroundContainer.Show();
		} else {
			backgroundContainer.Hide();
		}
	}

	// Runs whenever the window resizes
    private void OnWindowResize() {
        EmitSignal(SignalName.CameraZoneUpdate, ID);
		UpdateBackgroundSizeAndPosition();
    }

	// Runs whenever another body enters the camera zone
	private void OnBodyEntered(Node2D body) {
		if (body is Player) {
			SetSurroundingVisibility(true);

			Events.Instance.EmitSignal(Events.SignalName.CameraZoneEntered, ID);
		}
	}

	// Runs whenever another body exits the camera zone
	private void OnBodyExited(Node2D body) {
		if (body is Player) {
			SetSurroundingVisibility(false);
			
			Events.Instance.EmitSignal(Events.SignalName.CameraZoneExited, ID);
		}
	}

	// Runs when a camera zone listener is registered
	private void OnRegisterCameraZoneListener(Node2D node) {
		if (node is ICameraZoneListener) {
			var listener = node as ICameraZoneListener;

			if (IsVectorInBounds(node.GlobalPosition)) {
				listener.SetCameraZoneID(ID);
			}
		}
	}
}
