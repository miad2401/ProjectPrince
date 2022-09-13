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
	 * climbPower - Determines the speed at which the player climbs a ladder
	 * gravity - pulls the player down at a constant rate
	 * shotDelay - The time between each shot. In seconds.
	 * pushStrength - The amount of force that is used to push moveableObjects
	 * wallJumpTime - The maximum amount of time that a player is allowed to wall jump
	 * wallJumpStrength - The amount of force that is used during a wall jump
	 * magicJumpPower - Determine how high the player goes when they magic jump
	 * jumpXDeacceleration - (Not final) Factor that decreases x velocity deacceleration when l/r input stops
	 *
	 * jumping - Holds if the player is currently jumping
	 * magicJumping - Holds if the player has magicJumped
	 * wallJumping - Holds if the player is currently wall jumping
	 * canWallJump - Holds if the player is allowed to wall jump
	 * wallJumpTimePassed - Holds the amount of time since a wall jump has started
	 * wallClimbing - Holds if the player is currently wall climbing
	 * canWallClimb - Holds if the player can wall climb
	 * shotTimePassed - The amount of time passed since the last shot, once a shot is fired is reset to shotDelay, then is decreased every frame by delta
	 * 
	 * 
	 * velocity - the Velocity of the player. It is a Vector2, meaning it contains 2 variables, x and y.
	 * direction - Used to determine what direction the player was last looking at,
	 *             usually the same as velocity, but has a value when velocity is (0,0)
	 *             Note: Only used for x-value
	 */
	[Export] int xAcceleration;
	[Export] int maxHSpeed;
	[Export] int maxVSpeed;
	[Export] int jumpPower;
	[Export] int climbPower;
	[Export] int gravity;
	[Export] float shotDelay;
	[Export] int pushStrength;
	[Export] float wallJumpTime;
	[Export] int wallJumpStrength;
	[Export] int magicJumpPower;
	[Export] float jumpXDeacceleration;

	
	bool jumping = false;
	bool magicJumping = false;
	bool wallJumping = false;
	bool canWallJump = true;
	float wallJumpTimePassed = 0f;
	bool wallClimbing = false;
	bool canWallClimb = false;
	float shotTimePassed = 0f;

	Vector2 velocity = new Vector2();
	Vector2 direction = new Vector2();

	//This is a PackedScene variable that will hold the PlayerAttack. Allows us quick loading and lets us instance multiple attacks
	public PackedScene PlayerProjectilePath;
	//Used to determine what direction is up/down/left/right for the MoveAndCollide function
	Vector2 floor = new Vector2(0,-1);
	
	// Debug menu vars
	Control DebugMenu;
	Tabs PlayerMenu;
	Label Pos, XVelocity, YVelocity, IsAttacking, IsWallJumping, IsWallClimbing, OnWall, OnFloor, OnCeiling;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		//When the player is loaded, loads the PlayerAttack PackedScene to be used later
		PlayerProjectilePath = GD.Load<PackedScene>("res://Scenes/PlayerAttack.tscn");
		
		// Debug menu vars
		DebugMenu = GetNode<Control>("DebugMenu/DebugMenu");
		PlayerMenu = DebugMenu.GetNode<Tabs>("TabContainer/Player");
		Pos = PlayerMenu.GetNode<Label>("Left/Position");
		XVelocity = PlayerMenu.GetNode<Label>("Left/XVelocity");
		YVelocity = PlayerMenu.GetNode<Label>("Left/YVelocity");
		IsAttacking = PlayerMenu.GetNode<Label>("Left/IsAttacking");
		IsWallJumping = PlayerMenu.GetNode<Label>("Left/IsWallJumping");
		IsWallClimbing = PlayerMenu.GetNode<Label>("Left/IsWallClimbing");
		OnWall = PlayerMenu.GetNode<Label>("Right/IsOnWall");
		OnFloor = PlayerMenu.GetNode<Label>("Right/IsOnFloor");
		OnCeiling = PlayerMenu.GetNode<Label>("Right/IsOnCeiling");
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
		// Store playerSprite for flipping
		Sprite playerSprite = GetNode<Sprite>("Sprite");
		
		//By default, reset canWallJump and canWallClimb vars to their default values, as they are 
		//updated every frame
		canWallJump = true;
		canWallClimb = false;
		
		//Interaction with movable Objects
		//Gets the number of "Slides" and checks each one
		for (int i = 0; i < GetSlideCount(); i++) {
			//Sets the collision as a variable
			KinematicCollision2D collision = GetSlideCollision(i);
			//After Reset, the Collider is sometimes null, so check for it
			if (collision.Collider == null) {
				//If the collision is empty, skips to next collision
				continue;
			}
			//Checks if the collision was with a moveableObject
			if ((collision.Collider as Node).IsInGroup("MoveableObject")) {
				//Cannot walljump or wallclimb off moveable objects
				canWallJump = false;
				// If the player is not on floor (on top of object) and pressing jump, apply directional force
				// Necessary to prevent downward push force from being on top of object cancelling out jump force
				if (!(IsOnFloor() && (Input.IsActionPressed("move_jump")))) {
					//Sets the moving object as a variable
					RigidBody2D moveableObject = collision.Collider as RigidBody2D;
					//Sets a directional Impulse
					moveableObject.ApplyCentralImpulse(-collision.Normal * pushStrength);
				}
			}
			// Check for collision with a climable object
			else if ((collision.Collider as Node).IsInGroup("WallClimbable")) {
				//Can wallclimb and walljump off wallclimbable objects
				canWallJump = true;
				canWallClimb = true;
			}
			// Check for collision with an enemy
			else if ((collision.Collider as Node).IsInGroup("Enemy")) {
				//Cannot walljump or wallclimb off enemies
				canWallJump = false;
			}
		}
		
		
		/*** Walljumping delay check ***/
		// Used to put a buffer between when the player jumps off a wall and when the L/R keys receive input
		// Prevents a sort of stutter where the player will immediately turn back around from a walljump due to still having key pressed
		// Player is currently wallJumping
		if (wallJumping) {
			// Increase time passed timer
			wallJumpTimePassed += delta;
			// If the timer has passed the amount of time a wall jump takes
			if (wallJumpTimePassed >= wallJumpTime) {
				// Stop walljumping
				wallJumping = false;
				// Reset timer
				wallJumpTimePassed = 0f;
			}
		}
		
		//If ran into wall, stops the player from accelerating into the wall
		if (IsOnWall()) {
			velocity.x = 0;
		}

		//Checks if the left arrowkey is pressed
		if (Input.IsActionPressed("move_left")) {
			// If the player is moving left INTO a wall, not currently wall jumping, taps jump, 
			// and is falling/jumping, wall jump
			if (canWallJump && IsOnWall() && Input.IsActionJustPressed("move_jump") && velocity.y != 0.01f) {
				// Set walljumping tag to true
				wallJumping = true;
				jumping = true;
				// Flip direction
				direction.x = -direction.x;
				// Flip sprite
				playerSprite.Scale = new Vector2(direction.x, 1);
				// Apply x velocity in the new direction, by the force of wallJumpStrength
				velocity.x += direction.x * (xAcceleration*wallJumpStrength);
				// Set y velocity to negative jumpPower, allows for chain walljumping
				velocity.y = -jumpPower;
			}
			// Otherwise, if they are just moving left and NOT walljumping, move them left and such
			// Check if not walljumping to prevent a sort of "stutter", where player will still be holding
			// the left key after walljumping due to human reaction time, which would cancel out the wall jump
			else if (!wallJumping){
				//Checks if running into climbable object
				if (canWallClimb)
				{
					//If so, raises player into the air and sets wallclimbing to true
					velocity.y = -climbPower;
					wallClimbing = true;
				}
				// Not on a climable object
				else
				{
					//Decrease x velocity (go left)
					velocity.x -= xAcceleration;
					wallClimbing = false;
				}
				// Set direction to -1 (left)
				direction.x = -1;
				// Change sprite to face new direction
				playerSprite.Scale = new Vector2(direction.x, 1);
			}
		}
		//Checks if the right arrowkey is pressed
		else if (Input.IsActionPressed("move_right")) {
			// If the player is moving right INTO a wall, not currently wall jumping, taps jump, 
			// If the player is moving right INTO a wall, not currently wall jumping, taps jump, 
			// and is falling/jumping, wall jump
			if (canWallJump && IsOnWall() && Input.IsActionJustPressed("move_jump") && velocity.y != 0.01f) {
				// Set walljumping tag to true
				wallJumping = true;
				jumping = true;
				// Flip direction
				direction.x = -direction.x;
				// Flip sprite based off new direction
				playerSprite.Scale = new Vector2(direction.x, 1);
				// Apply x velocity in the new direction, by the force of wallJumpStrength
				velocity.x += direction.x * (xAcceleration*wallJumpStrength);
				// Cancel out current y velocity to allow for chain walljumps
				velocity.y = -jumpPower;
			}
			// Otherwise, if they are just moving left and NOT walljumping, move them left and such
			// Check if not walljumping to prevent a sort of "stutter", where player will still be holding
			// the left key after walljumping due to human reaction time, which would cancel out the wall jump
			else if (!wallJumping){
				//Checks if running into climbable object
				if (canWallClimb)
				{
					//If so, raises player into the air and sets wallclimbing to true
					velocity.y = -climbPower;
					wallClimbing = true;
				}
				// Not on climable object
				else
				{
					//Decrease x velocity (go right)
					velocity.x += xAcceleration;
					wallClimbing = false;
				}
				// Set directon to 1 (right)
				direction.x = 1;
				// Change sprite to face new direction
				playerSprite.Scale = new Vector2(direction.x, 1);
			}
		}
		//If neither left/right key has been pressed (player not being told to move), 
		//Slows down character's x velocity
		//This makes stopping l/r movement seem more smooth and natural
		else {
			// If currently still going right, apply left force until it cancels out
			if (velocity.x > 0) {
				// If player is currently jumping, divide the added force to slow down slower (for better realistic feel)
				//TODO: Take into account y velocity for divide factor
				velocity.x -= (jumping ? (xAcceleration/jumpXDeacceleration) : xAcceleration);
				if (velocity.x <= 0) {
					velocity.x = 0;
				}
			}
			// If currently still going left, apply right force until it cancels out
			else {
				// If player is currently jumping, divide the added force to slow down slower (for better realistic feel)
				//TODO: Take into account y velocity for divide factor
				velocity.x += (jumping ? (xAcceleration/jumpXDeacceleration) : xAcceleration);
				if (velocity.x >= 0) {
					velocity.x = 0;
				}
			}
		}
		
		// If player is currently jumping, and hasn't already magicJumped
		if (jumping && !magicJumping) {
			// Check for input
			if (Input.IsActionPressed("move_magicJump")) {
				// Magic jump
				magicJumping = true;
				velocity.y = -magicJumpPower;
			}
		}
		

		//Checks if the Player is touching the ceiling
		if (IsOnCeiling()) {
			// Stops Player's vertial movement
			velocity.y = 0;
		}

		//Apply the force of gravity
		velocity.y += gravity;

		//Checks if the Player is on the Floor
		if (IsOnFloor() && !wallClimbing) {
			//Resets vertical velocity to 0.01
			//0.01 instead of 0 because IsOnFloor() does not realize the player isn't on the floor if they don't move after beign called
			velocity.y = 0.01f;
			// Reset jumping flags
			jumping = false;
			magicJumping = false;
			wallJumping = false;
			//If the Player is on the floor and pressing the jump key, lets the player jump
			if (Input.IsActionPressed("move_jump")) {
				velocity.y -= jumpPower;
				jumping = true;
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
		
		// Update debug menu
		updateDMenu();
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
		if (Input.IsActionPressed("attack") && shotTimePassed < 0) {
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
			if (direction.x >= 0) {
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
	
	public void updateDMenu() {
		Sprite playerSprite = GetNode<Sprite>("Sprite");
		Pos.Text = "POS: " + "(X: " + playerSprite.GlobalPosition.x.ToString() + ", Y: " + playerSprite.GlobalPosition.y.ToString() + ")";
		XVelocity.Text = "XVel: " + velocity.x.ToString();
		YVelocity.Text = "YVel: " + velocity.y.ToString();
		IsAttacking.Text = "Attacking: " + (shotTimePassed > 0).ToString();
		IsWallJumping.Text = "WallJumping: " + wallJumping.ToString();
		IsWallClimbing.Text = "WallClimbing: " + wallClimbing.ToString();
		OnWall.Text = "OnWall: " + IsOnWall().ToString();
		OnFloor.Text = "OnFloor: " + IsOnFloor().ToString();
		OnCeiling.Text = "OnCeiling: " + IsOnCeiling().ToString();
		
		if (Input.IsActionJustPressed("ui_debug")) {
			DebugMenu.Visible = !DebugMenu.Visible;
		}
	}
}
