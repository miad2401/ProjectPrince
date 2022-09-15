using Godot;
using System;

public abstract class BaseEnemy : KinematicBody2D
{

	/* 
	 * Export - Changed within Godot editor itself
     * bool hurtPlayer - If true, kills player when touched
     * Direction direction - The Direction the enemy last moved towards
     * Vector2 velocity - The x and y movement of the enemy
     * Vector2 floor - Shows where the floor is, used for MoveAndSlide()
     */
	[Export] protected bool hurtPlayer;
	[Export] protected Direction direction;
	protected Vector2 velocity = new Vector2();
	protected Vector2 floor = new Vector2(0, -1);

	//Signal that tells the PauseMenu if the player died
	[Signal] public delegate void PlayerDeath();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Connects signal that tells the PauseMenu if the player died
		Connect(nameof(PlayerDeath), GetNode("/root/Main/GUI/PauseMenu"), "PlayerDied");
	}

	//Can be thought as being run every frame. Delta is the amount of time it took each frame to be made (This should be constant)
	public override void _PhysicsProcess(float delta)
	{
		//Method to move the enemy
		MoveEnemy(delta);

		//Method to Animate the Enemy
		AnimateEnemy();
	}

	public abstract void MoveEnemy(float delta);

	public void AnimateEnemy()
	{
		//Finds the child AnimationTree node and sets a references to it to a AnimationTree variable
		AnimationTree animationTree = GetNode<AnimationTree>("EnemyAnimationTree");
		//Finds the AnimationNodeStateMachinePlayback resource within the animationTree and sets it to its own variable
		//Because Godot doesn't allow arguments in the .Get() function, we also must cast it as a AnimationNodeStateMachinePlayback
		AnimationNodeStateMachinePlayback enemyANSMP = animationTree.Get("parameters/playback") as AnimationNodeStateMachinePlayback;
		if (velocity == Vector2.Zero)
		{
			//If not moving, switches to an idle animation
			enemyANSMP.Travel("Idle");
		}
		else
		{
			//If moving, switches to an walk animation
			enemyANSMP.Travel("Walk");
			//Changes direction of the animation based on the velocity
			animationTree.Set("parameters/Idle/blend_position", velocity);
			animationTree.Set("parameters/Walk/blend_position", velocity);
		}
	}
}
