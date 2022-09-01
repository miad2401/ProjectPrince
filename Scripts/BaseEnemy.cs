using Godot;
using System;

public class BaseEnemy : KinematicBody2D
{

	/*
	 * All of these variables can be changed within the editor, but are not changed within the script
	 * 
	 * hSpeed - Determines the speed the enemy can go left/right
	 * vSpeed - Determines the Max speed the enemy can go up/down
	 * jumpPower - Determines how high the enemy goes when they jump
	 * gravity - pulls the enemy down at a constant rate
	 */
	[Export] int hSpeed = 50;
	[Export] int maxVSpeed = 500;
	[Export] int jumpPower = 250;
	[Export] int gravity = 10;
	[Export] int pushStrength = 25;
	//for direction, true is right, false is left
	[Export] bool direction = false;

	//velocity is the Velocity of the enemy. It is a Vector2, meaning it contains 2 variables, x and y. 
	Vector2 velocity = new Vector2();
	//Used to determine what direction is up/down/left/right
	Vector2 floor = new Vector2(0, -1);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	//Can be thought as being run every frame. Delta is the amount of time it took each frame to be made (This should be constant)
	public override void _PhysicsProcess(float delta)
	{
		//Method to move the enemy
		MoveEnemy(delta);

		//Method to Animate the Enemy
		AnimateEnemy();
	}

	public void MoveEnemy(float delta)
	{
		//Interaction with movable Objects
		//Gets the number of "Slides" and checks each one
		for (int i = 0; i < GetSlideCount(); i++)
		{
			//Sets the collision as a variable
			KinematicCollision2D collision = GetSlideCollision(i);
			//Makes sure the Object collided with is moveable
			if ((collision.Collider as Node).IsInGroup("MoveableObject"))
			{
				//Sets the moving object as a variable
				RigidBody2D moveableObject = collision.Collider as RigidBody2D;
				//Sets a directional Impulse
				moveableObject.ApplyCentralImpulse(-collision.Normal * pushStrength);
			}
		}

		//If ran into wall, flips the enemy to head the other direction
		if (IsOnWall())
		{
			direction = !direction;
		}

		//Checks what direction the enemy is facing, and moves forward that direction
		if (direction)
		{
			velocity.x = hSpeed;
		}
		else
		{
			velocity.x = -hSpeed;
		}

		//Checks if the Enemy is touching the Ceiling
		if (IsOnCeiling())
		{
			//Stops Enemy's vertial movement
			velocity.y = 0;

		}

		//Apply the force of gravity
		velocity.y += gravity;

		//Checks if the Enemy is on the Floor
		if (IsOnFloor())
		{
			//Changes Enemy Velocity upwards to make them jump
			velocity.y = -jumpPower;
		}
	}

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

			//Verticy velocity is limited by the maxVSpeed
			//These values can be changed within the editor
			velocity.y = Math.Min(velocity.y, maxVSpeed);
			velocity.y = Math.Max(velocity.y, -maxVSpeed);
			//Moves the Enemy according to the velocity and defines what direction to go
			//"false, 4, 0.78598f" are default values
			//Last argument is for infinite_inertia, We turn this off so environment can be interacted with
			MoveAndSlide(velocity, floor, false, 4, 0.785398f, false);
		}
	}

}
