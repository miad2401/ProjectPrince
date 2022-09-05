using Godot;
using System;

public class Player : KinematicBody2D
{
	/*
	 * All of these variables can be changed within the editor, but are not changed within the script
	 *
	 * xAcceleration - Changes how fast the player accelerates left/right
	 * maxHSpeed - Determines the maximum speed the player can go left/right
	 * maxVSpeed - Determines the minimum speed the player can go up/down
	 * jumpPower - Determines how high the player goes when they jump
	 * gravity - pulls the player down at a constant rate
	 * shotDelay - The time between each shot. Will be changed in the future
	 *
	 * shotTimePassed - The amount of time passed since the last shot, once a shot is fired is reset to shotDelay, then is decreased every frame by delta
	 * velocity - the Velocity of the player. It is a Vector2, meaning it contains 2 variables, x and y.
	 * direction - Used to determine what direction the player was last looking at,
	 *             usually the same as velocity, but has a value when velocity is (0,0)
	 *             Note: Only used for x-value
	 */
	[Export] int xAcceleration = 10;
	[Export] int maxHSpeed = 200;
	[Export] int maxVSpeed = 1000;
	[Export] int jumpPower = 200;
	[Export] int gravity = 10;
	[Export] float shotDelay = 0.75f;
	[Export] int pushStrength = 5;

	float shotTimePassed = 0.0f;
	Vector2 velocity = new Vector2();
	Vector2 direction = new Vector2();

