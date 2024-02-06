using Godot;
using System;

/// <summary>
/// This component can be attached to an object so that it can figure out what camera zone it is in and run functions when the current camera zone is changed.
/// It is useful for making an object pause or resume when the player exits/enters its camera zone.
/// </summary>
public partial class CameraZoneListenerComponent : Node {
	// Track the parent node
	private ICameraZoneListener parent;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		var tempParent = GetParent();

		if (tempParent is ICameraZoneListener) {
			parent = tempParent as ICameraZoneListener;

			// Set up the event signals for the parent object
			Events.Instance.CameraZoneEntered += parent.OnCameraZoneEntered;
			Events.Instance.CameraZoneExited += parent.OnCameraZoneExited;

			Events.Instance.RegisterCameraZone += OnRegisterCameraZone;	

			// Emit the camera zone listener registration signal to make sure camera zones can check if this object is in the camera zone
			Events.Instance.EmitSignal(Events.SignalName.RegisterCameraZoneListener, parent as Node2D);	
		}
	}

	public override void _ExitTree() {
        base._ExitTree();

		Events.Instance.CameraZoneEntered -= parent.OnCameraZoneEntered;
		Events.Instance.CameraZoneExited -= parent.OnCameraZoneExited;
		Events.Instance.RegisterCameraZone -= OnRegisterCameraZone;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {}

	// Runs when a camera zone is registered
	private void OnRegisterCameraZone(CameraZone zone) {
		// ICameraZoneListener should always be a Node2D
		if (!(parent is Node2D)) {
			throw new Exception("The parent could not be casted to Node2D to access its position, so this object cannot belong to a camera zone!");
		}

		if (zone.IsVectorInBounds((parent as Node2D).GlobalPosition)) {
			parent.SetCameraZoneID(zone.ID);
		}
	}
}
