using Godot;
using System;

/// <summary>
/// StateSaverComponent can be attached to a node to help it maintain its own state, then save that state when the player reaches a checkpoint, or reload it if the player died in that room.
/// </summary>
public partial class StateSaverComponent : Node {
	// Reference to the parent node
	private IHasSaveData parent;

	// References to the parent functions
	private Events.PlayerHitEventHandler onPlayerHit;
	private Events.ReachedCheckpointEventHandler onReachedCheckpoint;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		var tempParent = GetParent();

		if (tempParent is IHasSaveData) {
			parent = tempParent as IHasSaveData;

			onPlayerHit = parent.ReloadCurrentState;
			onReachedCheckpoint = parent.SaveCurrentState;

			// Set up the event signals
			Events.Instance.PlayerHit += onPlayerHit;
			Events.Instance.ReachedCheckpoint += onReachedCheckpoint;
		}
	}

    public override void _ExitTree() {
		Events.Instance.PlayerHit -= onPlayerHit;
		Events.Instance.ReachedCheckpoint -= onReachedCheckpoint;

        base._ExitTree();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {}
}
