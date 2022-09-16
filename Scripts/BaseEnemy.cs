using Godot;
using System;

public abstract class BaseEnemy : KinematicBody2D
{

	/* 
	 * Export - Changed within Godot editor itself
	 * bool hurtPlayer - If true, kills player when touched
	 * Direction direction - The Direction the enemy last moved towards
	 * int pushStrength - The amount of force that is used to push moveableObjects
	 * bool pushable - If true, moveableObjects can be moved by this enemy
	 * Vector2 velocity - The x and y movement of the enemy
	 * Vector2 floor - Shows where the floor is, used for MoveAndSlide()
	 */
	[Export] protected bool hurtPlayer;
	[Export] protected Direction direction;
	[Export] protected int pushStrength;
	[Export] protected bool pushable;
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

	public void CheckForCollision()
	{
		//Interaction with movable Objects and Players
		//Gets the number of "Slides" and checks each one
		for (int i = 0; i < GetSlideCount(); i++)
		{
			//Sets the collision as a variable
			KinematicCollision2D collision = GetSlideCollision(i);
			//After Reset, the Collider is sometimes null, so check for it
			if (collision.Collider == null)
			{
				//If the collision is empty, skips to next collision
				continue;
			}
			//Checks if collided with the Player
			else if ((collision.Collider as Node).IsInGroup("Player"))
			{
				//If the enemy can hurt the player, calls the PlayerDeath signal
				if (hurtPlayer)
				{
					EmitSignal(nameof(PlayerDeath));
					//Used to make sure PlayerDeath isn't called twice
					hurtPlayer = false;
				}
			}
			//Makes sure the Object collided with is moveable
			else if (pushable && (collision.Collider as Node).IsInGroup("MoveableObject"))
			{
				//Sets the moving object as a variable
				RigidBody2D moveableObject = collision.Collider as RigidBody2D;
				//Sets a directional Impulse
				moveableObject.ApplyCentralImpulse(-collision.Normal * pushStrength);
			}
		}
	}

	public abstract void AnimateEnemy();
}
