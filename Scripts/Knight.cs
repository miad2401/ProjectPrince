using Godot;
using System;

public class Knight : BaseEnemy
{
	/*
	 * Fields
	 * int hSpeed - The speed that the knight accelerates horizontally
	 * int maxVSpeed - The maximum speed that the knight can move vertically
	 * int gravity - The constant rate that the knight is pulled towards the ground
	 */
	[Export] private int hSpeed;
	[Export] private int maxVSpeed;
	[Export] private int gravity;

	AnimationTree animationTree;
	AnimationNodeStateMachinePlayback enemyANSMP;
	/*
     * Inherited Fields
     * bool hurtPlayer - If true, kills player when touched
     * Direction direction - The Direction the enemy last moved towards
     * Vector2 velocity - The x and y movement of the bat
     * Vector2 floor - Shows where the floor is, used for MoveAndSlide()
     */

	public override void _Ready()
    {
        base._Ready();

		//Finds the child AnimationTree node and sets a references to it to a AnimationTree variable
		animationTree = GetNode<AnimationTree>("EnemyAnimationTree");
		//Finds the AnimationNodeStateMachinePlayback resource within the animationTree and sets it to its own variable
		//Because Godot doesn't allow arguments in the .Get() function, we also must cast it as a AnimationNodeStateMachinePlayback
		enemyANSMP = animationTree.Get("parameters/playback") as AnimationNodeStateMachinePlayback;

		if (GetDirection() == Direction.Left)
        {
			animationTree.Set("parameters/Idle/blend_position", -1);
		}
        else
        {
			animationTree.Set("parameters/Idle/blend_position", 1);
		}

    }

    public override void MoveEnemy(float delta)
    {
		CheckForCollision();

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

        if (IsOnFloor())
        {
			velocity.y = 0;
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

    public override void AnimateEnemy()
    {
        if (velocity.x == 0 && !IsOnWall())
		{
			//If not moving, switches to an idle animation
			enemyANSMP.Travel("Idle");
		}
		else
		{
			//If moving, switches to an run animation
			enemyANSMP.Travel("Run");
			//Changes direction of the animation based on the velocity
			animationTree.Set("parameters/Idle/blend_position", velocity.x);
			animationTree.Set("parameters/Run/blend_position", velocity.x);
		}
	}

	//Setters
	public void SetHSpeed(int hs)
    {
		hSpeed = hs;
    }
	public void SetMaxVSpeed(int mvs)
    {
		maxVSpeed = mvs;
    }
	public void SetGravity(int g)
    {
		gravity = g;
    }
}
