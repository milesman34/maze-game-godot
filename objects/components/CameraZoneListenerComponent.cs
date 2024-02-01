using Godot;
using System;

// This attachable component lets the parent figure out what camera zone it is in and run functions when the camera zone is changed
public partial class CameraZoneListenerComponent : Node
{
	// Track the parent node
	private ICameraZoneListener parent;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var tempParent = GetParent();

		if (tempParent is ICameraZoneListener) {
			parent = tempParent as ICameraZoneListener;

			Events.instance.CameraZoneEntered += parent.OnCameraZoneEntered;
			Events.instance.CameraZoneExited += parent.OnCameraZoneExited;

			Events.instance.RegisterCameraZone += OnRegisterCameraZone;	

			// Emit the camera zone listener registration signal to make sure camera zones can check if this object is in the camera zone
			Events.instance.EmitSignal(Events.SignalName.RegisterCameraZoneListener, parent as Node2D);	
		}
	}

	public override void _ExitTree()
    {
        base._ExitTree();

		Events.instance.CameraZoneEntered -= parent.OnCameraZoneEntered;
		Events.instance.CameraZoneExited -= parent.OnCameraZoneExited;
		Events.instance.RegisterCameraZone -= OnRegisterCameraZone;
    }

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

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
