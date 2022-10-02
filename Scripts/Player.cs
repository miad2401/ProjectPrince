using Godot;
using System;

public class Player : KinematicBody2D
{
	/*
	 * Exports
	 * 
	 * xAcceleration - Changes how fast the player accelerates left/right
	 * maxHSpeed - Determines the maximum speed the player can go left/right
	 * maxVSpeed - Determines the minimum speed the player can go up/down
	 * jumpPower - Determines how high the player goes when they jump
	 * gravity - pulls the player down at a constant rate
	 * 
	 * 
	 */

	[Export] public int xAcceleration;
	[Export] public int maxHSpeed;
	[Export] public int maxVSpeed;
	[Export] public int jumpPower;
	[Export] public int gravity;
	[Export] public int wallJumpStrength;
	[Export] public int wallFallingSpeed;
	[Export] public int numExtraJumpFrames;

	private Vector2 velocity = new Vector2();
	private AnimationTree playerAnimationTree;
	private AnimationNodeStateMachinePlayback playerANSMP;
	private Vector2 floor = new Vector2(0, -1);
	private Vector2 lastDirection = new Vector2(0,0);
	private bool movingHorizontally;
	private int framesSinceMissingFloor;
	private int numRightWalls;
	private int numLeftWalls;
	private bool onFloor;
	// Debug menu vars
	Control DebugMenu;
	Tabs PlayerMenu;
	Label Pos, XVelocity, YVelocity, IsAttacking, IsWallJumping, IsWallClimbing, OnWall, OnFloor, OnCeiling, SwordHitboxL, Swinging;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() 
	{
		framesSinceMissingFloor = numExtraJumpFrames + 1;
		playerAnimationTree = GetNode<AnimationTree>("AnimationTree");
		playerANSMP = playerAnimationTree.Get("parameters/playback") as AnimationNodeStateMachinePlayback;

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
		SwordHitboxL = PlayerMenu.GetNode<Label>("Right/SwordHitboxL");
		Swinging = PlayerMenu.GetNode<Label>("Right/Swinging");
	}

	//Can be thought as being run every frame. Delta is the amount of time it took each frame to be made (This should be constant)
	public override void _PhysicsProcess(float delta) {
		//Changes the Character's movement velocity
		//MoveCharacterOld(delta);
		MoveCharacter(delta);

		//Shoots a projectile
		//AttackOld(delta);
		Attack(delta);
		//Changes the Character's animations
		AnimatePlayer();
	}

	public void MoveCharacter(float delta)
    {
		movingHorizontally = false;
		framesSinceMissingFloor++;

		//Input for Left/Right movement
		if (IsOnWall())
        {
			velocity.x = 0;
        }

		if(Input.IsActionPressed("move_left") == Input.IsActionPressed("move_right"))
        {
			// If currently still going right, apply left force until it cancels out
			if (velocity.x > 0)
			{
				velocity.x -= xAcceleration;
				if (velocity.x <= 0)
				{
					velocity.x = 0;
				}
			}
			// If currently still going left, apply right force until it cancels out
			else
			{
				velocity.x += xAcceleration;
				if (velocity.x >= 0)
				{
					velocity.x = 0;
				}
			}
		}
        else if (Input.IsActionPressed("move_left"))
        {
			velocity.x = Mathf.Clamp(velocity.x -= xAcceleration, -maxHSpeed, maxHSpeed);
			movingHorizontally = true;
			lastDirection.x = -1;
        }
        else if (Input.IsActionPressed("move_right"))
		{
			velocity.x = Mathf.Clamp(velocity.x += xAcceleration, -maxHSpeed, maxHSpeed);
			movingHorizontally = true;
			lastDirection.x = 1;
		}

        //Input for Up/Down Movement
        if (IsOnCeiling())
        {
			velocity.y = 0;
        }

        if (!IsOnFloor())
        {
			velocity.y = Mathf.Clamp(velocity.y += gravity, -maxVSpeed, maxVSpeed);

		}
        if (IsOnFloor())
        {
			framesSinceMissingFloor = 0;
			velocity.y = 0;
		}
		if (Input.IsActionPressed("move_jump"))
        {
			if (IsOnFloor() ||framesSinceMissingFloor <= numExtraJumpFrames)
            {
				framesSinceMissingFloor = numExtraJumpFrames + 1;
				velocity.y = Mathf.Clamp(velocity.y = -jumpPower, -maxVSpeed, maxVSpeed);
			}
		}
		if (!onFloor && (numRightWalls >= 2 || numLeftWalls >= 2))
		{
			if (Input.IsActionJustPressed("move_jump") && ((Input.IsActionPressed("move_right") && numRightWalls >= 2) || (Input.IsActionPressed("move_left") && numLeftWalls >= 2)))
			{
				lastDirection.x = -lastDirection.x;
				velocity.x += lastDirection.x * (xAcceleration * wallJumpStrength);
				velocity.y = -jumpPower;
			}
            else
            {
				velocity.y = Mathf.Clamp(velocity.y, -maxVSpeed, wallFallingSpeed);
			}
		}

		MoveAndSlide(velocity, floor);

		updateDMenu();
    }

