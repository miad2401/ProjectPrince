using Godot;
using System;

public class Knight : BaseEnemy
{
	/*
	 * Fields
	 * int hSpeed - The speed that the knight accelerates horizontally
	 * int maxVSpeed - The maximum speed that the knight can move vertically
	 * int jumpPower - The power at which the knight can jump
	 * int gravity - The constant rate that the knight is pulled towards the ground
	 * int pushStrength - The amount of force that is used to push moveableObjects
	 */
	[Export] private int hSpeed;
	[Export] private int maxVSpeed;
	[Export] private int jumpPower;
	[Export] private int gravity;
	[Export] private int pushStrength;

	/*
     * Inherited Fields
     * bool hurtPlayer - If true, kills player when touched
     * Direction direction - The Direction the enemy last moved towards
     * Vector2 velocity - The x and y movement of the bat
     * Vector2 floor - Shows where the floor is, used for MoveAndSlide()
     */

	public override void MoveEnemy(float delta)
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
				}
			}
			//Makes sure the Object collided with is moveable
			else if ((collision.Collider as Node).IsInGroup("MoveableObject"))
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
			if(direction == Direction.Right)
            {
				direction = Direction.Left;
            }
            else
            {
				direction = Direction.Right;
            }
		}

		//Checks what direction the enemy is facing, and moves forward that direction
		if (direction == Direction.Right)
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
