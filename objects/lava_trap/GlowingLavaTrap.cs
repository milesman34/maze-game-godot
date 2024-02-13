using Godot;
using System;

/// <summary>
/// GlowingLavaTraps are a type of obstacle which glow several times to warn the player, then become lava, which fades out over some amount of time.
/// </summary>
[Tool]
public partial class GlowingLavaTrap : Area2D {
	/// <summary>
	/// How long in between each glow/pulse?
	/// </summary>
	[Export]
	public float GlowDelay { get; set; } = 4;

	/// <summary>
	/// How long should each glow take to fade out?
	/// </summary>
	[Export]
	public float GlowFadeTime { get; set; } = 1;

	/// <summary>
	/// How long should the lava take to fade out?
	/// </summary>
	[Export]
	public float LavaFadeTime { get; set; } = 3;

	/// <summary>
	/// How long should it take after the lava finishes fading to start the next cycle
	/// </summary>
	[Export]
	public float TimeBetweenCycles { get; set; } = 6;

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

	// Reference to the glow timer
	private Timer glowTimer;

	// Reference to the post cycle timer
	private Timer postCycleTimer;

	// Reference to the animation player
	private AnimationPlayer animationPlayer;

	// Number of elapsed pulses for the current cycle
	private int elapsedPulses = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		// Make sprite invisible
		GetNode<Sprite2D>("Sprite").SelfModulate = new Color(255, 255, 255, 0);

		// Disable collision shape
		collisionShape = GetNode<CollisionShape2D>("CollisionShape");
		
		collisionShape.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);

		// Set up other references
		glowTimer = GetNode<Timer>("GlowTimer");
		glowTimer.WaitTime = GlowDelay;

		postCycleTimer = GetNode<Timer>("PostCycleTimer");
		postCycleTimer.WaitTime = TimeBetweenCycles;

		// Set up animations + lengths of each animation
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

		// Set up activation timer
		if (ActivationTimeDelay > 0) {
			var actTimer = GetNode<Timer>("ActivationDelayTimer");

			actTimer.WaitTime = ActivationTimeDelay;
			actTimer.Start();
		} else {
			StartCycle();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {}

	// Starts the initial cycle of the lava trap
	private void StartCycle() {
		glowTimer.Start();
	}

	private void OnActivationDelayTimerTimeout() {
		StartCycle();
	}

	private void OnGlowTimerTimeout() {
		elapsedPulses++;

		if (elapsedPulses > GlowTimes) {
			elapsedPulses = 0;

			glowTimer.Stop();

			// Enable special lava pulse
			// To get the animation to be long enough, set the animation speed multi to be the inverse of the target time.
			animationPlayer.SpeedScale = 1 / LavaFadeTime;
			animationPlayer.Play("LavaPulse");

			postCycleTimer.Start();
		} else {
			// Activate pulse effect
			animationPlayer.SpeedScale = 1 / GlowFadeTime;
			animationPlayer.Play("SmallPulse");
		}
	}

	private void OnPostCycleTimerTimeout() {
		StartCycle();
	}
}
