using Godot;
using System;

/// <summary>
/// Lava trap that activates after the player touches it, after a delay.
/// </summary>
public partial class LavaTouchTrap : Area2D {
	/// <summary>
	/// Possible states for the lava touch trap.
	/// </summary>
	private enum LavaTouchTrapState {
		/// <summary>
		/// The trap has not been activated.
		/// </summary>
		Off,
		
		/// <summary>
		/// The track has been activated but is still waiting to harm the player.
		/// </summary>
		Activated,

		/// <summary>
		/// The trap has been activated and can harm the player.
		/// </summary>
		On
	}

	/// <summary>
	/// How many seconds the trap waits after being activated until it hurts the player.
	/// </summary>
	[Export]
	public float ActivateDelay { get; set; } = 2;

	/// <summary>
	/// How long the lava should take to fade out.
	/// </summary>
	[Export]
	public float LavaFadeTime { get; set; } = 1;

	// Reference to each of the sprites
	private Sprite2D baseSprite;
	private Sprite2D activatedSprite;
	private Sprite2D lavaSprite;

	// Reference to the activation timer
	private Timer activationTimer;

	// Reference to the lava fade timer
	private Timer fadeTimer;

	// Reference to the animation player
	private AnimationPlayer animationPlayer;

	// Current state of the trap
	private LavaTouchTrapState currentState;

	// Reference to the collision shape that harms the player
	private CollisionShape2D harmCollisionShape;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		// Get references to the sprites
		baseSprite = GetNode<Sprite2D>("BaseSprite");
		activatedSprite = GetNode<Sprite2D>("ActivatedSprite");
		lavaSprite = GetNode<Sprite2D>("%LavaSprite");

		// Make the non-base sprites invisible for now
		activatedSprite.Visible = false;
		lavaSprite.Visible = false;

		// Set up references to the timers
		activationTimer = GetNode<Timer>("ActivateTimer");
		activationTimer.WaitTime = ActivateDelay;

		fadeTimer = GetNode<Timer>("FadeTimer");
		fadeTimer.WaitTime = LavaFadeTime;

		// Set up reference to the animation player
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

		// Set up reference to the collision object
		harmCollisionShape = GetNode<CollisionShape2D>("%CollisionShapeHarm");
		harmCollisionShape.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);

		SwitchState(LavaTouchTrapState.Off);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {}

	// Switches the state of the trap
	private void SwitchState(LavaTouchTrapState newState) {
		currentState = newState;

		switch (newState) {
			case LavaTouchTrapState.Off:
				lavaSprite.Visible = false;

				harmCollisionShape.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
				break;

			case LavaTouchTrapState.Activated:
				activationTimer.Start();
				baseSprite.Visible = false;
				activatedSprite.Visible = true;
				break;

			case LavaTouchTrapState.On:
				activatedSprite.Visible = false;
				baseSprite.Visible = true;
				lavaSprite.Visible = true;
				
				animationPlayer.SpeedScale = 1 / LavaFadeTime;
				animationPlayer.Play("LavaFade");
				
				harmCollisionShape.SetDeferred(CollisionShape2D.PropertyName.Disabled, false);

				fadeTimer.Start();
				break;
		}
	}

	private void OnBodyEntered(PhysicsBody2D body) {
		if (body is Player && currentState == LavaTouchTrapState.Off) {
			SwitchState(LavaTouchTrapState.Activated);
		}
	}

	private void OnActivateTimerTimeout() {
		SwitchState(LavaTouchTrapState.On);
	}

	private void OnFadeTimerTimeout() {
		SwitchState(LavaTouchTrapState.Off);
	}
}
