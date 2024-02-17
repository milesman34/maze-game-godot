using Godot;
using System;

public partial class CameraZoneMoving : CameraZone {
	/// <summary>
	/// Size of the internal camera display in units.
	/// </summary>
	[Export]
	public Vector2 InternalSize;

	// Reference to the player's current position
	private Vector2 playerPos = Vector2.Zero;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		base._Ready();

		// The moving camera zone can display the whole background, it just needs to override the function for the camera zone position/scale
		Events.Instance.PlayerPositionChanged += OnPlayerPositionChanged;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {}

    public override void _ExitTree() {
		Events.Instance.PlayerPositionChanged -= OnPlayerPositionChanged;

        base._ExitTree();
    }

	// Calculates the default positioning regardless of the player
	private Vector2 GetDefaultPos() {
		return StartPosition * Constants.TileSize + InternalSize * Constants.TileSize / 2.0f;
	}
	
	// Helper function to calculate the new camera position (since calling GetCameraPosition apparently doesn't work from certain methods?)
	// I really don't know why it won't just let me call GetCameraPosition from the UpdateBackgroundSizeAndPosition function
	// But since it keeps not returning the correct answer, I'll just make a helper function
	private Vector2 GetCameraPosHelper() {
		// To calculate the position, we want the camera to be in the center of the room
		var myPosition = StartPosition * Constants.TileSize;

		var internalSizePixels = InternalSize * Constants.TileSize;

		var defaultPos = GetDefaultPos();

		// This seems to work I guess
		var newPos = new Vector2(
			Math.Max(defaultPos.X, Math.Min(myPosition.X + Size.X * Constants.TileSize - internalSizePixels.X / 2.0f, playerPos.X)),
			Math.Max(defaultPos.Y, Math.Min(myPosition.Y + Size.Y * Constants.TileSize - internalSizePixels.Y / 2.0f, playerPos.Y))
		);

		return newPos;
	}

    public override Vector2 GetCameraPosition() {
		return GetCameraPosHelper();
	}

	public override Vector2 GetCameraZoom() {
        var zoom = GetEffectiveZoom();

        return new Vector2(zoom, zoom);
	}

	protected override float GetEffectiveZoom() {
		var viewportSize = GetViewportRect().Size;

        // Get scale factors for both x and y
        var xScale = viewportSize.X / InternalSize.X;
        var yScale = viewportSize.Y / InternalSize.Y;

        return Math.Min(xScale, yScale) / Constants.TileSize;
    }

	public override Vector2 GetCameraZoneSize() {
		return InternalSize * Constants.TileSize;
	}

	// Runs when the player's position is changed
	private void OnPlayerPositionChanged(Vector2 vector) {
		playerPos = vector;
		CallDeferred(MethodName.UpdateBackgroundSizeAndPosition);
		// GD.Print(playerPos);
		EmitSignal(SignalName.CameraZoneUpdate, id);
	}

	// This variant accepts the camera zone position as a parameter
	protected override void UpdateBackgroundSizeAndPosition() {
		// Seems we just needed to have it relative, based on the background size
		var zoneSize = GetCameraZoneSize();

		// Let's be realistic about the maximum hypothetical size that an object in the room could extend past it
		var ExtendFactor = 10;

		// These positions seem to be based on the top left coordinate
		backgroundLeft.Size = new Vector2(zoneSize.X * ExtendFactor, zoneSize.Y * ExtendFactor * 3);
		backgroundLeft.Position = new Vector2(-zoneSize.X * ExtendFactor - zoneSize.X / 2f, -zoneSize.Y * ExtendFactor * 1.5f);
		backgroundRight.Size = new Vector2(zoneSize.X * ExtendFactor, zoneSize.Y * ExtendFactor * 5); // backgroundRight wasn't rendering how I wanted so I increased the size
		backgroundRight.Position = new Vector2(zoneSize.X / 2.0f, -zoneSize.Y * ExtendFactor * 2.5f);
		backgroundTop.Size = new Vector2(zoneSize.X, zoneSize.Y * ExtendFactor);
		backgroundTop.Position = new Vector2(-zoneSize.X / 2.0f, -zoneSize.Y * ExtendFactor - zoneSize.Y / 2.0f);
		backgroundBottom.Size = new Vector2(zoneSize.X, zoneSize.Y * ExtendFactor);
		backgroundBottom.Position = new Vector2(-zoneSize.X / 2.0f, zoneSize.Y / 2.0f);

		// If I had to guess, its going to be the difference between the real size and internal size
		var defaultPos = GetDefaultPos();
		var cameraPos = GetCameraPosHelper();

		// The position is based on subtraction, so we start with the difference between the real size and internal size on one side, then subtract the difference between the default position and the current camera position
		backgroundContainer.Position = -(Size - InternalSize) * Constants.TileSize / 2.0f - (defaultPos - cameraPos);
	}
}
