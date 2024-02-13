using Godot;
using System;

/// <summary>
/// GlowingLavaTraps are a type of obstacle which glow several times to warn the player, then become lava, which fades out over some amount of time.
/// </summary>
[Tool]
public partial class GlowingLavaTrap : Area2D, ICameraZoneListener {
	/// <summary>
	/// Possible states for the GlowingLavaTrap
	/// </summary>
	private enum GlowingLavaTrapState {
		/// <summary>
		/// The lava trap is waiting to activate the main cycle timer.
		/// </summary>
		ActivationWait,
		
		/// <summary>
		/// The main warning part of the cycle.
		/// </summary>
		CycleStart,

		/// <summary>
		/// The part of the cycle where the lava hurts the player.
		/// </summary>
		CycleHarm,

		/// <summary>
		/// The part between cycles before starting the next cycle.
		/// </summary>
		BetweenCycles,

		/// <summary>
		/// State when the glowing lava trap is disabled.
		/// </summary>
		Disabled
	}

	/// <summary>
	/// How long in between each glow/pulse?
	/// </summary>
	[Export]
	public float GlowDelay { get; set; } = 2;

	/// <summary>
	/// How long should each glow take to fade out?
	/// </summary>
	[Export]
	public float GlowFadeTime { get; set; } = 0.5f;

	/// <summary>
	/// How long should the lava take to fade out?
	/// </summary>
	[Export]
	public float LavaFadeTime { get; set; } = 1;

	/// <summary>
	/// How long should it take after the lava finishes fading to start the next cycle
	/// </summary>
	[Export]
	public float TimeBetweenCycles { get; set; } = 4;

	/// <summary>
	/// How much time should the lava trap take to activate the timer
	/// </summary>
	[Export]
	public float ActivationTimeDelay { get; set; } = 0;

	/// <summary>
	/// How many times the lava trap should glow before turning to lava
	/// </summary>
	[Export]
	public int GlowTimes { get; set; } = 2;

	// Reference to the collision object
	private CollisionShape2D collisionShape;

	// Reference to the activation delay timer
	private Timer activationDelayTimer;

	// Reference to the glow timer
	private Timer glowTimer;

	// Reference to the post cycle timer
	private Timer postCycleTimer;

	// Reference to the lava harm timer
	private Timer lavaHarmTimer;

	// Reference to the animation player
	private AnimationPlayer animationPlayer;

	// Number of elapsed pulses for the current cycle
	private int elapsedPulses = 0;

	// Track the current camera zone this object is in
	private int cameraZone = -1;

	// Are we in the current camera zone ID?
	private bool isGameInCurrentCameraZone = false;

	// Current state of the lava trap
	private GlowingLavaTrapState currentState;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		SetUpDefaultSpriteAndCollisionProperties();

		// Set up other references
		glowTimer = GetNode<Timer>("GlowTimer");
		glowTimer.WaitTime = GlowDelay;

		postCycleTimer = GetNode<Timer>("PostCycleTimer");
		postCycleTimer.WaitTime = TimeBetweenCycles;

		lavaHarmTimer = GetNode<Timer>("LavaHarmTimer");
		lavaHarmTimer.WaitTime = LavaFadeTime;

		activationDelayTimer = GetNode<Timer>("ActivationDelayTimer");

		// Set up animations
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

		SwitchState(GlowingLavaTrapState.ActivationWait);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {}

    public void SetCameraZoneID(int ID) {
        cameraZone = ID;
    }

	// Switches the state of the lava trap
	private void SwitchState(GlowingLavaTrapState newState) {
		currentState = newState;

		switch (newState) {
			case GlowingLavaTrapState.ActivationWait:
				// Set up activation timer
				if (ActivationTimeDelay > 0) {
					activationDelayTimer.WaitTime = ActivationTimeDelay;
					activationDelayTimer.Start();
				} else {
					SwitchState(GlowingLavaTrapState.CycleStart);
				}

				break;

			case GlowingLavaTrapState.CycleStart:
				glowTimer.Start();
				break;

			case GlowingLavaTrapState.CycleHarm:
				// Enable special lava pulse
				// To get the animation to be long enough, set the animation speed multi to be the inverse of the target time.
				animationPlayer.SpeedScale = 1 / LavaFadeTime;
				animationPlayer.Play("LavaPulse");

				postCycleTimer.Start();
				lavaHarmTimer.Start();

				// Enable collision
				collisionShape.SetDeferred(CollisionShape2D.PropertyName.Disabled, false);
				break;

			case GlowingLavaTrapState.BetweenCycles:
				// Disable collision
				collisionShape.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
				break;

			case GlowingLavaTrapState.Disabled:
				// Stop all the timers + the animation player
				glowTimer.Stop();
				postCycleTimer.Stop();
				activationDelayTimer.Stop();
				lavaHarmTimer.Stop();

				elapsedPulses = 0;

				animationPlayer.Stop();
				
				SetUpDefaultSpriteAndCollisionProperties();
				break;
		}
	}

	// Sets up the sprite and collision properties by default
	private void SetUpDefaultSpriteAndCollisionProperties() {
		// Make sprite invisible
		GetNode<Sprite2D>("Sprite").SelfModulate = new Color(255, 255, 255, 0);
		GetNode<ColorRect>("Overlay").Color = new Color(255, 255, 255, 0);

		// Disable collision shape
		collisionShape = GetNode<CollisionShape2D>("CollisionShape");
		
		collisionShape.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
	}

    public void OnCameraZoneEntered(int ID) {
        if (cameraZone != ID) { // We are entering a new room, so pause the timers
			OnRoomExited();
		} else { // Set that we are in the current room
			isGameInCurrentCameraZone = true;

			OnRoomEntered();
		}
    }

    public void OnCameraZoneExited(int ID) {
        if (cameraZone != ID && isGameInCurrentCameraZone) { // We are leaving a different room to enter this room, so unpause the shooter
			OnRoomEntered();
		} else if (cameraZone == ID) { // We are leaving this room
			isGameInCurrentCameraZone = false;
			OnRoomExited();
		}
    }

	private void OnActivationDelayTimerTimeout() {
		SwitchState(GlowingLavaTrapState.CycleStart);
	}

	private void OnGlowTimerTimeout() {
		elapsedPulses++;

		if (elapsedPulses > GlowTimes) {
			elapsedPulses = 0;

			SwitchState(GlowingLavaTrapState.CycleHarm);
		} else {
			// Activate pulse effect
			animationPlayer.SpeedScale = 1 / GlowFadeTime;
			animationPlayer.Play("SmallPulse");
		}
	}

	private void OnLavaHarmTimerTimeout() {
		SwitchState(GlowingLavaTrapState.BetweenCycles);
	}

	private void OnPostCycleTimerTimeout() {
		SwitchState(GlowingLavaTrapState.CycleStart);
	}

	// Runs when the current room is entered
	private void OnRoomEntered() {
		SwitchState(GlowingLavaTrapState.ActivationWait);
	}

	// Runs when the current room is exited
	private void OnRoomExited() {
		SwitchState(GlowingLavaTrapState.Disabled);
	}
}