	//This is a PackedScene variable that will hold the PlayerAttack. Allows us quick loading and lets us instance multiple attacks
	public PackedScene PlayerProjectilePath;
	//Used to determine what direction is up/down/left/right for the MoveAndCollide function
	Vector2 floor = new Vector2(0,-1);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		//When the player is loaded, loads the PlayerAttack PackedScene to be used later
		PlayerProjectilePath = GD.Load<PackedScene>("res://Scenes/PlayerAttack.tscn");
	}

	//Can be thought as being run every frame. Delta is the amount of time it took each frame to be made (This should be constant)
	public override void _PhysicsProcess(float delta) {
		//Changes the Character's movement velocity
		MoveCharacter(delta);

		//Shoots a projectile
		Fire(delta);

		//Changes the Character's animations
		AnimatePlayer();
	}

	//Changes the Character's movement velocity
	public void MoveCharacter(float delta) {
		Sprite playerSprite = GetNode<Sprite>("Sprite");
		//Interaction with movable Objects
		//Gets the number of "Slides" and checks each one
		for (int i = 0; i < GetSlideCount(); i++) {
			//Sets the collision as a variable
			KinematicCollision2D collision = GetSlideCollision(i);
			//Makes sure the Object collided with is moveable
			if((collision.Collider as Node).IsInGroup("MoveableObject")) {
				//Sets the moving object as a variable
				RigidBody2D moveableObject = collision.Collider as RigidBody2D;
				//Sets a directional Impulse
				moveableObject.ApplyCentralImpulse(-collision.Normal * pushStrength);
			}
		}
		//If ran into wall, stops the player
		if (IsOnWall()) {
			velocity.x = 0;
		}
		//Checks if the right arrowkey is pressed, and if so, sets the x portion of the velocity to 1
		if (Input.IsActionPressed("ui_right")) {
			velocity.x += xAcceleration;
			//Direction is the last direction the player moved, +1 is right
			playerSprite.Scale = new Vector2(1, 1);
			direction.x = 1;
		}
		//Checks if the left arrowkey is pressed, and if so, sets the x portion of the velocity to -1
		else if (Input.IsActionPressed("ui_left")) {
			velocity.x -= xAcceleration;
			//Direction is the last direction the player moved, -1 is right
			playerSprite.Scale = new Vector2(-1, 1);
			direction.x = -1;
		}
		//If neither left/right key has been pressed, slows down the character 2x as fast as the player accelerates
		else {
			if (velocity.x > 0) {
				velocity.x -= xAcceleration * 2;
				if (velocity.x < 0) {
					velocity.x = 0;
				}
			}
			else {
				velocity.x += xAcceleration * 2;
				if (velocity.x > 0) {
					velocity.x = 0;
				}
			}
		}

		//Checks if the Player is touching the Ceiling
		if (IsOnCeiling()) {
			//Stops Player's vertial movement
			velocity.y = 0;
		}

		//Apply the force of gravity
		velocity.y += gravity;

		//Checks if the Player is on the Floor
		if (IsOnFloor()) {
			//Resets vertical velocity to 0.01
			//0.01 instead of 0 because IsOnFloor() does not realize the player isn't on the floor if they don't move after beign called
			velocity.y = 0.01f;
			//If the Player is on the floor and pressing the jump key, lets the player jump
			if (Input.IsActionPressed("ui_jump")) {
				velocity.y -= jumpPower;
			}
		}

		//Velocity is limited by the maxHSpeed and the maxVSpeed
		//These values can be changed within the editor
		velocity.x = Math.Min(velocity.x, maxHSpeed);
		velocity.x = Math.Max(velocity.x, -maxHSpeed);
		velocity.y = Math.Min(velocity.y, maxVSpeed);
		velocity.y = Math.Max(velocity.y, -maxVSpeed);
		//Moves the player according to the velocity and defines what direction to go
		//"false, 4, 0.78598f" are default values
		//Last argument is for infinite_inertia, We turn this off so environment can be interacted with
		MoveAndSlide(velocity, floor, false, 4, 0.785398f, false);
	}

	//Changes the Character's animations
	public void AnimatePlayer() {
		//Finds the child AnimationTree node and sets a references to it to a AnimationTree variable
		AnimationTree animationTree = GetNode<AnimationTree>("AnimationTree");
		//Finds the AnimationNodeStateMachinePlayback resource within the animationTree and sets it to its own variable
		//Because Godot doesn't allow arguments in the .Get() function, we also must cast it as a AnimationNodeStateMachinePlayback
		AnimationNodeStateMachinePlayback myANSMP = animationTree.Get("parameters/playback") as AnimationNodeStateMachinePlayback;
		if (velocity.x == 0 && velocity.y == 0.01f) {
			//If no key is being pressed, switches to an idle animation
			myANSMP.Travel("Idle");
		}
		else {
			//If a key is being pressed, switches to an walk animation
			myANSMP.Travel("Walk");
		}
	}

	//Shoots a projectile from the player
	public void Fire(float delta) {
		//Decreases the amount of time left needed to fire a shot
		shotTimePassed -= delta;

		//Checks that the fire key (Space) is pressed and that enough time has passed to fire another shot
		if (Input.IsActionPressed("ui_accept") && shotTimePassed < 0) {
			//Resets the shot cooldown
			shotTimePassed = shotDelay;
			//Creates an instance of the PlayerAttack
			RigidBody2D PPInstance = PlayerProjectilePath.Instance() as RigidBody2D;
			//Finds the position of the Player's ProjectilePosition node
			//This is used to determine where to spawn the shot
			Position2D p2D = GetNode<Position2D>("ProjectilePosition");
			//Creates a reference to the instanced attack's CollisionPoly
			CollisionPolygon2D pCollision = PPInstance.GetNode<CollisionPolygon2D>("CollisionPoly");
			//Checks if the player is facing forward
			if (direction.x > 0) {
				//Sets the starting position of the attack to the right of the player
				p2D.Position = new Vector2(8,0);
				//Sets the position of the CollisionPoly to 0, which is a right-facing collisionbox
				pCollision.RotationDegrees = 0;
			}
			else {
				//Sets the starting position of the attack to the left of the player
				p2D.Position = new Vector2(-8, 0);
				//Sets the position of the CollisionPoly to 0, which is a left-facing collisionbox
				pCollision.RotationDegrees = 180;
			}
			//Sets the position of the instanced attack to Player's ProjectilePosition Node
			PPInstance.Position = GetNode<Position2D>("ProjectilePosition").GlobalPosition;
			//Adds the instanced attack to the main scene
			GetParent().AddChild(PPInstance);
		}
	}
}