	public void Attack(float delta)
    {

    }

	public void AnimatePlayer()
    {
		if(velocity.x == 0 && !movingHorizontally)
        {
			playerANSMP.Travel("Idle");
        }
        else
        {
			playerANSMP.Travel("Run");
		}
		playerAnimationTree.Set("parameters/" + playerANSMP.GetCurrentNode() + "/blend_position", lastDirection.x);
    }

	public void updateDMenu()
	{
		Sprite playerSprite = GetNode<Sprite>("Sprite");
		Pos.Text = "POS: " + "(X: " + playerSprite.GlobalPosition.x.ToString() + ", Y: " + playerSprite.GlobalPosition.y.ToString() + ")";
		XVelocity.Text = "XVel: " + velocity.x.ToString();
		YVelocity.Text = "YVel: " + velocity.y.ToString();
		//IsAttacking.Text = "Attacking: " + (shotTimePassed > 0).ToString();
		//IsWallJumping.Text = "WallJumping: " + wallJumping.ToString();
		//IsWallClimbing.Text = "WallClimbing: " + wallClimbing.ToString();
		OnWall.Text = "OnWall: " + IsOnWall().ToString();
		OnFloor.Text = "OnFloor: " + onFloor.ToString();
		OnCeiling.Text = "OnCeiling: " + IsOnCeiling().ToString();
		//SwordHitboxL.Text = "SH-POS: " + "(X: " + SwordHitbox.GlobalPosition.x.ToString() + ", Y: " + SwordHitbox.GlobalPosition.y.ToString() + ")";

		if (Input.IsActionJustPressed("ui_debug"))
		{
			DebugMenu.Visible = !DebugMenu.Visible;
		}
	}

	// Sword hitbox, disabled when not swinging
	public void _on_SwordHitbox_body_entered(Node body) {
		// If sword touches enemy
		if (body.IsInGroup("Enemy")) {
			body.QueueFree();
		}
		else if (body.IsInGroup("Rival"))
		{
			(body as Rival).GotHit();
		}
	}

	public void MoveCamera(Vector2 newGlobalPosition)
	{
		GetNode<Camera2D>("Camera2D").GlobalPosition = newGlobalPosition;
	}

	public void OnWalljumpBodyTouched(Node body, bool entered, bool right)
    {
        if (body.Name.Equals("Baseground"))
        {
            if (right)
            {
				numRightWalls += entered ? 1 : -1;
			}
            else
            {
				numLeftWalls += entered ? 1 : -1;
			}
			GD.Print("num left: " + numLeftWalls + ". num right: " + numRightWalls);
		}
    }

	public void OnFloorDetectorBodyTouched(Node body, bool entered)
    {
		if (body.Name.Equals("Baseground"))
		{
			if (entered)
			{
				onFloor = true;
			}
			else
			{
				onFloor = false;
			}
		}
	}
}



